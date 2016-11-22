using ApprovalTests.Namers;
using ApprovalTests.Reporters;
using Acklann.Daterpillar.TextTransformation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using Tests.Daterpillar.Constants;
using Tests.Daterpillar.Helpers;

namespace Test.Daterpillar.Tests
{
    [TestClass]
    [DeploymentItem(SampleData.Folder)]
    [DeploymentItem(KnownFile.DbConfig)]
    [DeploymentItem(KnownFile.x86SQLiteInterop)]
    [UseApprovalSubdirectory(nameof(ApprovalTests))]
    [UseReporter(typeof(DiffReporter), typeof(ClipboardReporter))]
    public class SQLiteScriptBuilderTest : DbTemplateBuilderTestBase
    {
        private static readonly string DataSource = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "datapillar-test.db3");

        [ClassCleanup]
        public static void Cleanup()
        {
            ApprovalTests.Maintenance.ApprovalMaintenance.CleanUpAbandonedFiles();
#if !DEBUG
            if (System.IO.File.Exists(DataSource)) System.IO.File.Delete(DataSource);
#endif
        }

        [TestMethod]
        [Owner(Dev.Ackara)]
        [TestCategory(Trait.Integration)]
        public void Create_should_generate_a_sqlite_script_that_builds_a_new_schema_when_settings_are_enabled()
        {
            using (var connection = DatabaseHelper.CreateSQLiteConnection(DataSource))
            {
                var settings = new ScriptBuilderSettings()
                {
                    AppendScripts = true,
                    AppendComments = true,
                    CreateDatabase = true,
                    TruncateDatabaseIfItExist = true
                };

                RunSchemaTest<SQLiteScriptBuilder>(settings, connection);
            }
        }

        [TestMethod]
        [TestCategory(Trait.Integration)]
        public void Create_should_generate_a_sqlite_script_that_adds_a_new_column_when_invoked()
        {
            using (var connection = DatabaseHelper.CreateSQLiteConnection(DataSource))
            {
                RunColumnTest<SQLiteScriptBuilder>(connection);
            }
        }

        [TestMethod]
        [Owner(Dev.Ackara)]
        [TestCategory(Trait.Integration)]
        public void Create_should_generate_a_sqlite_script_that_adds_a_new_index_when_invoked()
        {
            using (var connection = DatabaseHelper.CreateSQLiteConnection(DataSource))
            {
                RunIndexTest<SQLiteScriptBuilder>(connection);
            }
        }

        [TestMethod]
        [Owner(Dev.Ackara)]
        [TestCategory(Trait.Integration)]
        public void Create_should_generate_a_sqlite_script_that_adds_a_new_foreign_key_when_invoked()
        {
            using (var connection = DatabaseHelper.CreateSQLiteConnection(DataSource))
            {
                RunForeignKeyTest<SQLiteScriptBuilder>(connection);
            }
        }

        [TestMethod]
        [Owner(Dev.Ackara)]
        [TestCategory(Trait.Integration)]
        public void Drop_should_generate_a_sqlite_script_that_removes_a_schema_when_invoked()
        {
            using (var connection = DatabaseHelper.CreateSQLiteConnection(DataSource))
            {
                RunSchemaDropTest<SQLiteScriptBuilder>(connection);
            }
        }

        [TestMethod]
        [Owner(Dev.Ackara)]
        [TestCategory(Trait.Integration)]
        public void Drop_should_generate_a_sqlite_script_that_removes_a_column_when_invoked()
        {
            using (var connection = DatabaseHelper.CreateSQLiteConnection(DataSource))
            {
                RunColumnDropTest<SQLiteScriptBuilder>(connection);
            }
        }

        [TestMethod]
        [Owner(Dev.Ackara)]
        [TestCategory(Trait.Integration)]
        public void Drop_should_generate_a_sqlite_script_that_removes_a_table_when_invoked()
        {
            using (var connection = DatabaseHelper.CreateSQLiteConnection(DataSource))
            {
                RunTableDropTest<SQLiteScriptBuilder>(connection);
            }
        }

        [TestMethod]
        [Owner(Dev.Ackara)]
        [TestCategory(Trait.Integration)]
        public void Drop_should_generate_a_sqlite_script_that_removes_a_index_when_invoked()
        {
            using (var connection = DatabaseHelper.CreateSQLiteConnection(DataSource))
            {
                RunIndexDropTest<SQLiteScriptBuilder>(connection);
            }
        }

        [TestMethod]
        [Owner(Dev.Ackara)]
        [TestCategory(Trait.Integration)]
        public void Drop_should_generate_a_sqlite_script_that_removes_a_foreign_key_when_invoked()
        {
            using (var connection = DatabaseHelper.CreateSQLiteConnection(DataSource))
            {
                RunForeignKeyDropTest<SQLiteScriptBuilder>(connection);
            }
        }

        [TestMethod]
        [Owner(Dev.Ackara)]
        [TestCategory(Trait.Integration)]
        public void Alter_should_generate_a_sqlite_modify_script_for_a_table_when_invoked()
        {
            using (var connection = DatabaseHelper.CreateSQLiteConnection(DataSource))
            {
                RunAlterTableTest<SQLiteScriptBuilder>(connection);
            }
        }

        [TestMethod]
        [Owner(Dev.Ackara)]
        [TestCategory(Trait.Integration)]
        public void Alter_should_generate_a_sqlite_modify_script_for_a_column_when_invoked()
        {
            using (var connection = DatabaseHelper.CreateSQLiteConnection(DataSource))
            {
                RunAlterColumnTest<SQLiteScriptBuilder>(connection);
            }
        }
    }
}