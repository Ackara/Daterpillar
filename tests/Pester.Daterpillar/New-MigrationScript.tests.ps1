Import-Module "$PSScriptRoot\helper.psm1" -Force;
$sessionDir = Install-TestEnviroment "migration";
$module = Get-ChildItem $sessionDir -Filter "*Automation.dll" | Select-Object -ExpandProperty FullName -First 1;
Import-Module $module -Force;

$samplesDir = Get-MSTestSamplesDirectory;

Describe "New-MigrationScript" {
	$source = Get-ChildItem $samplesDir -Recurse -Filter "*source*.xml" | Select-Object -First 1;
	$targets = Get-ChildItem "$samplesDir\cmdlets" -Recurse -Filter "*.xml";

	It "should return a migration script" {
		$src = $source | ConvertTo-Schema;
		$tgt = $targets | Select-Object -ExpandProperty FullName -Last 1 | ConvertTo-Schema;
		$result = New-MigrationScript -from $tgt -to $src;

		#Write-Host "script: [$($result.Script)]";
		#Write-Host "modifications: [$($result.Modifications)]";
		$result.Script | Should Not BeNullOrEmpty;
		$result.Modifications | Should Not BeNullOrEmpty;
		$result.State | Should BeOfType Ackara.Daterpillar.Migration.MigrationState;
	}

	It "should return a migration script when values are passed from the pipeline" {
		$src = $source | ConvertTo-Schema;
		$results = $targets | ConvertTo-Schema | New-MigrationScript $src -Syntax "cs";
		
		#Write-Host "count: $($results.Length)";
		$results.Length | Should BeGreaterThan 1;
	}
}

$module = ([IO.Path]::GetFileNameWithoutExtension($module));
if (Get-Module $module) { Remove-Module $module; }