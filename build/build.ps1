Properties {
    # Credentials
    $Password = "";
    $Username = "";
    $NuGetKey = "";
    
    # Paths and URIs
    $ProjectDirectory = (Split-Path $PSScriptRoot -Parent);
    $TempDirectory = "$ProjectDirectory\build\temp";
    $BinDirectory = "$ProjectDirectory\build\bin";
    
    $NuGetSource = "https://www.nuget.org/api/v2/package";
    $NugetEXE = "$ProjectDirectory\tools\nuget.exe";
    $NuGetPackageDirectory = "$BinDirectory\\nupkg";
    
    # Msbuild Args
    $BuildConfiguration = "Release";
    $BuildPlatform = "Any CPU";
}

Task default -depends Init, Compile, Push-NuGetIconToCDN, Push-XmlSchemaToServer, Publish-NuGetPackages;

Task Init -description "Initialize the build n' deploy procedure." -action {
    Assert(Test-Path $ProjectDirectory -PathType Container) "`$ProjectionDirectory cannot be null or empty.";
    Import-Module "$ProjectDirectory\tools\ftp.psm1";

    # Cleanup directories.
    foreach($dir in @($BinDirectory, $NuGetPackageDirectory, $TempDirectory))
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
    Assert("Debug", "Release" -contains $BuildConfiguration) "'BuildConfiguration' must be 'Debug' or 'Release'.";
    Assert("x86", "x64", "Any CPU" -contains $BuildPlatform) "'BuildPlatform' must be 'x86', 'x64' or 'Any CPU'.";

    # Build Visual Studio solution.
    $solution = (Get-ChildItem $ProjectDirectory -Recurse -Filter "*.sln" | Select-Object -ExpandProperty FullName -First 1);
    Exec { msbuild $solution "/p:Configuration=$BuildConfiguration;Platform=$BuildPlatform" | Out-Null }
}

Task Merge-Modules -description "Merge command line tool .dll libraries into one .exe." -depends Compile -action {
    $ilmerge = "$ProjectDirectory\tools\ILMerge.exe";
    if(-not (Test-Path $ilmerge -PathType Leaf)) { Invoke-WebRequest "https://www.microsoft.com/en-us/download/confirmation.aspx?id=17630" -OutFile $ilmerge; }

    Push-Location "$ProjectDirectory\src\Daterpillar.CommandLine\bin\$BuildConfiguration\";

    try
    {
        $se = "$PWD\SelfContainedProgram.exe";
        $tem = "$PWD\Gigobyte.Daterpillar.CommandLine.exe";
        $tem1 = "C:\Users\Ackee\Projects\Daterpillar\src\Daterpillar.CommandLine\bin\Release\CommandLine.dll";
        & $ilmerge $($tem) $($tem1) /out:$($se) /target:winexe
    }
    finally { Pop-Location; }
    

}

Task Push-NuGetIconToCDN -description "Upload an image to https://cloudinary.com CDN." -depends Init -action {
    Assert(-not [String]::IsNullOrEmpty($Username)) "'Username' cannot be null or empty";
    Assert(-not [String]::IsNullOrEmpty($Password)) "'Password' cannot be null or empty";
    
    $icon = "$ProjectDirectory\daterpillar.png";
    if(Test-Path $icon -PathType Leaf)
    {
        Write-Host "`t* Uploading '$icon' to server...";
            ..\tools\Push-ItemWithWinSCP.ps1 -Hostname "gigobyte.com" -Username $Username -Password $Password -Path $icon -Destination "static.gigobyte.com/wwwroot/images/daterpillar.png";
    }
    else { throw "Could not find '$icon'."; }
}

Task Push-XmlSchemaToServer -description "Upload the 'xddl.xsd' file to the FTP server." -depend Init -action {
    Assert(-not [String]::IsNullOrEmpty($Username)) "'Username' cannot be null or empty";
    Assert(-not [String]::IsNullOrEmpty($Password)) "'FtpPassword' cannot be null or empty";

    $xddl = "$ProjectDirectory\src\xddl.xsd";
    if(Test-Path $xddl -PathType Leaf)
    {
        Write-Host "`t* Uploading '$xddl' to server...";
            ..\tools\Push-ItemWithWinSCP.ps1 -Hostname "gigobyte.com" -Username $Username -Password $Password -Path $xddl -Destination "static.gigobyte.com/wwwroot/schema/v1/xddl.xsd";
    }
    else { throw "Could not find '$xddl'."; }
}

Task Create-NuGetPackages -description "Create a nuget package for all non test projects." -depends Compile -action {
    Assert(Test-Path $NuGetPackageDirectory -PathType Container) "Could not find '$NuGetPackageDirectory'.";

    Push-Location $NuGetPackageDirectory;
    try
    {
        foreach($project in (Get-ChildItem "$ProjectDirectory\src" -Recurse -Filter "*.csproj" -Exclude @("*Test*") | Select-Object -ExpandProperty FullName))
        {
            $nuspec = [System.IO.Path]::ChangeExtension($project, ".nuspec");
            if(Test-Path $nuspec -PathType Leaf)
            {
                Exec { (& $NugetEXE pack $($project) -IncludeReferencedProjects -Prop Configuration=$($BuildConfiguration)) | Out-Null; }
            }
        }
    }
    finally { Pop-Location; }
}

Task Publish-NuGetPackages -description "Publish nuget packages to nuget.org" -depends Create-NuGetPackages -action {
    Assert(-not [System.String]::IsNullOrEmpty($NuGetSource)) "The 'NuGetSource' cannot be null nor empty.";
    Assert(-not [System.String]::IsNullOrEmpty($NuGetKey)) "The 'NugetKey' cannot be null nor empty.";
    
    foreach($package in (Get-ChildItem $NuGetPackageDirectory | Select-Object -ExpandProperty FullName))
    {
        Exec { & $NugetEXE push $($package) $($NuGetKey) -Source $($NuGetSource); }
    }
}
