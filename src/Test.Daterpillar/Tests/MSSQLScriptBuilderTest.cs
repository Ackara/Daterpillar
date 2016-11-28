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
    public class MSSQLScriptBuilderTest : DbTemplateBuilderTestBase
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

                RunSchemaTest<MSSQLScriptBuilder>(settings, connection);
            }
        }

        [TestMethod]
        [Owner(Dev.Ackara)]
        [TestCategory(Trait.Integration)]
        public void Create_should_generate_a_tsql_script_that_builds_a_new_schema_when_settings_are_disabled()
        {
            using (var connection = DatabaseHelper.CreateMSSQLConnection())
            {
                var settings = new ScriptBuilderSettings()
                {
                    AppendScripts = false,
                    AppendComments = false,
                    CreateDatabase = false,
                    TruncateDatabaseIfItExist = false
                };

                RunSchemaTestWithDisabledSettings<MSSQLScriptBuilder>(settings, connection);
            }
        }

        [TestMethod]
        [TestCategory(Trait.Integration)]
        public void Create_should_generate_a_tsql_script_that_adds_a_new_column_when_invoked()
        {
            using (var connection = DatabaseHelper.CreateMSSQLConnection())
            {
                RunColumnTest<MSSQLScriptBuilder>(connection);
            }
        }

        [TestMethod]
        [Owner(Dev.Ackara)]
        [TestCategory(Trait.Integration)]
        public void Create_should_generate_a_tsql_script_that_adds_a_new_index_when_invoked()
        {
            using (var connection = DatabaseHelper.CreateMSSQLConnection())
            {
                RunIndexTest<MSSQLScriptBuilder>(connection);
            }
        }

        [TestMethod]
        [Owner(Dev.Ackara)]
        [TestCategory(Trait.Integration)]
        public void Create_should_generate_a_tsql_script_that_adds_a_new_foreign_key_when_invoked()
        {
            using (var connection = DatabaseHelper.CreateMSSQLConnection())
            {
                RunForeignKeyTest<MSSQLScriptBuilder>(connection);
            }
        }

        [TestMethod]
        [Owner(Dev.Ackara)]
        [TestCategory(Trait.Integration)]
        public void Drop_should_generate_a_tsql_script_that_removes_a_schema_when_invoked()
        {
            using (var connection = DatabaseHelper.CreateMSSQLConnection())
            {
                RunSchemaDropTest<MSSQLScriptBuilder>(connection);
            }
        }

        [TestMethod]
        [Owner(Dev.Ackara)]
        [TestCategory(Trait.Integration)]
        public void Drop_should_generate_a_tsql_script_that_removes_a_column_when_invoked()
        {
            using (var connection = DatabaseHelper.CreateMSSQLConnection())
            {
                RunColumnDropTest<MSSQLScriptBuilder>(connection);
            }
        }

        [TestMethod]
        [Owner(Dev.Ackara)]
        [TestCategory(Trait.Integration)]
        public void Drop_should_generate_a_tsql_script_that_removes_a_table_when_invoked()
        {
            using (var connection = DatabaseHelper.CreateMSSQLConnection())
            {
                RunTableDropTest<MSSQLScriptBuilder>(connection);
            }
        }

        [TestMethod]
        [Owner(Dev.Ackara)]
        [TestCategory(Trait.Integration)]
        public void Drop_should_generate_a_tsql_script_that_removes_a_index_when_invoked()
        {
            using (var connection = DatabaseHelper.CreateMSSQLConnection())
            {
                RunIndexDropTest<MSSQLScriptBuilder>(connection);
            }
        }

        [TestMethod]
        [Owner(Dev.Ackara)]
        [TestCategory(Trait.Integration)]
        public void Drop_should_generate_a_tsql_script_that_removes_a_foreign_key_when_invoked()
        {
            using (var connection = DatabaseHelper.CreateMSSQLConnection())
            {
                RunForeignKeyDropTest<MSSQLScriptBuilder>(connection);
            }
        }

        [TestMethod]
        [Owner(Dev.Ackara)]
        [TestCategory(Trait.Integration)]
        public void Alter_should_generate_a_tsql_modify_script_for_a_table_when_invoked()
        {
            using (var connection = DatabaseHelper.CreateMSSQLConnection())
            {
                RunAlterTableTest<MSSQLScriptBuilder>(connection);
            }
        }

        [TestMethod]
        [Owner(Dev.Ackara)]
        [TestCategory(Trait.Integration)]
        public void Alter_should_generate_a_tsql_modify_script_for_a_column_when_invoked()
        {
            using (var connection = DatabaseHelper.CreateMSSQLConnection())
            {
                RunAlterColumnTest<MSSQLScriptBuilder>(connection);
            }
        }
    }
}