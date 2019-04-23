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

$folders = $Language.Split(@(';', ',', ' '));
if ($folders.Length == 1)
{
	$result = New-DaterpillarMigrationScript $Language $Destination $OldSchemaFilePath $NewSchemaFilePath -OmitDropStatements:([Convert]::ToBoolean($OmitDropStatements));
	return $result.Warning;
}
elseif ($folders.Length -gt 1)
{
	$builder = [System.Text.StringBuilder]::new();
	foreach($lang in $folders)
	{
		[stirng]$dest = $Destination;
		if ([IO.Path]::HasExtension($Destination))
		{
			$dir = Split-Path $Destination -Parent;
			$dest = Join-Path $dir "$lang\$(Split-Path $Destination -Leaf)";
		}
		else
		{ $dest  = Join-Path $Destination $lang; }
		
		$result = New-DaterpillarMigrationScript $lang $dest $OldSchemaFilePath $NewSchemaFilePath -OmitDropStatements:([Convert]::ToBoolean($OmitDropStatements));
		$builder.AppendLine($result.Warning);
	}
	return $builder.ToString();
}