using Ackara.Daterpillar;
using Ackara.Daterpillar.Scripting;
using ApprovalTests;
using ApprovalTests.Namers;
using ApprovalTests.Reporters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace MSTest.Daterpillar.Tests
{
    [TestClass]
    [DeploymentItem(FName.Samples)]
    [DeploymentItem(FName.X86), DeploymentItem(FName.X64)]
    [UseReporter(typeof(DiffReporter), typeof(ClipboardReporter))]
    public class ScriptBuilderTest
    {
        public static IDictionary<string, Type> SqlScriptBuilders = new Dictionary<string, Type>()
        {
            {nameof(Syntax.MSSQL), typeof(MSSQLScriptBuilder) },
            {nameof(Syntax.MySQL), typeof(MySQLScriptBuilder) },
            {nameof(Syntax.SQLite), typeof(SQLiteScriptBuilder) }
        };

        public TestContext TestContext { get; set; }

        [ClassInitialize]
        public static void Setup(TestContext context)
        {
            using (var connection = ConnectionFactory.CreateMSSQLConnection(MOCKDB))
            {
                connection.UseSchema(FName.scriptingTest_partial_schemaXML);
            }

            using (var connection = ConnectionFactory.CreateMySQLConnection(MOCKDB))
            {
                connection.UseSchema(FName.scriptingTest_partial_schemaXML);
            }

            using (var connection = ConnectionFactory.CreateSQLiteConnection(MOCKDB))
            {
                connection.UseSchema(FName.scriptingTest_partial_schemaXML);
            }
        }

        [TestMethod]
        public void AppendLine_should_return_the_specified_string_passed_when_invoked()
        {
            // Arrange
            string input = "useful text";
            var sut = new MSSQLScriptBuilder();

            // Act
            sut.Append("this is ");
            sut.AppendLine(input);
            var results = sut.GetContent();

            // Assert
            results.ShouldMatch($"this is {input}\\s+");
        }

        [TestMethod]
        [DataSource(SourceName.supportedDatabases)]
        public void Append_should_return_a_sql_script_that_creates_a_new_schema_when_all_settings_are_enabled()
        {
            string dbType = Convert.ToString(TestContext.DataRow[0]);
            var builderType = SqlScriptBuilders[dbType];
            var builder = (IScriptBuilder)Activator.CreateInstance(builderType, new SqlScriptBuilderSettings()
            {
                ShowHeader = true,
                IgnoreScripts = false,
                IgnoreComments = false,
                AppendCreateSchemaCommand = true,
            });

            AppendSchema(builder, ConnectionFactory.CreateConnection(dbType, "dtpl_emptyDb"));
        }

        [TestMethod]
        [DataSource(SourceName.supportedDatabases)]
        public void Append_should_return_a_sql_script_that_creates_a_new_schema_when_all_settings_are_disabled()
        {
            string dbType = Convert.ToString(TestContext.DataRow[0]);
            var builderType = SqlScriptBuilders[dbType];
            var builder = (IScriptBuilder)Activator.CreateInstance(builderType, new SqlScriptBuilderSettings()
            {
                ShowHeader = false,
                IgnoreScripts = true,
                IgnoreComments = true,
                AppendCreateSchemaCommand = false,
            });

            AppendSchema(builder, ConnectionFactory.CreateConnection(dbType, "dtpl_emptyDb"));
        }

        [TestMethod]
        [DataSource(SourceName.supportedDatabases)]
        public void Append_should_return_a_sql_script_that_creates_a_new_table_when_invoked()
        {
            string dbType = Convert.ToString(TestContext.DataRow[0]);
            var builderType = SqlScriptBuilders[dbType];
            var builder = (IScriptBuilder)Activator.CreateInstance(builderType, new SqlScriptBuilderSettings()
            {
                ShowHeader = false,
                IgnoreScripts = true,
                IgnoreComments = true,
                AppendCreateSchemaCommand = false,
            });

            AppendTable(builder, ConnectionFactory.CreateConnection(dbType, MOCKDB));
        }

        [TestMethod]
        [DataSource(SourceName.supportedDatabases)]
        public void Append_should_return_a_sql_script_that_add_a_column_to_an_existing_table_when_invoked()
        {
            string dbType = Convert.ToString(TestContext.DataRow[0]);
            var builderType = SqlScriptBuilders[dbType];
            var builder = (IScriptBuilder)Activator.CreateInstance(builderType, new SqlScriptBuilderSettings()
            {
                ShowHeader = false,
                IgnoreScripts = true,
                IgnoreComments = true,
                AppendCreateSchemaCommand = false,
            });

            AppendColumn(builder, ConnectionFactory.CreateConnection(dbType, MOCKDB));
        }

        [TestMethod]
        [DataSource(SourceName.supportedDatabases)]
        public void Append_should_return_a_sql_script_that_add_a_foreign_key_to_an_existing_table_when_invoked()
        {
            string dbType = Convert.ToString(TestContext.DataRow[0]);
            var builderType = SqlScriptBuilders[dbType];
            var builder = (IScriptBuilder)Activator.CreateInstance(builderType, new SqlScriptBuilderSettings()
            {
                ShowHeader = false,
                IgnoreScripts = true,
                IgnoreComments = true,
                AppendCreateSchemaCommand = false,
            });

            AppendForeignKey(builder, ConnectionFactory.CreateConnection(dbType, MOCKDB));
        }

        [TestMethod]
        [DataSource(SourceName.supportedDatabases)]
        public void Append_should_return_a_sql_script_that_add_an_index_to_an_existing_table_when_invoked()
        {
            string dbType = Convert.ToString(TestContext.DataRow[0]);
            var builderType = SqlScriptBuilders[dbType];
            var builder = (IScriptBuilder)Activator.CreateInstance(builderType, new SqlScriptBuilderSettings()
            {
                ShowHeader = false,
                IgnoreScripts = true,
                IgnoreComments = true,
                AppendCreateSchemaCommand = false,
            });

            AppendIndex(builder, ConnectionFactory.CreateConnection(dbType, MOCKDB));
        }

        [TestMethod]
        [DataSource(SourceName.supportedDatabases)]
        public void Remove_should_return_a_sql_script_that_deletes_a_database_when_invoked()
        {
            // Arrange
            string dbType = Convert.ToString(TestContext.DataRow[0]);
            var builderType = SqlScriptBuilders[dbType];
            var builder = (IScriptBuilder)Activator.CreateInstance(builderType, new SqlScriptBuilderSettings()
            {
                ShowHeader = false,
                IgnoreScripts = true,
                IgnoreComments = true,
                AppendCreateSchemaCommand = false,
            });

            IDbConnection connection = ConnectionFactory.CreateConnection(dbType, "dtpl_deleteMe");
            var schema = new Schema();
            schema.Name = connection.Database;
            schema.Add(new Table("tbl",
                new Column("Id", new DataType("int")),
                new Column("Name", new DataType("varchar", 64))));

            // Act
            builder.Remove(schema);
            connection.UseEmptyDatabase();

            if (dbType.Contains(nameof(Syntax.MSSQL)))
            {
                if (connection.State != ConnectionState.Open) connection.Open();
                connection.ChangeDatabase("master");
            }

            // Assert
            Verify(builder, connection, nameof(Remove_should_return_a_sql_script_that_deletes_a_database_when_invoked));
        }

        [TestMethod]
        [DataSource(SourceName.supportedDatabases)]
        public void Remove_should_return_a_sql_script_that_deletes_an_existing_table_when_invoked()
        {
            string dbType = Convert.ToString(TestContext.DataRow[0]);
            var builderType = SqlScriptBuilders[dbType];
            var builder = (IScriptBuilder)Activator.CreateInstance(builderType, new SqlScriptBuilderSettings()
            {
                ShowHeader = false,
                IgnoreScripts = true,
                IgnoreComments = true,
                AppendCreateSchemaCommand = false,
            });

            RemoveTable(builder, ConnectionFactory.CreateConnection(dbType, MOCKDB));
        }

        [TestMethod]
        [DataSource(SourceName.supportedDatabases)]
        public void Remove_should_return_a_sql_script_that_deletes_an_existing_column_when_invoked()
        {
            string dbType = Convert.ToString(TestContext.DataRow[0]);
            var builderType = SqlScriptBuilders[dbType];
            var builder = (IScriptBuilder)Activator.CreateInstance(builderType, new SqlScriptBuilderSettings()
            {
                ShowHeader = false,
                IgnoreScripts = true,
                IgnoreComments = true,
                AppendCreateSchemaCommand = false,
            });

            RemoveColumn(builder, ConnectionFactory.CreateConnection(dbType, MOCKDB));
        }

        [TestMethod]
        [DataSource(SourceName.supportedDatabases)]
        public void Remove_should_return_a_sql_script_that_deletes_an_existing_index_when_invoked()
        {
            string dbType = Convert.ToString(TestContext.DataRow[0]);
            var builderType = SqlScriptBuilders[dbType];
            var builder = (IScriptBuilder)Activator.CreateInstance(builderType, new SqlScriptBuilderSettings()
            {
                ShowHeader = false,
                IgnoreScripts = true,
                IgnoreComments = true,
                AppendCreateSchemaCommand = false,
            });

            RemoveIndex(builder, ConnectionFactory.CreateConnection(dbType, MOCKDB));
        }

        [TestMethod]
        [DataSource(SourceName.supportedDatabases)]
        public void Remove_should_return_a_sql_script_that_deletes_an_existing_foreignKey_when_invoked()
        {
            string dbType = Convert.ToString(TestContext.DataRow[0]);
            var builderType = SqlScriptBuilders[dbType];
            var builder = (IScriptBuilder)Activator.CreateInstance(builderType, new SqlScriptBuilderSettings()
            {
                ShowHeader = false,
                IgnoreScripts = true,
                IgnoreComments = true,
                AppendCreateSchemaCommand = false,
            });

            RemoveForeignKey(builder, ConnectionFactory.CreateConnection(dbType, MOCKDB));
        }

        [TestMethod]
        [DataSource(SourceName.supportedDatabases)]
        public void Update_should_return_a_sql_script_that_alters_an_existing_column_when_invoked()
        {
            string dbType = Convert.ToString(TestContext.DataRow[0]);
            var builderType = SqlScriptBuilders[dbType];
            var builder = (IScriptBuilder)Activator.CreateInstance(builderType, new SqlScriptBuilderSettings()
            {
                ShowHeader = false,
                IgnoreScripts = true,
                IgnoreComments = true,
                AppendCreateSchemaCommand = false,
            });

            UpdateColumn(builder, ConnectionFactory.CreateConnection(dbType, MOCKDB));
        }

        #region Test Methods

        private const string MOCKDB = "dtpl_mockDb";

        private void AppendSchema(IScriptBuilder builder, IDbConnection connection, [CallerMemberName]string testName = null)
        {
            using (connection)
            {
                // Arrange
                var schema = MockData.GetSchema(FName.scriptingTest_partial_schemaXML);
                schema.Name = connection.Database;

                // Act
                builder.Append(schema);
                var content = builder.GetContent();

                connection.UseEmptyDatabase();
                var scriptWasExecutedSuccessfully = connection.TryExecuteScript(content, out string errorMsg);

                var header = scriptWasExecutedSuccessfully ? string.Empty :
                    string.Format("/*{0}THE SCRIPT FAILED!!!{0}{0}ERROR:{0}{1}*/{0}{0}", Environment.NewLine, errorMsg);

                var regex = new Regex(@"created\s*\w+:\s+\w+ \d{0,2},\s+\d{4}", RegexOptions.IgnoreCase);
                bool headerWasAppended = regex.IsMatch(content);
                content = (headerWasAppended ? regex.Replace(content, "CREATED ON:   May 11, 2017") : content);

                var pathToScriptFile = Path.Combine(Path.GetTempPath(), $"{testName}.sql");
                File.WriteAllText(pathToScriptFile, string.Concat(header, content));

                // Assert
                content.ShouldNotBeNullOrWhiteSpace();
                using (ApprovalResults.ForScenario(builder.GetType().Name))
                {
                    Approvals.VerifyFile(pathToScriptFile);
                }
                scriptWasExecutedSuccessfully.ShouldBeTrue(errorMsg);
            }
        }

        private void AppendTable(IScriptBuilder builder, IDbConnection connection, [CallerMemberName]string testName = null)
        {
            // Arrange
            var mockTable = new Table("archetype",
                new Column("Id", new DataType("int")),
                new Column("Name", new DataType("varchar", 256)),

                new Index(IndexType.PrimaryKey, new ColumnName("Id")),
                new Index(true, IndexType.Index, new ColumnName("Name", Order.Descending)));

            // Act
            builder.Append(mockTable);

            // Assert
            Verify(builder, connection, testName);
        }

        private void AppendColumn(IScriptBuilder builder, IDbConnection connection, [CallerMemberName]string testName = null)
        {
            // Arrange
            var column = new Column("Scale", new DataType("int"));
            column.Table = new Table("card");

            // Act
            builder.Append(column);

            // Assert
            Verify(builder, connection, testName);
        }

        private void AppendForeignKey(IScriptBuilder builder, IDbConnection connection, [CallerMemberName]string testName = null)
        {
            TestContext.WriteLine($"sut: {builder.GetType().Name}");

            // Arrange
            var schema = MockData.GetSchema(FName.scriptingTest_partial_schemaXML);
            var constraint = new ForeignKey("Card_Type_Id", "card_type", "Id", ReferentialAction.Cascade, ReferentialAction.Cascade);
            constraint.Table = schema.Tables.First(x => x.Name == "card");

            // Act
            builder.Append(constraint);

            // Assert
            Verify(builder, connection, testName);
        }

        private void AppendIndex(IScriptBuilder builder, IDbConnection connection, [CallerMemberName]string testName = null)
        {
            // Arrange
            var index = new Index(IndexType.Index, new ColumnName("Level", Order.Descending));
            index.Table = new Table("card");

            // Act
            builder.Append(index);

            // Assert
            Verify(builder, connection, testName);
        }

        /* ----- */

        private void RemoveTable(IScriptBuilder builder, IDbConnection connection, [CallerMemberName]string testName = null)
        {
            // Arrange
            Schema schema = MockData.GetSchema(FName.scriptingTest_partial_schemaXML);
            Table obsoleteTable = schema.Tables.First(x => x.Name == "ability");

            // Act
            builder.Remove(obsoleteTable);

            // Assert
            Verify(builder, connection, testName);
        }

        private void RemoveColumn(IScriptBuilder builder, IDbConnection connection, [CallerMemberName]string testName = null)
        {
            // Arrange
            Schema schema = MockData.GetSchema(FName.scriptingTest_partial_schemaXML);
            Column obsoleteColumn = schema.Tables.First(x => x.Name == "card_extras").Columns.First(x => x.Name == "Trivia");

            // Act
            builder.Remove(obsoleteColumn);

            // Assert
            Verify(builder, connection, testName);
        }

        private void RemoveIndex(IScriptBuilder builder, IDbConnection connection, [CallerMemberName]string testName = null)
        {
            // Arrange
            Schema schema = MockData.GetSchema(FName.scriptingTest_partial_schemaXML);
            Index obsoleteIndex = schema.Tables.First(x => x.Name == "pack").Indexes.First(x => x.Type == IndexType.Index);
            schema.Name = connection.Database;

            // Act
            builder.Remove(obsoleteIndex);

            // Assert
            Verify(builder, connection, testName);
        }

        private void RemoveForeignKey(IScriptBuilder builder, IDbConnection connection, [CallerMemberName]string testName = null)
        {
            // Arrange
            Schema schema = MockData.GetSchema(FName.scriptingTest_partial_schemaXML);
            ForeignKey obsoleteConstraint = schema.Tables.First(x => x.Name == "card_number").ForeignKeys.First(x => x.Name == "key_with_custom_name");

            // Act
            builder.Remove(obsoleteConstraint);

            // Assert
            Verify(builder, connection, testName);
        }

        /* ----- */

        private void UpdateTable(IScriptBuilder builder, IDbConnection connection, [CallerMemberName]string testName = null)
        {
            var schema = MockData.GetSchema(FName.scriptingTest_partial_schemaXML);
            var oldTable = schema.Tables.First(x => x.Name == "attribute");

            var newTable = new Table("attribute") { Schema = new Schema() };

            // Act
            builder.Update(oldTable, newTable);

            // Assert
            Verify(builder, connection, testName);
        }

        private void UpdateColumn(IScriptBuilder builder, IDbConnection connection, [CallerMemberName]string testName = null)
        {
            // Arrange
            var schema1 = MockData.GetSchema(FName.scriptingTest_partial_schemaXML);
            var oldColumn = schema1.Tables.First(x => x.Name == "card").Columns.First(x => x.Name == "Name");

            var schema2 = schema1.Clone();
            var newColumn = schema2.Tables.First(x => x.Name == "card").Columns.First(x => x.Name == "Name");
            var index = schema2.GetIndexes().First(x => x.Table.Name == "card" && x.Columns.Count(y => y.Name == "Name") == 1);
            newColumn.Name = "name_of_card";
            newColumn.DataType = new DataType("varchar", 256);
            index.Columns[0].Name = newColumn.Name;

            // Act
            builder.Update(oldColumn, newColumn);

            // Assert
            Verify(builder, connection, testName);
        }

        private void Verify(IScriptBuilder builder, IDbConnection connection, string testName)
        {
            using (connection)
            {
                var content = builder.GetContent();
                var scriptWasExecutedSuccessfully = connection.TryExecuteScript(content, out string errorMsg);

                var header = scriptWasExecutedSuccessfully ? string.Empty :
                    string.Format("/*{0}THE SCRIPT FAILED!!!{0}ERROR: {1}*/{0}{0}", Environment.NewLine, errorMsg);

                var pathToScriptFile = Path.Combine(Path.GetTempPath(), $"{testName}.sql");
                File.WriteAllText(pathToScriptFile, string.Concat(header, content));

                content.ShouldNotBeNullOrWhiteSpace();
                using (ApprovalResults.ForScenario(builder.GetType().Name))
                {
                    Approvals.VerifyFile(pathToScriptFile);
                }
                scriptWasExecutedSuccessfully.ShouldBeTrue(errorMsg);
            }
        }

        #endregion Test Methods
    }
}