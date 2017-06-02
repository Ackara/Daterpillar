using Acklann.Daterpillar;
using Acklann.Daterpillar.Linq;
using ApprovalTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MSTest.Daterpillar.Fake;
using Shouldly;
using System;

namespace MSTest.Daterpillar.Tests
{
    [TestClass]
    public class LinqExtensionMethodsTest
    {
        [DataTestMethod]
        [DataRow(22, "'22'", null)]
        [DataRow(null, "null", null)]
        [DataRow(12.54f, "'12.54'", null)]
        [DataRow(AnEnum.DarkGray, "'3'", null)]
        [DataRow("2015-1-1 1:1:1", "'2015-01-01 01:01:01'", typeof(DateTime))]
        //
        [DataRow("foo", "'foo'", null)]
        [DataRow("abc ' '' def", "'abc '' '''' def'", null)]
        public void ToSQL_return_a_string_that_represents_a_sql_value(object input, string expected, Type conversionType = null)
        {
            // Arrange
            object sample = (conversionType == null ? input : Convert.ChangeType(input, conversionType));

            // Act
            var results = sample.ToSQL();

            // Assert
            results.ShouldBe(expected);
        }

        [TestMethod]
        public void ToQuery_should_return_a_sql_query_for_the_specified_object()
        {
            // Arrange
            var sample = new SimpleTable()
            {
                Id = 2,
                Amount = 123.5M,
                Name = "foo boo coo"
            };

            var expectedValue = $"SELECT * FROM {SimpleTable.Table} WHERE {nameof(SimpleTable.Id)}='2';";

            // Act
            var results = sample.ToQuery();

            // Assert
            results.ShouldBe(expectedValue);
        }

        [TestMethod]
        public void ToInsertCommand_should_return_a_sql_insert_command_for_the_specified_object()
        {
            // Arrange
            var sample = new SimpleTable()
            {
                Id = 2,
                Name = "foo",
                Sex = "m",
                Body = null,
                Amount = 123,
                Rate = 123.45f,
                Day = DayOfWeek.Friday,
                Date = new DateTime(2000, 1, 1, 0, 0, 0)
            };

            // Act
            var sql = sample.ToInsertCommand(Syntax.MySQL);

            // Assert
            sql.ShouldNotBeNullOrEmpty();
            Approvals.Verify(sql);
        }

        [TestMethod]
        public void ToInsertCommand_should_return_a_sql_insert_command_for_the_specified_collection()
        {
            // Arrange
            var sample = CreateSamples();

            // Act
            var sql = sample.ToInsertCommand(Syntax.MSSQL);

            // Assert
            sql.ShouldNotBeNullOrEmpty();
            Approvals.Verify(sql);
        }

        [TestMethod]
        public void ToDeleteCommand_should_return_a_sql_delete_command_for_the_specified_object()
        {
            // Arrange
            var sample = new ComplexTable()
            {
                CompositeKey1 = 11,
                CompositeKey2 = 22,
                StandaloneIdx = 03
            };

            var expectedValue = $"DELETE FROM [{nameof(ComplexTable)}] WHERE [{nameof(ComplexTable.CompositeKey1)}]='{11}' AND [{nameof(ComplexTable.CompositeKey2)}]='{22}';";

            // Act
            var results = sample.ToDeleteCommand(Syntax.SQLite);

            // Assert
            results.ShouldBe(expectedValue);
        }

        private SimpleTable[] CreateSamples()
        {
            return new SimpleTable[]
            {
                new SimpleTable()
                {
                    Id = 2,
                    Name = "foo",
                    Sex = "m",
                    Body = null,
                    Amount = 123,
                    Rate = 123.45f,
                    Day = DayOfWeek.Friday,
                    Date = new DateTime(2000, 1, 1, 0, 0, 0)
                },
                new SimpleTable()
                {
                    Id = 25,
                    Name = "key",
                    Sex = "m",
                    Body = null,
                    Amount = 10,
                    Rate = 12f,
                    Day = DayOfWeek.Wednesday,
                    Date = new DateTime(2000, 1, 1, 1, 1, 1)
                },
                new SimpleTable()
                {
                    Id = 253,
                    Name = "ratt",
                    Sex = "f",
                    Body = "me a foo cat dog's paw",
                    Amount = 521,
                    Rate = 123.9f,
                    Day = DayOfWeek.Thursday,
                    Date = new DateTime(2010, 1, 1, 9, 5, 4)
                }
            };
        }
    }
}