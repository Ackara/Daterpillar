<#
.SYNOPSIS
This script functions as a bootstrapper to other build and developer tasks.
#>

Param(
	[Parameter(Position=1)]
	[string[]]$Tasks = @("default"),
	
	[Parameter(Position=2)]
	[Alias("conn")]
	[hashtable]$ConnectionStrings = @{},

	[string]$NuGetKey,
	[string]$BranchName,
	[string]$BuildConfiguration = "Release",

	[switch]$Major,
	[switch]$Minor,
	[switch]$Help
)

Write-Host $ConnectionStrings;
Write-Host $ConnectionStrings["mysql"];
Write-Host "branch: $BranchName";




# Assign Variables
if ([string]::IsNullOrEmpty($BranchName))
{
	$results = (& git branch);
	$regex = New-Object Regex('\*\s*(?<name>\w+)');
	if ($regex.IsMatch($results))
	{
		$BranchName = $regex.Match($results).Groups["name"].Value;
	}
}

# Restore packages
$nuget = "$PSScriptRoot\tools\nuget.exe";
if (-not (Test-Path $nuget -PathType Leaf))
{
	if (-not (Test-Path "$PSScriptRoot\tools" -PathType Container)) { New-Item "$PSScriptRoot\tools" -ItemType Directory | Out-Null; }
	Invoke-WebRequest https://dist.nuget.org/win-x86-commandline/latest/nuget.exe -OutFile $nuget;
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
			"BranchName"=$BranchName;
			"NuGetKey"=$NuGetKey;
			"Nuget"=$nuget;
			"Major"=$Major;
			"Minor"=$Minor;
		}
}