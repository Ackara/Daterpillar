#
# restore_miissingFiles.ps1
#

Param(
	[Alias('connstr')]
	[hashtable]$ConnectionStrings
)

$projectRoot = Split-Path $PSScriptRoot -Parent;
$appConfig = Get-Item "$projectRoot\tests\MSTest*\app.config";

if (-not $appConfig)
{
	if (-not $ConnectionStrings)
	{
		$ConnectionStrings = @{};
		$ConnectionStrings.Add("ftp",   "server=localhost;user=your_username;password=your_password;");
		$ConnectionStrings.Add("mysql", "server=localhost;user=your_username;password=your_password;");
		$ConnectionStrings.Add("mssql", "server=localhost;user=your_username;password=your_password;");
		Write-Warning "Edit the <connectionString> entries in the app.config file." -ForegroundColor Yellow;
	}

	$entries = "";
	foreach ($item in $ConnectionStrings.GetEnumerator())
	{
		$entries += "<add name=`"$($item.Name)`" connectionString=`"$($item.Value)`" />`n`r"
	}

	$content = @"
	<?xml version="1.0" encoding="utf-8"?>

	<configuration>
		<configSections />
		<connectionStrings>
			$entries
		</connectionStrings>
	</configuration>
"@.Trim() | Out-File $appConfig -Encoding utf8;
	& $appConfig;
	Write-Host "`t* restored the '$($appConfig.FullName)' file.";
}
else { Write-Host "`t* all files are present." -ForegroundColor Green; }
