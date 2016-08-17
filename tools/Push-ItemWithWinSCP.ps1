<#

.SYNOPSIS
Upload an item(s) to a FTP server using WinSCP.

.PARAMETER Hostname
The address/hostname of the server.

.PARAMETER Username
The authorized username.

.PARAMETER Password
The password.

.PARAMETER Path
The path of the item to upload.

.PARAMETER Destination
The destination path.

.EXAMPLE
Push-ItemWithWinSCP "C:\somefolder\file.txt" "example.com/wwwroot/file.txt" -Hostname "example.com" -Username "your_username" -Password "your_password";
This example uploads a file to a FTP server.

#>
[CmdletBinding()]
Param(
	[Parameter(Mandatory=$true, ValueFromPipeline=$true, Position=1)]
	[string]$Path = "",
	
	[Parameter(Mandatory=$true)]
	[string]$Hostname = "",

	[Parameter(Mandatory=$true)]
	[string]$Username = "",

	[Parameter(Mandatory=$true)]
	[string]$Password = "",

	[Parameter(Mandatory=$true)]
	[string]$Destination = ""
)

BEGIN
{
    $extractor = New-Object -ComObject shell.application;

    $winscpExe = (Get-ChildItem $PWD -Filter "WinSCP.exe" -Recurse | Select-Object -ExpandProperty FullName -First 1);
    if (-not $winscpExe)
    {
        Write-Host "- Downloading 'WinSCP.exe' ...";
        $winscpZip = "$PWD\winscp-portable.zip";
        if (-not (Test-Path $winscpZip -PathType Leaf)) { Invoke-WebRequest "https://sourceforge.net/projects/winscp/files/WinSCP/5.9.1/WinSCP-5.9.1-Portable.zip/download" -OutFile $winscpZip; }

        $tempDir = "$PWD\winscp-portable";
        if (Test-Path $tempDir -PathType Container) { Remove-Item $tempDir -Recurse -Force; }
        New-Item $tempDir -ItemType Directory | Out-Null;

        $shellApplication = New-Object -ComObject shell.application;
        $shellApplication.NameSpace($tempDir).CopyHere(($shellApplication.NameSpace($winscpZip)).Items());

        $winscpExe = "$tempDir\WinSCP.exe";
    }

    $winscpDll = (Get-ChildItem $PWD -Filter "WinSCPnet.dll" -Recurse | Select-Object -ExpandProperty FullName -First 1);
    if (-not $winscpDll)
    {
        Write-Host "- Downloading 'WinSCP.dll' ...";
        $winscpZip = "$PWD\winscp.zip";
        if (-not (Test-Path $winscpZip -PathType Leaf)) { Invoke-WebRequest "https://www.nuget.org/api/v2/package/WinSCP/5.9.1" -OutFile $winscpZip; }

        $tempDir = "$PWD\winscp";
        if (Test-Path $tempDir -PathType Container) { Remove-Item $tempDir -Recurse -Force; }
        New-Item $tempDir -ItemType Directory | Out-Null;

        $shellApplication = New-Object -ComObject shell.application;
        $shellApplication.NameSpace($tempDir).CopyHere(($shellApplication.NameSpace($winscpZip)).Items());

        $winscpDll = "$tempDir\lib\WinSCPnet.dll";
    }

    Copy-Item $winscpExe -Destination "$(Split-Path $winscpDll -Parent)\winscp.exe";
    Import-Module $winscpDll;

    $sessionOptions = New-Object WinSCP.SessionOptions -Property @{
        Protocol = [WinSCP.Protocol]::Ftp
        HostName = $Hostname
        UserName = $Username
        Password = $Password
    }

    Write-Host "- Opening FTP connection ...";
    $session = New-Object WinSCP.Session;
    $session.Open($sessionOptions);
}

PROCESS
{
    Write-Host "`t- Uploading '$Path' at '$Destination' ...";
	$transferOptions = New-Object WinSCP.TransferOptions;
    $transferOptions.TransferMode = [WinSCP.TransferMode]::Binary;

    $transferResult = $session.PutFiles($Path, $Destination, $false, $transferOptions);
    if ($transferResult.IsSuccess) { Write-Host "`t`t** Success **" -ForegroundColor Green; } else { $transferResult.Check(); }
}

END
{
    $session.Dispose();
	Remove-Module WinSCPnet;
}