<#

.SYSNOPSIS
This script initiates this project deployment process.

.PARAMETER TaskList
A collection of tasks to invoke.

.PARAMETER NugetKey
The nuget.org API key.

.NOTES
This script depends on the psake module.

#>

Param(
    [Parameter(Position=1)]
    [string]$NugetKey,

    [Parameter()]
    [string[]]$TaskList = @("default")
)

Clear-Host;
Push-Location (Split-Path $PSScriptRoot -Parent);

# Restore nuget packages
$nuget = "$PWD\tools\nuget.exe";
if(-not (Test-Path $nuget -PathType Leaf))
{
    if(-not (Test-Path "$PWD\tools" -PathType Container)) { New-Item "$PWD\tools" -ItemType Directory | Out-Null; }
    Invoke-WebRequest -Uri "https://dist.nuget.org/win-x86-commandline/latest/nuget.exe" -OutFile $nuget;
}
$solution = Get-ChildItem -Recurse -Filter "*sln" | Select-Object -ExpandProperty FullName -First 1;
& $nuget restore $($solution);

# Import psake module
Remove-Module [p]sake;
$psake = (Get-ChildItem ".\src\packages\psake*\tools\psake.psm1").FullName | Sort-Object $_ | Select-Object -Last 1;
Import-Module $psake;

Invoke-psake -buildFile "$PWD\build\deploy.ps1" `
    -taskList $TaskList `
    -framework 4.5.2 `
    -properties @{
        "NugetEXE"=$nuget;
        "NugetKey"=$NugetKey.Trim()
    };

Pop-Location;