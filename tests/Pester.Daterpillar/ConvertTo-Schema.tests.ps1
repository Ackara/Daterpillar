Param($BuildConfiguation = "Debug")

Import-Module "$PSScriptRoot\helper.psm1" -Force;
$context = New-TestEnviroment $BuildConfiguation "convertto-schema";
Import-Module $context.modulePath -Force;

Describe "ConvertTo-Schema" {
	Context "Invoke" {
		It "should deserialize a schema.xml file to a Schema object." {
			$sample = Get-Item "$($context.samples)\cmdlets\source_schema.xml";
			$result = $sample | ConvertTo-Schema;
			$result | Should BeOfType Acklann.Daterpillar.Schema;
		}
	}
}

if (Get-Module $context.moduleName) { Remove-Module $context.moduleName; }