using ApprovalTests;
using Gigobyte.Daterpillar;
using Gigobyte.Daterpillar.TextTransformation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data;
using Tests.Daterpillar.Constants;
using Tests.Daterpillar.Helpers;

namespace Test.Daterpillar.Tests
{
    public abstract class DbTemplateBuilderTestBase
    {
        private const string DBNAME = "daterpillar1";

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
            System.Diagnostics.Debug.WriteLine(errorMsg);

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
            sut.Create(tbl1.CreateColumn("Address", new DataType("varchar"), autoIncrement: false, nullable: false, comment: "This is a comment"));
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
            System.Diagnostics.Debug.WriteLine(errorMsg);

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

            var constraint = new ForeignKey()
            {
                LocalColumn = "CategoryId",
                ForeignTable = "tbl2",
                ForeignColumn = "Id",
                OnUpdate = ForeignKeyRule.CASCADE,
                OnDelete = ForeignKeyRule.CASCADE,
                TableRef = tbl1
            };

            sut.Create(constraint);

            string errorMsg;
            var script = sut.GetContent();
            var theScriptWorked = DatabaseHelper.TryRunScript(connection, script, out errorMsg);
            System.Diagnostics.Debug.WriteLine(errorMsg);

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

            // Assert
            Approvals.Verify(script);
        }

        protected void RunColumnDropTest<T>(IDbConnection connection)
        {
            // Arrange
            var sut = (IScriptBuilder)Activator.CreateInstance(typeof(T));
            var schema = new Schema() { Name = DBNAME };
            var tbl1 = schema.CreateTable("tbl1");
            tbl1.CreateColumn("Id", new DataType("int"), autoIncrement: true);
            tbl1.CreateColumn("Name");
            var col = tbl1.CreateColumn("RefCol", new DataType("int"));

            var tbl2 = schema.CreateTable("tbl2");
            tbl2.CreateColumn("Id", new DataType("int"), autoIncrement: true);
            tbl2.CreateColumn("Name");
            tbl2.CreateColumn("CategoryId", new DataType("int"));
            tbl2.CreateIndex("cat_idx", IndexType.Index, false, new IndexColumn("CategoryId"));
            tbl2.CreateForeignKey("CategoryId", tbl1.Name, col.Name);

            // Act
            DatabaseHelper.TryDropDatabase(connection, schema.Name);
            DatabaseHelper.CreateSchema(connection, sut, schema);
            sut.Clear();

            sut.Drop(col);
            var script = sut.GetContent();
            string errorMsg;
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
            var schema = new Schema() { Name = DBNAME };
            var tbl1 = schema.CreateTable("tbl1");
            tbl1.CreateColumn("Id", new DataType("int"), autoIncrement: true);
            tbl1.CreateColumn("Name");

            // Act
            DatabaseHelper.TryDropDatabase(connection, schema.Name);
            DatabaseHelper.CreateSchema(connection, sut, schema, false);
            sut.Clear();

            string errorMsg;
            sut.Drop(tbl1);
            string script = sut.GetContent();
            bool theScriptWorked = DatabaseHelper.TryRunScript(connection, script, out errorMsg);
            System.Diagnostics.Debug.WriteLine(errorMsg);

            // Assert
            Approvals.Verify(script);
            Assert.IsTrue(theScriptWorked);
        }

        protected void RunIndexDropTest<T>(IDbConnection connection)
        {
            // Arrange
            var sut = (IScriptBuilder)Activator.CreateInstance(typeof(T));
            var schema = new Schema() { Name = DBNAME };
            var tbl1 = schema.CreateTable("tbl1");
            tbl1.CreateColumn("Col1");
            var idx1 = tbl1.CreateIndex("idx1", IndexType.Index, false, new IndexColumn("Col1"));

            // Act
            DatabaseHelper.TryDropDatabase(connection, schema.Name);
            DatabaseHelper.CreateSchema(connection, sut, schema, false);
            sut.Clear();

            string errorMsg;
            sut.Drop(idx1);
            string script = sut.GetContent();
            bool theScriptWorked = DatabaseHelper.TryRunScript(connection, script, out errorMsg);
            System.Diagnostics.Debug.WriteLine(errorMsg);

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
            DatabaseHelper.CreateSchema(connection, sut, schema);
            sut.Clear();

            string errorMsg;
            sut.Drop(constraint);
            string script = sut.GetContent();
            bool theScriptWorked = DatabaseHelper.TryRunScript(connection, script, out errorMsg);
            System.Diagnostics.Debug.WriteLine(errorMsg);

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
            tbl1.Comment = "This is comment.";
            tbl1.CreateColumn("Id", new DataType("int"));
            tbl1.CreateColumn("Name");

            // Act
            DatabaseHelper.TryDropDatabase(connection, schema.Name);
            DatabaseHelper.CreateSchema(connection, sut, schema);
            sut.Clear();

            var tbl2 = schema.CreateTable("tbl2");
            tbl2.Comment = "The comment was changed.";

            string errorMsg;
            sut.AlterTable(tbl1, tbl2);
            string script = sut.GetContent();
            bool theScriptWorked = DatabaseHelper.TryRunScript(connection, script, out errorMsg);
            System.Diagnostics.Debug.WriteLine(errorMsg);

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
            tbl1.CreateColumn("Id", new DataType("int"));
            var originalCol = tbl1.CreateColumn("Name", new DataType("varchar", 64), autoIncrement: false, nullable: true, comment: "The original comment.");

            // Act
            DatabaseHelper.TryDropDatabase(connection, schema.Name);
            DatabaseHelper.CreateSchema(connection, sut, schema);
            sut.Clear();

            var modifiedCol = new Column()
            {
                Name = "Modified",
                DataType = new DataType("varchar", 16),
                Comment = "Modified comment.",
                IsNullable = false,
                TableRef = tbl1
            };

            sut.AlterTable(originalCol, modifiedCol);
            var script = sut.GetContent();
            string errorMsg;
            bool theScriptWorked = DatabaseHelper.TryRunScript(connection, script, out errorMsg);
            System.Diagnostics.Debug.WriteLine(errorMsg);

            // Assert
            Approvals.Verify(script);
            Assert.IsTrue(theScriptWorked);
        }
    }
}