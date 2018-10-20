#

Properties {
	$MigrationDirectory = "";
}

Task "Default" -depends @("restore", "build", "test", "pack");

Task "Publish" -alias "push" -description "This task compiles, test and publish the project to nuget.org and powershell gallery." `
-depends @("version", "build", "test", "pack", "push-nuget", "push-ps");

# ==========

Task "Initialize-Project" -alias "configure" -description "This task initialize the project." `
-depends @("restore") -action {
	# Create the 'secrets.json' file.
    if (-not (Test-Path $SecretsJson))
    {
		$content = @"
{
	"nugetKey": null,
	"psGalleryKey": null,
	"connection":
	{
		"MySQL":
		{
			"connectionString": "server=localhost;user=;password=;"
		},

		"TSQL":
		{
			"connectionString": "server=localhost;user=;password=;"
		}
	}
}
"@;
		$content | Out-File $SecretsJson -Encoding utf8;
    }

	# Create test project connection strings.
	[string]$projectDir = Join-Path $RootDir "tests\*.MSTest\" | Resolve-Path;
	$credentials = Get-Content $SecretsJson | ConvertFrom-Json | Select-Object -ExpandProperty "connection";
	$configFile = Join-Path $projectDir "connections.json";
	$credentials | ConvertTo-Json | Out-File $configFile -Encoding utf8;
	Write-Host "  *  Updated $(Split-Path $configFile -Leaf)";

	# Restore packages.
    [string]$sln = Resolve-Path "$RootDir/*.sln";
    Write-Header "dotnet restore";
    Exec { &dotnet restore $sln; }
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