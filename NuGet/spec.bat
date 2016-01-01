CD %~dp0
SET name=Daterpillar.Utilities
nuget.exe spec ..\Code\%name%\%name%
START notepad ..\Code\%name%\%name%.nuspec
PAUSE