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
	[Parameter(Position=1)]
	[string]$MySQLConnectionString = "your_mysql_connection_string",

	[Parameter(Position=2)]
	[string]$MSSQLConnectionString = "your_mssql_connection_string",

	[switch]$Overwrite
)

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

$rootDirectory = (Split-Path $PSScriptRoot -Parent);
$databaseConfig = "$rootDirectory\src\Test.Daterpillar\database.config.xml";

if(-not (Test-Path $databaseConfig -PathType Leaf))
{
	New-Item $databaseConfig -Value $content -ItemType File | Out-Null;
	Write-Host "Created '$databaseConfig'.";
}
elseif ($Overwrite)
{
	if(Test-Path $databaseConfig -PathType Leaf)
	{
		Write-Host "Overwritten '$databaseConfig'.";
		Remove-Item $databaseConfig;
		New-Item $databaseConfig -Value $content -ItemType File | Out-Null;
	}
}
