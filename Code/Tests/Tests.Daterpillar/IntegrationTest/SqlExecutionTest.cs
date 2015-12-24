using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Daterpillar.IntegrationTest
{
    [TestClass]
    [DeploymentItem(Artifact.MusicSQLiteSchema)]
    [DeploymentItem(Artifact.x86SQLiteInterop)]
    [DeploymentItem(Artifact.x64SQLiteInterop)]
    public class SqlExecutionTest
    {
        internal static string ConnectionString;

        [ClassInitialize]
        public static void Setup(TestContext context)
        {
            BuildSQLiteDatabase();
        }

        #region Private Members

        private static void BuildSQLiteDatabase()
        {
            string schema = Samples.GetFileContent(Artifact.MusicSQLiteSchema);
            using (var connection = DbFactory.CreateSQLiteConnection(schema))
            {
                ConnectionString = connection.ConnectionString;
                connection.Close();
            }
        }

        #endregion Private Members
    }
}