using Gigobyte.Daterpillar;
using Gigobyte.Daterpillar.Data;
using Gigobyte.Daterpillar.Linq;
using Gigobyte.Daterpillar.TextTransformation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tests.Daterpillar.Constants;
using Tests.Daterpillar.Helpers;

namespace Test.Daterpillar.Tests
{
    [TestClass]
    public class DatabaseTest
    {
        private static readonly string SQLiteDb = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "daterpillar-test.db3");

        // SQLite

        [TestMethod]
        [Owner(Dev.Ackara)]
        [TestCategory(Trait.Integration)]
        public void Execute_should_retrieve_data_from_a_sqlite_database_when_invoked()
        {
            using (var connection = DatabaseHelper.CreateSQLiteConnection(SQLiteDb))
            {
                RunSelectQueryTest(connection, new SQLiteScriptBuilder());
            }
        }

        [TestMethod]
        [Owner(Dev.Ackara)]
        [TestCategory(Trait.Integration)]
        public void Insert_should_add_a_record_to_a_sqlite_database_when_invoked()
        {
            using (var connection = DatabaseHelper.CreateSQLiteConnection(SQLiteDb))
            {
                RunSingleRecordInsertionTest(connection, new SQLiteScriptBuilder());
            }
        }

        [TestMethod]
        [Owner(Dev.Ackara)]
        [TestCategory(Trait.Integration)]
        public void Insert_should_add_multiple_records_to_a_sqlite_database_when_invoked()
        {
            using (var connection = DatabaseHelper.CreateSQLiteConnection(SQLiteDb))
            {
                RunMultiRecordInsertionTest(connection, new SQLiteScriptBuilder());
            }
        }

        // T-SQL

        [TestMethod]
        [Owner(Dev.Ackara)]
        [TestCategory(Trait.Integration)]
        public void Execute_should_retrieve_data_from_a_tql_database_when_invoked()
        {
            using (var connection = DatabaseHelper.CreateMSSQLConnection())
            {
                RunSelectQueryTest(connection, new TSQLScriptBuilder());
            }
        }

        [TestMethod]
        [Owner(Dev.Ackara)]
        [TestCategory(Trait.Integration)]
        public void Insert_should_add_a_record_to_a_tql_database_when_invoked()
        {
            using (var connection = DatabaseHelper.CreateMSSQLConnection())
            {
                RunSingleRecordInsertionTest(connection, new TSQLScriptBuilder());
            }
        }

        [TestMethod]
        [Owner(Dev.Ackara)]
        [TestCategory(Trait.Integration)]
        public void Insert_should_add_multiple_records_to_a_tql_database_when_invoked()
        {
            using (var connection = DatabaseHelper.CreateMSSQLConnection())
            {
                RunMultiRecordInsertionTest(connection, new TSQLScriptBuilder());
            }
        }

        // MySQL

        [TestMethod]
        [Owner(Dev.Ackara)]
        [TestCategory(Trait.Integration)]
        public void Execute_should_retrieve_data_from_a_a_mysql_database_when_invoked()
        {
            using (var connection = DatabaseHelper.CreateMySQLConnection())
            {
                RunSelectQueryTest(connection, new MySQLScriptBuilder());
            }
        }

        [TestMethod]
        [Owner(Dev.Ackara)]
        [TestCategory(Trait.Integration)]
        public void Insert_should_add_a_record_to_a_mysql_database_when_invoked()
        {
            using (var connection = DatabaseHelper.CreateMySQLConnection())
            {
                RunSingleRecordInsertionTest(connection, new MySQLScriptBuilder());
            }
        }

        [TestMethod]
        [Owner(Dev.Ackara)]
        [TestCategory(Trait.Integration)]
        public void Insert_should_add_multiple_records_to_a_mysql_database_when_invoked()
        {
            using (var connection = DatabaseHelper.CreateMySQLConnection())
            {
                RunMultiRecordInsertionTest(connection, new MySQLScriptBuilder());
            }
        }

        #region Helpers

        protected void RunSelectQueryTest(IDbConnection connection, IScriptBuilder builder)
        {
            // Arrange
            var sut = new AdoNetConnectionWrapper(connection);
            var schema = Schema.Load(SampleData.GetFile(KnownFile.MockSchema4XML).OpenRead());

            // Act
            DatabaseHelper.TryDropDatabase(connection, schema.Name);
            DatabaseHelper.CreateSchema(connection, builder, schema);

            IEnumerable<Record> results;
            using (sut) { results = sut.Execute<Record>(new Query().SelectAll().From(Record.Table)).ToArray(); }

            // Assert
            Assert.IsTrue(results.Count() > 0, "The query did not return any records.");
        }

        protected void RunSingleRecordInsertionTest(IDbConnection connection, IScriptBuilder builder)
        {
            // Arrange
            Record result;
            var newRecord = new Record()
            {
                CharValue = "e",
                DecimalValue = 9.95M,
                DateValue = DateTime.Now,
                StringValue = "she sold sea shells"
            };

            var schema = Schema.Load(SampleData.GetFile(KnownFile.MockSchema4XML).OpenRead());
            var sut = new AdoNetConnectionWrapper(connection);

            // Act
            DatabaseHelper.TryDropDatabase(connection, schema.Name);
            DatabaseHelper.CreateSchema(connection, builder, schema);

            using (sut)
            {
                sut.Insert(newRecord);
                sut.Commit();
                result = sut.Execute<Record>(new Query().SelectAll().From(Record.Table).Where($"{nameof(Record.StringValue)}='{newRecord.StringValue}'")).First();
            }

            // Assert
            Assert.AreNotEqual(0, result.Id);
            Assert.AreEqual(newRecord.StringValue, result.StringValue);
        }

        protected void RunMultiRecordInsertionTest(IDbConnection connection, IScriptBuilder builder)
        {
            // Arrange
            var newRecords = new Record[]
            {
                new Record()
                {
                    StringValue = "123 A",
                    DecimalValue = 1.99M,
                    CharValue = "k",
                    DateValue = DateTime.Now
                },
                new Record()
                {
                    StringValue = "123 B",
                    DecimalValue = 2.45M,
                    CharValue = "z",
                    DateValue = DateTime.UtcNow
                }
            };

            Record[] results;
            var sut = new AdoNetConnectionWrapper(connection);
            var schema = Schema.Load(SampleData.GetFile(KnownFile.MockSchema4XML).OpenRead());

            // Act
            DatabaseHelper.TryDropDatabase(connection, schema.Name);
            DatabaseHelper.CreateSchema(connection, builder, schema);

            using (sut)
            {
                sut.Insert(newRecords);
                sut.Commit();

                results = sut.Execute<Record>(new Query().SelectAll().From(Record.Table)
                    .Where($"{nameof(Record.StringValue)} LIKE '123%'"))
                    .ToArray();
            }

            // Assert
            Assert.AreEqual(newRecords.Length, results.Count());
        }

        [Table(Table)]
        public class Record : EntityBase
        {
            public const string Table = nameof(Record);

            [Column(nameof(Id), Key = true, AutoIncrement = true)]
            public int Id { get; set; }

            [Column(nameof(DecimalValue))]
            public decimal DecimalValue { get; set; }

            [Column(nameof(CharValue))]
            public string CharValue { get; set; }

            [Column(nameof(StringValue))]
            public string StringValue { get; set; }

            [Column(nameof(DateValue))]
            public DateTime DateValue { get; set; }
        }

        #endregion Helpers
    }
}