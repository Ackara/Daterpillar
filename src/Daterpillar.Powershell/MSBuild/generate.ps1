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

$dropSwitch = ([Convert]::ToBoolean($OmitDropStatements));
Join-Path $PSScriptRoot "*.psd1" | Resolve-Path | Import-Module -Force;
New-MigrationScript $OldSchemaFilePath $NewSchemaFilePath $Destination $Language -OmitDropStatements:$dropSwitch | Out-Null;