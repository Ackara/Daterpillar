using ApprovalTests.Namers;
using ApprovalTests.Reporters;
using Acklann.Daterpillar.TextTransformation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.Daterpillar.Constants;
using Tests.Daterpillar.Helpers;

namespace Test.Daterpillar.Tests
{
    [TestClass]
    [DeploymentItem(SampleData.Folder)]
    [DeploymentItem(KnownFile.DbConfig)]
    [UseApprovalSubdirectory(nameof(ApprovalTests))]
    [UseReporter(typeof(DiffReporter), typeof(ClipboardReporter))]
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
            using (var connection = DatabaseHelper.CreateMSSQLConnection())
            {
                var settings = new ScriptBuilderSettings()
                {
                    AppendScripts = true,
                    AppendComments = true,
                    CreateDatabase = true,
                    TruncateDatabaseIfItExist = true
                };

                RunSchemaTest<TSQLScriptBuilder>(settings, connection);
            }
        }

        [TestMethod]
        [TestCategory(Trait.Integration)]
        public void Create_should_generate_a_tsql_script_that_adds_a_new_column_when_invoked()
        {
            using (var connection = DatabaseHelper.CreateMSSQLConnection())
            {
                RunColumnTest<TSQLScriptBuilder>(connection);
            }
        }

        [TestMethod]
        [Owner(Dev.Ackara)]
        [TestCategory(Trait.Integration)]
        public void Create_should_generate_a_tsql_script_that_adds_a_new_index_when_invoked()
        {
            using (var connection = DatabaseHelper.CreateMSSQLConnection())
            {
                RunIndexTest<TSQLScriptBuilder>(connection);
            }
        }

        [TestMethod]
        [Owner(Dev.Ackara)]
        [TestCategory(Trait.Integration)]
        public void Create_should_generate_a_tsql_script_that_adds_a_new_foreign_key_when_invoked()
        {
            using (var connection = DatabaseHelper.CreateMSSQLConnection())
            {
                RunForeignKeyTest<TSQLScriptBuilder>(connection);
            }
        }

        [TestMethod]
        [Owner(Dev.Ackara)]
        [TestCategory(Trait.Integration)]
        public void Drop_should_generate_a_tsql_script_that_removes_a_schema_when_invoked()
        {
            using (var connection = DatabaseHelper.CreateMSSQLConnection())
            {
                RunSchemaDropTest<TSQLScriptBuilder>(connection);
            }
        }

        [TestMethod]
        [Owner(Dev.Ackara)]
        [TestCategory(Trait.Integration)]
        public void Drop_should_generate_a_tsql_script_that_removes_a_column_when_invoked()
        {
            using (var connection = DatabaseHelper.CreateMSSQLConnection())
            {
                RunColumnDropTest<TSQLScriptBuilder>(connection);
            }
        }

        [TestMethod]
        [Owner(Dev.Ackara)]
        [TestCategory(Trait.Integration)]
        public void Drop_should_generate_a_tsql_script_that_removes_a_table_when_invoked()
        {
            using (var connection = DatabaseHelper.CreateMSSQLConnection())
            {
                RunTableDropTest<TSQLScriptBuilder>(connection);
            }
        }

        [TestMethod]
        [Owner(Dev.Ackara)]
        [TestCategory(Trait.Integration)]
        public void Drop_should_generate_a_tsql_script_that_removes_a_index_when_invoked()
        {
            using (var connection = DatabaseHelper.CreateMSSQLConnection())
            {
                RunIndexDropTest<TSQLScriptBuilder>(connection);
            }
        }

        [TestMethod]
        [Owner(Dev.Ackara)]
        [TestCategory(Trait.Integration)]
        public void Drop_should_generate_a_tsql_script_that_removes_a_foreign_key_when_invoked()
        {
            using (var connection = DatabaseHelper.CreateMSSQLConnection())
            {
                RunForeignKeyDropTest<TSQLScriptBuilder>(connection);
            }
        }

        [TestMethod]
        [Owner(Dev.Ackara)]
        [TestCategory(Trait.Integration)]
        public void Alter_should_generate_a_tsql_modify_script_for_a_table_when_invoked()
        {
            using (var connection = DatabaseHelper.CreateMSSQLConnection())
            {
                RunAlterTableTest<TSQLScriptBuilder>(connection);
            }
        }

        [TestMethod]
        [Owner(Dev.Ackara)]
        [TestCategory(Trait.Integration)]
        public void Alter_should_generate_a_tsql_modify_script_for_a_column_when_invoked()
        {
            using (var connection = DatabaseHelper.CreateMSSQLConnection())
            {
                RunAlterColumnTest<TSQLScriptBuilder>(connection);
            }
        }
    }
}