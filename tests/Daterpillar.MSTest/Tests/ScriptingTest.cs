using Acklann.Daterpillar.Compilation;
using Acklann.Daterpillar.Configuration;
using Acklann.Diffa;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using System;
using System.IO;
using System.Linq;

namespace Acklann.Daterpillar.Tests
{
    [TestClass]
    public class ScriptingTest
    {
        [DataTestMethod]
        [DataRow(Syntax.TSQL)]
        //[DataRow(Syntax.SQLite)]
        public void Can_generate_scripts_to_create_sql_objects(Syntax syntax)
        {
            #region Arrange

            var scriptFile = Path.Combine(Path.GetTempPath(), $"dtp-create.{syntax}".ToLowerInvariant());
            var schema = new Schema();

            var genre = new Table("genre",
                new Column("Id", new DataType(SchemaType.INT), true),
                new Column("Name", new DataType(SchemaType.VARCHAR))
                )
            { Schema = schema };

            var song = new Table("song",
                new Column("Id", SchemaType.INT, true),
                new Column("Name", new DataType(SchemaType.VARCHAR, 15)),
                new Column("GenreId", SchemaType.MEDIUMINT),
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
            var generic = new Script("-- If you are reading this, I don't discriminate.", Syntax.Generic);
            var virus = new Script("-- If you are reading this, it's to late. (I should not be here!)", (syntax + 1));

            var name_idx = new Index(IndexType.Index, new ColumnName("Name", Order.DESC)) { Table = song };
            var releaseDate = new Column("ReleaseDate", new DataType(SchemaType.TIMESTAMP), defaultValue: "$(now)") { Table = song };

            var song_fk = new ForeignKey("SongId", "song", "Id") { Table = album };
            var pKey = new Index(IndexType.PrimaryKey, new ColumnName("SongId"), new ColumnName("ArtistId")) { Table = album };

            var factory = new SqlWriterFactory();

            #endregion Arrange

            // Act
            TestData.CreateDirectory(scriptFile);
            if (File.Exists(scriptFile)) File.Delete(scriptFile);

            using (var file = File.OpenWrite(scriptFile))
            using (var writer = factory.CreateInstance(syntax, file))
            {
                System.Diagnostics.Debug.WriteLine($"using {writer.GetType().Name}");

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

            TestScript(scriptFile, syntax, out string sql, out bool generatedSqlIsExecutable);

            // Assert
            Diff.Approve(sql, ".sql", syntax);
            generatedSqlIsExecutable.ShouldBeTrue();
        }

        [DataTestMethod]
        [DataRow(Syntax.TSQL)]
        [DataRow(Syntax.SQLite)]
        public void Can_generate_scripts_to_drop_sql_objects(Syntax syntax)
        {
            // Arrange
            var factory = new SqlWriterFactory();
            var scriptFile = Path.Combine(Path.GetTempPath(), "dtp-drop.sql");

            var schema = Database.Sample.CreateInstance();
            var service = schema.Tables[2];

            // Act
            TestData.CreateDirectory(scriptFile);
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
        [DataRow(Syntax.TSQL)]
        [DataRow(Syntax.SQLite)]
        public void Can_generate_scripts_to_alter_sql_objects(Syntax syntax)
        {
            // Arrange
            var scriptFile = Path.Combine(Path.GetTempPath(), "dtp-alter.sql");
            var schema = Database.Sample.CreateInstance();
            var factory = new SqlWriterFactory();

            var service = schema.Tables[2];

            // Act
            TestData.CreateDirectory(scriptFile);
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
                writer.Alter(service);
            }

            TestScript(scriptFile, syntax, out string sql, out bool generatedSqlIsExecutable);

            // Assert
            Diff.Approve(sql, ".sql", syntax);
            generatedSqlIsExecutable.ShouldBeTrue();
        }

        private static void TestScript(string scriptFile, Syntax syntax, out string sql, out bool generatedSqlIsExecutable)
        {
            using (var db = new Database(syntax, "dtp-scriptingTest"))
            {
                db.Refresh();

                sql = File.ReadAllText(scriptFile);
                generatedSqlIsExecutable = db.TryExecute(sql, out string error);
                var nl = string.Concat(Enumerable.Repeat(Environment.NewLine, 3));
                sql = string.Format("{0}SYNTAX: {1}{3}{2}", error, syntax, sql, nl);
            }
        }
    }
}