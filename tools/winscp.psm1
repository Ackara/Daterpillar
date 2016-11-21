
function Install-WinSCP()
{
<#

.SYNOPSIS
This script the downloads and installs WinSCP from nuget.org at the current location.

#>
    
    $winscp = Get-ChildItem -Filter "winscp*.exe" -Recurse;
    if (-not $winscp)
    {
        $tempDir = "$PWD\temp";
        $nupkg = "$PWD\winscp.zip";
        
        try
        {
            if (-not (Test-Path $nupkg -PathType Leaf))
            {
                Write-Verbose "Downloading 'WinSCP.nupkg' from nuget.org...";
                Invoke-WebRequest "https://www.nuget.org/api/v2/package/WinSCP/5.9.2" -OutFile $nupkg;
            }
    
            Expand-Archive $nupkg $tempDir;
    
            $winscpDir = "$PWD\winscp";
            if (-not (Test-Path $winscpDir -PathType Container)) { New-Item $winscpDir -ItemType Directory | Out-Null; }
            
            foreach ($file in (Get-ChildItem $tempDir -Include @("WinSCP.exe", "WinSCPnet.dll") -Recurse | Select-Object -ExpandProperty FullName))
            {
                $name = Split-Path $file -Leaf;
                Copy-Item $file "$winscpDir\$name";
            }
    
            Push-Location $winscpDir;
        }
        finally
        {
            if (Test-Path $nupkg -PathType Leaf) { Remove-Item $nupkg; }
            if (Test-Path $tempDir -PathType Container) { Remove-Item $tempDir -Recurse; }
        }
    }
    else
    {
        $winscpDir = Split-Path $winscp.FullName -Parent;
        if ($PWD.Path -ne $winscpDir) { Push-Location $winscpDir; }
    }
}

function Send-WinSCPItems()
{
<#

.SYNOPSIS
This function uploads files to an FTP server.

#>
    [CmdletBinding()]
    Param(
        [Alias("from", "src")]
        [Parameter(Mandatory=$true)]
        [string]$Source,

        [Parameter(Mandatory=$true)]
        [string]$Host,

        [Alias("to", "path")]
        [Parameter(Mandatory=$true)]
        [string]$Destination,

        [Parameter(Mandatory=$true)]
        [string]$Username,

        [Parameter(Mandatory=$true)]
        [string]$Password
    )

    BEGIN
    {
        [bool]$wasSucessful = $true;

        Install-WinSCP;
        Add-Type -Path "WinSCPnet.dll";
        Pop-Location;

        # Setup the connection
        $sessionArgs = New-Object WinSCP.SessionOptions;
        $sessionArgs.Protocol = ([WinSCP.Protocol]::Ftp);
        $sessionArgs.HostName = $Host;
        $sessionArgs.UserName = $Username;
        $sessionArgs.Password = $Password;
        
        $connection =New-Object WinSCP.Session;
        $connection.Open($sessionArgs);

        $transferArgs = New-Object WinSCP.TransferOptions;
        $transferArgs.TransferMode = [WinSCP.TransferMode]::Binary;
        $transferArgs.OverwriteMode = ([WinSCP.OverwriteMode]::Overwrite);
    }

    PROCESS
    {
        if ($connection.Opened)
        {
            try
            {
                Write-Verbose "Uploading '$Source' to $Host...";
                $outcome = $connection.PutFiles($Source, $Destination, $false, $transferArgs);
                Write-Verbose "sucessful!";

                # Throw an exception if a transfer failed.
                $outcome.Check();
            }
            catch
            {
                $wasSucessful = $false;
                Write-Warning $_.Exception.Message;
            }
        }
    }

    END
    {
        if ($connection -and $connection.Opened) { $connection.Dispose(); }
        return $wasSucessful;
    }
}
