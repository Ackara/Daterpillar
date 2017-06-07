<#
.SYNOPSIS
Psake build tasks.
#>

Properties {
	# Paths & Tools
	$Nuget = "";
	$ProjectRoot = "";
	$ManifestPath = "$PSScriptRoot\manifest.json";
	$Manifest = Get-Content $ManifestPath | Out-String | ConvertFrom-Json;

	# User Args
	$TestCase = "";
	$BranchName = "";
	$BuildConfiguration = "";
	$SkipMSBuild = $false;
}

Task "default" -description "This task compiles, test and publish the project to nuget.org and powershell gallery." `
-depends @() -precondition {
}

Task "test" -description "This task runs all tests." -depends @("pester", "vstest");

#----------

Task "init" -description "This task imports all dependencies." -action {
	foreach ($name in @("vssetup", "Buildbox.SemVer", "Buildbox.Utils"))
	{
		$path = "$ProjectRoot\tools\$name\*\*.psd1";
		$module = Get-Item $path -ErrorAction SilentlyContinue;
		if (-not $module) { Save-Module $name -Path "$ProjectRoot\tools"; }
		Get-Item $path | Import-Module -Force;
		if (Get-Module $name) { Write-Host "`t* imported $name module"; }
	}

	foreach ($psd1 in (Get-ChildItem "$ProjectRoot\packages\*\tools\*.psd1" -Exclude @("*psake*")))
	{
		Import-Module $psd1.FullName -Force;
		if (Get-Module ([IO.Path]::GetFileNameWithoutExtension($psd1.Name))) { Write-Host "`t* imported $($psd1.Name) module"; }
	}
	
	foreach ($psm1 in (Get-ChildItem "$PSScriptRoot\*.psm1"))
	{
		Import-Module $psm1.FullName -Force;
		if (Get-Module ([IO.Path]::GetFileNameWithoutExtension($psm1.Name))) { Write-Host "`t* imported $($psm1.Name) module"; }
	}
}

Task "compile" -description "This task builds the solution using msbuild." `
-depends @("init") -precondition { return (-not $SkipMSBuild) } -action {
	Assert ("Debug", "Release" -contains $BuildConfiguration) "`$BuildConfiguration was '$BuildConfiguration' but expected 'Debug' or 'Release'.";

	Write-LineBreak "MSBUILD";
	$msbuild = Get-MSBuildPath;
	$sln = (Get-item "$ProjectRoot\*.sln").FullName;
	Exec { & $msbuild $sln "/p:Configuration=$BuildConfiguration" "/verbosity:minimal"; }
	Write-LineBreak;
}

Task "pester" -description "This task runs all specified pester tests." `
-depends @("init", "compile") -action {
	$results = "";
	if ([String]::IsNullOrEmpty($TestCase))
	{
		$results = Invoke-Pester -Script "$ProjectRoot\tests\Pester*\*test*.ps1" -PassThru;
	}
	else
	{
		$results = Invoke-Pester -Script "$ProjectRoot\tests\Pester*\$($TestCase)*test*.ps1" -PassThru;
	}

	Assert ($results.FailedCount -eq 0) "'$($results.FailedCount)' pester tests failed.";
}

Task "vstest" -description "This task runs all visual studio tests." `
-depends @("init") -action {
	Write-LineBreak "MSTest";
	foreach ($csproj in (Get-ChildItem "$ProjectRoot\tests" -Recurse -Filter "*.csproj" | Select-Object -ExpandProperty FullName))
	{
		Exec { & dotnet test $csproj --configuration $BuildConfiguration --verbosity minimal; }
	}
	Write-LineBreak;
}

Task "pack" -description "This task packages the project to be published to all online repositories." `
-depends @("init", "compile") -action {
	$msbuild = Get-MSBuildPath;
	$artifactsDir = "$ProjectRoot\artifacts";
	if (Test-Path $artifactsDir -PathType Container) { Remove-Item $artifactsDir -Recurse -Force; }
	New-Item $artifactsDir -ItemType Directory | Out-Null;
	
	$releaseNotes = Get-Content "$ProjectRoot\releaseNotes.txt" | Out-String;
	$version = (Get-VersionNumber -config $ManifestPath).ToString($true);
	$suffix = Get-BranchSuffix $BranchName -config $ManifestPath;
	$suffix = (& { if ([String]::IsNullOrEmpty($suffix)) { return ""; } else { return "-$suffix"; } })
	
	$metadata += "PackageVersion=$version$($suffix);";
	$metadata += "packageOutputPath=$artifactsDir;";
	$metadata += "authors=$($Manifest.metadata.author);";
	$metadata += "copyright=$($Manifest.metadata.copyright);";
	$metadata += "packageTags=$($Manifest.metadata.tags);";
	$metadata += "packageProjectUrl=$($Manifest.metadata.projectUrl);";
	$metadata += "packageIconUrl=$($Manifest.metadata.iconUrl);";
	$metadata += "packageLicenseUrl=$($Manifest.metadata.licenseUrl);";
	$metadata += "packageRequireLicenseAcceptance=$true;";
	$metadata += "packageReleaseNotes=$releaseNotes;";
	$metadata += "configuration=$BuildConfiguration;";
	
	foreach ($proj in (Get-ChildItem "$ProjectRoot\src" -Recurse -Filter "*.csproj"))
	{
		$contents = Get-Content $proj.FullName | Out-String;
		$properties = "title=$([IO.Path]::GetFileNameWithoutExtension($proj.Name));";
		$description = Get-Content "$($proj.DirectoryName)\readme.txt" | Out-String;
		$properties += "description=$description;";
		$properties += $metadata.Trim(';');
		
		Push-Location $proj.DirectoryName;
		try
		{
			if ([Regex]::IsMatch($contents, '(?i)<TargetFramework>netstandard[0-9.]+</TargetFramework>'))
			{
				Write-LineBreak "MSBUILD";
				Exec { & $msbuild "/t:pack" "/p:$properties" "/verbosity:minimal"; }
			}
			else
			{
				Write-LineBreak "NUGET";
				Exec { & $nuget pack $proj.FullName -OutputDirectory $artifactsDir -Properties $properties -IncludeReferencedProjects; }
			}
		}
		finally { Pop-Location; }
	}
	Write-LineBreak;

	$moduleDir = "$artifactsDir\Daterpillar.Automation";
	New-Item $moduleDir -ItemType Directory | Out-Null;
	Get-ChildItem "$ProjectRoot\src\Daterpillar.Automation\bin\$BuildConfiguation\*\*" | Copy-Item -Destination "$moduleDir" -Recurse;
	Get-ChildItem $moduleDir -Exclude @("*.dll", "*.psd1", "x*") | Remove-Item;
}

Task "publish" -description "This task publishes all nuget packages and modules." `
-depends @("pack") -action {
}

#----------

Task "version" -description "This task increments the project's version numbers." `
-depends @("init") -action {

}

#region ----- HELPER FUNCTIONS -----

function Get-MSBuildPath()
{
	$vsPath = Get-VSSetupInstance | Select-VSSetupInstance -Latest | Select-Object -ExpandProperty InstallationPath;
	$msbuild = Get-Item "$vsPath\MSBuild\*\Bin\MSBuild.exe";
	Assert ($msbuild) "unable to find msbuild.exe on this machine.";
	return $msbuild.FullName;
}

function Get-PackageSuffix()
{
	$suffix = Get-BranchSuffix $BranchName -config $ManifestPath;
	if ([String]::IsNullOrEmpty($suffix)) { return ""; } else { return "-$suffix"; }
}

#endregion
