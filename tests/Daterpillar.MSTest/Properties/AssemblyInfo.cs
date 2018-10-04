using Acklann.Daterpillar;
using Acklann.Diffa;
using Acklann.Diffa.Reporters;

[assembly: Use(typeof(DiffReporter))]
[assembly: SaveFilesAt("approved-results")]
[assembly: Include(TestData.File.MusicDataXML)]