Param(
	[ValidateNotNullOrEmpty()]
	[Parameter(Mandatory)]
	[string]$Destination
)

$dll = Join-Path $PSScriptRoot "Daterpillar.dll";
[string]$templatePath = Join-Path $PSScriptRoot "*.tt" | Resolve-Path; 

[string]$content = Get-Content $templatePath | Out-String;
$content = $content -ireplace '(?i)<#@ assembly name="(nuget|\$\(TargetDir\)\\Daterpillar\.dll)" #>', "<#@ assembly name=`"$($dll)`" #>";
Out-File -InputObject $content -FilePath $Destination;
