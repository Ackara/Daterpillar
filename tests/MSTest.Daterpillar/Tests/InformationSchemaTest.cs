using Acklann.Daterpillar;
using Acklann.Daterpillar.Migration;
using ApprovalTests;
using ApprovalTests.Namers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using System;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;

namespace MSTest.Daterpillar.Tests
{
    [TestClass]
    [DeploymentItem(FName.X86)]
    [DeploymentItem(FName.Samples)]
    [DeploymentItem(FName.daterpillarXSD)]
    public class InformationSchemaTest
    {
        private const string DBNAME = "dtpl_info_mockDb";

        public TestContext TestContext { get; set; }

        [TestMethod]
        [DataSource(SourceName.supportedDatabases)]
        public void FetchSchema_should_return_a_schema_object_retrieved_from_an_existing_sql_database()
        {
            var connectionType = (Syntax)Enum.Parse(typeof(Syntax), TestContext.DataRow[0].ToString());
            TestContext.WriteLine("context: {0}", connectionType);
            using (var connection = ConnectionFactory.CreateConnection(connectionType, DBNAME))
            {
                RunTest(connection, InformationSchemaFactory.CreateInstance(connection));
            }
        }

        #region Private Members

        private void RunTest(IDbConnection connection, IInformationSchema sut, [CallerMemberName]string caller = null)
        {
            // Act
            var localSchema = MockData.GetSchema(FName.scriptingTest_partial_schemaXML);

            connection.UseSchema(FName.scriptingTest_partial_schemaXML);
            var remoteSchema = sut.FetchSchema();
            var xml = remoteSchema.ToXml();

            bool schemasAreIdentical = Compare(localSchema, remoteSchema);

            // Assert
            schemasAreIdentical.ShouldBeTrue();
            using (ApprovalResults.ForScenario(sut.GetType().Name))
            {
                Approvals.VerifyXml(xml);
            }
        }

        private bool Compare(Schema left, Schema right)
        {
            left.Tables = left.Tables.OrderBy(x => x.Name).ToList();
            right.Tables = left.Tables.OrderBy(x => x.Name).ToList();

            for (int t = 0; t < left.Tables.Count; t++)
            {
                if (left.Tables.Count != right.Tables.Count) return false;
                if (left.Tables[t].Name != right.Tables[t].Name) return false;
                if (left.Tables[t].Columns.Count != right.Tables[t].Columns.Count) return false;
                if (left.Tables[t].Indexes.Count != right.Tables[t].Indexes.Count) return false;
                if (left.Tables[t].ForeignKeys.Count != right.Tables[t].ForeignKeys.Count) return false;

                left.Tables[t].Indexes = left.Tables[t].Indexes.OrderBy(x => x.Name).ToList();
                right.Tables[t].Indexes = right.Tables[t].Indexes.OrderBy(x => x.Name).ToList();

                for (int i = 0; i < left.Tables[t].Indexes.Count; i++)
                {
                    if (left.Tables[t].Indexes[i].Name != right.Tables[t].Indexes[i].Name) return false;
                }

                left.Tables[t].ForeignKeys = left.Tables[t].ForeignKeys.OrderBy(x => x.Name).ToList();
                right.Tables[t].ForeignKeys = right.Tables[t].ForeignKeys.OrderBy(x => x.Name).ToList();

                for (int f = 0; f < left.Tables[t].ForeignKeys.Count; f++)
                {
                    if (left.Tables[t].ForeignKeys[f].Name != right.Tables[t].ForeignKeys[f].Name) return false;
                }
            }

            return true;
        }

        #endregion Private Members
    }
}