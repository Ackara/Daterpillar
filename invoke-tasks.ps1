<#
.SYNOPSIS
This script functions as a bootstrapper to other build and developer tasks.
#>

Param(
	[string[]]$Tasks = @("default"),

	[Alias("conn", "connections")]
	[hashtable]$ConnectionStrings = @{},

	[string]$NuGetKey,
	[string]$BranchName,
	[string[]]$TestCase,
	[string]$BuildConfiguration = "Release",

	[switch]$Major,
	[switch]$Minor,
	[switch]$Help
)

# Assign Variables
$BranchName = $env:BUILD_SOURCEBRANCHNAME;
if ([string]::IsNullOrEmpty($BranchName))
{
	$results = (& git branch);
	$regex = New-Object Regex('\*\s*(?<name>\w+)');
	if ($regex.IsMatch($results))
	{
		$BranchName = $regex.Match($results).Groups["name"].Value;
	}
}

if ($ConnectionStrings.Count -eq 0)
{
	$ConnectionStrings.Add("mysql", "server=localhost;user=your_username;password=your_password;");
	$ConnectionStrings.Add("mssql", "server=localhost;user=your_username;password=your_password;");
}

# Restore packages
$nuget = "$PSScriptRoot\tools\nuget.exe";
if (-not (Test-Path $nuget -PathType Leaf))
{
	if (-not (Test-Path "$PSScriptRoot\tools" -PathType Container)) { New-Item "$PSScriptRoot\tools" -ItemType Directory | Out-Null; }
	Invoke-WebRequest https://dist.nuget.org/win-x86-commandline/latest/nuget.exe -OutFile $nuget;
}

& $nuget restore "$PSScriptRoot\Daterpillar.sln" | Out-Null;

# Invoke Psake
Import-Module "$PSScriptRoot\packages\psake.4.6.0\tools\psake.psd1" -Force;
$taskFile = "$PSScriptRoot\build\tasks.ps1";

if ($Help)
{ Invoke-psake -buildFile $taskFile -detailedDocs; }
else
{
	Invoke-psake $taskFile -taskList $Tasks -nologo -notr -framework 4.6.1 `
		-properties @{
			"BuildConfiguration"=$BuildConfiguration;
			"ConnectionStrings"=$ConnectionStrings;
			"BranchName"=$BranchName;
			"TestCase"=$TestCase;
			"NuGetKey"=$NuGetKey;
			"Nuget"=$nuget;
			"Major"=$Major;
			"Minor"=$Minor;
		}
}