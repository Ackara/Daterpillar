<#
.SYNOPSIS
Psake build tasks.
#>

Properties {
	$Config = (Get-Content "$PSScriptRoot\solution.json" | Out-String | ConvertFrom-Json);

	# Paths
	$RootDir = (Split-Path $PSScriptRoot -Parent);
	$ReleaseNotesTXT = "$RootDir\releaseNotes.txt";
	$SolutionJSON = "$PSScriptRoot\solution.json";
	$ArtifactsDir = "$RootDir\artifacts";
	$Nuget = "";

	# User Args
	$NuGetKey = "";
	$BranchName = "";
	$TestCase = "";
	$BuildConfiguration = "";
	$ConnectionStrings = @{};

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

    foreach ($psd1 in (Get-ChildItem "$RootDir\tools" -Recurse -Filter "*.psd1" | Select-Object -ExpandProperty FullName))
    {
        Import-Module $psd1;
        Write-Host "`t* imported $(Split-Path $psd1 -Leaf) module.";
    }

	$pester = "$RootDir\packages\Pester*\tools\Pester.psd1";
	if (Test-Path $pester -PathType Leaf)
	{
		Import-Module $pester -Force;
		Write-Host "`t* imported pester module.";
	}
}

Task "Setup" -description "This task will generate all missing/sensitive files missing from the project." `
-depends @("Add-AppConfigFiles");

Task "Increment-VersionNumber" -alias "version" -description "This task increments the patch version number within all neccessary files." `
-depends @("Init") -action {
	$commitMsg = Show-Inputbox "please enter your release notes" "RELEASE NOTES";
	Update-VersionNumber "$RootDir\src" -Message $commitMsg -UsecommitMessageAsDescription -ConfigFile $SolutionJSON -Major:$Major -Minor:$Minor -Patch;
	$version = Get-VersionNumber -ConfigFile $SolutionJSON;

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
				& git add build\solution.json;
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

    Push-Location $RootDir;
	Write-LineBreak "MSBUILD";
    try
    {
        $msbuild = Find-MSBuildPath;
	    $sln = Get-Item "$RootDir\*.sln" | Select-Object -ExpandProperty FullName;
        Exec { & dotnet restore; }
        Exec { & $msbuild $sln "/p:Configuration=$BuildConfiguration"; }
    }
    finally { Pop-Location; }
	Write-LineBreak;
}

Task "Run-Tests" -alias "test" -description "This task runs all automated tests." `
-depends @("Build-Solution") -action {
	Assert ("Debug", "Release" -contains $BuildConfiguration) "`$BuildConfiguration was '$BuildConfiguration' but expected 'Debug' or 'Release'.";

	foreach ($proj in (Get-ChildItem "$RootDir\tests" -Recurse -Filter "*.*proj" | Select-Object -ExpandProperty FullName))
	{
		if ([string]::IsNullOrEmpty($TestCase))
		{ Exec { & dotnet test $proj --configuration $BuildConfiguration --verbosity minimal; } }
		else
		{ Exec { & dotnet test $proj --configuration $BuildConfiguration --filter ClassName~$TestCase --verbosity minimal; } }
	}

	foreach ($script in (Get-ChildItem "$RootDir\tests" -Recurse -Filter "*test*.ps1" | Select-Object -ExpandProperty FullName))
	{
		if ([string]::IsNullOrEmpty($TestCase))
		{ Invoke-Pester -Script $script -ErrorAction Stop; }
		else
		{ Invoke-Pester -Script $script -ErrorAction Stop -TestName $TestCase; }
	}
}

