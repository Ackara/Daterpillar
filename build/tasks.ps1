<#
.SYNOPSIS
Psake build tasks.
#>

Properties {
	# Paths & Tools
	$Nuget = "";
	$ProjectRoot = (Split-Path $PSScriptRoot -Parent);
	$ManifestPath = "$PSScriptRoot\manifest.json";
	$Manifest = Get-Content $ManifestPath | Out-String | ConvertFrom-Json;
	$ReleaseNotesPath = "$ProjectRoot\releaseNotes.txt";
	$ArtifactsDir = "$ProjectRoot\artifacts";

	# User Args
	$TestCase = "";
	$NuGetKey = "";
	$BranchName = "";
	$PSGalleryKey = "";
	$SkipMSBuild = $false;
	$BuildConfiguration = "";
	$ConnectionStrings = @{};
	$Major = $false;
	$Minor = $false;
}

Task "default" -description "This task compiles, test and publish the project to nuget.org and powershell gallery." `
-depends @() -precondition {
}

Task "test" -description "This task runs all tests." -depends @("pester", "vstest");

#----------

Task "init" -description "This task imports all dependencies." -action {
	foreach ($name in @("vssetup", "Buildbox.SemVer", "Buildbox.Utils", "WinSCP"))
	{
		$path = "$ProjectRoot\tools\$name\*\*.psd1";
		$module = Get-Item $path -ErrorAction SilentlyContinue;
		if (-not $module) { Save-Module $name -Path "$ProjectRoot\tools"; }
		Get-Item $path | Import-Module -Force;
		if (Get-Module $name) { Write-Host "`t* imported $name module"; }
	}

	foreach ($psd1 in (Get-ChildItem "$ProjectRoot\packages\*\tools\*.psd1" -Exclude @("*psake*")))
	{
		Import-Module $psd1.FullName -Force;
		if (Get-Module ([IO.Path]::GetFileNameWithoutExtension($psd1.Name))) { Write-Host "`t* imported $($psd1.Name) module"; }
	}
	
	foreach ($psm1 in (Get-ChildItem "$PSScriptRoot\*.psm1"))
	{
		Import-Module $psm1.FullName -Force;
		if (Get-Module ([IO.Path]::GetFileNameWithoutExtension($psm1.Name))) { Write-Host "`t* imported $($psm1.Name) module"; }
	}
}

Task "compile" -description "This task builds the solution using msbuild." `
-depends @("init") -precondition { return (-not $SkipMSBuild) } -action {
	Assert ("Debug", "Release" -contains $BuildConfiguration) "`$BuildConfiguration was '$BuildConfiguration' but expected 'Debug' or 'Release'.";

	Write-LineBreak "MSBUILD";
	$msbuild = Get-MSBuildPath;
	$sln = (Get-item "$ProjectRoot\*.sln").FullName;
	Exec { & $msbuild $sln "/p:Configuration=$BuildConfiguration" "/verbosity:minimal"; }
	Write-LineBreak;
}

Task "pester" -description "This task runs all specified pester tests." `
-depends @("init", "compile") -action {
	$results = "";
	if ([String]::IsNullOrEmpty($TestCase))
	{
		$results = Invoke-Pester -PassThru -Script @{ Path = "$ProjectRoot\tests\Pester*\*test*.ps1"; Arguments = @($BuildConfiguration) };
	}
	else
	{
		$results = Invoke-Pester -PassThru -Script @{ Path = "$ProjectRoot\tests\Pester*\$($TestCase)*test*.ps1"; Arguments = @($BuildConfiguration) }
	}

	Assert ($results.FailedCount -eq 0) "'$($results.FailedCount)' pester tests failed.";
}

Task "vstest" -alias "mstest" -description "This task runs all visual studio tests." `
-depends @("init") -action {
	Write-LineBreak "MSTest";
	foreach ($csproj in (Get-ChildItem "$ProjectRoot\tests" -Recurse -Filter "*.csproj" | Select-Object -ExpandProperty FullName))
	{
		Exec { & dotnet test $csproj --configuration $BuildConfiguration --verbosity minimal; }
	}
	Write-LineBreak;
}


