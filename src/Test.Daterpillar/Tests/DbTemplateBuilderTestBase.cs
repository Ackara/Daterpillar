using ApprovalTests;
using Gigobyte.Daterpillar;
using Gigobyte.Daterpillar.TextTransformation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data;
using System.Linq;
using Tests.Daterpillar.Constants;
using Tests.Daterpillar.Helpers;

namespace Test.Daterpillar.Tests
{
    public abstract class DbTemplateBuilderTestBase
    {
        internal const string DBNAME = "daterpillar1";

        protected void RunSchemaTest<T>(ScriptBuilderSettings settings, IDbConnection connection) where T : IScriptBuilder
        {
            // Arrange
            IScriptBuilder sut = (IScriptBuilder)Activator.CreateInstance(typeof(T), settings);
            var schema = Schema.Load(SampleData.GetFile(KnownFile.MockSchemaXML).OpenRead());

            // Act
            schema.Name = DBNAME;
            connection.TryDropDatabase(schema.Name);

            string errorMsg;
            sut.Create(schema);
            var script = sut.GetContent();
            var theScriptWorks = DatabaseHelper.TryRunScript(connection, script, out errorMsg);

            // Assert
            Approvals.Verify(script);
            Assert.IsTrue(theScriptWorks, errorMsg);
        }

        protected void RunIndexTest<T>(IDbConnection connection) where T : IScriptBuilder
        {
            // Arrange
            IScriptBuilder sut = (IScriptBuilder)Activator.CreateInstance(typeof(T));

            var schema = new Schema() { Name = DBNAME };
            var tbl1 = schema.CreateTable("tbl1");
            tbl1.CreateColumn("Col1");
            tbl1.CreateColumn("Col2");
            tbl1.CreateColumn("Col3");
            tbl1.CreateColumn("Col4");

            // Act
            DatabaseHelper.TryDropDatabase(connection, DBNAME);
            DatabaseHelper.CreateSchema(connection, sut, schema, false);

            Index index1 = tbl1.CreateIndex("idx1", IndexType.PrimaryKey, false, new IndexColumn("Col1"));
            Index index3 = tbl1.CreateIndex("idx3", IndexType.Index, true, new IndexColumn("Col4", SortOrder.DESC));
            Index index2 = tbl1.CreateIndex("idx2", IndexType.Index, false, new IndexColumn("Col2"), new IndexColumn("Col3"));

            string errorMsg;
            sut.Create(index1);
            sut.Create(index2);
            sut.Create(index3);
            var script = sut.GetContent();
            var theScriptsWorks = DatabaseHelper.TryRunScript(connection, script, out errorMsg);

            // Assert
            Approvals.Verify(script);
            Assert.IsTrue(theScriptsWorks, errorMsg);
        }

        protected void RunForeignKeyTest<T>(IDbConnection connection) where T : IScriptBuilder
        {
            // Arrange
            IScriptBuilder sut = (IScriptBuilder)Activator.CreateInstance(typeof(T));

            var schema = new Schema() { Name = DBNAME };
            var tbl1 = schema.CreateTable("tbl1");
            tbl1.CreateColumn("Id", new DataType("int"), autoIncrement: true);
            tbl1.CreateColumn("Name");

            var tbl2 = schema.CreateTable("tbl2");
            tbl2.CreateColumn("Id", new DataType("int"));
            tbl2.CreateColumn("Name");

            // Act
            DatabaseHelper.TryDropDatabase(connection, DBNAME);
            DatabaseHelper.CreateSchema(connection, sut, schema, false);

            var constraint = tbl2.CreateForeignKey("Id", "tbl1", "Id");
            sut.Create(constraint);

            string errorMsg;
            var script = sut.GetContent();
            var theScriptsWorks = DatabaseHelper.TryRunScript(connection, script, out errorMsg);

            // Assert
            Approvals.Verify(script);
            Assert.IsTrue(theScriptsWorks, errorMsg);
        }

