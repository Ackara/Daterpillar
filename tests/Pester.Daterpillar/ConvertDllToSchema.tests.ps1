Import-Module "$PSScriptRoot\helper.psm1" -Force;
$sessionDir = Install-MSTestProject "dllToSchema";
$solutionDir = Get-SolutionDirectory;

Describe "Convert-DllToSchema" {
	$script = Get-ChildItem "$solutionDir\src\*Automation*" -Recurse -Filter "*Convert-DllToSchema.ps1" | Select-Object -First 1;
	$sampleDll = Get-ChildItem $sessionDir -Filter "*MSTest.Daterpillar*.dll" | Select-Object -ExpandProperty FullName -First 1;
	$schemaFile = (& $script $sampleDll);
	
	It "should generate a schema xml file when a assembly dll is passed." {
		$schemaFile | Should Exist;
		$schemaFile | Should Contain "</schema>";
	}
}