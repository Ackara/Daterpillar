CD %~dp0
FOR /r %%f in (*.txt) DO (
	DEL %%f 
)

SET fn=..\Code\Daterpillar.Core\Daterpillar.Core.csproj
IF exist %fn% (
	nuget.exe pack %fn%
)

SET fn=..\Code\Daterpillar.Core\Daterpillar.DotNet.csproj
IF exist %fn% (
	nuget.exe pack %fn%
)

PAUSE