Param($BuildConfiguation = "Debug")

Import-Module "$PSScriptRoot\helper.psm1" -Force;
$context = New-TestEnviroment $BuildConfiguation "new-migrationScript";
Import-Module $context.modulePath -Force;

Describe "New-MigrationScript" {
	Context "New" {
		$source = Get-Item "$($context.samples)\cmdlets\*source*.xml";
		$targets = Get-ChildItem "$($context.samples)\cmdlets" -Filter "*.xml";

		It "should return mysql migration scripts." {
			$src = $source | ConvertTo-Schema;
			$results = $targets | ConvertTo-Schema | New-MigrationScript $src -ext "mysql";
			$results.Length | Should BeGreaterThan 1;
		}
	}
}

if (Get-Module $context.moduleName) { Remove-Module $context.moduleName; }