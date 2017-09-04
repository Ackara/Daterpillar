﻿<#
.SYNOPSIS
This function genertes a xml schema from a dll assembly.

.DESCRIPTION
This function reads a specified assembly file, and generates a 'http://static.acklann.com/schema/v2/daterpillar.xsd' xml file
using the CLR types defined and decorated with the appropriate [System.Attribute] types.

.PARAMETER AssemblyFile
The absolute path to the assembly file.

.PARAMETER OutputPath
The absolute path to save the generated schema file.

.INPUTS
[String]

.OUTPUTS
None

.EXAMPLE
Convert-DllToSchema "path_to_dll_file.dll";
This example generates a schema file from from the types defined in the '.dll' file.

#>

Param(
	[Parameter(Mandatory)]
	[string]$AssemblyFile,

	[string]$OutputPath
)

if (Test-Path $AssemblyFile -PathType Leaf)
{
	$moduleName = "Acklann.Daterpillar";

	if (-not (Get-Module $moduleName))
	{

		if (Test-Path "$PSScriptRoot\$moduleName.dll" -PathType Leaf)
		{ 
			Import-Module "$PSScriptRoot\$moduleName.dll"; 
			Write-Verbose "imported '$PSScriptRoot\$moduleName.dll' module."; 
		}
		else
		{ 
			$assemblyDir = Split-Path $AssemblyFile -Parent;
			Import-Module "$assemblyDir\$moduleName.dll"; 
			Write-Verbose "imported '$assemblyDir\$moduleName.dll' module.";
		}
	}

	$dll = [System.Reflection.Assembly]::LoadFrom($AssemblyFile);
	[Acklann.Daterpillar.Schema]$schema = [Acklann.Daterpillar.AssemblyToSchemaConverter]::ToSchema($dll);

	if([String]::IsNullOrEmpty($OutputPath))
	{
		$OutputPath = [IO.Path]::ChangeExtension($AssemblyFile, "schema.xml");
	}
	$encoding = New-Object System.Text.UTF8Encoding $false;
	[System.IO.File]::WriteAllText($OutputPath, $schema.ToXml(), $encoding);

	Write-Verbose "Schema was created at '$OutputPath'.";
	return $OutputPath;
}
else { throw "Cannot find the given .dll file at '$AssemblyFile'." }