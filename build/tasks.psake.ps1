# SYNOPSIS: This is a psake task file.
Join-Path $PSScriptRoot "toolkit.psm1" | Import-Module -Force;
FormatTaskName "$(Write-Header -ReturnAsString)`r`n  {0}`r`n$(Write-Header -ReturnAsString)";

Properties {
	$Dependencies = @("Ncrement");

	# Files & Folders
	$SolutionFolder = (Split-Path $PSScriptRoot -Parent);
	$ManifestFilePath = (Join-Path $PSScriptRoot  "manifest.json");
	$SecretsFilePath = (Join-Path $SolutionFolder "secrets.json");
	$ArtifactsFolder = (Join-Path $SolutionFolder "artifacts");
	$ToolsFolder = "";

	# Arguments
    $ShouldCommitChanges = $true;
	$CurrentBranch = "";
	$Configuration = "";
	$Filter = $null;
	$DryRun = $false;
	$Major = $false;
	$Minor = $false;
	$Force = $false;
}

Task "Default" -depends @("configure", "build", "test", "pack");

Task "Deploy" -alias "publish" -description "This task compiles, test then publish all packages to their respective destination." `
-depends @("clean", "build", "xsd", "test", "pack", "push-nuget", "push-ps", "tag");

# ======================================================================

Task "Configure-Environment" -alias "configure" -description "This task generates all files required for development." `
-depends @("restore") -action {
	# Generating the build manifest file.
	if (-not (Test-Path $ManifestFilePath)) { New-NcrementManifest | ConvertTo-Json | Out-File $ManifestFilePath -Encoding utf8; }
	Write-Host "  * added 'build/$(Split-Path $ManifestFilePath -Leaf)' to the solution.";

	# Generating a secrets file template to store sensitive information.
	if (-not (Test-Path $SecretsFilePath))
	{
		$content = "{ 'nugetKey': null, 'mysql': 'server=;user=;password=;', 'tsql': 'server=;user=;password=;' }";
		$content | ConvertFrom-Json | ConvertTo-Json | Out-File $SecretsFilePath -Encoding utf8;
	}
	Write-Host "  * added '$(Split-Path $SecretsFilePath -Leaf)' to the solution.";
}

Task "Package-Solution" -alias "pack" -description "This task generates all deployment packages." `
-depends @("restore") -action {
	if (Test-Path $ArtifactsFolder) { Remove-Item $ArtifactsFolder -Recurse -Force; }
	New-Item $ArtifactsFolder -ItemType Directory | Out-Null;
	$version = ConvertTo-NcrementVersionNumber $ManifestFilePath $CurrentBranch;

	# Building the powersehll manifest.
	$moduleFolder = Join-Path $ArtifactsFolder (Split-Path $SolutionFolder -Leaf);
	if (-not (Test-Path $moduleFolder)) { New-Item $moduleFolder -ItemType Directory | Out-Null; }
	$projectFile = Join-Path $SolutionFolder "src/*.Powershell/*.*proj" | Get-Item;

	Write-Header "dotnet: publish '$($projectFile.BaseName)'";
	Exec { &dotnet publish $projectFile.FullName --configuration $Configuration --output $moduleFolder; }
	Write-Header;

	$manifest = Get-Content $ManifestFilePath | ConvertFrom-Json;
	$dll = Join-Path $moduleFolder "*.Powershell.dll" | Get-Item;
	$psd1 = Get-ChildItem $projectFile.DirectoryName -Filter "*.psd1" | Select-Object -First 1;
	Get-ChildItem $moduleFolder -Filter "*.psd1" | Remove-Item;
	Copy-Item $psd1.FullName -Destination (Join-Path $moduleFolder "$(Split-Path $moduleFolder -Leaf).psd1") -Force;
	Join-Path $projectFile.DirectoryName "bin/$Configuration/*/*-help.xml" | Get-Item | Copy-Item -Destination $moduleFolder -Force;

	# Building the nuget package.
	$projectFile = Join-Path $SolutionFolder "src/$(Split-Path $SolutionFolder -Leaf)/*.*proj" | Get-Item;
	$projectFile | Invoke-NugetPack $ArtifactsFolder $Configuration $version.FullVersion;
	Get-ChildItem $ArtifactsFolder -Recurse -File -Filter "*.nupkg" | Expand-NugetPackage (Join-Path $ArtifactsFolder "msbuild");
}

Task "Generate-XmlSchemaFromDll" -alias "xsd" -description "This task generates a '.xsd' file from the project's '.dll' file." `
-precondition { return Test-XsdExe; } `
-action {
	Join-Path $SolutionFolder "src/*/$(Split-Path $SolutionFolder -Leaf).csproj" | Get-ChildItem `
		| Export-XmlSchemaFromDll $Configuration -FullyQualifiedTypeName "Acklann.Daterpillar.Configuration.Schema" -Force;
}

