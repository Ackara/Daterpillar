#

Properties {
}

Task "Publish" -alias "push" -description "This task compiles, test and publish the project to nuget.org and powershell gallery." `
-depends @("version", "build", "test", "pack", "push-nuget", "push-ps");

# ==========

Task "Initialize-Project" -alias "configure" -description "" `
-depends @("restore") -action {

}

Task "Generate-Packages" -alias "pack" -description "This task packages the project to be published to all on-line repositories." `
-depends @("restore") -action {
	if (Test-Path $ArtifactsDir) { Remove-Item $ArtifactsDir -Recurse -Force; }
	New-Item $ArtifactsDir -ItemType Directory | Out-Null;

	# Creating powershell package.
	$proj = Join-Path $RootDir "src\*.CLI\*.*proj" | Get-Item;
	Write-Header "dotnet: publish '$($proj.Basename)' (FDD)";
	Exec { &dotnet publish $proj.FullName --configuration $Configuration; }
	
	$psd1 = Get-ChildItem (Join-Path $RootDir "src\*.CLI\bin\$Configuration\*\NShellit") -Filter "*.psd1" -Recurse `
			| Select-Object -First 1;
	Copy-Item $psd1.DirectoryName -Destination $ArtifactsDir -Recurse -Force;
	
	# Creating the nuget package.
	$proj = Join-Path $RootDir "src\$SolutionName\*.*proj" | Get-Item;
	Write-Header "dotnet: pack '$($proj.Basename)' (FDD)";
	Exec { &dotnet pack $proj.FullName --configuration $Configuration --output $ArtifactsDir ; }

	# Creating test MSBuild target
	[string]$nupkg = Join-Path $ArtifactsDir "*.nupkg" | Get-Item;
	[string]$zip = [System.IO.Path]::ChangeExtension($nupkg, ".zip");
	Copy-Item $nupkg -Destination $zip -Force;
	
	Expand-Archive $zip -DestinationPath (Join-Path $ArtifactsDir "msbuild");
	Remove-Item $zip -Force -Recurse;
}
