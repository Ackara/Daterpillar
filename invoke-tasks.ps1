<#
.SYNOPSIS

#>

Param(
	[string[]]$Tasks = @("setup"),

	[string]$NuGetKey,
	[string]$Branch,
	[string]$ReleaseTag,
	[string]$BuildConfiguration = "Release",

	[switch]$Major,
	[switch]$Minor,
	[switch]$Help
)

# Assign Variables

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
	Invoke-psake $taskFile -taskList $Tasks -nologo -notr `
		-properties @{
			"BuildConfiguration"=$BuildConfiguration;

			"Major"=$Major;
			"Minor"=$Minor;
		}
}