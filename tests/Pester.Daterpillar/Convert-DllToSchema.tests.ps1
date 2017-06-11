﻿Param($BuildConfiguation = "Debug")

Import-Module "$PSScriptRoot\helper.psm1" -Force;
$context = New-TestEnviroment $BuildConfiguation "convert-dllToSchema";

Describe "Convert-DllToSchema" {
	Context "Convert" {
		Get-ChildItem "$($context.projectRoot)\tests\MSTest*\bin\$BuildConfiguation" | Copy-Item -Recurse -Destination $context.out;
		Get-ChildItem "$($context.in)" -Recurse -Include @("Convert-DllToSchema.ps1", "*Daterpillar.dll") | Copy-Item -Destination $context.temp;
		$sample = Get-Item "$($context.out)\$BuildConfiguation\MSTest.Daterpillar.dll";
		$expected = [IO.Path]::ChangeExtension($sample.FullName, "schema.xml");
		$script = "$($context.temp)\Convert-DllToSchema.ps1";

		It "should generate a schema xml file using an assembly dll." {
			$result = (& $script $sample.FullName);
			$result | Should Exist;
			$result | Should Be $expected;
		}
	}
}
