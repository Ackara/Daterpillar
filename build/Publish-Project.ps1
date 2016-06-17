<#

.SYSNOPSIS
This script initiates this project deployment process.

.PARAMETER Tasks
A collection of tasks to invoke.

.NOTES
This script depends on the psake module.

#>

Param(
    [Parameter()]
    [string[]]$TaskList = @()
)

Push-Location (Split-Path $PSScriptRoot -Parent);

# Restore nuget packages
$nuget = "$PWD\tools\nuget.exe";
if(-not (Test-Path $nuget -PathType Leaf))
{
    Invoke-WebRequest -Uri "" -OutFile $nuget;
}
$solution = Get-ChildItem -Recurse -Filter "*sln" | Select-Object -First 1;
& $nuget restore $($solution);

# Import psake module
Remove-Module [p]sake;
$psake = (Get-ChildItem "..\src\packages\psake*\tools\psake.psm1").FullName | Sort-Object $_ | Select-Object -Last 1;
Import-Module $psake;

if($TaskList.Count -le 0)
{
    Invoke-psake -buildFile .\deploy.ps1;
}
else
{
    Invoke-psake -buildFile .\deploy.ps1 -taskList $TaskList;
}