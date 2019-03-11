Join-Path $PSScriptRoot "helper.psm1" | Import-Module -Force;
$context = New-TestEnvironment;
Import-Module $context.ModulePath -Force;

Describe "help" {
	It "[PS] Should have full-detailed help" {
		foreach ($name in @("Clear-DaterpillarSchema", "Show-DaterpillarMigrationHistory", "New-DaterpillarMigrationScript", "Export-DaterpillarSchema", "Update-DaterpillarSchema"))
		{
			$menu = help $name | Out-String;
			#$menu | Write-Host; Write-Host "===============";
			$menu | Should Not BeNullOrEmpty;
		}
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
			$targetPath | Export-DaterpillarSchema -Verbose | Should Exist;
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

		$resultFile = New-DaterpillarMigrationScript "mysql" $outFolder $oldSchema $newSchema;
		$resultFile.Script | Should Exist;
		$resultFile.Script.Length | Should BeGreaterThan 100;

		$resultFile = $newSchema | New-DaterpillarMigrationScript "mysql" $outFolder $oldSchema;
		$resultFile.Script | Should Exist;
	}
}

Describe "Show-DaterpillarMigrationHistory" {
	$outFolder = New-TestFolder $context "show-history";

	$db = Join-Path $outFolder "sample.sqlite";
	New-Item $db -ItemType File | Out-Null;

	It "[PS] Can display database migration history" {
		$v1Script = Join-Path $outFolder "V1__init.sql";
		New-Item $v1Script -ItemType File | Out-Null;
		"CREATE TABLE user (Id INT NOT NULL, Name VARCHAR(64) NOT NULL);" | Out-File $v1Script -Encoding utf8;

		$result = Show-DaterpillarMigrationHistory "SQLite" $db "user" "pass" "master" $outFolder;
		$result.History | Should BeNullOrEmpty;

		$result = Show-DaterpillarMigrationHistory "SQLite" $db "user" "pass" "master" $outFolder -PassThru;
		$result.History | Should Not BeNullOrEmpty;
	}
}

Describe "Update-Schema" {
	$outFolder = New-TestFolder $context "update-schema";

	$db = Join-Path $outFolder "sample.sqlite";
	New-Item $db -ItemType File | Out-Null;

	It "[PS] Can apply migration scripts to database." {
		$v1Script = Join-Path $outFolder "V1__init.sql";
		New-Item $v1Script -ItemType File | Out-Null;
		"CREATE TABLE user (Id INT NOT NULL, Name VARCHAR(64) NOT NULL);" | Out-File $v1Script -Encoding utf8;

		$result = Update-DaterpillarSchema "SQLite" $db "user" "pass" "master" $outFolder;
		$result | Should Be $true;

		$result = Update-DaterpillarSchema "SQLite" -sql $outFolder -connstr "host=$db" -Verbose;
		$result | Should Be $true;

		Update-DaterpillarSchema "SQLite" -sql $outFolder -connstr "host=$db" | Show-DaterpillarMigrationHistory "SQLite";
	}

	It "[PS] Should throw exception on error" {
		$v2Script = Join-Path $outFolder "V2__error.sql";
		New-Item $v2Script -ItemType File | Out-Null;
		"CREATE TABLE product (Id INT NOT NULL, Name VARCHAR(64)) NOT NULL);" | Out-File $v2Script -Encoding utf8;

		{ Update-DaterpillarSchema "SQLite" "host" "user" "pass" $db $outFolder; } | Should Throw;
	}
}