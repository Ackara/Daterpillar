using ApprovalTests.Namers;
using ApprovalTests.Reporters;
using Gigobyte.Daterpillar.TextTransformation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.Daterpillar.Constants;
using Tests.Daterpillar.Helpers;

namespace Test.Daterpillar.Tests
{
    //[TestClass]
    [DeploymentItem(SampleData.Folder)]
    [DeploymentItem(KnownFile.DbConfig)]
    [UseApprovalSubdirectory(nameof(ApprovalTests))]
    [UseReporter(typeof(DiffReporter), typeof(ClipboardReporter))]
    public class SQLiteScriptBuilderTest : DbTemplateBuilderTestBase
    {
        [ClassCleanup]
        public static void Cleanup()
        {
            ApprovalTests.Maintenance.ApprovalMaintenance.CleanUpAbandonedFiles();
        }

        [TestMethod]
        [Owner(Dev.Ackara)]
        [TestCategory(Trait.Integration)]
        public void Create_should_generate_a_sqlite_script_that_builds_a_new_schema_when_settings_are_enabled()
        {
            using (var connection = DatabaseHelper.CreateSQLiteConnection())
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
        public void Create_should_generate_a_sqlite_script_that_adds_a_new_column_when_invoked()
        {
            using (var connection = DatabaseHelper.CreateSQLiteConnection())
            {
                RunColumnTest<TSQLScriptBuilder>(connection);
            }
        }

        [TestMethod]
        [Owner(Dev.Ackara)]
        [TestCategory(Trait.Integration)]
        public void Create_should_generate_a_sqlite_script_that_adds_a_new_index_when_invoked()
        {
            using (var connection = DatabaseHelper.CreateSQLiteConnection())
            {
                RunIndexTest<TSQLScriptBuilder>(connection);
            }
        }

        [TestMethod]
        [Owner(Dev.Ackara)]
        [TestCategory(Trait.Integration)]
        public void Create_should_generate_a_sqlite_script_that_adds_a_new_foreign_key_when_invoked()
        {
            using (var connection = DatabaseHelper.CreateSQLiteConnection())
            {
                RunForeignKeyTest<TSQLScriptBuilder>(connection);
            }
        }

        [TestMethod]
        [Owner(Dev.Ackara)]
        [TestCategory(Trait.Integration)]
        public void Drop_should_generate_a_sqlite_script_that_removes_a_schema_when_invoked()
        {
            using (var connection = DatabaseHelper.CreateSQLiteConnection())
            {
                RunSchemaDropTest<TSQLScriptBuilder>(connection);
            }
        }

        [TestMethod]
        [Owner(Dev.Ackara)]
        [TestCategory(Trait.Integration)]
        public void Drop_should_generate_a_sqlite_script_that_removes_a_column_when_invoked()
        {
            using (var connection = DatabaseHelper.CreateSQLiteConnection())
            {
                RunColumnDropTest<TSQLScriptBuilder>(connection);
            }
        }

        [TestMethod]
        [Owner(Dev.Ackara)]
        [TestCategory(Trait.Integration)]
        public void Drop_should_generate_a_sqlite_script_that_removes_a_table_when_invoked()
        {
            using (var connection = DatabaseHelper.CreateSQLiteConnection())
            {
                RunTableDropTest<TSQLScriptBuilder>(connection);
            }
        }

        [TestMethod]
        [Owner(Dev.Ackara)]
        [TestCategory(Trait.Integration)]
        public void Drop_should_generate_a_sqlite_script_that_removes_a_index_when_invoked()
        {
            using (var connection = DatabaseHelper.CreateSQLiteConnection())
            {
                RunIndexDropTest<TSQLScriptBuilder>(connection);
            }
        }

        [TestMethod]
        [Owner(Dev.Ackara)]
        [TestCategory(Trait.Integration)]
        public void Drop_should_generate_a_sqlite_script_that_removes_a_foreign_key_when_invoked()
        {
            using (var connection = DatabaseHelper.CreateSQLiteConnection())
            {
                RunForeignKeyDropTest<TSQLScriptBuilder>(connection);
            }
        }

        [TestMethod]
        [Owner(Dev.Ackara)]
        [TestCategory(Trait.Integration)]
        public void Alter_should_generate_a_sqlite_modify_script_for_a_table_when_invoked()
        {
            using (var connection = DatabaseHelper.CreateSQLiteConnection())
            {
                RunAlterTableTest<TSQLScriptBuilder>(connection);
            }
        }

        [TestMethod]
        [Owner(Dev.Ackara)]
        [TestCategory(Trait.Integration)]
        public void Alter_should_generate_a_sqlite_modify_script_for_a_column_when_invoked()
        {
            using (var connection = DatabaseHelper.CreateSQLiteConnection())
            {
                RunAlterColumnTest<TSQLScriptBuilder>(connection);
            }
        }
    }
}