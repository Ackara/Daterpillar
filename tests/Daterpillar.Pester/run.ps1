Param(
	[string]$TestName = "*",
	[switch]$Force
)

Clear-Host;
if ($Force)
{
	try
	{
		Push-Location $PSScriptRoot;
		$projectFolder = "../../src/*.Powershell/bin" | Resolve-Path;
		if ($projectFolder -and (Test-Path $projectFolder)) { Remove-Item $projectFolder -Recurse -Force; }
	}
	finally { Pop-Location; }
}

foreach ($script in (Get-ChildItem $PSScriptRoot -Filter "*.tests.ps1"))
{
	Invoke-Pester -Script $script.FullName -TestName "*$TestName*";
}