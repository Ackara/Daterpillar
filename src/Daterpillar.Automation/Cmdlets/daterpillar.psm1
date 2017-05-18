function New-DbConnection()
{
	Param(
		[Parameter(ValueFromPipeline=$true)]
		$ConnectionString
	)

	Write-Host "connStr: $ConnectionString";
}