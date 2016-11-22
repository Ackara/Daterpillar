<#

.SYNOPSIS
This module provides functions for incrementing and retrieveing the current version number of projects within the solution.

#>

$Script:Major = 3;
$Script:Minor = 0;
$Script:regex = New-Object Regex('(?i)\[assembly:\s*assembly(\w+)?version\s*\("(?<version>(\d+\.){1,2}(\*|\d+)?(\.(?<rev>\d+))?)"\s*\)\s*\]');

function New-RevisionNumber()
{
<#

.SYNOPSIS
This script generates a new revision number when invoked.

.INPUTS
None

.OUTPUTS
None

#>
	[string]$version = Get-VersionNumber;
	$rev = $version.Split('.')[3];
	return ([Convert]::ToInt16($rev) + 1);
}

function New-VersionNumber()
{
<#

.SYNOPSIS
This script generates a new version number when invoked.

.INPUTS
None

.OUTPUTS
None

#>

	$build = [Convert]::ToUInt16([DateTime]::UtcNow.DayOfYear);
	$revision = New-RevisionNumber;
	return "$Script:Major.$Script:Minor.$build.$revision";
}

function Get-VersionNumber()
{
	$rootDirectory = (Split-Path $PSScriptRoot -Parent);
	$project = (Get-ChildItem "$rootDirectory\src" -Filter "*.csproj" -Recurse | Select-Object -ExpandProperty FullName -First 1);
	$projectDir = (Split-Path $project -Parent);
	$assemblyInfo = "$projectDir\Properties\AssemblyInfo.cs";
	
	if(Test-Path $assemblyInfo -PathType Leaf)
	{
		[string]$content = [IO.File]::ReadAllText($assemblyInfo);
		$match = $Script:regex.Match($content);
		return $match.Groups["version"].Value;
	}
}

function Update-VersionNumbers()
{
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

	$version = New-VersionNumber;
	
	$rootDirectory = (Split-Path $PSScriptRoot -Parent);
	foreach($project in (Get-ChildItem "$rootDirectory\src" -Filter "*.csproj" -Recurse | Select-Object -ExpandProperty FullName))
	{
		$projectDir = (Split-Path $project -Parent);
		$assemblyInfo = "$projectDir\Properties\AssemblyInfo.cs";
		$regex = New-Object Regex('(?i)\[assembly:\s*assembly(\w+)?version\s*\("(?<version>(\d+\.){1,2}(\*|\d+)?(\.\d+)?)"\s*\)\s*\]');
	
		if(Test-Path $assemblyInfo -PathType Leaf)
		{
			[string]$content = [IO.File]::ReadAllText($assemblyInfo);
			$matches = $regex.Matches($content);
	
			for($i = 0; $i -lt $matches.Count; $i++)
			{
				$oldVersion = $matches[$i].Groups["version"];
				
				$content = $content.Remove($oldVersion.Index, $oldVersion.Length);
				$content = $content.Insert($oldVersion.Index, $version);
				Out-File $assemblyInfo -InputObject $content;
				$matches = $Script:regex.Matches($content);
			}
	
			& git add $assemblyInfo;
			Write-Host "Updated '$(Split-Path $project -Leaf)' version number to $version." -ForegroundColor Green;
		}
	}
	
	& git commit --message "Update the solution version numbers to $version" | Out-Null;
}
