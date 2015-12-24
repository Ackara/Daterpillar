using Gigobyte.Daterpillar.Data.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Tests.Daterpillar.UnitTest
{
    [TestClass]
    public class SqlWriterTest
    {
        public TestContext TestContext { get; set; }

        /// <summary>
        /// Assert <see cref="SqlWriter.ConvertToSelectCommand(Gigobyte.Daterpillar.Data.EntityBase, SqlStyle)"/> converts a specified <see cref="Gigobyte.Daterpillar.Data.EntityBase"/> into a SQL query.
        /// </summary>
        [TestMethod]
        [Owner(Str.Ackara)]
        public void ConvertEntityToQuery()
        {
            var song = Samples.GetSong();
            var query = SqlWriter.ConvertToSelectCommand(song, SqlStyle.SQL);
            string expected = $"SELECT * FROM song WHERE Id='{song.Id}';";

            TestContext.WriteLine("e: " + expected);
            TestContext.WriteLine("a: " + query);

            Assert.AreEqual(expected, query);
        }

        /// <summary>
        /// Assert <see cref="SqlWriter.ConvertToInsertCommand(Gigobyte.Daterpillar.Data.EntityBase, SqlStyle)"/> converts a specified <see cref="Gigobyte.Daterpillar.Data.EntityBase"/> to a SQL insert command.
        /// </summary>
        [TestMethod]
        [Owner(Str.Ackara)]
        public void ConvertEntityToInsertCommand()
        {
            var song = Samples.GetSong();
            var query = SqlWriter.ConvertToInsertCommand(song, SqlStyle.SQLite);
            string expected = $"INSERT INTO [song] ([Name], [Length], [Price], [Album_Id], [Artist_Id], [Genre_Id]) VALUES ('{song.Name}', '{song.Length}', '{song.Price}', '{song.AlbumId}', '{song.ArtistId}', '{song.GenreId}');";

            TestContext.WriteLine("e: " + expected);
            TestContext.WriteLine("a: " + query);

            Assert.AreEqual(expected, query);
        }

        /// <summary>
        /// Assert <see cref="SqlWriter.ConvertToDeleteCommand(Gigobyte.Daterpillar.Data.EntityBase, SqlStyle)"/> converts a specified <see cref="Gigobyte.Daterpillar.Data.EntityBase"/> to a SQL delete command.
        /// </summary>
        [TestMethod]
        [Owner(Str.Ackara)]
        public void ConvertEntityToDeleteCommand()
        {
            var song = Samples.GetSong();
            var query = SqlWriter.ConvertToDeleteCommand(song, SqlStyle.SQLite);
            string expected = $"DELETE FROM [song] WHERE [Id]='{song.Id}';";

            TestContext.WriteLine("e: " + expected);
            TestContext.WriteLine("a: " + query);

            Assert.AreEqual(expected, query);
        }

        /// <summary>
        /// Assert <see cref="SqlWriter.EscapeValue(object)"/> escapes a <see cref="DateTime"/> value.
        /// </summary>
        [TestMethod]
        [Owner(Str.Ackara)]
        public void ConvertDateToSqlValue()
        {
            Assert.AreEqual("'2015-01-01 13:11:11'", SqlWriter.EscapeValue(new DateTime(2015, 01, 01, 13, 11, 11, DateTimeKind.Utc)));
        }

        /// <summary>
        /// Assert <see cref="SqlWriter.EscapeValue(object)"/> escapes a <see cref="int"/> value.
        /// </summary>
        [TestMethod]
        [Owner(Str.Ackara)]
        public void ConvertIntegerToSqlValue()
        {
            Assert.AreEqual("'123'", SqlWriter.EscapeValue(123));
        }

        /// <summary>
        /// Assert <see cref="SqlWriter.EscapeValue(object)"/> escapes a <see cref="float"/> value.
        /// </summary>
        [TestMethod]
        [Owner(Str.Ackara)]
        public void ConvertFloatToSqlValue()
        {
            Assert.AreEqual("'123.45'", SqlWriter.EscapeValue(123.45F));
        }

        /// <summary>
        /// Assert <see cref="SqlWriter.EscapeValue(object)"/> escapes a <see cref="bool"/> value.
        /// </summary>
        [TestMethod]
        [Owner(Str.Ackara)]
        public void ConvertBooleanToSqlValue()
        {
            Assert.AreEqual("'1'", SqlWriter.EscapeValue(true));
        }

        /// <summary>
        /// Assert <see cref="SqlWriter.EscapeValue(object)"/> escapes a <see cref="string"/> value.
        /// </summary>
        [TestMethod]
        [Owner(Str.Ackara)]
        public void ConvertStringToSqlValue()
        {
            Assert.AreEqual("'the dog''s bowl. ''_'' ^_^ ''_'''", SqlWriter.EscapeValue("the dog's bowl. '_' ^_^ '_'"));
        }

        /// <summary>
        /// Assert <see cref="SqlWriter.EscapeValue(object)"/> escapes a <see cref="Enum"/> value.
        /// </summary>
        [TestMethod]
        [Owner(Str.Ackara)]
        public void ConvertEnumToSqlValue()
        {
            Assert.AreEqual("'0'", SqlWriter.EscapeValue(DayOfWeek.Sunday));
        }
    }
}