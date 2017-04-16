<#
.SYNOPSIS
Psake build tasks.
#>

Properties {
	# Paths
	$RootDir = (Split-Path $PSScriptRoot -Parent);
	$ArtifactsDir = "$RootDir\artifacts";
    $BuildJson = "$RootDir\build\build.json";
    $Nuget = "";
    
	# User Args
    $TestNames = @();
	$NuGetKey = "";
	$VersionSuffix = "";
    $BuildConfiguration = "";
}

Task "default" -depends @("Setup");

Task "deploy" -description "This task will build, test then publish the project." `
-depends @("Build-Solution", "Run-Tests", "Publish-Packages");

# -----

Task "Init" -description "This task loads and creates all denpendencies." -action {
    $buildboxDir = (Get-Item "$RootDir\packages\Ackara.Buildbox.*\tools" | Sort-Object $_.Name | Select-Object -ExpandProperty FullName -Last 1);
    foreach ($module in @("semver.dll"))
    {
        $pathToModulue = Get-Item "$buildboxDir\*\*$module" | Select-Object -ExpandProperty FullName;
        if (Test-Path $pathToModulue -PathType Leaf)
        {
            Import-Module $pathToModulue -Force;
            Write-Host "`t* imported $(Split-Path $pathToModulue -Leaf) module.";
        }
    }

    $pester = "$RootDir\packages\Pester*\tools\Pester.psd1";
    if (Test-Path $pester -PathType Leaf)
    {
        Import-Module $pester -Force;
        Write-Host "`t* imported pester module.";
    }
}


Task "Setup" -description "This task will generate all missing/sensitive files missing from the project." -action {
}


Task "Increment-VersionNumber" -alias "version" -description "This task increments the patch version number within all neccessary files." `
-depends @("Init") -action {
    $ver = Get-VersionNumber -ConfigFile $BuildJson;
    echo $ver;
}

Task "Build-Solution" -alias "compile" -description "This task complites the solution." `
-depends @("Init") -action {
    Assert ("Debug", "Release" -contains $BuildConfiguration) "`$BuildConfiguration was '$BuildConfiguration' but expected 'Debug' or 'Release'.";

    $sln = Get-Item "$RootDir\*.sln" | Select-Object -ExpandProperty FullName;
    Exec { msbuild $sln "/p:Configuration=$BuildConfiguration;Platform=Any CPU"  "/v:minimal"; }; 
}


Task "Run-Tests" -alias "test" -description "This task runs all automated tests." `
-depends @() -action {
    Assert ("Debug", "Release" -contains $BuildConfiguration) "`$BuildConfiguration was '$BuildConfiguration' but expected 'Debug' or 'Release'.";

    foreach ($proj in (Get-ChildItem "$RootDir\tests" -Recurse -Filter "*.*proj" | Select-Object -ExpandProperty FullName))
    {
        Exec { & dotnet test $proj --configuration $BuildConfiguration --verbosity minimal; }
    }

    foreach ($script in (Get-ChildItem "$RootDir\tests" -Recurse -Filter "*test*.ps1" | Select-Object -ExpandProperty FullName))
    {
        Invoke-Pester -Script $script;
    }
}


Task "Create-Packages" -alias "pack" -description "This task creates all deployment artifacts." -action {
    $nupkgsDir = "$Artifacts\nupkgs";
    if (Test-Path $ArtifactsDir -PathType Container) { Remove-Item $ArtifactsDir -Recurse -Force; }
    New-Item $ArtifactsDir -ItemType Directory | Out-Null;

    foreach ($proj in (Get-ChildItem "$RootDir\src" -Recurse -Filter "*.*proj" | Select-Object -ExpandProperty FullName))
    {
        $nuspec = [IO.Path]::ChangeExtension($proj, ".nuspec");
        if (Test-Path $nuspec -PathType Leaf)
        {
            $properties = "";
            $properties += "Configuration=$BuildConfiguration";


            if ([String]::IsNullOrEmpty($VersionSuffix))
            { Exec { & $nuget $nuspec -OutputDirectory $nupkgsDir -Properties $properties -IncludeReferencedProjects; } }
            else
            { Exec { & $nuget $nuspec -OutputDirectory $nupkgsDir -Properties $properties -IncludeReferencedProjects -Suffix $VersionSuffix; } }
        }
    }
}


Task "Publish-Packages" -alias "publish" -description "This task deploys all deployment artifacts." `
-depends @("Create-Packages") -action {
    foreach ($nupkg in $ArtifactsDir)
    {
        if ([string]::IsNullOrEmpty($NuGetKey))
        { Exec { & $nuget push $nupkg; } }
        else
        { Exec { & $nuget push $nupkg -ApiKey $NuGetKey; } }
    }
}