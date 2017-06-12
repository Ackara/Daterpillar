Param($BuildConfiguation = "Debug")

Import-Module "$PSScriptRoot\helper.psm1" -Force;
$context = New-TestEnviroment $BuildConfiguation "convert-dllToSchema";
Import-Module $context.modulePath;

Describe "Convert-DllToSchema" {
	Context "Convert" {
		Get-ChildItem "$($context.projectRoot)\tests\MSTest*\bin\$BuildConfiguation" | Copy-Item -Recurse -Destination $context.temp;
		$sample = Get-Item "$($context.temp)\$BuildConfiguation\MSTest.Daterpillar.dll";
		$expected = [IO.Path]::ChangeExtension($sample.FullName, "schema.xml");
		$script = "$($context.in)\Convert-DllToSchema.ps1";

		It "should generate a schema xml file using an assembly dll." {
			$result = (& $script $sample.FullName -Verbose);
			$schema = $result | ConvertTo-Schema;

			$result | Should Exist;
			$result | Should Be $expected;
			$schema | Should BeOfType Acklann.Daterpillar.Schema;
		}
	}
}
