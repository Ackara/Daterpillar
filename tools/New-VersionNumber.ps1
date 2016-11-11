<#

.SYNOPSIS
This script generates a new version number when invoked.

#>

function GetRevisionNumber()
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

$major = 3;
$minor = 1;
$build = [Convert]::ToInt32([DateTime]::UtcNow.ToString("yyMMdd").SubString(1));
$revision = GetRevisionNumber;
return "$major.$minor.$build.$revision";
