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
        //[ClassInitialize]
        public static void Setup(TestContext context) => Database.Refresh();

        [DataTestMethod]
        [DataRow(Syntax.SQLite)]
        public void Can_generate_scripts_to_create_sql_objects(Syntax syntax)
        {
            #region Arrange

            string sql = null;
            bool generatedSqlIsExecutable = false;
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
                new Column("Year", SchemaType.SMALLINT)
                )
            { Schema = schema };

            var seed = new Script()
            {
                Syntax = syntax,
                Content = "INSERT INTO genre (Name) VAlUES ('Hip Hop'), ('R&B'), ('Country');",
            };

            var generic = new Script("-- If you are reading this, I don't discriminate.", Syntax.Generic);
            var virus = new Script("-- If you are reading this, it's to late. (I should not be here!)", (syntax + 1));

            var name_idx = new Index(IndexType.Index, new ColumnName("Name", Order.DESC)) { Table = song };
            var releaseDate = new Column("ReleaseDate", new DataType(SchemaType.TIMESTAMP), defaultValue: "''") { Table = song };

            var song_fk = new ForeignKey("SongId", "song", "Id") { Table = album };
            var pKey = new Index(IndexType.PrimaryKey, new ColumnName("SongId"), new ColumnName("ArtistId")) { Table = album };

            var factory = new SqlWriterFactory();

            #endregion Arrange

            // Act
            Database.Refresh(syntax);
            TestData.CreateDirectory(scriptFile);
            if (File.Exists(scriptFile)) File.Delete(scriptFile);

            using (var file = File.OpenWrite(scriptFile))
            using (var writer = factory.CreateInstance(syntax, file))
            {
                System.Diagnostics.Debug.WriteLine($"using {writer.GetType().Name}");

                schema.Add(genre, song, album);
                schema.Add(seed, virus, generic);
                writer.Create(schema);

                writer.Create(releaseDate);
                song.Columns.Add(releaseDate);

                writer.Create(song_fk);
                album.ForeignKeys.Add(song_fk);

                writer.Create(name_idx);
                song.Indecies.Add(name_idx);

                writer.Create(pKey);
                file.Flush();
            }

            using (var db = new Database(syntax))
            {
                sql = File.ReadAllText(scriptFile);
                generatedSqlIsExecutable = db.TryExecute(sql, out string error);
                var nl = string.Concat(Enumerable.Repeat(Environment.NewLine, 3));
                sql = string.Format("{0}SYNTAX: {1}{3}{2}", error, syntax, sql, nl);
            }

            // Assert
            Diff.Approve(sql, ".sql", syntax);
            generatedSqlIsExecutable.ShouldBeTrue();
        }

        [DataTestMethod]
        [DataRow(Syntax.SQLite)]
        public void Can_generate_scripts_to_drop_sql_objects(Syntax syntax)
        {
            throw new System.NotImplementedException();
        }

        [DataTestMethod]
        [DataRow(Syntax.SQLite)]
        public void Can_generate_scripts_to_alter_sql_objects(Syntax syntax)
        {
            throw new System.NotImplementedException();
        }

        private static bool IsWellFormed(string file, Syntax syntax)
        {
            return false;
        }
    }
}