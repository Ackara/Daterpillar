<#
.SYNOPSIS
Psake build tasks.
#>

Properties {
	$ManifestPath = "$PSScriptRoot\manifest.json";
	$Manifest = (Get-Content $ManifestPath | Out-String | ConvertFrom-Json);

	# Paths
	$RootDir = (Split-Path $PSScriptRoot -Parent);
	$ReleaseNotesTXT = "$RootDir\releaseNotes.txt";
	$ArtifactsDir = "$RootDir\artifacts";
	$Nuget = "";

	# User Args
	$NuGetKey = "";
	$BranchName = "";
	$TestCases = @();
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

	foreach ($psd1 in (Get-ChildItem "$RootDir\tools" -Recurse -Filter "*.ps*1" | Select-Object -ExpandProperty FullName))
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

	foreach ($folder in @($ArtifactsDir))
	{
		if (Test-Path $folder -PathType Container) { Remove-Item $folder -Recurse; }
		New-Item $folder -ItemType Directory | Out-Null;
	}
}


Task "Setup" -description "This task will generate all missing/sensitive files missing from the project." `
-depends @("Add-AppConfigFiles");


Task "Increment-VersionNumber" -alias "version" -description "This task increments the patch version number within all neccessary files." `
-depends @("Init") -action {
	$commitMsg = Show-Inputbox "please enter your release notes" "RELEASE NOTES";
	Update-VersionNumber "$RootDir\src" -Message $commitMsg -UsecommitMessageAsDescription -ConfigFile $ManifestPath -Major:$Major -Minor:$Minor -Patch;
	$version = Get-VersionNumber -ConfigFile $ManifestPath;

	if (-not [string]::IsNullOrEmpty($commitMsg))
	{
		try
		{
			Push-Location $RootDir;
			$ver = "version $($version.Major).$($version.Minor).$($version.Patch)";
			$notes = "$ver`n";
			$notes += "$([System.Linq.Enumerable]::Repeat('-', $ver.Length))`n`n";
			$notes += $commitMsg;
			$contents = Get-Content $ReleaseNotesTXT | Out-String;
			"$notes`n`n$contents".Trim() | Out-File $ReleaseNotesTXT -Encoding ascii;
			Exec {
				& git add releaseNotes.txt;
				& git add build\manifest.json;
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


Task "Run-PowershellTests" -alias "pester" -description "This task runs all powershell test scripts." `
-depends @() -action {
	foreach ($script in (Get-ChildItem "$RootDir\tests" -Recurse -Filter "*tests.ps1"))
	{
		if ($TestCases.Length -gt 0)
		{
			foreach ($testName in $TestCases)
			{
				if ($script.Name -match $testName) { Invoke-Pester -Script $script.FullName; }
			}
		}
		else { Invoke-Pester -Script $script.FullName; }
	}
}


Task "Create-Packages" -alias "pack" -description "This task creates all deployment artifacts." `
-depends @("Init") -action {
	$version = Get-VersionNumber -ConfigFile $ManifestPath;
	$suffix = Get-BranchSuffix $BranchName;
	if (-not [String]::IsNullOrEmpty($suffix)) { $suffix = "-$suffix"; }
	
	$properties = "";
	$properties += "PackageVersion=$($version.ToString($true))$suffix;";
	$properties += "PackageTags=$($Manifest.metadata.tags);";
	$properties += "Authors=$($Manifest.metadata.owner);";
	$properties += "Configuration=$BuildConfiguration;";
	$properties += "PackageIconUrl=$($Manifest.metadata.iconUrl);";
	$properties += "PackageLicenseUrl=$($Manifest.metadata.licenseUrl);";
	$properties += "Copyright=$($Manifest.metadata.copyright);";
	$properties += "PackageProjectUrl=$($Manifest.metadata.projectUrl);";
	$properties += "RepositoryUrl=$($Manifest.metadata.repositoryUrl);";
	$properties += "Description=$($Manifest.metadata.description);";
	$properties += "PackageRequireLicenseAcceptance=true;";
	$properties += "PackageReleaseNotes=$(Get-Content $ReleaseNotesTXT | Out-String);";

	foreach ($proj in (Get-ChildItem "$RootDir\src" -Recurse -Filter "*.csproj" | Select-Object -ExpandProperty FullName))
	{
		$properties += "Title=$([IO.Path]::GetFileNameWithoutExtension($proj));";
		$contents = Get-Content $proj | Out-String;

		if ([Regex]::IsMatch($contents, '(?i)<TargetFramework>netstandard[0-9.]+</TargetFramework>'))
		{
			try
			{
				Split-Path $proj -Parent | Push-Location;
				$properties = $properties.TrimEnd(';');
				$msbuild = Find-MSBuildPath;
				Exec { & $msbuild /t:pack /p:$properties; }
				Get-ChildItem $PWD -Recurse -Filter "*.nupkg" | Move-Item -Destination $ArtifactsDir;
			}
			finally { Pop-Location; }
		}
		else
		{
			
			try
			{
				Split-Path $proj -Parent | Push-Location;
				$properties = $properties.TrimEnd(';');
				Exec { & $nuget pack $proj -OutputDirectory $ArtifactsDir -Properties $properties -IncludeReferencedProjects; }
			}
			finally { Pop-Location; }
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
	<add name="mssql" connectionString="$mssqlConnStr" />
	<add name="mysql" connectionString="$mysqlConnStr" />
  </connectionStrings>
</configuration>
"@ | Out-File $mstestProjectConfig -Encoding utf8;
	}
}