Task "pack" -description "This task packages the project to be published to all online repositories." `
-depends @("init", "compile") -action {
	$msbuild = Get-MSBuildPath;
	if (Test-Path $ArtifactsDir -PathType Container) { Remove-Item $ArtifactsDir -Recurse -Force; }
	New-Item $ArtifactsDir -ItemType Directory | Out-Null;
	
	$releaseNotes = Get-Content $ReleaseNotesPath | Out-String;
	$version = (Get-VersionNumber -config $ManifestPath).ToString($true);
	$suffix = Get-BranchSuffix $BranchName -config $ManifestPath;
	$suffix = (& { if ([String]::IsNullOrEmpty($suffix)) { return ""; } else { return "-$suffix"; } })
	
	$metadata += "packageOutputPath=$ArtifactsDir;";
	$metadata += "PackageVersion=$version$($suffix);";
	$metadata += "packageReleaseNotes=$releaseNotes;";
	$metadata += "configuration=$BuildConfiguration;";
	$metadata += "authors=$($Manifest.metadata.author);";
	$metadata += "packageRequireLicenseAcceptance=$true;";
	$metadata += "packageTags=$($Manifest.metadata.tags);";
	$metadata += "Copyright=$($Manifest.metadata.copyright);";
	$metadata += "packageIconUrl=$($Manifest.metadata.iconUrl);";
	$metadata += "packageProjectUrl=$($Manifest.metadata.projectUrl);";
	$metadata += "packageLicenseUrl=$($Manifest.metadata.licenseUrl);";
	
	foreach ($proj in (Get-ChildItem "$ProjectRoot\src" -Recurse -Filter "*.csproj"))
	{
		$contents = Get-Content $proj.FullName | Out-String;
		$properties = "title=$([IO.Path]::GetFileNameWithoutExtension($proj.Name));";
		$description = Get-Content "$($proj.DirectoryName)\readme.txt" | Out-String;
		$properties += "description=$description;";
		$properties += $metadata.Trim(';');
		Push-Location $proj.DirectoryName;

		try
		{
			if ([Regex]::IsMatch($contents, '(?i)<TargetFramework>netstandard[0-9.]+</TargetFramework>'))
			{
				Write-LineBreak "MSBUILD";
				Exec { & $msbuild "/t:pack" "/p:$properties" "/verbosity:minimal"; }
			}
			else
			{
				Write-LineBreak "NUGET";
				Exec { & $nuget pack $proj.FullName -OutputDirectory $ArtifactsDir -Properties $properties -IncludeReferencedProjects; }
			}
		}
		finally { Pop-Location; }
	}
	Write-LineBreak;

	foreach ($module in (Get-ChildItem "$ProjectRoot\src\*\*" -Filter "*.psd1"))
	{
		Update-ModuleManifest -Path $module.FullName `
		-ModuleVersion $version `
		-Author $Manifest.metadata.author `
		-ReleaseNotes $releaseNotes.Trim() `
		-IconUri $Manifest.metadata.iconUrl `
		-CompanyName $Manifest.metadata.author `
		-Copyright $Manifest.metadata.copyright `
		-ProjectUri $Manifest.metadata.projectUrl `
		-LicenseUri $Manifest.metadata.licenseUrl `
		-Tags ($Manifest.metadata.tags.Split(' ')) `
		-CmdletsToExport "*" -FunctionsToExport "*" `
		-Description (Get-Content "$($module.DirectoryName)\readme.txt" | Out-String);

		$outDir = "$ArtifactsDir\$([IO.Path]::GetFileNameWithoutExtension($module.Name))";
		Get-Item "$($module.DirectoryName)\bin\$BuildConfiguration" | Copy-Item -Recurse -Destination $outDir;
		Get-ChildItem $outDir -Filter "*.config" | Remove-Item -Force;
		Get-Item "$outDir\Cmdlets" | Remove-Item -Force -Recurse;
	}
}

