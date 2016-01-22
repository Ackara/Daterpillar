# ----------------------------------------------------------------------
# This script generates .nuspec for each .csproj in the project
# in the directory excluding the test projects.
# ----------------------------------------------------------------------
Clear-Host;
$publishPackages = $false;
$projectRootDir = [System.IO.Path]::GetDirectoryName($PSScriptRoot);
$nuget = [System.IO.Path]::Combine([System.IO.Path]::GetTempPath(), "nuget.exe");

# Download nuget.exe
& $([System.IO.Path]::Combine($PSScriptRoot, "get-nuget.ps1"));

# Generate and publish .nupkg files
foreach($path in [System.IO.Directory]::GetFiles($projectRootDir, "*.csproj", [System.IO.SearchOption]::AllDirectories))
{
    $fileName = [System.IO.Path]::GetFileName($path);
    if(($fileName.StartsWith("Test") -eq $false) -and ($fileName.Contains(".UWP.") -eq $false))
    {
        Set-Location $([System.IO.Path]::GetDirectoryName($path));
        & $nuget pack $($fileName) -Prop Configuration=Release -IncludeReferencedProjects;
        
        # Publish .nupkg
        if($publishPackages -eq $true)
        {
            $nupkg = Get-Nupkg([System.IO.Path]::GetDirectoryName($path));
            & $nuget push $($nupkg);
            [System.IO.File]::Delete($nupkg);
        }
    }
}

echo "DONE!";
Pause;

# ----------------------
# Functions
# ----------------------
function Get-Nupkg($dir)
{
    [string] $nupkg = $null;
    foreach($path in [System.IO.Directory]::GetFiles($dir, "*.nupkg", [System.IO.SearchOption]::TopDirectoryOnly))
    {
        $nupkg = $path;
        break;
    }

    return $nupkg;
}