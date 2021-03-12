﻿using Acklann.Daterpillar.Configuration;
using Acklann.Daterpillar.Linq;
using Acklann.Daterpillar.Prototyping;
using Acklann.Daterpillar.Writers;
using Acklann.Diffa;
using AutoBogus;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace Acklann.Daterpillar.Tests
{
    [TestClass]
    public class ScriptingTest
    {
        [ClassInitialize]
        public static void Setup(TestContext _)
        {
        }

        [DataTestMethod]
        [DataRow(Language.TSQL)]
        [DataRow(Language.MySQL)]
        [DataRow(Language.SQLite)]
        public void Can_generate_script_to_create_a_database_schema(Language syntax)
        {
            // Arrange

            #region Construct Schema

            var scriptFile = Path.Combine(Path.GetTempPath(), $"dtp-create.{syntax}".ToLowerInvariant());
            var schema = new Schema();

            var genre = new Table("genre",
                new Column("Id", new DataType(SchemaType.INT), true),
                new Column("Name", new DataType(SchemaType.VARCHAR))
                )
            { Schema = schema, Comment = "Represents a music genre." };

            var song = new Table("song",
                new Column("Id", SchemaType.INT, true) { Comment = "The unique identifier." },
                new Column("Name", new DataType(SchemaType.VARCHAR, 15)),
                new Column("GenreId", SchemaType.INT),
                new Column("Track", SchemaType.TINYINT, defaultValue: "1"),
                new Column("Lyrics", new DataType(SchemaType.VARCHAR), nullable: true),

                new ForeignKey("GenreId", "genre", "Id"),
                new Index(IndexType.Index, new ColumnName("GenreId"))
                )
            { Schema = schema };

            var album = new Table("album",
                new Column("SongId", SchemaType.INT),
                new Column("ArtistId", SchemaType.INT),
                new Column("Name", SchemaType.VARCHAR),
                new Column("Year", SchemaType.SMALLINT),
                new Column("Price", SchemaType.DECIMAL)
                )
            { Schema = schema };

            var genreSeed = new Script("INSERT INTO genre (Name) VAlUES ('Hip Hop'), ('R&B'), ('Country');", syntax);
            var songSeed = new Script("INSERT INTO song (Name, GenreId, Track) VALUES ('Survival', '1', '1');", syntax);
            var albumSeed = new Script("INSERT INTO album (SongId, ArtistId, Name, Year, Price) VALUES ('1', '1', 'Scorpion', '2018', '14.99');");
            var generic = new Script("-- If you are reading this, I don't discriminate.", Language.SQL);
            var virus = new Script("-- If you are reading this, it's to late. (I should not be here!)", (syntax + 1));

            var name_idx = new Index(IndexType.Index, true, new ColumnName("Name", Order.DESC)) { Table = song };
            var releaseDate = new Column("ReleaseDate", new DataType(SchemaType.TIMESTAMP), defaultValue: "$(now)") { Table = song };

            var song_fk = new ForeignKey("SongId", "song", "Id") { Table = album };
            var pKey = new Index(IndexType.PrimaryKey, new ColumnName("SongId"), new ColumnName("ArtistId")) { Table = album };

            #endregion Construct Schema

            // Act
            Sample.CreateDirectory(scriptFile);
            if (File.Exists(scriptFile)) File.Delete(scriptFile);

            var factory = new SqlWriterFactory();
            using (var file = File.OpenWrite(scriptFile))
            using (var writer = factory.CreateInstance(syntax, file))
            {
                schema.Add(genre, song, album);
                schema.Add(virus, generic, genreSeed, songSeed, albumSeed);
                writer.Create(schema);

                writer.Create(releaseDate);
                song.Columns.Add(releaseDate);

                writer.Create(song_fk);
                album.ForeignKeys.Add(song_fk);

                writer.Create(name_idx);
                song.Indecies.Add(name_idx);

                writer.Create(pKey);
                writer.Flush();
            }

            TestScript(scriptFile, syntax, out string results, out bool executedScriptSuccessfully);

            // Assert
            Diff.Approve(results, ".sql", syntax);
            executedScriptSuccessfully.ShouldBeTrue();
        }

        [DataTestMethod]
        [DataRow(Language.TSQL)]
        [DataRow(Language.MySQL)]
        [DataRow(Language.SQLite)]
        public void Can_generate_script_to_drop_a_database_schema(Language syntax)
        {
            // Arrange
            var factory = new SqlWriterFactory();
            var scriptFile = Path.Combine(Path.GetTempPath(), "dtp-drop.sql");

            var schema = CreateSchemaInstance();
            var service = schema.Tables[2];

            // Act
            Sample.CreateDirectory(scriptFile);
            if (File.Exists(scriptFile)) File.Delete(scriptFile);

            using (var file = File.OpenWrite(scriptFile))
            using (var writer = factory.CreateInstance(syntax, file))
            {
                writer.Drop(schema.Tables[0]);

                writer.Drop(service.ForeignKeys[0]);
                service.ForeignKeys.RemoveAt(0);

                writer.Drop(service.Columns[3]);
                service.Columns.RemoveAt(3);

                writer.Drop(service.Indecies[0]);
            }

            TestScript(scriptFile, syntax, out string sql, out bool sqlIsExecutable);

            // Assert
            Diff.Approve(sql, ".sql", syntax);
            sqlIsExecutable.ShouldBeTrue();
        }

        [DataTestMethod]
        [DataRow(Language.TSQL)]
        [DataRow(Language.MySQL)]
        [DataRow(Language.SQLite)]
        public void Can_generate_script_to_modify_a_database_schema(Language syntax)
        {
            // Arrange
            var scriptFile = Path.Combine(Path.GetTempPath(), "dtp-alter.sql");
            var schema = CreateSchemaInstance();
            var factory = new SqlWriterFactory();

            var service = schema.Tables[2];
            var oldTable = service.Clone();

            // Act
            Sample.CreateDirectory(scriptFile);
            if (File.Exists(scriptFile)) File.Delete(scriptFile);

            using (var file = File.OpenWrite(scriptFile))
            using (var writer = factory.CreateInstance(syntax, file))
            {
                writer.Rename("placeholder", "publisher");
                writer.Rename(service.Columns[4], "ActiveUsers");
                service.ForeignKeys[0].LocalColumn = "ActiveUsers";
                service.Columns[4].Name = "ActiveUsers";

                var replacement = service.Columns[2];
                replacement.IsNullable = false;
                replacement.DefaultValue = "0";
                replacement.Comment = "Get or set the number of customers.";
                writer.Alter(replacement);

                service.Comment = "Represents a streaming service.";
                writer.Alter(oldTable, service);
            }

            TestScript(scriptFile, syntax, out string sql, out bool generatedSqlIsExecutable);

            // Assert
            Diff.Approve(sql, ".sql", syntax);
            generatedSqlIsExecutable.ShouldBeTrue();
        }

        [DataTestMethod]
        [DataRow(Language.TSQL)]
        [DataRow(Language.MySQL)]
        [DataRow(Language.SQLite)]
        public void Can_generate_insert_commands(Language kind)
        {
            // Arrange
            var script = new StringBuilder();
            var tableName = "foo";
            var createStatement = $"create table {tableName}(id int, name varchar(64), age int);";
            var columns = new string[] { "id", "name", "age" };
            var values1 = new object[] { 1, "sally", 12 };
            var values2 = new object[] { 2, "peggy", 21 };
            var values3 = new object[] { 3, "don", 40 };

            var sample1 = A.Fake<IEntity>();
            A.CallTo(() => sample1.GetTableName()).Returns(tableName);
            A.CallTo(() => sample1.GetColumnList()).Returns(columns);

            var sample2 = A.Fake<IEntity>();
            A.CallTo(() => sample2.GetTableName()).Returns(tableName);
            A.CallTo(() => sample2.GetColumnList()).Returns(columns);

            // Act
            var case1 = SqlComposer.GenerateInsertStatements();

            A.CallTo(() => sample1.GetValueList()).Returns(new object[] { 1, "'sally'", 12 });
            var case2 = SqlComposer.GenerateInsertStatements(sample1);

            A.CallTo(() => sample1.GetValueList()).Returns(new object[] { 2, "'mark'", 21 });
            A.CallTo(() => sample2.GetValueList()).Returns(new object[] { 3, "'mary'", 25 });
            var case3 = SqlComposer.GenerateInsertStatements(sample1, sample2);

            A.CallTo(() => sample1.GetValueList()).Returns(new object[] { 4, "'jim'", 40 });
            A.CallTo(() => sample2.GetValueList()).Returns(new object[] { 5, "'sal'", 50 });
            var case4 = SqlComposer.GenerateJoinedInsertStatements(sample1, sample2);

            using (var connection = TestDatabase.CreateConnection(kind))
            {
                System.Diagnostics.Debug.WriteLine($"connection: {connection.ConnectionString}");
                connection.TryExecute($"drop table {tableName};", out string errorMsg);
                bool failed = !connection.TryExecute(createStatement, out errorMsg);
                if (failed) Assert.Fail($"Failed to create {tableName} table.\n\n{errorMsg}");

                var separator = string.Concat(Enumerable.Repeat('=', 50));
                foreach (var item in (case2.Concat(case3).Append(case4)))
                {
                    connection.TryExecute(item, out errorMsg);
                    script.Append(errorMsg)
                          .AppendLine(item)
                          .AppendLine();
                }
            }

            // Assert
            case1.ShouldBeEmpty();
            Diff.Approve(script, ".sql", kind);
        }

        [TestMethod]
        public void Can_query_database()
        {
            // Arrange
            Contact[] results = null;
            var database = TestDatabase.CreateDatabase<Contact>();
            var records = AutoFaker.Generate<Contact>(100).ToArray();

            // Act
            if (database.State != ConnectionState.Open) database.Open();

            using (database)
            using (var command = database.CreateCommand())
            {
                if (!database.TryExecute(SqlComposer.GenerateJoinedInsertStatements(records), out string errorMsg))
                    Assert.Fail(errorMsg);

                results = database.Select<Contact>($"select * from {nameof(Contact)};").ToArray();
            }

            // Assert
            results.ShouldNotBeEmpty();
            records.ShouldAllBe(x => !string.IsNullOrEmpty(x.Name) && !string.IsNullOrEmpty(x.Email));
            records.ShouldAllBe(x => x.TimeBorn != default);
        }

        [TestMethod]
        public void Can_build_select_statement()
        {
            // Arrange
            var sut = new QueryBuilder();

            // Act
            sut.SelectAll()
                .From("profile");
            var case1 = sut.ToString();

            sut.Select("id", "name")
                .From("profile")
                .Where($"id = 234");
            var case2 = sut.ToString();

            sut.OrderBy("name")
                .Limit(10);
            var case3 = sut.ToString(Language.TSQL);

            sut.Limit(5).Offset(10);
            var case4 = sut.ToString(Language.TSQL);

            var case5 = sut.ToString(Language.MySQL);

            var case6 = sut.ToString();

            // Assert
            case1.ShouldMatch(@"(?i)select\s+\*\s+from profile\s*;");
            case2.ShouldMatch(@"(?i)select\s+id, name\s+from profile\s+where id = 234\s+;");
            case3.ShouldMatch(@"(?i)select top 10\s+id, name\s+from profile\s+where id = 234\s+order by\s+name\s+;");
            case4.ShouldMatch(@"(?i)select\s+id, name\s+from profile\s+where id = 234\s+order by\s+name\s+offset \d+ rows fetch next \d+ rows only\s+;");
            case5.ShouldMatch(@"(?i)select\s+id, name\s+from profile\s+where id = 234\s+order by\s+name\s+limit \d+\s+offset \d+\s+;");
        }

        [TestMethod]
        public void Can_build_update_statement()
        {
            // Arrange
            var builder = new UpdateBuilder("foo");

            // Act
            builder.Set("id", 123)
                .Set("name", "abc")
                .Predicate("id", "test");
            var case1 = builder.ToString();

            builder.Predicate("id", "test").And("name", "jane");
            var case2 = builder.ToString();

            builder.Set("age", 12);
            var case3 = builder.ToString();

            // Assert
            case1.ShouldMatch(@"(?i)update foo set id='?123'?, name='abc' where id='test';");
            case2.ShouldMatch(@"(?i)update foo set id='?123'?, name='abc' where id='test' and name='jane';");
            case3.ShouldMatch(@"(?i)update foo set id='?123'?, name='abc', age='?12'? where id='test' and name='jane';");
        }

        [DataTestMethod]
        [DataRow("", "''")]
        [DataRow(22, "'22'")]
        [DataRow(null, "null")]
        [DataRow("foo", "'foo'")]
        [DataRow(12.54f, "'12.54'")]
        [DataRow(DayOfWeek.Friday, "'5'")]
        [DataRow("abc ' '' def", "'abc '' '''' def'")]
        [DataRow("2015-1-1 1:1:1", "'2015-01-01 01:01:01'")]
        public void Can_serialize_an_object_to_a_sql_value(object input, string expectedValue)
        {
            if (DateTime.TryParse(input?.ToString(), out DateTime dt))
                input = dt;

            SqlComposer.Serialize(input).ShouldBe(expectedValue);
        }

        #region Backing Members

        private static void TestScript(string scriptFile, Language syntax, out string results, out bool scriptExecutionWasSuccessful)
        {
            string schemaName = "dtp_scripting_test";
            using (var database = TestDatabase.CreateConnection(syntax, schemaName))
            {
                database.RebuildSchema(schemaName);

                results = File.ReadAllText(scriptFile);
                scriptExecutionWasSuccessful = database.TryExecute(results, out string error);
                var nl = string.Concat(Enumerable.Repeat(Environment.NewLine, 3));
                results = string.Format("{0}SYNTAX: {1}{3}{2}", error, syntax, results, nl);
            }
        }

        private static Schema CreateSchemaInstance()
        {
            var schema = new Schema();
            schema.Add(
                new Table("zombie"),

                new Table("placeholder",
                    new Column("Id", SchemaType.INT, autoIncrement: true),
                    new Column("Name", SchemaType.VARCHAR, nullable: true)
                ),

                new Table("service",
                    new Column("Id", SchemaType.INT, autoIncrement: true),
                    new Column("Name", SchemaType.VARCHAR),
                    new Column("Subscribers", SchemaType.INT),
                    new Column("Zombie", SchemaType.VARCHAR),
                    new Column("Zombie_fk", SchemaType.INT),

                    new ForeignKey("Zombie_fk", "placeholder", "Id"),
                    new Index(IndexType.Index, new ColumnName("Subscribers"))
                )
                );

            return schema;
        }

        #endregion Backing Members
    }
}