<#

.SYNOPSIS
This scripts builds and publishes this solution to nuget.org.

.PARAMETER NugetURL
The download url of the 'nuget.exe' (CLI tool).

.PARAMETER Args
A list of arguments to pass to the psake 'tasks.ps1'.

.PARAMETER NugetApiKey
The API key of the nuget account in which this package will be published under.

.PARAMETER MySQLConnectionString
The connection string of MySQL server.

.PARAMETER MSSQLConnectionString
The connection string of MSSQL server.

#>

[CmdletBinding()]
Param(
	[string[]]$Tasks = @("default"),

	[string]$NugetURL = "https://acklann.pkgs.visualstudio.com/_apis/public/nuget/client/CredentialProviderBundle.zip",

	$Args = @{},

	[string]$NuGetAPIKey,

	[string]$MySQLConnectionString,

	[string]$MSSQLConnectionString
)

$rootDirectory = (Split-Path $PSScriptRoot -Parent);
$toolsDirectory = "$rootDirectory\tools";
$nuget = "$toolsDirectory\nuget.exe";

Write-Host "restoring nuget packages...";

if(-not (Test-Path $toolsDirectory -PathType Container)) { New-Item $toolsDirectory -ItemType Directory | Out-Null }
if(-not (Test-Path $nuget -PathType Leaf)) 
{ 
	Write-Host "`t* downloading '$(Split-Path $NugetURL -Leaf)'.";

	$bundleZIP = "$toolsDirectory\bundle.zip";
	Invoke-WebRequest $NugetURL -OutFile $bundleZIP;
	Write-Host "`t* done! saved file at '$bundleZIP'.";

	Add-Type -AssemblyName "System.IO.Compression.FileSystem";
	[IO.Compression.ZipFile]::ExtractToDirectory($bundleZIP, $toolsDirectory);
	Remove-Item $bundleZIP;
}

& $nuget restore "$rootDirectory\src\Daterpillar.sln";

# *** PSAKE *** #
# Importing psake module

Remove-Module [p]sake;
$psakeModule = (Get-ChildItem "$rootDirectory\src\[Pp]ackages\psake*\tools\psake.psm1").FullName | Sort-Object $_ | Select-Object -Last 1;
Import-Module $psakeModule;

# Run psake tasks

Invoke-psake `
	-buildFile "$rootDirectory\build\tasks.ps1" `
	-taskList $Tasks `
	-framework 4.5.2 `
	-properties $Args;

if(-not $psake.build_success) { exit 1; }
