function Send-FileToFtpServer
{
<#

.SYNOPSIS
This cmdlet uploads a file to an ftp server.

#>
    Param(
        [Parameter(Mandatory=$true)][string]$Username,
        [Parameter(Mandatory=$true)][string]$Password,
        [Parameter(Mandatory=$true)][string]$Path,
        [Parameter(Mandatory=$true)][string]$Destination,
        [Parameter(Mandatory=$false)][bool]$UsePassive = $false
    )
    
    # Test to see if the file being uploaded exist.
    if(([System.IO.File]::Exists($Path) -eq $false) -or ([System.Uri]::IsWellFormedUriString($Destination, [System.UriKind]::Absolute) -eq $false))
    {
        throw "Cannot perform operation, verify the boty values are correct. path:'$Path' destination: '$Destination'.";
    }

    # Create FTP request object.
    $request = [System.Net.FtpWebRequest]::Create("$Destination");
    $request.Method = [System.Net.WebRequestMethods+Ftp]::UploadFile;
    $request.Credentials = new-object System.Net.NetworkCredential($Username, $Password);
    $request.UsePassive = $UsePassive;
    $request.UseBinary = $true;
    
    # Convert to the file into bytes.
    $fileContent = Get-Content $Path -Encoding byte ;
    $request.ContentLength = $fileContent.Length;
    [System.IO.Stream]$requestStream;

    try
    {
        # Get stream request by bytes
        $requestStream = $request.GetRequestStream();
        $requestStream.Write($fileContent, 0, $fileContent.Length);
        $requestStream.Close();
        
        [System.Net.FtpWebResponse]$response = $request.GetResponse();
        Write-Verbose $response.StatusDescription;

        return $true;
    }
    catch
    {
        Write-Error $_;
        return $false;
    }
    finally
    {
        $request.Dispose();
    }
}

