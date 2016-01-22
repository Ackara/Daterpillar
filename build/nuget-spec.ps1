# ----------------------------------------------------------------------
# This script generates .nuspec for each .csproj in the project
# in the directory excluding the test projects.
# ----------------------------------------------------------------------
Clear-Host;
$projectRootDir = [System.IO.Path]::GetDirectoryName($PSScriptRoot);
$nuget = [System.IO.Path]::Combine([System.IO.Path]::GetTempPath(), "nuget.exe");

# Download nuget.exe
& $([System.IO.Path]::Combine($PSScriptRoot, "get-nuget.ps1"));

# Generate .nuspec files
foreach($path in [System.IO.Directory]::GetFiles($projectRootDir, "*.csproj", [System.IO.SearchOption]::AllDirectories))
{
    $fileName = [System.IO.Path]::GetFileName($path);
    if($fileName.StartsWith("Test") -eq $false)
    {
        $specFile = [System.IO.Path]::ChangeExtension($path, ".nuspec");
        if([System.IO.File]::Exists($specFile) -eq $false)
        {
            Set-Location $([System.IO.Path]::GetDirectoryName($path));
            & $nuget spec;
        }
    }
}