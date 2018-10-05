#

Properties {
	$MigrationDirectory = "";
}

Task "Default" -depends @("restore", "compile", "test", "pack");

Task "Publish" -alias "push" -description "This task compiles, test and publish the project to nuget.org and powershell gallery." `
-depends @("version", "build", "test", "pack", "push-nuget", "push-ps");

# ==========

Task "Initialize-Project" -alias "configure" -description "" `
-depends @("restore") -action {
	# Create the 'secrets.json' file
    if (-not (Test-Path $SecretsJson))
    {
		$writer = [System.Text.StringBuilder]::new();
		$content = @"
{
	"nugetKey": null,
	"psGalleryKey": null,
	"connections":
	[
		{
			"name": "SQLite",
			"jdbcurl": "jdbc:sqlite:{0}",
			"database": "Data Source=;"
		},

		{
			"name": "MySQL",
			"jdbcurl": "jdbc:mysql://{0}/{1}",
			"database": "server=;user=;password=;database=;"
		},

		{
			"name": "MSSQL",
			"jdbcurl": "jdbc:sqlserver:////{0};databaseName={1}",
			"database": "server=;user=;password=;database=;"
		}
	]
}
"@;
		$content | Out-File $SecretsJson -Encoding utf8;
    }

	return;

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

#region ----- DB Migration -----

Task "Rebuild-FlywayLocalDb" -alias "rebuild-db" -description "This task rebuilds the local database using flyway." `
-depends @("restore") -action{
	[string]$flyway = Get-Flyway;
	$credential = Get-Secret "local";
	Assert (-not [string]::IsNullOrEmpty($credential.database)) "A connection string for your local database was not provided.";

	$db = [ConnectionInfo]::new($credential, $credential.database);
	Write-Header "flyway: clean ($($db.ToFlywayUrl()))";
	Exec { &$flyway clean $db.ToFlywayUrl() $db.ToFlyUser() $db.ToFlyPassword(); }
	Write-Header "flyway: migrate ($($db.ToFlywayUrl()))";
	Exec { &$flyway migrate $db.ToFlywayUrl() $db.ToFlyUser() $db.ToFlyPassword() $([ConnectionInfo]::ConvertToFlywayLocation($MigrationDirectory)); }
	Exec { &$flyway info $db.ToFlywayUrl() $db.ToFlyUser() $db.ToFlyPassword() $([ConnectionInfo]::ConvertToFlywayLocation($MigrationDirectory)); }
}

#endregion