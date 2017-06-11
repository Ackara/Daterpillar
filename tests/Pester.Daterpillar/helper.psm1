function New-TestEnviroment([string]$buldConfig, [string]$name)
{
	$projectRoot = Get-ProjectRoot;
	#$testDir = "$projectRoot\TestResults\$name-$((Get-Date).ToString('yyMdh_mmss'))";
	$testDir = "$projectRoot\TestResults\pester-$((Get-Date).ToString('yyMdh_mmss'))";
	$in = "$testDir\bin";
	$out = "$testDir\out";
	$temp = "$testDir\in";
	$samples = Get-Item "$projectRoot\tests\MSTest*\Samples";
	
	if (-not (Test-Path $testDir -PathType Container))
	{
		foreach ($folder in @($in, $out, $temp))
		{
			if (-not (Test-Path $folder -PathType Container)) { New-Item $folder -ItemType Directory | Out-Null; }
		}

		Get-ChildItem "$projectRoot\src\*Automation\bin\$buldConfig" -Recurse | Where-Object { $_.Name -notcontains "sqlite.interop.dll" } | Copy-Item -Destination $in;
		Get-Item "$projectRoot\src\*Automation\bin\$buldConfig\x86" | Copy-Item -Recurse -Destination $in -ErrorAction SilentlyContinue;
		Get-Item "$projectRoot\src\*Automation\bin\$buldConfig\x64" | Copy-Item -Recurse -Destination $in -ErrorAction SilentlyContinue;
	}
	$module = Get-ChildItem $in -Recurse -Filter "*.psd1" | Select-Object -ExpandProperty FullName -First 1;

	[xml]$appconfig = Get-Item "$projectRoot\tests\MSTest*\app.config" | Get-Content;
	$mssql = $appconfig.SelectSingleNode(".//configuration/connectionStrings/add[@name='mssql']").Attributes["connectionString"].Value;
	$mysql = $appconfig.SelectSingleNode(".//configuration/connectionStrings/add[@name='mysql']").Attributes["connectionString"].Value;
	
	return New-Object psobject -Property @{
		name = $name;
		mssql = $mssql;
		mysql = $mysql;
		modulePath = $module;
		moduleName = [IO.Path]::GetFileNameWithoutExtension($module);
		samples = $samples.FullName;
		projectRoot = $projectRoot;
		directory = $testDir;
		temp = $temp;
		out = $out;
		in = $in;
	};
}

function Get-ProjectRoot()
{
	$path = $PSScriptRoot;
	for ($i = 0; $i -lt 2; $i++)
	{
		$path = Split-Path $path -Parent;
	}

	return $path;
}