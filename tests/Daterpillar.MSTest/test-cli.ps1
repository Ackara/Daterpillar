[string]$dll = Resolve-Path "..\..\src\*.CLI\bin\Debug\*\*.CLI.dll";

$from = "abc";
$to = "C:\Users\Ackeem\Projects\Ackara\Daterpillar\tests\Daterpillar.MSTest\bin\Debug\netcoreapp2.1\test-data\bad_schema.xml";
$v = "1.2";

Clear-Host;
&dotnet $dll migrate $from $to $v;


