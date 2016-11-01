using ApprovalTests.Namers;
using ApprovalTests.Reporters;
using Gigobyte.Daterpillar.TextTransformation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.Daterpillar.Constants;
using Tests.Daterpillar.Helpers;

namespace Test.Daterpillar.Tests
{
    [TestClass]
    [DeploymentItem(SampleData.Folder)]
    [UseApprovalSubdirectory(nameof(ApprovalTests))]
    [UseReporter(typeof(FileLauncherReporter), typeof(ClipboardReporter))]
    public class MSSQLTemplateBuilderTest : DbTemplateBuilderTestBase
    {
        [ClassCleanup]
        public static void Cleanup()
        {
            ApprovalTests.Maintenance.ApprovalMaintenance.CleanUpAbandonedFiles();
        }

        [TestMethod]
        [Owner(Dev.Ackara)]
        [TestCategory(Trait.Integration)]
        public void Create_should_generate_a_tsql_script_that_builds_a_new_schema_when_settings_are_enabled()
        {
            var settings = new TemplateBuilderSettings()
            {
                AppendScripts = true,
                AppendComments = true,
                TruncateDatabaseIfItExist = true
            };

            RunSchemaTest<MSSQLTemplateBuilder>(settings, DatabaseHelper.CreateMSSQLConnection());
        }

        [TestMethod]
        [Owner(Dev.Ackara)]
        [TestCategory(Trait.Integration)]
        public void Create_should_generate_a_tsql_script_that_builds_a_new_schema_when_settings_are_disabled()
        {
            var settings = new TemplateBuilderSettings()
            {
                AppendScripts = false,
                AppendComments = false,
                TruncateDatabaseIfItExist = false
            };

            RunSchemaTest<MSSQLTemplateBuilder>(settings, DatabaseHelper.CreateMSSQLConnection());
        }

        
    }
}