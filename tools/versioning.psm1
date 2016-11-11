$Script:versionRegex = New-Object Regex('(?i)\[assembly:\s*assembly(\w+)?version\s*\("(?<version>(\d+\.){1,2}(\*|\d+)?(\.\d+)?)"\s*\)\s*\]');

function Update-VersionNumber()
{
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
    			$matches = $regex.Matches($content);
    		}
    
    		& git add $assemblyInfo;
    		Write-Host "Updated '$(Split-Path $project -Leaf)' version number to $version." -ForegroundColor Green;
    	}
    }
    
    & git commit --message "Update version numbers to $version" | Out-Null;
}

function Get-VersionNumber()
{
    $rootDirectory = (Split-Path $PSScriptRoot -Parent);
    $project = (Get-ChildItem "$rootDirectory\src" -Filter "*.csproj" -Recurse | Select-Object -ExpandProperty FullName -First 1)
    $projectDir = (Split-Path $project -Parent);
    $assemblyInfo = "$projectDir\Properties\AssemblyInfo.cs";
    $regex = New-Object Regex('(?i)\[assembly:\s*assembly(\w+)?version\s*\("(?<version>(\d+\.){1,2}(\*|\d+)?(\.\d+)?)"\s*\)\s*\]');
    
    if(Test-Path $assemblyInfo -PathType Leaf)
    {
    	[string]$content = [IO.File]::ReadAllText($assemblyInfo);
    	$match = $regex.Match($content);
        return $match.Groups["version"].Value;
    }
}

function New-VersionNumber()
{
<#

.SYNOPSIS
This script generates a new version number when invoked.

#>

    $major = 3;
    $minor = 1;
    $build = [Convert]::ToInt32([DateTime]::UtcNow.ToString("yyMMdd").SubString(1));
    $revision = New-RevisionNumber;
    return "$major.$minor.$build.$revision";
}

function New-RevisionNumber()
{
	$datePart = [DateTime]::UtcNow.ToString("yyMMdd");
	$revisionFile = [String]::Concat($env:TEMP, "\revision_", $datePart, "_.tmp");

	if(Test-Path $revisionFile -PathType Leaf)
	{
		$content = Get-Content $revisionFile;
		$number = ([Convert]::ToInt32($content.Trim()) + 1);
		Out-File $revisionFile -InputObject $number;
		return $number;
	}
	else
	{
		New-Item $revisionFile -Value "1" | Out-Null;
		return 1;
	}
}
