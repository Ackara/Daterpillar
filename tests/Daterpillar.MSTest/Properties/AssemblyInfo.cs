using Acklann.Daterpillar;
using Acklann.Diffa;
using Acklann.Diffa.Reporters;

[assembly: Reporter(typeof(DiffReporter))]
[assembly: ApprovedFolder("approved-results")]
[assembly: Include(TestData.File.MusicDataXML)]