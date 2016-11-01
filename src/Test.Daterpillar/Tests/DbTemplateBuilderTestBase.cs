using ApprovalTests;
using Gigobyte.Daterpillar;
using Gigobyte.Daterpillar.TextTransformation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data;
using Telerik.JustMock;
using Telerik.JustMock.Helpers;
using Tests.Daterpillar.Constants;
using Tests.Daterpillar.Helpers;

namespace Test.Daterpillar.Tests
{
    public abstract class DbTemplateBuilderTestBase
    {
        public void RunSchemaTest<T>(TemplateBuilderSettings settings, IDbConnection connection) where T : ITemplateBuilder
        {
            // Arrange
            var mockTypeResolver = Mock.Create<ITypeNameResolver>();
            mockTypeResolver.Arrange(x => x.GetName(Arg.IsAny<DataType>()))
                .Returns("string")
                .OccursAtLeast(1);

            ITemplateBuilder sut = (ITemplateBuilder)Activator.CreateInstance(typeof(T), settings, mockTypeResolver);

            var schema = Schema.Load(SampleData.GetFile(KnownFile.MockSchemaXML).OpenRead());

            // Act
            string errorMsg;
            sut.Create(schema);
            var script = sut.GetContent();
            var theScriptWorks = TryRunScript(connection, script, out errorMsg);

            // Assert
            mockTypeResolver.Assert();
            Approvals.Verify(script);
            Assert.IsTrue(theScriptWorks, errorMsg);
        }

        internal void RunIndexTest<T>(IDbConnection connection) where T : ITemplateBuilder
        {
            // Arrange
            

            var mockTypeResolver = Mock.Create<ITypeNameResolver>();
            mockTypeResolver.Arrange(x => x.GetName(Arg.IsAny<DataType>()))
                .Returns("string")
                .OccursAtLeast(1);

            ITemplateBuilder sut = (ITemplateBuilder)Activator.CreateInstance(typeof(T), mockTypeResolver);

            // Act
            string errorMsg;

            //sut.Create(index1);
            //sut.Create(index2);
            var script = sut.GetContent();
            var theScriptsWorks = TryRunScript(connection, script, out errorMsg);

            // Assert
            mockTypeResolver.Assert();
            Approvals.Verify(script);
            Assert.IsTrue(theScriptsWorks, errorMsg);
        }

        internal void RunForeignKeyTest()
        {
            throw new System.NotImplementedException();
        }

        internal void RunSchemaDropTest()
        {
            throw new System.NotImplementedException();
        }

        internal void RunTableDropTest()
        {
            throw new System.NotImplementedException();
        }

        internal void RunIndexDropTest()
        {
            throw new System.NotImplementedException();
        }

        internal void RunForeignKeyDropTest()
        {
            throw new System.NotImplementedException();
        }

        internal bool TryRunScript(IDbConnection connection, string sql, out string error)
        {
            error = "";
            IDbCommand command = null;

            try
            {
                if (connection.State != ConnectionState.Open) connection.Open();

                command = connection.CreateCommand();
                command.CommandText = sql;
                command.ExecuteNonQuery();

                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return false;
            }
            finally
            {
                command?.Dispose();
                connection?.Dispose();
            }
        }
    }
}