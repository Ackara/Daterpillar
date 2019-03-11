Param(
	[Parameter(Mandatory)]
	[ValidateNotNullOrEmpty()]
	[string]$OldSchemaFilePath,

	[Parameter(Mandatory)]
	[ValidateScript({Test-Path $_})]
	[string]$NewSchemaFilePath,

	[Parameter(Mandatory)]
	[ValidateNotNullOrEmpty()]
	[string]$Destination,

	[Parameter(Mandatory)]
	[ValidateNotNullOrEmpty()]
	[string]$Language,

	[Parameter(Mandatory)]
	[ValidateNotNullOrEmpty()]
	[string]$OmitDropStatements
)

Join-Path $PSScriptRoot "*.psd1" | Resolve-Path | Import-Module -Force;
$result = New-DaterpillarMigrationScript $Language $Destination $OldSchemaFilePath $NewSchemaFilePath -OmitDropStatements:([Convert]::ToBoolean($OmitDropStatements));
return $result.Script;