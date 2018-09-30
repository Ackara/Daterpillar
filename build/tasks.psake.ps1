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

	# Creating nuget package.
	$proj = Join-Path $RootDir "src\$SolutionName\*.csproj" | Get-Item;
	Write-Host "dotnet: publish '$($proj.Basename)'";

}
