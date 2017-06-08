<#
.SYNOPSIS
This script functions as a bootstrapper to other build and developer tasks.
#>

Param(
	[Parameter(Position=1)]
	[string[]]$Tasks = @("default"),

	[Alias("test")]
	[Parameter(Position=2)]
	[string]$TestCase,

	[Alias("conn", "connStr")]
	[hashtable]$ConnectionStrings = @{},

	[Alias("build", "config")]
	[string]$BuildConfiguration = "Release",

	[Alias("nKey")]
	[string]$NuGetKey,

	[Alias("psKey")]
	[string]$PSGalleryKey,

	[switch]$SkipCompilation,
	[switch]$Major,
	[switch]$Minor,
	[switch]$Help
)

# Assign Variables
$branchName = $env:BUILD_SOURCEBRANCHNAME;
if ([string]::IsNullOrEmpty($branchName))
{
	$results = (& git branch);
	$regex = New-Object Regex('\*\s*(?<name>\w+)');
	if ($regex.IsMatch($results))
	{
		$branchName = $regex.Match($results).Groups["name"].Value;
	}
}
Write-Host "current branch: '$BranchName'";

# Restore packages
$nuget = "$PSScriptRoot\tools\nuget.exe";
if (-not (Test-Path $nuget -PathType Leaf))
{
	if (-not (Test-Path "$PSScriptRoot\tools" -PathType Container)) { New-Item "$PSScriptRoot\tools" -ItemType Directory | Out-Null; }
	Invoke-WebRequest "https://dist.nuget.org/win-x86-commandline/latest/nuget.exe" -OutFile $nuget;
}
& $nuget restore $(Get-Item "$PSScriptRoot\*.sln" | Select-Object -ExpandProperty FullName) -Verbosity quiet;

# Invoke Psake
Get-Item "$PSScriptRoot\packages\psake*\tools\*.psd1" | Import-Module -Force;
$buildFile = "$PSScriptRoot\build\tasks.ps1";

if ($Help) { Invoke-psake -buildFile $buildFile -detailedDocs; }
else
{
	Invoke-psake $buildFile -taskList $Tasks -nologo -notr `
		-properties @{
			"BuildConfiguration"=$BuildConfiguration;
			"ConnectionStrings"=$ConnectionStrings;
			"ProjectRoot"=$PSScriptRoot;
			"BranchName"=$branchName;
			"TestCase"=$TestCase;
			"NuGetKey"=$NuGetKey;
			"PSGalleryKey"=$PSGalleryKey;
			"SkipMSBuild"=$SkipCompilation;
			"Nuget"=$nuget;
			"Major"=$Major.IsPresent;
			"Minor"=$Minor.IsPresent;
		}

	if (-not $psake.build_success) { exit 1; }
}
