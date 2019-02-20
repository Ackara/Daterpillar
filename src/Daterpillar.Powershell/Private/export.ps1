Param(
	[Parameter(Mandatory)]
	[ValidateScript({Test-Path $_})]
	[string]$AssemblyFile,

	[Parameter(Mandatory)]
	[ValidateScript({Test-Path $_})]
	[string]$ProjectDirectory
)

Join-Path $PSScriptRoot "*.psd1" | Resolve-Path | Import-Module -Force;
Export-Schema $ProjectDirectory $AssemblyFile | Out-Null;