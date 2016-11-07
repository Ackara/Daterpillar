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
    public class MySQLScriptBuilderTest : DbTemplateBuilderTestBase
    { 
        [ClassCleanup]
        public static void Cleanup()
        {
            ApprovalTests.Maintenance.ApprovalMaintenance.CleanUpAbandonedFiles();
        }

        [TestMethod]
        [Owner(Dev.Ackara)]
        [TestCategory(Trait.Integration)]
        public void Create_should_generate_a_mysql_script_that_builds_a_new_schema_when_settings_are_enabled()
        {
            using (var connection = DatabaseHelper.CreateMySQLConnection())
            {
                var settings = new ScriptBuilderSettings()
                {
                    AppendScripts = false,
                    AppendComments = true,
                    CreateDatabase = true,
                    TruncateDatabaseIfItExist = true
                };

                RunSchemaTest<MySQLScriptBuilder>(settings, connection);
            }
        }

        [TestMethod]
        [TestCategory(Trait.Integration)]
        public void Create_should_generate_a_mysql_script_that_adds_a_new_column_when_invoked()
        {
            using (var connection = DatabaseHelper.CreateMySQLConnection())
            {
                RunColumnTest<MySQLScriptBuilder>(connection);
            }
        }

        [TestMethod]
        [Owner(Dev.Ackara)]
        [TestCategory(Trait.Integration)]
        public void Create_should_generate_a_mysql_script_that_adds_a_new_index_when_invoked()
        {
            using (var connection = DatabaseHelper.CreateMySQLConnection())
            {
                RunIndexTest<MySQLScriptBuilder>(connection);
            }
        }

        [TestMethod]
        [Owner(Dev.Ackara)]
        [TestCategory(Trait.Integration)]
        public void Create_should_generate_a_mysql_script_that_adds_a_new_foreign_key_when_invoked()
        {
            using (var connection = DatabaseHelper.CreateMySQLConnection())
            {
                RunForeignKeyTest<MySQLScriptBuilder>(connection);
            }
        }

        [TestMethod]
        [Owner(Dev.Ackara)]
        [TestCategory(Trait.Integration)]
        public void Drop_should_generate_a_mysql_script_that_removes_a_schema_when_invoked()
        {
            using (var connection = DatabaseHelper.CreateMySQLConnection())
            {
                RunSchemaDropTest<MySQLScriptBuilder>(connection);
            }
        }

        [TestMethod]
        [Owner(Dev.Ackara)]
        [TestCategory(Trait.Integration)]
        public void Drop_should_generate_a_mysql_script_that_removes_a_column_when_invoked()
        {
            using (var connection = DatabaseHelper.CreateMySQLConnection())
            {
                RunColumnDropTest<MySQLScriptBuilder>(connection);
            }
        }

        [TestMethod]
        [Owner(Dev.Ackara)]
        [TestCategory(Trait.Integration)]
        public void Drop_should_generate_a_mysql_script_that_removes_a_table_when_invoked()
        {
            using (var connection = DatabaseHelper.CreateMySQLConnection())
            {
                RunTableDropTest<MySQLScriptBuilder>(connection);
            }
        }

        [TestMethod]
        [Owner(Dev.Ackara)]
        [TestCategory(Trait.Integration)]
        public void Drop_should_generate_a_mysql_script_that_removes_a_index_when_invoked()
        {
            using (var connection = DatabaseHelper.CreateMySQLConnection())
            {
                RunIndexDropTest<MySQLScriptBuilder>(connection);
            }
        }

        [TestMethod]
        [Owner(Dev.Ackara)]
        [TestCategory(Trait.Integration)]
        public void Drop_should_generate_a_mysql_script_that_removes_a_foreign_key_when_invoked()
        {
            using (var connection = DatabaseHelper.CreateMySQLConnection())
            {
                RunForeignKeyDropTest<MySQLScriptBuilder>(connection);
            }
        }

        [TestMethod]
        [Owner(Dev.Ackara)]
        [TestCategory(Trait.Integration)]
        public void Alter_should_generate_a_mysql_modify_script_for_a_table_when_invoked()
        {
            using (var connection = DatabaseHelper.CreateMySQLConnection())
            {
                RunAlterTableTest<MySQLScriptBuilder>(connection);
            }
        }

        [TestMethod]
        [Owner(Dev.Ackara)]
        [TestCategory(Trait.Integration)]
        public void Alter_should_generate_a_mysql_modify_script_for_a_column_when_invoked()
        {
            using (var connection = DatabaseHelper.CreateMySQLConnection())
            {
                RunAlterColumnTest<MySQLScriptBuilder>(connection);
            }
        }
    }
}