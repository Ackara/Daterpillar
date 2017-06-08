Param($BuildConfiguation = "Debug")

Import-Module "$PSScriptRoot\helper.psm1" -Force;
$context = New-TestEnviroment $BuildConfiguation "convertto-script";
Import-Module $context.modulePath -Force;

Describe "ConvertTo-Script" {
	Context "Invoke" {
		It "should transform a Schema object into a sql script." {
			$sample = Get-Item "$($context.samples)\cmdlets\source_schema.xml";
			$result = $sample | ConvertTo-Schema | ConvertTo-Script -ext "mysql";
			$result | Should Not BeNullOrEmpty;
		}
	}
}

if (Get-Module $context.moduleName) { Remove-Module $context.moduleName; }