using Ackara.Daterpillar.Scripting;
using ApprovalTests;
using ApprovalTests.Namers;
using ApprovalTests.Reporters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using System;
using System.Data;
using System.IO;
using static MSTest.Daterpillar.MockData;

namespace MSTest.Daterpillar.Tests
{
    [TestClass]
    [UseReporter(typeof(FileLauncherReporter), typeof(ClipboardReporter))]
    public class ScriptBuilderTest
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        [TestCategory(nameof(System.Data.SQLite))]
        public void Append_should_return_a_sqlite_script_that_creates_a_new_schema_when_invoked()
        {
            AppendSchema(new SQLiteScriptBuilder(), ConnectionFactory.CreateSQLiteConnection());
        }

        #region Helpers

        private void AppendSchema(IScriptBuilder builder, IDbConnection connection)
        {
            using (connection)
            {
                // Arrange
                var schema = MockData.GetSchema(scriptingTest_mock_schemaXML);

                // Act
                builder.Append(schema);
                var content = builder.GetContent();

                Helper.CreateEmptyDatabase(connection);
                var scriptWasExecutedSucessfully = connection.TryExecuteScript(content, out string errorMsg);

                var header = scriptWasExecutedSucessfully ? string.Empty :
                    string.Format("-- THE SCRIPT FAILED!!!{0}-- ERROR: {1}{0}{0}", Environment.NewLine, errorMsg);

                var script = Path.Combine(TestContext.DeploymentDirectory, $"{builder.GetType().Name}.{nameof(AppendSchema)}.sql");
                File.WriteAllText(script, string.Concat(header, content));
                
                // Assert
                content.ShouldNotBeNullOrWhiteSpace();
                Approvals.VerifyFile(script);
                scriptWasExecutedSucessfully.ShouldBeTrue(errorMsg);
            }
        }

        #endregion Helpers
    }
}