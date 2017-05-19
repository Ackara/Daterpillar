Import-Module "$PSScriptRoot\helper.psm1" -Force;
$sessionDir = Install-TestEnviroment "remove database";
$module = Get-ChildItem $sessionDir -Filter "*.psd1" | Select-Object -ExpandProperty FullName -First 1;
Import-Module $module -Force;

Describe "Remove-Database" {
	[xml]$appconfig = Get-Item "$(Split-Path $PSScriptRoot -Parent)\MStest*\app.config" | Get-Content;

	It "should drop an existing mssql database." {
		$connStr = $appconfig.SelectSingleNode(".//configuration/connectionStrings/add[@name='mssql']").Attributes["connectionString"].Value;
		$result = $connStr | Remove-Database -Syntax "mssql" -Database "dtpl_psDrop1";

		$result | Should BeOfType System.Boolean;
	}

	It "should drop an existing mysql database." {
		$connStr = $appconfig.SelectSingleNode(".//configuration/connectionStrings/add[@name='mysql']").Attributes["connectionString"].Value;
		$result = $connStr | Remove-Database -Syntax "mysql" -Database "dtpl_psDrop1";

		$result | Should BeOfType System.Boolean;
	}

	It "should drop an existing sqlite database." {
		$dbFile = "$sessionDir\dtpl_add_pester.db3";
		$result = Remove-Database $dbFile  -s "sqlite";

		$dbFile | Should Not Exist;
		$result | Should BeOfType System.Boolean;
	}
}

$module = ([IO.Path]::GetFileNameWithoutExtension($module));
if (Get-Module $module) { Remove-Module $module; }
