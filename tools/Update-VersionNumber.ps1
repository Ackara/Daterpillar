<#

.SYNOPSIS
This script increments the version number for all projects within the solution.

.DESCRIPTION
This script updates the version number for all projects within the solution.
In addition commit the update to source control, with that said GIT is required.

.INPUTS
None

.OUTPUTS
None

Example
.\Update-VersionNumber.ps1;
This example increments the solution version number.

#>

$version = .\New-VersionNumber.ps1;

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
			$matches = $regex.Matches($content);
		}

		& git add $assemblyInfo;
		Write-Host "Updated '$(Split-Path $project -Leaf)' version number to $version." -ForegroundColor Green;
	}
}

& git commit --message "Update version numbers to $version" | Out-Null;
