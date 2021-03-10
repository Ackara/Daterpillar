using Acklann.Daterpillar.Configuration;
using Acklann.Daterpillar.Writers;
using Acklann.Diffa;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using System;
using System.IO;
using System.Linq;

namespace Acklann.Daterpillar.Tests
{
    [TestClass]
    [TestCategory("sql")]
    public class ScriptingTest
    {
        [ClassInitialize]
        public static void Setup(TestContext context)
        {

        }

        public TestContext TestContext { get; set; }

        // ==================== SELECT ==================== //



        // ==================== placholder ==================== //

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

        private static void TestScript(string scriptFile, Language syntax, out string results, out bool scriptExecutionWasSuccessful)
        {
            string schemaName = "dtp_scripting_test";
            using (var database = MockDatabase.CreateConnection(syntax, schemaName))
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
    }
}