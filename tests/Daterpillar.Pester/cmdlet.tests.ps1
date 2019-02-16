Join-Path $PSScriptRoot "helper.psm1" | Import-Module -Force;
$context = New-TestEnvironment;
Import-Module $context.ModulePath -Force;

Describe "help" {
	It "should have full-detailed help" {
		$menu = help New-MigrationScript | Out-String;
		$menu | Write-Host;
		$menu | Should Not BeNullOrEmpty;
	}
}

Describe "New-MigrationScript" {
	It "can generate migration script via powershell" {
		$outFolder = New-TestFolder $context "new-script";

		$newSchema = Join-Path $outFolder "lastest.schema.xml";
		$oldSchema = Join-Path $outFolder "snapshot.schema.xml";
		Get-SampleFile $context "music.xml" | Copy-Item -Destination $newSchema;

		$resultFile = New-MigrationScript $oldSchema $newSchema $outFolder "mysql";
		$resultFile.FullName | Should Exist;
		$resultFile.Length | Should BeGreaterThan 100;
	}
}