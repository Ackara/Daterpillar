<#

.SYNOPSIS
This script initiates the project's build and deployment process.

.PARAMETER TaskList
A collection of psake tasks to invoke.

.NOTES
This script depends on the psake module.

#>

Param(
    [Parameter(Position = 1)]
    [string[]]$TaskList = @("default"),

    [Parameter()]
    [string]$NuGetUri = "https://dist.nuget.org/win-x86-commandline/v3.4.3/nuget.exe",
    
    [Parameter()]
    [string]$NuGetAPIKey = "",

    [Parameter()]
    [string]$FtpUser = "",

    [Parameter()]
    [string]$FtpPassword = ""
)

Clear-Host;
Push-Location (Split-Path $PSScriptRoot -Parent);

# Restore NuGet packages.
    $nuget = "$PWD\tools\nuget.exe";
    if(-not (Test-Path $nuget -PathType Leaf))
    {
        $toolsDir = "$PWD\tools";
        if(-not (Test-Path $toolsDir -PathType Container)) { New-Item  $toolsDir -ItemType Directory | Out-Null; }
        Invoke-WebRequest -Uri $NuGetUri -OutFile $nuget;
    }
    & $nuget restore $(Get-ChildItem -Filter "*.sln" -Recurse | Select-Object -ExpandProperty FullName -First 1);

# Import psake module
    Remove-Module [p]sake;
    $psakeModule = (Get-ChildItem "$PWD\src\[Pp]ackages\psake*\tools\psake.psm1").FullName | Sort-Object $_ | Select-Object -Last 1;
    Import-Module $psakeModule;

# Start deployment
Invoke-psake `
    -buildFile "$PWD\build\build.ps1" `
    -taskList $TaskList `
    -framework 4.5.2 `
    -properties @{
        "NugetEXE" = $nuget;
        "NuGetKey" = $NuGetAPIKey;

        "Username" = $FtpUser;
        "Password" = $FtpPassword;
    };

Pop-Location;
if(-not $psake.build_success) { exit 1; }