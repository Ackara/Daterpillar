﻿<#
.SYNOPSIS
Psake build tasks.
#>

Properties {
	$Config = (Get-Content "$PSScriptRoot\solution.json" | Out-String | ConvertFrom-Json);

	# Paths
	$RootDir = (Split-Path $PSScriptRoot -Parent);
	$ReleaseNotesTXT = "$RootDir\releaseNotes.txt";
	$SemVerJson = "$PSScriptRoot\version.json";
	$ArtifactsDir = "$RootDir\artifacts";
	$Nuget = "";

	# User Args
	$NuGetKey = "";
	$BranchName = "";
	$TestNames = @();
	$BuildConfiguration = "";

	$Major = $false;
	$Minor = $false;
}

Task "default" -depends @("Setup");

Task "deploy" -description "This task will build, test then publish the project." `
-depends @("Build-Solution", "Run-Tests", "Publish-Packages");

# -----

Task "Init" -description "This task loads and creates all denpendencies." -action {
	$buildboxDir = (Get-Item "$RootDir\packages\Ackara.Buildbox.*\tools" | Sort-Object $_.Name | Select-Object -ExpandProperty FullName -Last 1);
	foreach ($module in @("semver.dll", "utils.psm1"))
	{
		$pathToModulue = Get-Item "$buildboxDir\*\*$module" | Select-Object -ExpandProperty FullName;
		if (Test-Path $pathToModulue -PathType Leaf)
		{
			Import-Module $pathToModulue -Force;
			Write-Host "`t* imported $(Split-Path $pathToModulue -Leaf) module.";
		}
	}

	$pester = "$RootDir\packages\Pester*\tools\Pester.psd1";
	if (Test-Path $pester -PathType Leaf)
	{
		Import-Module $pester -Force;
		Write-Host "`t* imported pester module.";
	}
}

Task "Setup" -description "This task will generate all missing/sensitive files missing from the project." -action {
}

Task "Increment-VersionNumber" -alias "version" -description "This task increments the patch version number within all neccessary files." `
-depends @("Init") -action {
	$commitMsg = Show-Inputbox "please enter your release notes" "RELEASE NOTES";
	Update-VersionNumber "$RootDir\src" -Message $commitMsg -UsecommitMessageAsDescription -ConfigFile $SemVerJson -Major:$Major -Minor:$Minor -Patch;
	$version = Get-VersionNumber -ConfigFile $SemVerJson;

	if (-not [string]::IsNullOrEmpty($commitMsg))
	{
		try
		{
			Push-Location $RootDir;
			$notes = "version $($version.Major).$($version.Minor).$($version.Patch)`n";
			$notes += "----------------`n";
			$notes += $commitMsg;

			$contents = Get-Content $ReleaseNotesTXT | Out-String;
			"$notes`n`n$contents".Trim() | Out-File $ReleaseNotesTXT -Encoding ascii;
			Exec {
				& git add releaseNotes.txt;
				& git add build\version.json;
				& git commit --amend --no-edit;
				& git tag "v$($version.ToString($true))";
			}
		}
		finally { Pop-Location; }
	}
}

Task "Build-Solution" -alias "compile" -description "This task complites the solution." `
-depends @("Init") -action {
	Assert ("Debug", "Release" -contains $BuildConfiguration) "`$BuildConfiguration was '$BuildConfiguration' but expected 'Debug' or 'Release'.";

	$sln = Get-Item "$RootDir\*.sln" | Select-Object -ExpandProperty FullName;
	Exec { msbuild $sln "/p:Configuration=$BuildConfiguration;Platform=Any CPU"  "/v:minimal"; };
}

Task "Run-Tests" -alias "test" -description "This task runs all automated tests." `
-depends @("Build-Solution") -action {
	Assert ("Debug", "Release" -contains $BuildConfiguration) "`$BuildConfiguration was '$BuildConfiguration' but expected 'Debug' or 'Release'.";

	foreach ($proj in (Get-ChildItem "$RootDir\tests" -Recurse -Filter "*.*proj" | Select-Object -ExpandProperty FullName))
	{
		Exec { & dotnet test $proj --configuration $BuildConfiguration --verbosity minimal; }
	}

	foreach ($script in (Get-ChildItem "$RootDir\tests" -Recurse -Filter "*test*.ps1" | Select-Object -ExpandProperty FullName))
	{
		Invoke-Pester -Script $script;
	}
}

Task "Create-Packages" -alias "pack" -description "This task creates all deployment artifacts." `
-depends @("Init") -action {
	$nupkgsDir = "$ArtifactsDir\nupkgs";
	if (Test-Path $ArtifactsDir -PathType Container) { Remove-Item $ArtifactsDir -Recurse -Force; }
	New-Item $nupkgsDir -ItemType Directory | Out-Null;

	foreach ($proj in (Get-ChildItem "$RootDir\src" -Recurse -Filter "*.*proj" | Select-Object -ExpandProperty FullName))
	{
		$nuspec = [IO.Path]::ChangeExtension($proj, ".nuspec");
		if (Test-Path $nuspec -PathType Leaf)
		{
			[xml]$csproj = Get-Content $proj;
			$version = Get-VersionNumber -ConfigFile $SemVerJson;

			$properties = "";
			$properties += "id=$($csproj.SelectSingleNode('.//Project//AssemblyName').InnerText);";
			$properties += "version=$($version.Major).$($version.Minor).$($version.Patch);";
			$properties += "owner=$($Config.metadata.owner);";
			$properties += "copyright=$($Config.metadata.copyright);";
			$properties += "license=$($Config.metadata.licenseUrl);";
			$properties += "projectUrl=$($Config.metadata.projectUrl);";
			$properties += "iconUrl=$($Config.metadata.iconUrl);";
			$properties += "description=$($Config.metadata.description);";
			$properties += "tags=$($Config.metadata.tags);";
			$properties += "Configuration=$BuildConfiguration;";
			$properties += "releaseNotes=$(Get-Content $ReleaseNotesTXT | Out-String);";
			$properties += "targetFramework=$($csproj.SelectSingleNode('.//Project//TargetFramework').InnerText)";
			$versionSuffix = Get-VersionSuffix;

			if ([String]::IsNullOrEmpty($VersionSuffix))
			{ Exec { & $nuget pack $nuspec -OutputDirectory $ArtifactsDir -Properties $properties -IncludeReferencedProjects; } }
			else
			{ Exec { & $nuget pack $nuspec -OutputDirectory $ArtifactsDir -Properties $properties -IncludeReferencedProjects -Suffix $versionSuffix; } }
		}
	}
}

Task "Publish-Packages" -alias "publish" -description "This task deploys all deployment artifacts." `
-depends @("Create-Packages") -action {
	foreach ($nupkg in $ArtifactsDir)
	{
		if ([string]::IsNullOrEmpty($NuGetKey))
		{ Exec { & $nuget push $nupkg; } }
		else
		{ Exec { & $nuget push $nupkg -ApiKey $NuGetKey; } }
	}
}

function Get-VersionSuffix()
{
	$server = (Get-Content $SemVerJson | Out-String | ConvertFrom-Json);
	$suffix = $server.branchSuffixMap.$BranchName;
	if ([string]::IsNullOrEmpty($suffix)) { $suffix = $server.branchSuffixMap."*"; }
	if ([string]::IsNullOrEmpty($suffix)) { return ""; } else { return $suffix; }
}