using Acklann.Daterpillar;
using Acklann.Daterpillar.Attributes;
using Acklann.Diffa;
using Acklann.Diffa.Reporters;

[assembly: Reporter(typeof(DiffReporter))]
[assembly: ApprovedFolder("approved-results")]
[assembly: Include(Sample.File.MusicDataXML)]