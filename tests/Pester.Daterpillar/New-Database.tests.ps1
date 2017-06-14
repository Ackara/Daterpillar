Param($BuildConfiguation = "Debug")

Import-Module "$PSScriptRoot\helper.psm1" -Force;
$context = New-TestEnviroment $BuildConfiguation "add-database";
Import-Module $context.modulePath -Force;

Describe "New-Database" {
	Context "Invoke" {
		It "should create a new mssql database." {
			$results = $context.mssql | New-Database -Syntax "mssql" -name "dtpl_psAdd";
			$results | Should BeOfType System.Boolean;
		}

		It "should create a new mysql database." {
			$results = New-Database -conn $context.mysql -syn "mysql" -d "dtpl_psAdd";
			$results | Should BeOfType System.Boolean;
		}

		It "should create a new sqlite database." {
			$dbFile = "$($context.out)\dtpl_psAdd.db3";
			$results = New-Database $dbFile -syn "sqlite";

			$dbFile | Should Exist;
			$results | Should BeOfType System.Boolean;
		}
	}
}

if (Get-Module $context.moduleName) { Remove-Module $context.moduleName; }