Param($BuildConfiguation = "Debug")

Import-Module "$PSScriptRoot\helper.psm1" -Force;
$context = New-TestEnviroment $BuildConfiguation "convertto-schema";
Import-Module $context.modulePath -Force;

Describe "Remove-Database" {
	Context "Remove" {
		It "should drop a mysql database." {
			$result = $context.mysql | Remove-Database -d "dtpl_psDrop" -s "mysql";
			$result | Should BeOfType System.Boolean;
		}

		It "should drop a mssql database." {
			$result = $context.mssql | Remove-Database -d "dtpl_psDrop" -s "MSSQL";
			$result | Should BeOfType System.Boolean;
		}

		It "should drop a sqlite database." {
			$dbFile = "$($context.in)\dtpl_psDrop.db3";
			$result = Remove-Database $dbFile -d "dtpl_psDrop" -s "sqlite";

			$dbFile | Should Not Exist;
			$result | Should BeOfType System.Boolean;
		}
	}
}

if (Get-Module $context.moduleName) { Remove-Module $context.moduleName; }