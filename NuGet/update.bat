CD %~dp0

SET fn=..\Code\Daterpillar.Core\Daterpillar.Core.nuspec
IF exist %fn% (
	nuget.exe pack %fn% -IncludeReferencedProjects
)