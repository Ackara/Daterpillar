using Acklann.Daterpillar.Prototyping;
using Acklann.Daterpillar.Scripting;
using AutoBogus;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using System.Collections.Generic;
using System.Linq;

namespace Acklann.Daterpillar.Tests
{
    [TestClass]
    public class CRUDTest
    {
        [ClassInitialize]
        public static void Setup(TestContext _)
        {
            SqlValidator.CreateDatabase(_supportedLanguages);
        }

        [DataTestMethod, DynamicData(nameof(GetCreateTestCases), DynamicDataSourceType.Method)]
        public void Can_generate_insert_statment_for_data_object(Language connectionType, object record)
        {
            // Arrange

            using var connection = SqlValidator.CreateConnection(connectionType);

            // Act

            var sql = SqlComposer.Create(record, connectionType);

            // Assert

            SqlValidator.TryExecute(connection, sql, out string error).ShouldBeTrue(error);
        }

        [DataTestMethod, DynamicData(nameof(GetReadTestCases), DynamicDataSourceType.Method)]
        public void Can_instantiate_object_from_sql_result(string query, object record)
        {
            // Arrange

            object[] results = null;
            var recordType = record.GetType();
            var connectionType = Language.SQLite;
            using var connection = SqlValidator.CreateConnection(connectionType);

            // Act

            var sql = SqlComposer.Create(record, connectionType);
            if (!SqlValidator.TryExecute(connection, sql, out string errorMessage)) Assert.Fail(errorMessage);

            using (var command = connection.CreateCommand())
            {
                command.CommandText = query;
                using (var dataset = command.ExecuteReader())
                {
                    results = SqlComposer.Read(recordType, dataset).ToArray();
                }
            }

            // Assert

            results.ShouldNotBeNull();
            results.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void Can()
        {
            throw new System.NotImplementedException();
        }

        #region Backing Members

        private static IEnumerable<object[]> GetCreateTestCases()
        {
            foreach (var lang in _supportedLanguages)
            {
                yield return new object[] { lang, AutoFaker.Generate<Song>() };
            }
        }

        private static IEnumerable<object[]> GetReadTestCases()
        {
            var record1 = AutoFaker.Generate<Artist>();
            yield return new object[] { $"select * from artist where Name='{record1.Name}'", record1 };
        }

        private static readonly Language[] _supportedLanguages = new[]
        {
            Language.TSQL,
            Language.MySQL,
            Language.SQLite
        };

        #endregion Backing Members
    }
}