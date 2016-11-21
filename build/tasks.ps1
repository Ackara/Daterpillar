Properties {
    # Paths
	$RootDirectory = (Split-Path $PSScriptRoot -Parent);
	$PackageDirectory = "$RootDirectory\build\packages";
	$ToolsDirectory = "$RootDirectory\tools";

    # Ftp Server
    $Server = $null;
    $Username = $null;
    $Password = $null;

	# Nuget
	$Nuget = "$RootDirectory\tools\nuget.exe";
	$NugetSource = $null;
	$NugetKey = $null;

	# Msbuild Args
	$BuildConfiguration = "Release";
	$BuildPlatform = "Any CPU";
}

Task VSTS -depends Init, Create-Packages, Publish-Packages, Tag-NewRelease;
Task default -depends Init, Build-Solution, Run-Tests, Create-Packages, Publish-Packages, Tag-NewRelease;

Task Init -description "Create and cleanup all working folders." `
-action{
	foreach($directory in @($PackageDirectory))
	{
		if(Test-Path $directory -PathType Container) { Remove-Item $directory -Recurse; }
		New-Item $directory -ItemType Directory | Out-Null;
	}

	# Import modules
	foreach($module in (Get-ChildItem $ToolsDirectory -Filter "*.psm1" | Select-Object -ExpandProperty FullName))
	{
		Import-Module $module -Force;
	}
}


Task Build-Solution -description "Build and compile the solution." `
-action {
	Assert("Debug", "Release" -contains $BuildConfiguration) "'BuildConfiguration' must be 'Debug' or 'Release'.";
	Assert("x86", "x64", "Any CPU" -contains $BuildPlatform) "'BuildPlatform' must be 'x86', 'x64' or 'Any CPU'.";

	# Build Visual Studio solution.
	$solution = (Get-ChildItem $RootDirectory -Recurse -Filter "*.sln" | Select-Object -ExpandProperty FullName -First 1);
	Exec { msbuild $solution "/p:Configuration=$BuildConfiguration;Platform=$BuildPlatform" | Out-Null }
}


Task Run-Tests -description "Run all automated tests." `
-action {
	$vstestEXE = Get-ChildItem "C:\Program Files (x86)\Microsoft Visual Studio*\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe" `
		| Select-Object -ExpandProperty FullName `
		| Sort-Object $_ | Select-Object -Last 1;

	Push-Location "$RootDirectory\src";
	Exec { & $vstestEXE "$RootDirectory\src\Test.Mockaroo\bin\$BuildConfiguration\Test.Mockaroo.dll" /Logger:trx; }
	Pop-Location;
}


Task Create-Packages -description "Create all packages." `
-depends Init `
-action {
	Assert("Debug", "Release" -contains $BuildConfiguration) "'BuildConfiguration' must be 'Debug' or 'Release'.";

	foreach($nuspec in (Get-ChildItem "$RootDirectory\src" -Filter "*.nuspec" -Recurse | Select-Object -ExpandProperty FullName))
	{
		$csproj = [IO.Path]::ChangeExtension($nuspec, ".csproj");
		if(Test-Path $csproj -PathType Leaf)
		{
			Exec { & $nuget pack $csproj -Prop Configuration=$BuildConfiguration -OutputDirectory $PackageDirectory -IncludeReferencedProjects; }
		}
	}
}


Task Publish-Packages -description "Publish all nuget packages." `
-depends Init `
-action {
	Assert(-not [String]::IsNullOrEmpty($NugetKey)) "'NugetKey' is not assigned a value.";
	Assert(-not [String]::IsNullOrEmpty($NugetSource)) "'NugetSource' is not assigned a value.";

	foreach($nupkg in (Get-ChildItem $PackageDirectory -Filter "*.nupkg" | Select-Object -ExpandProperty FullName))
	{
		Exec { & $nuget push $nupkg $NugetKey -Source $NugetSource; }
	}
}

Task Upload-XmlSchema -description "Upload schema.xsd to public server." `
-depends Init `
-action {
    Assert(-not [String]::IsNullOrEmpty($Server)) "'Server' is not assigned a value.";
    Assert(-not [String]::IsNullOrEmpty($Username)) "'Username' is not assigned a value.";
    Assert(-not [String]::IsNullOrEmpty($Password)) "'Password' is not assigned a value.";

    Send-WinSCPItems -From "$RootDirectory\src\daterpillar.xsd" -Host $Server -Path "/static.acklann.com/wwwroot/schema/v1/" -Username $Username -Password $Password;
    Send-WinSCPItems -From "$RootDirectory\daterpillar.png" -Host $Server -Path "/static.acklann.com/wwwroot/images/" -Username $Username -Password $Password;
}

Task Tag-NewRelease -description "Tag the repo with the current version number." `
-depends Init `
-action {
	$versionNumber = Get-VersionNumber;
	git tag v$versionNumber;
	git push origin v$versionNumber;
}