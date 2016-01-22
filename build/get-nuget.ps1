# ---------------------------------------------------
# This script downloads the NuGet.exe if not exist.
# ---------------------------------------------------
Clear-Host;
$nugetEXE = [System.IO.Path]::Combine([System.IO.Path]::GetTempPath(), "nuget.exe"); 
if([System.IO.File]::Exists($nugetEXE) -eq $false)
{
    echo "downloading nuget.exe";
    $client = New-Object -TypeName System.Net.WebClient;
    $client.DownloadFile("https://dist.nuget.org/win-x86-commandline/latest/nuget.exe", $nugetEXE);
    $client.Dispose();
    echo "nuget.exe download complete";

    # Set NuGet API key
    $apiKeyPath = [System.IO.Path]::Combine($env:LOCALAPPDATA, "API_Keys", "nuget.txt");
    if([System.IO.File]::Exists($apiKeyPath) -eq $true)
    {
        & $nugetEXE setApiKey $([System.IO.File]::ReadAllText($apiKeyPath));
    }
    else
    {
        echo "Unable to locate nuget api key at: $apiKeyPath";
    }
}