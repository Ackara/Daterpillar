Param($BuildConfiguation = "Debug")

Import-Module "$PSScriptRoot\helper.psm1" -Force;
$context = New-TestEnviroment $BuildConfiguation "convertto-schema";
Import-Module $context.modulePath -Force;

Describe "ConvertTo-Schema" {
	Context "ConvertTo" {
		$sample = Get-Item "$($context.samples)\cmdlets\source_schema.xml";

		It "should deserialize a schema.xml file to a Schema object." {
			$result = $sample | ConvertTo-Schema;
			$result | Should BeOfType Acklann.Daterpillar.Schema;
		}

		It "should fetch schema from db server." {
			$dbname = "dtpl_toSchema";
			$connStr = $context.mysql;
			$syntax = "mysql";

			$connStr | New-Database -DeleteIfExist -Name $dbname -ext $syntax;
			$schemaWasCreated = $sample | ConvertTo-Schema | ConvertTo-Script -ext $syntax | Invoke-SqlCommand -conn "$($context.mysql)database=$dbname;" -type $syntax;
			$schema = [Acklann.Daterpillar.Migration.ServerManagerFactory]::CreateInstance($syntax, "$connStr;database=$dbname").GetConnection() | ConvertTo-Schema;

			$schemaWasCreated | Should Be $true;
			$schema | Should BeOfType Acklann.Daterpillar.Schema;
			$schema.Tables.Count | Should Be 3;
		}
	}
}

if (Get-Module $context.moduleName) { Remove-Module $context.moduleName; }