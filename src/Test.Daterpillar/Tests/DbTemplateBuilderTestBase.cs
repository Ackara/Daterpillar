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
            var theScriptWorked = DatabaseHelper.TryRunScript(connection, script, out errorMsg);
            System.Diagnostics.Debug.WriteLineIf(theScriptWorked, "** The script works! **");

            // Assert
            Approvals.Verify(script);
            Assert.IsTrue(theScriptWorked, errorMsg);
        }

        protected void RunColumnTest<T>(IDbConnection connection)
        {
            // Arrange
            var sut = (IScriptBuilder)Activator.CreateInstance(typeof(T));
            var schema = new Schema() { Name = DBNAME };
            var tbl1 = schema.CreateTable("tbl1");
            tbl1.CreateColumn("Id", new DataType("int"), autoIncrement: true);
            tbl1.CreateColumn("Name");

            // Act
            DatabaseHelper.TryDropDatabase(connection, schema.Name);
            DatabaseHelper.CreateSchema(connection, sut, schema);
            sut.Clear();

            string errorMsg;
            var address = tbl1.CreateColumn("Address", new DataType("varchar"));
            sut.Create(address);
            var script = sut.GetContent();
            bool theScriptWorked = DatabaseHelper.TryRunScript(connection, script, out errorMsg);
            System.Diagnostics.Debug.WriteLine(errorMsg);

            // Assert
            Approvals.Verify(script);
            Assert.IsTrue(theScriptWorked, errorMsg);
        }

        protected void RunIndexTest<T>(IDbConnection connection) where T : IScriptBuilder
        {
            // Arrange
            var sut = (IScriptBuilder)Activator.CreateInstance(typeof(T));

            var schema = new Schema() { Name = DBNAME };
            var tbl1 = schema.CreateTable("tbl1");
            tbl1.CreateColumn("Col1");
            tbl1.CreateColumn("Col2");
            tbl1.CreateColumn("Col3");
            tbl1.CreateColumn("Col4");

            // Act
            DatabaseHelper.TryDropDatabase(connection, schema.Name);
            DatabaseHelper.CreateSchema(connection, sut, schema);
            sut.Clear();

            Index index1 = tbl1.CreateIndex("idx1", IndexType.PrimaryKey, false, new IndexColumn("Col1"));
            Index index3 = tbl1.CreateIndex("idx3", IndexType.Index, true, new IndexColumn("Col4", SortOrder.DESC));
            Index index2 = tbl1.CreateIndex("idx2", IndexType.Index, false, new IndexColumn("Col2"), new IndexColumn("Col3"));

            string errorMsg;
            sut.Create(index1);
            sut.Create(index2);
            sut.Create(index3);
            var script = sut.GetContent();
            var theScriptWorked = DatabaseHelper.TryRunScript(connection, script, out errorMsg);
            System.Diagnostics.Debug.WriteLineIf(theScriptWorked, "** The script works! **");

            // Assert
            Approvals.Verify(script);
            Assert.IsTrue(theScriptWorked, errorMsg);
        }

        protected void RunForeignKeyTest<T>(IDbConnection connection) where T : IScriptBuilder
        {
            // Arrange
            var sut = (IScriptBuilder)Activator.CreateInstance(typeof(T));

            var schema = new Schema() { Name = DBNAME };
            var tbl1 = schema.CreateTable("tbl1");
            tbl1.CreateColumn("Id", new DataType("int"), true);
            tbl1.CreateColumn("Name");
            tbl1.CreateColumn("CategoryId", new DataType("int"));
            tbl1.CreateIndex("cat_idx", IndexType.Index, false, new IndexColumn("CategoryId"));

            var tbl2 = schema.CreateTable("tbl2");
            tbl2.CreateColumn("Id", new DataType("int"), autoIncrement: true);
            tbl2.CreateColumn("Name");

            // Act
            DatabaseHelper.TryDropDatabase(connection, schema.Name);
            DatabaseHelper.CreateSchema(connection, sut, schema, false);
            sut.Clear();

            var constraint = tbl1.CreateForeignKey("CategoryId", "tbl2", "Id", ForeignKeyRule.CASCADE, ForeignKeyRule.CASCADE);
            sut.Create(constraint);

            string errorMsg;
            var script = sut.GetContent();
            var theScriptWorked = DatabaseHelper.TryRunScript(connection, script, out errorMsg);
            System.Diagnostics.Debug.WriteLineIf(theScriptWorked, "** The script works! **");

            // Assert
            Approvals.Verify(script);
            Assert.IsTrue(theScriptWorked, errorMsg);
        }

        protected void RunSchemaDropTest<T>(IDbConnection connection) where T : IScriptBuilder
        {
            // Arrange
            var sut = (IScriptBuilder)Activator.CreateInstance(typeof(T));
            var schema = new Schema() { Name = DBNAME };
            var tbl1 = schema.CreateTable("tbl1");
            tbl1.CreateColumn("Id");
            tbl1.CreateColumn("Name");

            // Act
            DatabaseHelper.TryDropDatabase(connection, schema.Name);
            DatabaseHelper.CreateSchema(connection, sut, schema);
            sut.Clear();

            sut.Drop(schema);
            var script = sut.GetContent();

            string errorMsg;
            bool theScriptWorked = DatabaseHelper.TryRunScript(connection, script, out errorMsg);
            System.Diagnostics.Debug.WriteLineIf(theScriptWorked, "** The script works! **");

            // Assert
            Approvals.Verify(script);
            Assert.IsTrue(theScriptWorked, errorMsg);
        }

        public void RunColumnDropTest<T>(IDbConnection connection)
        {
            // Arrange
            var sut = (IScriptBuilder)Activator.CreateInstance(typeof(T));
            var schema = new Schema() { Name = DBNAME };
            var tbl1 = schema.CreateTable("tbl1");
            tbl1.CreateColumn("Id", new DataType("int"), true);
            tbl1.CreateColumn("Name");

            var tbl2 = schema.CreateTable("tbl2");
            tbl2.CreateColumn("Id", new DataType("int"), true);
            tbl2.CreateColumn("Name");
            var col = tbl2.CreateColumn("CategoryId");
            tbl2.CreateIndex("cat_idx", IndexType.Index, false, new IndexColumn("CategoryId"));
            tbl2.CreateForeignKey(col.Name, tbl1.Name, "Id");

            // Act
            DatabaseHelper.TryDropDatabase(connection, schema.Name);
            DatabaseHelper.CreateSchema(connection, sut, schema);
            sut.Clear();

            string errorMsg;
            sut.Drop(col);
            var script = sut.GetContent();
            var theScriptWorked = DatabaseHelper.TryRunScript(connection, script, out errorMsg);
            System.Diagnostics.Debug.WriteLine(errorMsg);

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
            DatabaseHelper.TryDropDatabase(connection, schema.Name);
            DatabaseHelper.CreateSchema(connection, sut, schema, false);
            sut.Clear();

            string errorMsg;
            sut.Drop(schema.Tables.First());
            string script = sut.GetContent();
            bool theScriptWorked = DatabaseHelper.TryRunScript(connection, script, out errorMsg);
            System.Diagnostics.Debug.WriteLineIf(theScriptWorked, "** The script works! **");

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
            DatabaseHelper.TryDropDatabase(connection, schema.Name);
            DatabaseHelper.CreateSchema(connection, sut, schema, false);
            sut.Clear();

            string errorMsg;
            sut.Drop(schema.GetIndexes().First());
            string script = sut.GetContent();
            bool theScriptWorked = DatabaseHelper.TryRunScript(connection, script, out errorMsg);
            System.Diagnostics.Debug.WriteLineIf(theScriptWorked, "** The script works! **");

            // Assert
            Approvals.Verify(script);
            Assert.IsTrue(theScriptWorked);
        }

        protected void RunForeignKeyDropTest<T>(IDbConnection connection)
        {
            // Arrange
            var sut = (IScriptBuilder)Activator.CreateInstance(typeof(T));
            var schema = new Schema() { Name = DBNAME };
            var tbl1 = schema.CreateTable("tbl1");
            tbl1.CreateColumn("Id", new DataType("int"), autoIncrement: true);
            tbl1.CreateColumn("Name");

            var tbl2 = schema.CreateTable("tbl2");
            tbl2.CreateColumn("Id", new DataType("int"), autoIncrement: true);
            tbl2.CreateColumn("Name");
            tbl2.CreateColumn("CategoryId", new DataType("int"));
            tbl2.CreateIndex("cat_idx", IndexType.Index, false, new IndexColumn("CategoryId"));
            var constraint = tbl2.CreateForeignKey("CategoryId", "tbl1", "Id", ForeignKeyRule.CASCADE, ForeignKeyRule.CASCADE);

            // Act
            DatabaseHelper.TryDropDatabase(connection, schema.Name);
            DatabaseHelper.CreateSchema(connection, sut, schema, false);
            sut.Clear();

            string errorMsg;
            sut.Drop(constraint);
            string script = sut.GetContent();
            bool theScriptWorked = DatabaseHelper.TryRunScript(connection, script, out errorMsg);
            System.Diagnostics.Debug.WriteLineIf(theScriptWorked, "** The script works! **");

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
            DatabaseHelper.TryDropDatabase(connection, schema.Name);
            DatabaseHelper.CreateSchema(connection, sut, schema);
            sut.Clear();

            var tbl2 = schema.CreateTable("tbl1b");
            tbl2.CreateColumn("Col1");
            tbl2.CreateColumn("Col2");

            string errorMsg;
            sut.AlterTable(tbl1, tbl2);
            string script = sut.GetContent();
            bool theScriptWorked = DatabaseHelper.TryRunScript(connection, script, out errorMsg);
            System.Diagnostics.Debug.WriteLineIf(theScriptWorked, "** The script works! **");

            // Assert
            Approvals.Verify(script);
            Assert.IsTrue(theScriptWorked);
        }

        protected void RunAlterColumnTest<T>(IDbConnection connection)
        {
            // Arrange
            var sut = (IScriptBuilder)Activator.CreateInstance(typeof(T));
            var schema = new Schema() { Name = DBNAME };

            var tbl1 = schema.CreateTable("tbl1");
            tbl1.CreateColumn("Col1");
            tbl1.CreateColumn("Col2");
            tbl1.CreateColumn("Col3");
            tbl1.CreateColumn("Col4");
            var col5 = tbl1.CreateColumn("Col5");

            // Act
            DatabaseHelper.TryDropDatabase(connection, schema.Name);
            DatabaseHelper.CreateSchema(connection, sut, schema);
            sut.Clear();

            // Assert
        }
    }
}