Properties {
    
    # Paths
    $ProjectDirectory = (Split-Path $PSScriptRoot -Parent);
    $TempDirectory = "$ProjectDirectory\build\temp";
    $Nuget = "$ProjectDirectory\tools\nuget.exe";

    # Msbuild Args
    $BuildConfiguration = "Release";
    $BuildPlatform = "Any CPU";
}

Task default -depends Compile;

Task Init -description "Initialize the build n' deploy procedure." -action {
    Write-Host "`t* cleaning up directores...";
        foreach($dir in @($TempDirectory))
        {
            if(Test-Path $dir -PathType Container)
            {
               Remove-Item $dir -Force -Recurse;
               New-Item $dir -ItemType Directory | Out-Null;
            }
            else { New-Item $dir -ItemType Directory | Out-Null; }
        }
}

Task Compile -description "Build the solution." -depends Init -action {
    Assert("Debug", "Release" -contains $BuildConfiguration) "Value must be 'Debug' or 'Release'.";
    Assert("x86", "x64", "Any CPU" -contains $BuildPlatform) "Value must be 'x86', 'x64' or 'Any CPU'.";

    # Build Visual Studio solution.
    $solution = (Get-ChildItem -Path $ProjectDirectory -Filter "*.sln" -Recurse | Select-Object -ExpandProperty FullName -First 1); Write-Host "eee: $solution";
    Exec { msbuild $solution "/p:Configuration=$BuildConfiguration;Platform=$BuildPlatform" | Out-Null; }
}