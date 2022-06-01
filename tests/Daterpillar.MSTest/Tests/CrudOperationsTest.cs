using Acklann.Daterpillar.Prototyping;
using Acklann.Daterpillar.Scripting;
using AutoBogus;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Acklann.Daterpillar.Tests
{
    [TestClass]
    public class CrudOperationsTest
    {
        [ClassInitialize]
        public static void Setup(TestContext _)
        {
            SqlValidator.CreateDatabase(_supportedLanguages);

            CrudOperations.Configure((builder) =>
            {
                builder.OverrideSqlValueArrayItem<Contact, FullName>(x => x.Name, (context, value) =>
                {
                    context.SetValue(value.FirstName.Serialize());
                    context.SetValue(value.LastName.Serialize());
                });

                builder.OnAfterSqlDataRecordLoaded<Contact>((record, context) =>
                {
                    record.Name = new FullName
                    {
                        FirstName = Convert.ToString(context.GetValue(context.GetOrdinal("first_name"))),
                        LastName = Convert.ToString(context.GetValue(context.GetOrdinal("last_name"))),
                    };
                });
            });
        }

        [DataTestMethod, DynamicData(nameof(GetCreateTestCases), DynamicDataSourceType.Method)]
        public void Can_generate_insert_statment_for_data_object(Language connectionType, object record)
        {
            // Arrange

            using var connection = SqlValidator.CreateDefaultConnection(connectionType);

            // Act

            var sql = CrudOperations.Create(record, connectionType);

            // Assert

            SqlValidator.TryExecute(connection, sql, out string error).ShouldBeTrue(string.Concat(error, '\n', sql));
        }

        [DataTestMethod, DynamicData(nameof(GetReadTestCases), DynamicDataSourceType.Method)]
        public void Can_instantiate_objects_from_sql_result(string query, object record)
        {
            // Arrange

            object[] results = null;
            var recordType = record.GetType();
            var connectionType = Language.SQLite;
            using var connection = SqlValidator.CreateConnection(connectionType);

            // Act

            var sql = CrudOperations.Create(record, connectionType);
            if (!SqlValidator.TryExecute(connection, sql, out string errorMessage)) Assert.Fail(errorMessage);

            using (var command = connection.CreateCommand())
            {
                command.CommandText = query;
                using (var dataset = command.ExecuteReader())
                {
                    results = CrudOperations.Read(recordType, dataset).ToArray();
                }
            }

            // Assert

            results.ShouldNotBeNull();
            results.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void Can_load_object_from_IDataRecord()
        {
            // Arrange

            var connectionType = Language.SQLite;
            using var connection = SqlValidator.CreateConnection(connectionType);

            var record = AutoFaker.Generate<Contact>();
            Contact result;

            // Act

            var sql = CrudOperations.Create(record, connectionType);
            if (!SqlValidator.TryExecute(connection, sql, out string error)) Assert.Fail(error);

            using (var command = connection.CreateCommand())
            {
                command.CommandText = $"select * from contact where id={record.Id}";
                using (var dataset = command.ExecuteReader())
                {
                    result = CrudOperations.Read(record.GetType(), dataset).Cast<Contact>().First();
                }
            }

            // Assert

            result.Id.ShouldBe(record.Id);
            result.Name.FirstName.ShouldBe(record.Name.FirstName);
            result.Name.LastName.ShouldBe(record.Name.LastName);
        }

        #region Backing Members

        private static System.Data.IDbConnection CreateConnection()
        {
            throw new System.NotImplementedException();
        }

        private static IEnumerable<object[]> GetCreateTestCases()
        {
            foreach (var lang in _supportedLanguages)
            {
                yield return new object[] { lang, AutoFaker.Generate<Contact>() };
                yield return new object[] { lang, AutoFaker.Generate<Song>() };
            }
        }

        private static IEnumerable<object[]> GetReadTestCases()
        {
            var record3 = AutoFaker.Generate<TrackSequence>();
            yield return new object[] { $"select * from {nameof(TrackSequence)} where {nameof(TrackSequence.SongId)}={record3.SongId}", record3 };

            var record2 = AutoFaker.Generate<Contact>();
            yield return new object[] { $"select * from contact where Id={record2.Id}", record2 };

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