function New-TestEnvironment
{
	[string]$modulePath = $null, $moduleFolder = $null, $sampleFolder = $null;

	try
	{
		$moduleFolder = Join-Path ([System.IO.Path]::GetTempPath()) "daterpillar-pester";
		if (-not (Test-Path $moduleFolder)) { New-Item $moduleFolder -ItemType Directory | Out-Null; }

		Push-Location $PSScriptRoot;
		[string]$projectFile = "../../src/*.Powershell/*.*proj" | Resolve-Path;
		&dotnet publish $projectFile --configuration "Release" --output $moduleFolder;
		Join-Path (Split-Path $projectFile -Parent) "bin/Release/*/*.dll-Help.xml" | Get-Item | Copy-Item -Destination $moduleFolder -Force;
		$modulePath = Get-ChildItem $moduleFolder -Filter "*.psd1" | Select-Object -ExpandProperty FullName -First 1;

		$sampleFolder = Join-Path $moduleFolder "sample-data";
		if (-not (Test-Path $sampleFolder)) { New-Item $sampleFolder -ItemType Directory | Out-Null; }
		"../../tests/*.MSTest/*-data/" | Resolve-Path | Get-ChildItem -Recurse | Copy-Item -Destination $sampleFolder -Force;
	}
	finally { Pop-Location; }

	return [PSCustomObject]@{
		"ModulePath"=$modulePath;
		"ModuleFolder"=$moduleFolder;
		"SampleFolder"=$sampleFolder;
	};
}

function New-TestFolder
{
	Param(
		$Context,
		[Parameter(Mandatory)][string]$Name
	)

	$folder = Join-Path $Context.ModuleFolder $Name;
	if (Test-Path $folder) { Remove-Item $folder -Recurse -Force; }
	New-Item $folder -ItemType Directory | Out-Null;

	return $folder;
}

function Get-SampleFile
{
	Param(
		[Parameter(Mandatory)]
		$Context,
		[Parameter(Mandatory)]
		[string]$Name
	)

	return Get-ChildItem $Context.SampleFolder -File -Filter "*$Name*" | Select-Object -First 1;
}