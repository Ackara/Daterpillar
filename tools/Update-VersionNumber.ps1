<#

.SYNOPSIS
This script increments the version number for all projects within the solution.

.DESCRIPTION
This script updates the version number for all projects within the solution.
In addition, it will commit the update to source control; with that said GIT is required.

.INPUTS
None

.OUTPUTS
None

#>

$versioningModule = "$PSScriptRoot\versioning.psm1";
if(!(Get-Module "versioning")) { Import-Module $versioningModule; } else { Write-Host "The versioning module is already imported."; }
Update-VersionNumbers;
