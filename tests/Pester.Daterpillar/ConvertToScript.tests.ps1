Import-Module "$PSScriptRoot\helper.psm1" -Force;
$sessionDir = Install-TestEnviroment;
$module = Get-ChildItem $sessionDir -Filter "*Automation.dll" | Select-Object -ExpandProperty FullName -First 1;
Import-Module $module -Force;

$samplesDir = Get-MSTestSamplesDirectory;

Describe "ConvertTo-Script" {
	It "should return a script when powershell file object is passed." {
		$schemaFile = Get-ChildItem $samplesDir -Recurse -Filter "*mock_schema*.xml" | Select-Object -First 1;
		$result = ($schemaFile | ConvertTo-Script);
		
		#Write-Host "result: [$result]";
		$result | Should Not BeNullOrEmpty;
	}
}

$module = ([IO.Path]::GetFileNameWithoutExtension($module));
if (Get-Module $module) { Remove-Module $module; }