Task "publish" -alias "push" -description "This task publishes all nuget packages and modules." `
-depends @("pack") -action {
	Write-LineBreak "NUGET";
	foreach ($nupkg in (Get-ChildItem $ArtifactsDir -Filter "*.nupkg"))
	{
		Exec { & $nuget push $nupkg.FullName $NuGetKey -Source "https://api.nuget.org/v3/index.json"; }
	}

	if (-not [String]::IsNullOrEmpty($PSGalleryKey) -and ($BranchName -eq "master"))
	{
		foreach ($psd1 in (Get-ChildItem $ArtifactsDir -Recurse -Filter "*.psd1"))
		{
			if (Test-ModuleManifest -Path $psd1.FullName)
			{
				Publish-Module -Path $psd1.DirectoryName -NuGetApiKey $PSGalleryKey;
			}
			else { throw "the $($psd1.Name) manifest file contains errors."; }
		}
	}
	else { Write-Host "publishing to powershellgallery.com was intentionally ignored." -ForegroundColor DarkYellow; }
	Write-LineBreak;
}

#----------

Task "version" -alias "v" -description "This task increments the project's version numbers." `
-depends @("init") -action {
	#$msg = Show-Inputbox "enter your release notes." "RELEASE NOTES";
	#$msg = (& { if ([String]::IsNullOrEmpty($msg)) { return ""; } else { return $msg.Trim(); } });
	#$version = (Get-VersionNumber -config $ManifestPath).ToString($true);
	#$header = "version $version`n";
	#$header += [String]::Join("", [System.Linq.Enumerable]::Repeat("-", $header.Length - 1));
	#$releaseNotes = Get-Content $ReleaseNotesPath | Out-String;
	#if (-not [String]::IsNullOrEmpty($msg)) { "$header`n$msg`n`n`n$releaseNotes" | Out-File $ReleaseNotesPath -Encoding utf8; }
	
	Update-VersionNumber "$ProjectRoot\src" -config $ManifestPath -Major:$Major -Minor:$Minor -Patch;
}

Task "restore" -alias "setup" -description "This task restores all missing files." `
-depends @() -action { Exec { & "$PSScriptRoot\restore-missingFiles.ps1" $ConnectionStrings; } }

Task "icon" -description "This task pushed the project's logo to a web server." `
-depends @("init") -action {
	foreach ($img in (Get-ChildItem "$ProjectRoot\assets" -Recurse -Include @("*.png")))
	{
		$credentials = Get-Content "$PSScriptRoot\credentials.json" | Out-String | ConvertFrom-Json;
		$connStr = $credentials.ftp.Split(';');
		$host = $connStr[0].Split('=')[1];
		$user = $connStr[1].Split('=')[1];
		$password =  ConvertTo-SecureString $connStr[2].Split('=')[1] -AsPlainText -Force;
		
		$result = $session = New-WinSCPSession -HostName $host -Credential (New-Object PSCredential($user, $password)) -Protocol Ftp `
			| Send-WinSCPItem -Path $img.FullName -Destination "/$host/wwwroot/images/$($img.Name)";
		
		if ($result.IsSuccess)
		{ Write-Host "`t* '$($img.Name)' was uploaded to $host successfully." -ForegroundColor Green; }
		else
		{ throw "failed to update '$($img.Name)' $host." }
	}
}

#region ----- HELPER FUNCTIONS -----

function Get-MSBuildPath()
{
	$vsPath = Get-VSSetupInstance | Select-VSSetupInstance -Latest | Select-Object -ExpandProperty InstallationPath;
	$msbuild = Get-Item "$vsPath\MSBuild\*\Bin\MSBuild.exe";
	Assert ($msbuild) "unable to find msbuild.exe on this machine.";
	return $msbuild.FullName;
}

function Get-PackageSuffix()
{
	$suffix = Get-BranchSuffix $BranchName -config $ManifestPath;
	if ([String]::IsNullOrEmpty($suffix)) { return ""; } else { return "-$suffix"; }
}

#endregion
