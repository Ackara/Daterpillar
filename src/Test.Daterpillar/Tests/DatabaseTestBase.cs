using Gigobyte.Daterpillar;
using Gigobyte.Daterpillar.Data;
using Gigobyte.Daterpillar.Linq;
using Gigobyte.Daterpillar.TextTransformation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tests.Daterpillar.Constants;
using Tests.Daterpillar.Helpers;

namespace Test.Daterpillar.Tests
{
    public abstract class DatabaseTestBase
    {
        protected void RunSelectQueryTest(IDbConnection connection, IScriptBuilder builder)
        {
            // Arrange
            var schema = Schema.Load(SampleData.GetFile(KnownFile.MockSchema4XML).OpenRead());

            var sut = new AdoNetConnectionWrapper(connection);

            // Act
            DatabaseHelper.TryDropDatabase(connection, schema.Name);
            DatabaseHelper.CreateSchema(connection, builder, schema);

            IEnumerable<foo> results;
            using (sut) { results = sut.Execute<foo>(new Query().SelectAll().From(null)); }

            // Assert
            Assert.IsTrue(results.Count() > 0, "The query did not return any records.");
        }

        protected void RunSingleRecordInsertionTest(IDbConnection connection, IScriptBuilder builder)
        {
            // Arrange
            IEnumerable<foo> result;
            var newRecord = new foo()
            {
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
                result = sut.Execute<foo>(newRecord.ToSelectCommand());
            }

            // Assert
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(newRecord.StringValue, result.First().StringValue);
        }

        protected void RunMultiRecordInsertionTest(IDbConnection connection, IScriptBuilder builder)
        {
            // Arrange
            var newRecords = new foo[]
            {
                new foo()
                {

                },
                new foo()
                {

                }
            };

            IEnumerable<foo> results;
            var sut = new AdoNetConnectionWrapper(connection);
            var schema = Schema.Load(SampleData.GetFile(KnownFile.MockSchema4XML).OpenRead());

            // Act
            DatabaseHelper.TryDropDatabase(connection, schema.Name);
            DatabaseHelper.CreateSchema(connection, builder, schema);

            using (sut)
            {
                sut.Insert(newRecords);
                sut.Commit();

                results = sut.Execute<foo>(new Query().SelectAll().From(null)
                    .Where(null));
            }

            // Assert
            Assert.AreEqual(newRecords.Length, results.Count());
        }

        [Table("")]
        public class foo : EntityBase
        {
            public string StringValue { get; set; }
        }
    }
}