Task "Create-Packages" -alias "pack" -description "This task creates all deployment artifacts." `
-depends @("Init") -action {
	if (Test-Path $ArtifactsDir -PathType Container) { Remove-Item $ArtifactsDir -Recurse -Force; }
	New-Item $ArtifactsDir -ItemType Directory | Out-Null;

	foreach ($proj in (Get-ChildItem "$RootDir\src" -Recurse -Filter "*.*proj" | Select-Object -ExpandProperty FullName))
	{
		$nuspec = [IO.Path]::ChangeExtension($proj, ".nuspec");
		if (Test-Path $nuspec -PathType Leaf)
		{
			$versionSuffix = Get-VersionSuffix;
			$version = Get-VersionNumber -ConfigFile $SolutionJSON;
            $csproj = Get-Content $proj | Out-String;

			$properties = "";
			$properties += "tags=$($Config.metadata.tags);";
			$properties += "owner=$($Config.metadata.owner);";
			$properties += "Configuration=$BuildConfiguration;";
			$properties += "iconUrl=$($Config.metadata.iconUrl);";
			$properties += "license=$($Config.metadata.licenseUrl);";
			$properties += "copyright=$($Config.metadata.copyright);";
			$properties += "projectUrl=$($Config.metadata.projectUrl);";
			$properties += "description=$($Config.metadata.description);";
			$properties += "releaseNotes=$(Get-Content $ReleaseNotesTXT | Out-String);";
			$properties += "version=$($version.Major).$($version.Minor).$($version.Patch);";
            $properties += "id=$([Regex]::Match($csproj, '(?i)<AssemblyName>(?<name>[a-z0-9.]+)</AssemblyName>').Groups["name"].Value);";

            $match = [Regex]::Match($csproj, '(?i)<TargetFramework>(?<name>[a-z0-9.]+)</TargetFramework>');
            if ($match.Success)
            { $properties += "targetFramework=$($match.Groups["name"].Value)"; }

            if (-not $match.Success)
            {
                $match = [Regex]::Match($csproj, '(?i)<TargetFrameworkVersion>(?<name>[a-z0-9.]+)</TargetFrameworkVersion>');
                $properties += "targetFramework=net$($match.Groups["name"].Value.Replace('.', '').Trim('v'))";
            }

			if ([String]::IsNullOrEmpty($VersionSuffix))
			{ Exec { & $nuget pack $nuspec -OutputDirectory $ArtifactsDir -IncludeReferencedProjects -Properties $properties; } }
			else
			{ Exec { & $nuget pack $nuspec -OutputDirectory $ArtifactsDir -IncludeReferencedProjects -Properties $properties -Suffix $versionSuffix; } }
		}
	}
}

Task "Publish-Packages" -alias "publish" -description "This task deploys all deployment artifacts." `
-depends @("Create-Packages") -action {
	foreach ($nupkg in (Get-ChildItem $ArtifactsDir -Recurse -Filter "*.nupkg" | Select-Object -ExpandProperty FullName))
	{
		if ([string]::IsNullOrEmpty($NuGetKey))
		{ Exec { & $nuget push $nupkg -Source "https://api.nuget.org/v3/index.json"; } }
		else
		{ Exec { & $nuget push $nupkg -Source "https://api.nuget.org/v3/index.json" -ApiKey $NuGetKey; } }
	}
}

Task "Add-AppConfigFiles" -description "This task creates all missing app.config files within the solution." -action {
	$mstestProjectConfig = "$RootDir\tests\MSTest.Daterpillar\app.config";
	if (-not (Test-Path $mstestProjectConfig -PathType Leaf))
	{
		$mysqlConnStr = $ConnectionStrings["mysql"];
		$mssqlConnStr = $ConnectionStrings["mssql"];

@"
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <configSections />
  <connectionStrings>
	<add name="mssql"  connectionString="$mssqlConnStr" />
	<add name="mysql"  connectionString="$mysqlConnStr" />
  </connectionStrings>
</configuration>
"@ | Out-File $mstestProjectConfig -Encoding utf8;
	}
}

function Get-VersionSuffix()
{
	$server = (Get-Content $SolutionJSON | Out-String | ConvertFrom-Json);
	$suffix = $server.semanticVersion.branchSuffixMap.$BranchName;
	if ([string]::IsNullOrEmpty($suffix)) { $suffix = $server.semanticVersion.branchSuffixMap."*"; }
	if ([string]::IsNullOrEmpty($suffix)) { return ""; } else { return $suffix; }
}