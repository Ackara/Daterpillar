#
# restore_miissingFiles.ps1
#

Param(
	[Alias('connstr')]
	[hashtable]$ConnectionStrings
)

$projectRoot = Split-Path $PSScriptRoot -Parent;

if ((-not $ConnectionStrings) -or ($ConnectionStrings.Count -eq 0))
{
	$ConnectionStrings = @{};
	$ConnectionStrings.Add("ftp",   "server=localhost;user=your_username;password=your_password;");
	$ConnectionStrings.Add("mysql", "server=localhost;user=your_username;password=your_password;");
	$ConnectionStrings.Add("mssql", "server=localhost;user=your_username;password=your_password;");
}

$entries = "";
foreach ($item in $ConnectionStrings.GetEnumerator())
{
	$entries += "<add name=`"$($item.Name)`" connectionString=`"$($item.Value)`" />`n`r"
}

$appConfig = "$projectRoot\tests\MSTest.Daterpillar\app.config";
if (-not (Test-Path $appConfig -PathType Leaf))
{
	$content = @"
	<?xml version="1.0" encoding="utf-8"?>
	
	<configuration>
		<configSections />
		<connectionStrings>
			$entries
		</connectionStrings>
	</configuration>
"@.Trim() | Out-File $appConfig -Encoding utf8;
	Write-Host "`t* restored the 'app.config' file." -ForegroundColor Green;
    & $appConfig;
}
else { Write-Host "`t* app.config already exist." -ForegroundColor DarkGreen; }


$credentials = "$PSScriptRoot\credentials.json";
if (-not (Test-Path $credentials -PathType Leaf))
{
	$content = @"
{
	"ftp": "$($ConnectionStrings.ftp)",
	"mssql": "$($ConnectionStrings.mssql)",
	"mysql": "$($ConnectionStrings.mysql)"
}
"@ | Out-File $credentials -Encoding utf8;
	Write-Host "`t* restored the 'credentials.json' file." -ForegroundColor Green;
    & $credentials;
}
else { Write-Host "`t* credentials.json alread exist." -ForegroundColor DarkGreen; }