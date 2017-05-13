function Get-SolutionDirectory()
{
	$path = $PSScriptRoot;
	for ($i = 0; $i -lt 2; $i++)
	{
		$path = Split-Path $path -Parent;
	}

	return $path;
}

function Get-MSTestSamplesDirectory()
{
	$path = Split-Path $PSScriptRoot -Parent;
	$path = Get-Item "$path\mstes*\Samples" | Select-Object -ExpandProperty FullName;
	return $path;
}

function Install-TestEnviroment([string]$testName = "")
{
	# Create and assign paths
	$solutionDir = Get-SolutionDirectory;
	$testResultsDir = "$solutionDir\TestResults\pester-$testName-$((Get-Date).Ticks)";
	if (-not (Test-Path $testResultsDir -PathType Container)) { New-Item $testResultsDir -ItemType Directory | Out-Null; }

	# Build project
	$buildboxModule = Get-ChildItem "$solutionDir\packages\Ackara.Buildbox.*" -Recurse -Filter "*Buildbox.Utils.psm1" | Sort-Object $_.Name | Select-Object -ExpandProperty FullName -Last 1;
	Import-Module $buildboxModule -Force;

	$msbuild = Find-MSBuildPath;
	$automationProj = Get-ChildItem "$solutionDir\src" -Recurse -Filter "*Automation.csproj" | Select-Object -ExpandProperty FullName -First 1;
	Write-Host (& $msbuild $automationProj /v:minimal /p:OutDir=$testResultsDir);
	if (Get-Module Buildbox.Utils) { Remove-Module Buildbox.Utils; }
	
	return $testResultsDir;
}

function Uninstall-TestEnviroment([string]$sessionDir)
{
}