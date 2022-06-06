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
                builder.OverrideSqlValueArrayItem<Contact>((m => m.Name), (context, record) =>
                {
                    context.SetValue(record.Name.FirstName);
                    context.SetValue(record.Name.LastName);
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
        public void Can_build_insert_command_from_object(Language connectionType, object record)
        {
            // Arrange

            using var connection = SqlValidator.CreateDefaultConnection(connectionType);

            // Act

            var command = connection.CreateCommand();
            System.Diagnostics.Debug.WriteLine(command.CommandText);
            CrudOperations.Create(command, record, connectionType);

            // Assert

            SqlValidator.TryExecute(connection, command, out string error).ShouldBeTrue(error);
        }

        [DataTestMethod, DynamicData(nameof(GetReadTestCases), DynamicDataSourceType.Method)]
        public void Can_load_array_from_IDataRecord(string query, object record)
        {
            // Arrange

            object[] results = null;
            var recordType = record.GetType();
            var connectionType = Language.SQLite;
            using var connection = SqlValidator.CreateConnection(connectionType);
            var command = connection.CreateCommand();

            // Act

            CrudOperations.Create(command, record, connectionType);
            if (!SqlValidator.TryExecute(connection, command, out string errorMessage)) Assert.Fail(errorMessage);

            using (command = connection.CreateCommand())
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
            var command = connection.CreateCommand();

            var record = AutoFaker.Generate<Contact>();
            Contact result;

            // Act

            CrudOperations.Create(command, record, connectionType);
            if (!SqlValidator.TryExecute(connection, command, out string error)) Assert.Fail(error);

            using (command = connection.CreateCommand())
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

        [DataTestMethod, DynamicData(nameof(GetSupportedLang), DynamicDataSourceType.Method)]
        public void Can_build_update_command_from_object(Language connectionType)
        {
            // Arrange

            using var connection = SqlValidator.CreateDefaultConnection(connectionType);
            var command = connection.CreateCommand();
            var record = AutoFaker.Generate<Contact>();

            // Act

            CrudOperations.Create(command, record, connectionType);
            if (!SqlValidator.TryExecute(connection, command, out string error)) Assert.Fail(error);

            command = connection.CreateCommand();
            CrudOperations.Update(command, record, connectionType);
            System.Diagnostics.Debug.WriteLine(command.CommandText);

            // Assert

            SqlValidator.TryExecute(connection, command, out error).ShouldBeTrue(error);
        }

        [DataTestMethod, DynamicData(nameof(GetSupportedLang), DynamicDataSourceType.Method)]
        public void Can_build_delete_command_from_object(Language connectionType)
        {
            // Arrange

            using var connection = SqlValidator.CreateDefaultConnection(connectionType);
            var command = connection.CreateCommand();
            var record = AutoFaker.Generate<Contact>();

            // Act

            CrudOperations.Create(command, record, connectionType);
            if (!SqlValidator.TryExecute(connection, command, out string error)) Assert.Fail(error);

            command = connection.CreateCommand();
            CrudOperations.Delete(command, record, connectionType);
            System.Diagnostics.Debug.WriteLine(command.CommandText);

            // Assert

            SqlValidator.TryExecute(connection, command, out error).ShouldBeTrue(error);
        }

        [DataTestMethod, DynamicData(nameof(GetSupportedLang), DynamicDataSourceType.Method)]
        public void Can_execute_insert_command(Language connectionType)
        {
            // Arrange

            using var connection = SqlValidator.CreateConnection(connectionType);
            var record = AutoFaker.Generate<Song>();

            // Act

            var result = DbConnectionExtensions.Insert(connection, record, connectionType);

            // Assert

            result.Success.ShouldBeTrue(result.ErrorMessage);
        }

        [DataTestMethod, DynamicData(nameof(GetSupportedLang), DynamicDataSourceType.Method)]
        public void Can_execute_update_command(Language connectionType)
        {
            // Arrange

            using var connection = SqlValidator.CreateConnection(connectionType);
            var contact = AutoFaker.Generate<Contact>();
            var faker = new Bogus.Faker();

            // Act

            var result = DbConnectionExtensions.Insert(connection, contact, connectionType);
            if (!result.Success) Assert.Fail(result.ErrorMessage);

            contact.Name = new FullName { FirstName = faker.Name.FirstName(), LastName = faker.Name.LastName() };
            result = DbConnectionExtensions.Update(connection, contact, connectionType);
            var record = DbConnectionExtensions.SelectOne<Contact>(connection, $"select * from contact where {nameof(Contact.Id)}='{contact.Id}'");

            // Assert

            result.Success.ShouldBeTrue(result.ErrorMessage);
            result.Changes.ShouldBe(1);
            record.Success.ShouldBeTrue(record.ErrorMessage);
            record.Data.Name.FirstName.ShouldBe(contact.Name.FirstName);
            record.Data.Id.ShouldBe(contact.Id);
        }

        [DataTestMethod, DynamicData(nameof(GetSupportedLang), DynamicDataSourceType.Method)]
        public void Can_execute_delete_command(Language connectionType)
        {
            /// Arrange

            using var connection = SqlValidator.CreateConnection(connectionType);
            var contact = AutoFaker.Generate<Contact>();
            var faker = new Bogus.Faker();

            // Act

            var result = DbConnectionExtensions.Insert(connection, contact, connectionType);
            if (!result.Success) Assert.Fail(result.ErrorMessage);

            result = DbConnectionExtensions.Delete(connection, contact, connectionType);
            var record = DbConnectionExtensions.SelectOne<Contact>(connection, $"select * from contact where {nameof(Contact.Id)}='{contact.Id}'");

            // Assert

            result.Success.ShouldBeTrue(result.ErrorMessage);
            result.Changes.ShouldBe(1);
            record.Success.ShouldBeFalse(record.ErrorMessage);
        }

        #region Backing Members

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
            yield return new object[] { $"select * from {nameof(TrackSequence)} where {nameof(TrackSequence.SongId)}='{record3.SongId}'", record3 };

            var record2 = AutoFaker.Generate<Contact>();
            yield return new object[] { $"select * from contact where Id={record2.Id}", record2 };

            var record1 = AutoFaker.Generate<Artist>();
            yield return new object[] { $"select * from artist where Name='{record1.Name}'", record1 };
        }

        private static IEnumerable<object[]> GetSupportedLang()
        {
            foreach (var lang in _supportedLanguages)
            {
                yield return new object[] { lang };
            }
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