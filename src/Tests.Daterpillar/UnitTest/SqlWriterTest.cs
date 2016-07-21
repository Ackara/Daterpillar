using Gigobyte.Daterpillar.Annotation;
using Gigobyte.Daterpillar.Data;
using Gigobyte.Daterpillar.Data.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Tests.Daterpillar.Sample;

namespace Tests.Daterpillar.UnitTest
{
    [TestClass]
    public class SqlWriterTest
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        [Owner(Test.Dev.Ackara)]
        public void ConvertToSelectCommand_should_format_an_entity_object_into_a_select_query()
        {
            var song = CreateSong();
            var query = SqlWriter.ConvertToSelectCommand(song, QueryStyle.SQL);
            string expected = $"SELECT * FROM song WHERE Id='{song.Id}';";

            TestContext.WriteLine("e: " + expected);
            TestContext.WriteLine("a: " + query);

            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        [Owner(Test.Dev.Ackara)]
        public void ConvertToInsertCommand_should_format_an_entity_object_into_a_insert_command()
        {
            var song = CreateSong();
            var query = SqlWriter.ConvertToInsertCommand(song, QueryStyle.SQLite);
            string expected = $"INSERT INTO [song] ([Album_Id], [Artist_Id], [Genre_Id], [Name], [Length], [Price], [On_Device]) VALUES ('{song.AlbumId}', '{song.ArtistId}', '{song.GenreId}', '{song.Name}', '{song.Length}', '{song.Price}', '1');";

            TestContext.WriteLine("e: " + expected);
            TestContext.WriteLine("a: " + query);

            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        [Owner(Test.Dev.Ackara)]
        public void ConvertToDeleteCommand_should_format_an_entity_object_into_a_delete_command()
        {
            var song = CreateSong();
            var query = SqlWriter.ConvertToDeleteCommand(song, QueryStyle.SQLite);
            string expected = $"DELETE FROM [song] WHERE [Id]='{song.Id}';";

            TestContext.WriteLine("e: " + expected);
            TestContext.WriteLine("a: " + query);

            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        [Owner(Test.Dev.Ackara)]
        public void EscapeValue_should_convert_a_date_object_into_a_sql_value()
        {
            Assert.AreEqual("'2015-01-01 13:11:11'", SqlWriter.EscapeValue(new DateTime(2015, 01, 01, 13, 11, 11, DateTimeKind.Utc)));
        }

        [TestMethod]
        [Owner(Test.Dev.Ackara)]
        public void EscapeValue_should_convert_a_integer_object_into_a_sql_value()
        {
            Assert.AreEqual("'123'", SqlWriter.EscapeValue(123));
        }

        [TestMethod]
        [Owner(Test.Dev.Ackara)]
        public void EscapeValue_should_convert_a_float_object_into_a_sql_value()
        {
            Assert.AreEqual("'123.45'", SqlWriter.EscapeValue(123.45F));
        }

        [TestMethod]
        [Owner(Test.Dev.Ackara)]
        public void EscapeValue_should_convert_a_boolean_object_into_a_sql_value()
        {
            Assert.AreEqual("'1'", SqlWriter.EscapeValue(true));
        }

        [TestMethod]
        [Owner(Test.Dev.Ackara)]
        public void EscapeValue_should_convert_a_string_into_a_sql_value()
        {
            Assert.AreEqual("'the dog''s bowl. ''_'' ^_^ ''_'''", SqlWriter.EscapeValue("the dog's bowl. '_' ^_^ '_'"));
        }

        [TestMethod]
        [Owner(Test.Dev.Ackara)]
        public void EscapeValue_should_convert_a_enum_object_into_a_sql_value()
        {
            Assert.AreEqual("'0'", SqlWriter.EscapeValue(DayOfWeek.Sunday));
        }

        #region Private Members

        private static Song CreateSong([CallerMemberName]string name = null)
        {
            return new Song()
            {
                Id = 154,
                Name = name,
                Length = 12,
                Price = 1.29M,
                AlbumId = 1,
                ArtistId = 1,
                GenreId = 1,
                OnDevice = true
            };
        }

        
        #endregion Private Members
    }
}