        protected void RunSchemaDropTest<T>(IDbConnection connection) where T : IScriptBuilder
        {
            // Arrnage
            var sut = (IScriptBuilder)Activator.CreateInstance(typeof(T));
            var schema = Schema.Load(SampleData.GetFile(KnownFile.MockSchemaXML).OpenRead());

            // Act
            sut.Drop(schema);
            var script = sut.GetContent();

            string errorMsg;
            bool theScriptWorked = DatabaseHelper.TryRunScript(connection, script, out errorMsg);

            // Assert
            Approvals.Verify(script);
            Assert.IsTrue(theScriptWorked, errorMsg);
        }

        protected void RunTableDropTest<T>(IDbConnection connection) where T : IScriptBuilder
        {
            // Arrange
            var sut = (IScriptBuilder)Activator.CreateInstance(typeof(T));
            var schema = Schema.Load(SampleData.GetFile(KnownFile.MockSchemaXML).OpenRead());

            // Act
            DatabaseHelper.TryDropDatabase(connection, DBNAME);
            DatabaseHelper.CreateSchema(connection, sut, schema, false);

            string errorMsg;
            sut.Drop(schema.Tables.First());
            string script = sut.GetContent();
            bool theScriptWorked = DatabaseHelper.TryRunScript(connection, script, out errorMsg);

            // Assert
            Approvals.Verify(script);
            Assert.IsTrue(theScriptWorked);
        }

        protected void RunIndexDropTest<T>(IDbConnection connection)
        {
            // Arrange
            var sut = (IScriptBuilder)Activator.CreateInstance(typeof(T));
            var schema = Schema.Load(SampleData.GetFile(KnownFile.MockSchemaXML).OpenRead());

            // Act
            DatabaseHelper.TryDropDatabase(connection, DBNAME);
            DatabaseHelper.CreateSchema(connection, sut, schema, false);

            string errorMsg;
            sut.Drop(schema.GetIndexes().First());
            string script = sut.GetContent();
            bool theScriptWorked = DatabaseHelper.TryRunScript(connection, script, out errorMsg);

            // Assert
            Approvals.Verify(script);
            Assert.IsTrue(theScriptWorked);
        }

        protected void RunForeignKeyDropTest<T>(IDbConnection connection)
        {
            // Arrange
            var sut = (IScriptBuilder)Activator.CreateInstance(typeof(T));
            var schema = Schema.Load(SampleData.GetFile(KnownFile.MockSchemaXML).OpenRead());

            // Act
            DatabaseHelper.TryDropDatabase(connection, DBNAME);
            DatabaseHelper.CreateSchema(connection, sut, schema, false);

            string errorMsg;
            sut.Drop(schema.GetForeignKeys().First());
            string script = sut.GetContent();
            bool theScriptWorked = DatabaseHelper.TryRunScript(connection, script, out errorMsg);

            // Assert
            Approvals.Verify(script);
            Assert.IsTrue(theScriptWorked);
        }

        protected void RunAlterTableTest<T>(IDbConnection connection)
        {
            // Arrange
            var sut = (IScriptBuilder)Activator.CreateInstance(typeof(T));
            var schema = new Schema() { Name = DBNAME };

            var tbl1 = schema.CreateTable("tbl1");
            tbl1.CreateColumn("Col1");
            tbl1.CreateColumn("Col2");
            tbl1.CreateColumn("Col3");
            tbl1.CreateColumn("Col4");
            tbl1.CreateColumn("Col5");

            // Act
            DatabaseHelper.TryDropDatabase(connection, DBNAME);
            DatabaseHelper.CreateSchema(connection, sut, schema);

            var tbl2 = schema.CreateTable("tbl2");
            tbl2.CreateColumn("Col1");
            tbl2.CreateColumn("Col2");

            string errorMsg;
            sut.AlterTable(tbl1, tbl2);
            string script = sut.GetContent();
            bool theScriptsWorked = DatabaseHelper.TryRunScript(connection, script, out errorMsg);

            // Assert
            Approvals.Verify(script);
            Assert.IsTrue(theScriptsWorked);
        }

        protected void RunAlterColumnTest<T>(IDbConnection connection)
        {
            throw new System.NotImplementedException();
        }
    }
}