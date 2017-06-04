<#
.SYNOPSIS
This script functions as a bootstrapper to other build and developer tasks.
#>

Param(
	[Parameter(Position = 0)]
	[string[]]$Tasks = @("default"),

	[Alias("conn", "connections")]
	[hashtable]$ConnectionStrings = @{},

	[Alias("con", "bc")]
	[string]$BuildConfiguration = "Release",

	[Parameter(Position = 1)]
	[Alias("tn", "tc", "tests")]
	[string[]]$TestCases,

	[string]$NuGetKey,
	[string]$BranchName,
	[string]$NuGetVersion = "4.1.0",

	[switch]$Major,
	[switch]$Minor,
	[switch]$Help
)

# Assign Variables
$BranchName = $env:BUILD_SOURCEBRANCHNAME;
if ([string]::IsNullOrEmpty($BranchName))
{
	$results = (& git branch);
	$regex = New-Object Regex('\*\s*(?<name>\w+)');
	if ($regex.IsMatch($results))
	{
		$BranchName = $regex.Match($results).Groups["name"].Value;
	}
}
Write-Host "Branch: '$BranchName'";

if ($ConnectionStrings.Count -eq 0)
{
	$ConnectionStrings.Add("mysql", "server=localhost;user=your_username;password=your_password;");
	$ConnectionStrings.Add("mssql", "server=localhost;user=your_username;password=your_password;");
}

# Restore packages
$nuget = "$PSScriptRoot\tools\nuget.exe";
if (-not (Test-Path $nuget -PathType Leaf))
{
	if (-not (Test-Path "$PSScriptRoot\tools" -PathType Container)) { New-Item "$PSScriptRoot\tools" -ItemType Directory | Out-Null; }
	Invoke-WebRequest https://dist.nuget.org/win-x86-commandline/v$NuGetVersion/nuget.exe -OutFile $nuget;
}

& $nuget restore "$PSScriptRoot\Daterpillar.sln" | Out-Null;

# Invoke Psake
Import-Module "$PSScriptRoot\packages\psake.4.6.0\tools\psake.psd1" -Force;
$taskFile = "$PSScriptRoot\build\tasks.ps1";

if ($Help)
{ Invoke-psake -buildFile $taskFile -detailedDocs; }
else
{
	Invoke-psake $taskFile -taskList $Tasks -nologo -notr `
		-properties @{
			"BuildConfiguration"=$BuildConfiguration;
			"ConnectionStrings"=$ConnectionStrings;
			"BranchName"=$BranchName;
			"TestCases"=$TestCases;
			"NuGetKey"=$NuGetKey;
			"Nuget"=$nuget;
			"Major"=$Major;
			"Minor"=$Minor;
		}
	if(-not $psake.build_success) { exit 1; }
}