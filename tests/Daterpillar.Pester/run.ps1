Param([string]$TestName = "*")

Clear-Host;
foreach ($script in (Get-ChildItem $PSScriptRoot -Filter "*.tests.ps1"))
{
	Invoke-Pester -Script $script.FullName -TestName "*$TestName*";
}
