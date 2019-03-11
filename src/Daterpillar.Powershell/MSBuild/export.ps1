Param(
	[Parameter(Mandatory)]
	[ValidateScript({Test-Path $_})]
	[string]$AssemblyFile
)

Join-Path $PSScriptRoot "*.psd1" | Resolve-Path | Import-Module -Force;
$schemaxml = Export-DaterpillarSchema $AssemblyFile;
return $schemaxml.FullName;