#region ----- COMPILATION ----------------------------------------------

Task "Clean" -description "This task removes all generated files and folders from the solution." `
-action {
	Join-Path $SolutionFolder "*.sln" | Get-Item | Remove-GeneratedProjectItem -AdditionalItems @("artifacts");
	Get-ChildItem $SolutionFolder -Recurse -File -Filter "*.*proj" | Remove-GeneratedProjectItem -AdditionalItems @("package-lock.json");
}

Task "Import-BuildDependencies" -alias "restore" -description "This task imports all build dependencies." `
-action {
	# Installing all required dependencies.
	foreach ($moduleId in $Dependencies)
	{
		$modulePath = Join-Path $ToolsFolder "$moduleId/*/*.psd1";
		if (-not (Test-Path $modulePath)) { Save-Module $moduleId -Path $ToolsFolder; }
		Import-Module $modulePath -Force;
		Write-Host "  * imported the '$moduleId.$(Split-Path (Get-Item $modulePath).DirectoryName -Leaf)' powershell module.";
	}
}

Task "Increment-VersionNumber" -alias "version" -description "This task increments all of the projects version number." `
-depends @("restore") -action {
	$manifest = $ManifestFilePath | Step-NcrementVersionNumber -Major:$Major -Minor:$Minor -Patch;
	$manifest | ConvertTo-Json | Out-File $ManifestFilePath -Encoding utf8;
	Invoke-Tool { &git add $ManifestFilePath | Out-Null; };

	Join-Path $SolutionFolder "src/*/*.*proj" | Get-ChildItem -File | Update-NcrementProjectFile $manifest `
		| Write-FormatedMessage "  * updated '{0}' version number to '$(ConvertTo-NcrementVersionNumber $manifest | Select-Object -ExpandProperty Version)'.";

	Join-Path $SolutionFolder "src/*/*.*psd1" | Get-ChildItem -File | Update-NcrementProjectFile $manifest -Commit:$ShouldCommitChanges `
		| Write-FormatedMessage "  * updated '{0}' version number to '$(ConvertTo-NcrementVersionNumber $manifest | Select-Object -ExpandProperty Version)'.";
}

Task "Build-Solution" -alias "build" -description "This task compiles projects in the solution." `
-action {
	Get-Item "$SolutionFolder/*.sln" | Invoke-MSBuild $Configuration;
}

Task "Run-Tests" -alias "test" -description "This task invoke all tests within the 'tests' folder." `
-action { Join-Path $SolutionFolder "tests" | Get-ChildItem -Recurse -File -Filter "*MSTest.csproj" | Invoke-MSTest $Configuration; }

Task "Run-Benchmarks" -alias "benchmark" -description "This task invoke all benchmark tests within the 'tests' folder." `
-action { $projectFile = Join-Path $SolutionFolder "tests/*.Benchmark/*.*proj" | Get-Item | Invoke-BenchmarkDotNet -Filter $Filter -DryRun:$DryRun; }

#endregion

#region ----- PUBLISHING -----------------------------------------------

Task "Publish-NuGetPackages" -alias "push-nuget" -description "This task publish all nuget packages to nuget.org." `
-precondition { return Test-Path $ArtifactsFolder -PathType Container } `
-action { Get-ChildItem $ArtifactsFolder -Recurse -Filter "*.nupkg" | Publish-PackageToNuget $SecretsFilePath "nugetKey"; }

Task "Publish-PowershellModules" -alias "push-ps" -description "" `
-precondition { return Test-Path $ArtifactsFolder -PathType Container } `
-action { Join-Path $ArtifactsFolder "*/*.psd1" | Get-Item | Publish-PackageToPowershellGallery $SecretsFilePath "psGalleryKey"; }

Task "Publish-VSIXPackage" -alias "push-vsix" -description "This task publish all .vsix packages." `
-precondition { return Test-Path $ArtifactsFolder -PathType Container } `
-action { Get-ChildItem $ArtifactsFolder -Recurse -Filter "*.vsix" | Publish-PackageToVSIXGallery $ToolsFolder; }

Task "Add-GitReleaseTag" -alias "tag" -description "This task tags the last commit with the version number." `
-precondition { return $CurrentBranch -eq "master"; } `
-depends @("restore") -action { $ManifestFilePath | ConvertTo-NcrementVersionNumber | Select-Object -ExpandProperty Version | New-GitTag $CurrentBranch; }

#endregion