Properties {
    # Credentials
    $NugetKey = $null;
    
    # Paths
    $ProjectDirectory = (Split-Path $PSScriptRoot -Parent);
    $NugetPackages = "$ProjectDirectory\build\nuget";
    $TempDirectory = "$ProjectDirectory\build\temp";
    $NugetEXE = "$ProjectDirectory\tools\nuget.exe";
    
    # Msbuild Args
    $BuildConfiguration = "Release";
    $BuildPlatform = "Any CPU";
}

Task default -depends  Init, Create-Packages, Publish-NuGetPackages;

Task Init -description "Initialize the build n' deploy procedure." -action {
    Assert(Test-Path $ProjectDirectory -PathType Container) "'ProjectDirectory' do not exist.";
    
    Write-Host "`t* cleaning up directores...";
        foreach($dir in @($TempDirectory, $NugetPackages))
        {
            if(Test-Path $dir -PathType Container)
            {
               Remove-Item $dir -Force -Recurse;
               New-Item $dir -ItemType Directory | Out-Null;
            }
            else { New-Item $dir -ItemType Directory | Out-Null; }
        }

    Write-Host "`t* importing nuget module...";
        $nugetModule = (Get-ChildItem "$ProjectDirectory\src\packages\Gigobyte.DevOps*\tools\nuget.psm1").FullName | Sort-Object $_ | Select-Object -Last 1;
        Import-Module $nugetModule;
}

Task Compile -description "Build the solution." -depends Init -action {
    Assert("Debug", "Release" -contains $BuildConfiguration) "Value must be 'Debug' or 'Release'.";
    Assert("x86", "x64", "Any CPU" -contains $BuildPlatform) "Value must be 'x86', 'x64' or 'Any CPU'.";

    # Build Visual Studio solution.
    $solution = (Get-ChildItem -Path $ProjectDirectory -Filter "*.sln" -Recurse | Select-Object -ExpandProperty FullName -First 1);
    Exec { msbuild $solution "/p:Configuration=$BuildConfiguration;Platform=$BuildPlatform" | Out-Null; }
}

Task Create-Packages -description "Create nuget packages." -depends Compile -action {
    Push-Location $NugetPackages;
    foreach($project in (Get-ChildItem "$ProjectDirectory\src" -Recurse -Filter "*.csproj" | Select-Object -ExpandProperty FullName))
    {
        $nuspec = [System.IO.Path]::ChangeExtension($project, ".nuspec");
        if(Test-Path $nuspec -PathType Leaf)
        {
            Write-Host "`t* packaging $(Split-Path $project -Leaf)...";
            Exec { (& $NugetEXE pack $($project) -IncludeReferencedProjects -Prop Configuration=$($BuildConfiguration)) | Out-Null; }
        }
    }
    Pop-Location;
}

Task Publish-NuGetPackages -description "Publish nuget packages to nuget.org" -action {
    Assert(-not [System.String]::IsNullOrEmpty($NugetKey)) "The 'NugetKey' cannot be null or empty.";

    foreach($package in (Get-ChildItem $NugetPackages))
    {
        Exec { (& $NugetEXE push $($package) $($NugetKey)) | Out-Null }
    }
}