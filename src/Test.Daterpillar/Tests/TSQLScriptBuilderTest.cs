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
    [DeploymentItem(KnownFile.DbConfig)]
    [UseApprovalSubdirectory(nameof(ApprovalTests))]
    [UseReporter(typeof(FileLauncherReporter), typeof(ClipboardReporter))]
    public class TSQLScriptBuilderTest : DbTemplateBuilderTestBase
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
            var settings = new ScriptBuilderSettings()
            {
                AppendScripts = false,
                AppendComments = true,
                TruncateDatabaseIfItExist = true
            };

            RunSchemaTest<TSQLScriptBuilder>(settings, DatabaseHelper.CreateMSSQLConnection());
        }

        [TestMethod]
        [Owner(Dev.Ackara)]
        [TestCategory(Trait.Integration)]
        public void Create_should_generate_a_tsql_script_that_builds_a_new_schema_when_settings_are_disabled()
        {
            var settings = new ScriptBuilderSettings()
            {
                AppendScripts = false,
                AppendComments = false,
                TruncateDatabaseIfItExist = false
            };

            RunSchemaTest<TSQLScriptBuilder>(settings, DatabaseHelper.CreateMSSQLConnection());
        }

        
    }
}