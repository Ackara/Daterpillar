using ApprovalTests.Namers;
using ApprovalTests.Reporters;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("MSTest.Daterpillar")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("MSTest.Daterpillar")]
[assembly: AssemblyCopyright("Copyright ©  2017")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: ComVisible(false)]

[assembly: Guid("2999adaf-682d-43fb-ae21-37241fe456ca")]

// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]

[assembly: UseApprovalSubdirectory(nameof(ApprovalTests))]
[assembly: UseReporter(typeof(DiffReporter), typeof(ClipboardReporter))]
