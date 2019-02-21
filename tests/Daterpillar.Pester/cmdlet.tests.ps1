Join-Path $PSScriptRoot "helper.psm1" | Import-Module -Force;
$context = New-TestEnvironment;
Import-Module $context.ModulePath -Force;

Describe "help" {
	It "[PS] Should have full-detailed help" {
		$menu = help New-MigrationScript | Out-String;
		$menu | Write-Host;
		$menu | Should Not BeNullOrEmpty;
	}
}

Describe "Export-Schema" {
	$outFolder = New-TestFolder $context "export-schema";

	It "[PS] Can generate a '.schema.xml' file from an assembly." {
		try
		{
			Push-Location $PSScriptRoot;
			[string]$projectFolder = "../*.Sample/" | Resolve-Path;
			[string]$targetPath = Join-Path $projectFolder "bin/*/*/*.Sample.dll" | Resolve-Path | Select-Object -First 1;
			$expectedFile = [IO.Path]::ChangeExtension($targetPath, ".schema.xml");
			Pop-Location;

			Split-Path $targetPath -Parent | Push-Location;
			$targetPath | Export-Schema $projectFolder | Should Exist;
		}
		finally { Pop-Location; }
	}
}

Describe "New-MigrationScript" {
	It "[PS] Can generate migration script" {
		$outFolder = New-TestFolder $context "new-script";

		$newSchema = Join-Path $outFolder "lastest.schema.xml";
		$oldSchema = Join-Path $outFolder "snapshot.schema.xml";
		Get-SampleFile $context "music.xml" | Copy-Item -Destination $newSchema;

		$resultFile = New-MigrationScript $oldSchema $newSchema $outFolder "mysql";
		$resultFile.Script | Should Exist;
		$resultFile.Script.Length | Should BeGreaterThan 100;
	}
}

Describe "Sync-Schema" {
	$db = Join-Path $PSScriptRoot "sample.db";
	
	It "[PS] Can apply migration scripts to database." {

	}
}