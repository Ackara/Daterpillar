<#

.SYNOPSIS
The script add a 'user.config' file to the project if missing.

.PARAMETER MySQLConnection
The connection string to a MySQL database.

.PARAMETER MSSQLConnection
The connection string to a MSSQL database.

.PARAMETER Overwrite
Determines whether an existing file should be over written.

.EXAMPLE
New-UserConfig "server=localhost;user=john;=password=bacon;" "server=example.com;user=doe;password=applepie;";
This example adds a new 'user.config' file to the project.

#>

[CmdletBinding()]
Param(
    [Parameter(Mandatory=$true, Position=1)]
    [string]$MySQLConnectionString,

    [Parameter(Mandatory=$true, Position=2)]
    [string]$MSSQLConnectionString,

    [switch]$Overwrite
)

$rootDirectory = (Split-Path $PSScriptRoot -Parent);
$userConfig = "$rootDirectory\src\Test.Daterpillar\user.config";

Write-Host "config: '$userConfig'.";

$content = @"
<?xml version="1.0" encoding="utf-8" ?>

<configuration>
  <connectionStrings>
    <add name="mysql"
         providerName="MySql.Data.MySqlClient.MySqlConnection, MySql.Data.MySqlClient"
         connectionString="$MySQLConnectionString" />

    <add name="mssql"
         providerName="System.Data.SqlClient.SqlConnection, System.Data"
         connectionString="$MSSQLConnectionString" />
  </connectionStrings>
</configuration>
"@;

if(-not (Test-Path $userConfig -PathType Leaf))
{
    New-Item $userConfig -Value $content | Out-Null;
}
elseif ($Overwrite)
{
    if(Test-Path $userConfig -PathType Leaf)
    {
        Write-Host "Overriding '$userConfig'.";
        Remove-Item $userConfig;
        New-Item $userConfig -Value $content | Out-Null;
    }
}
