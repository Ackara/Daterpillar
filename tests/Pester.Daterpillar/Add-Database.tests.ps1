Import-Module "$PSScriptRoot\helper.psm1" -Force;
$sessionDir = Install-TestEnviroment "add database";
$module = Get-ChildItem $sessionDir -Filter "*.psd1" | Select-Object -ExpandProperty FullName -First 1;
Import-Module $module -Force;

Describe "Add-Database" {
	[xml]$appconfig = Get-Item "$(Split-Path $PSScriptRoot -Parent)\MStest*\app.config" | Get-Content;

	It "should create a new mssql database." {
		$connStr = $appconfig.SelectSingleNode(".//configuration/connectionStrings/add[@name='mssql']").Attributes["connectionString"].Value;
		$result = $connStr | Add-Database -Syntax "mssql" -Database "dtpl_psAdd1";

		$result | Should BeOfType System.Boolean;
	}

	It "should create a new mysql database." {
		$connStr = $appconfig.SelectSingleNode(".//configuration/connectionStrings/add[@name='mysql']").Attributes["connectionString"].Value;
		$result = $connStr | Add-Database -Syntax "mysql" -Database "dtpl_psAdd1";

		$result | Should Be $true;
	}

	It "should create a new sqlite database." {
		$dbFile = "$sessionDir\dtpl_add_pester.db3";
		if (Test-Path $dbFile -PathType Leaf) { Remove-Item $dbFile; }
		$result = Add-Database $dbFile  -s "sqlite";

		$dbFile | Should Exist;
		$result | Should BeOfType System.Boolean;
	}
}

$module = ([IO.Path]::GetFileNameWithoutExtension($module));
if (Get-Module $module) { Remove-Module $module; }
