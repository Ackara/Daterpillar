<#

.SYSNOPSIS
This script builds and publihes this project to a nuget feed.

.PARAMETER Tasks
The list of tasks to invoke.

.PARAMETER NugetClient
The download URL of the nuget client

.PARAMETER NugetSource
The address of the nuget feed in which packages will be publised to.

.PARAMETER NugetKey
The API key for the nuget feed.

#>

[CmdletBinding()]
Param(
	[Parameter(Position=4)]
	[string[]]$Tasks = @("default"),
	
	[Parameter(Position=2)]
	[string]$NugetKey = "",

	[Parameter(Position=1)]
	[string]$NugetSource = "https://api.nuget.org/v3/index.json",

	[Parameter(Position=3)]
	[string]$NugetClient = "https://dist.nuget.org/win-x86-commandline/v3.4.4/nuget.exe",

	[Parameter(Position=5)]
	[string]$Server = "",

	[Parameter(Position=6)]
	[string]$Username = "",

	[Parameter(Position=7)]
	[string]$Password = ""
)

$rootDirectory = (Split-Path $PSScriptRoot -Parent);
$toolsDirectory = "$rootDirectory\tools";
$nuget = "$toolsDirectory\nuget.exe";

# Download nuget client
if(-not (Test-Path $toolsDirectory -PathType Container)) { New-Item $toolsDirectory -ItemType Directory | Out-Null; }
if(-not (Test-Path $nuget -PathType Leaf))
{
	Write-Host "Downloading 'nuget.exe'...";
	Invoke-WebRequest $NugetClient -OutFile $nuget;
}

# Restore packages
$solution = Get-ChildItem "$rootDirectory\src" -Filter "*.sln" -Recurse | Select-Object -ExpandProperty FullName -First 1;
& $nuget restore $solution;

# Import Psake module
Remove-Module [p]sake;
$psakeModule = (Get-ChildItem "$rootDirectory\src\[Pp]ackages\psake*\tools\psake.psm1").FullName | Sort-Object $_ | Select-Object -Last 1;
Import-Module $psakeModule;

# Run psake tasks
Invoke-psake `
	-buildFile "$rootDirectory\build\tasks.ps1" `
	-taskList $Tasks `
	-framework 4.5.2 `
	-properties @{
		"Nuget"=$nuget;
		"NugetKey"=$NugetKey;
		"NugetSource"=$NugetSource;
		"Server"=$Server;
		"Username"=$Username;
		"Password"=$Password;
	};
	
if(-not $psake.build_success) { exit 1; }
