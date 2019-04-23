Param(
	[ValidateNotNullOrEmpty()]
	[Parameter(Mandatory)]
	[string]$Destination
)

$dll = Join-Path $PSScriptRoot "Daterpillar.dll";
[string]$templatePath = Join-Path $PSScriptRoot "*.tt" | Resolve-Path; 

[string]$content = Get-Content $templatePath | Out-String;
$content = $content -ireplace '<#@\s+assembly\s+name="nuget"\s+#>', "<#@ assembly name=`"$($dll)`" #>";
Out-File -InputObject $content -FilePath $Destination;
