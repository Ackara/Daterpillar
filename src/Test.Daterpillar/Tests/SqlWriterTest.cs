using Acklann.Daterpillar.Data;
using Acklann.Daterpillar.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Tests.Daterpillar.Constants;

namespace Tests.Daterpillar.Tests
{
    [TestClass]
    public class SqlWriterTest
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        [Owner(Dev.Ackara)]
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
        [Owner(Dev.Ackara)]
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
        [Owner(Dev.Ackara)]
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
        [Owner(Dev.Ackara)]
        public void EscapeValue_should_convert_a_date_object_into_a_sql_value()
        {
            Assert.AreEqual("'2015-01-01 13:11:11'", SqlWriter.EscapeValue(new DateTime(2015, 01, 01, 13, 11, 11, DateTimeKind.Utc)));
        }

        [TestMethod]
        [Owner(Dev.Ackara)]
        public void EscapeValue_should_convert_a_integer_object_into_a_sql_value()
        {
            Assert.AreEqual("'123'", SqlWriter.EscapeValue(123));
        }

        [TestMethod]
        [Owner(Dev.Ackara)]
        public void EscapeValue_should_convert_a_float_object_into_a_sql_value()
        {
            Assert.AreEqual("'123.45'", SqlWriter.EscapeValue(123.45F));
        }

        [TestMethod]
        [Owner(Dev.Ackara)]
        public void EscapeValue_should_convert_a_boolean_object_into_a_sql_value()
        {
            Assert.AreEqual("'1'", SqlWriter.EscapeValue(true));
        }

        [TestMethod]
        [Owner(Dev.Ackara)]
        public void EscapeValue_should_convert_a_string_into_a_sql_value()
        {
            Assert.AreEqual("'the dog''s bowl. ''_'' ^_^ ''_'''", SqlWriter.EscapeValue("the dog's bowl. '_' ^_^ '_'"));
        }

        [TestMethod]
        [Owner(Dev.Ackara)]
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

        [DataContract]
        [Table("song")]
        public partial class Song : EntityBase
        {
            #region Constants

            /// <summary>
            /// The song table identifier.
            /// </summary>
            public const string Table = "song";

            /// <summary>
            /// The [song].[Id] column identifier.
            /// </summary>
            public const string IdColumn = "Id";

            /// <summary>
            /// The [song].[Album_Id] column identifier.
            /// </summary>
            public const string AlbumIdColumn = "Album_Id";

            /// <summary>
            /// The [song].[Artist_Id] column identifier.
            /// </summary>
            public const string ArtistIdColumn = "Artist_Id";

            /// <summary>
            /// The [song].[Genre_Id] column identifier.
            /// </summary>
            public const string GenreIdColumn = "Genre_Id";

            /// <summary>
            /// The [song].[Name] column identifier.
            /// </summary>
            public const string NameColumn = "Name";

            /// <summary>
            /// The [song].[Length] column identifier.
            /// </summary>
            public const string LengthColumn = "Length";

            /// <summary>
            /// The [song].[Price] column identifier.
            /// </summary>
            public const string PriceColumn = "Price";

            /// <summary>
            /// The [song].[On_Device] column identifier.
            /// </summary>
            public const string OnDeviceColumn = "On_Device";

            #endregion Constants

            /// <summary>
            /// Get or set the [song].[Id] column value.
            /// </summary>
            [DataMember]
            [Column("Id", Key = true, AutoIncrement = true)]
            public virtual int Id { get; set; }

            /// <summary>
            /// Get or set the [song].[Album_Id] column value.
            /// </summary>
            [DataMember]
            [Column("Album_Id")]
            public virtual int AlbumId { get; set; }

            /// <summary>
            /// Get or set the [song].[Artist_Id] column value.
            /// </summary>
            [DataMember]
            [Column("Artist_Id")]
            public virtual int ArtistId { get; set; }

            /// <summary>
            /// Get or set the [song].[Genre_Id] column value.
            /// </summary>
            [DataMember]
            [Column("Genre_Id")]
            public virtual int GenreId { get; set; }

            /// <summary>
            /// Get or set the [song].[Name] column value.
            /// </summary>
            [DataMember]
            [Column("Name")]
            public virtual string Name { get; set; }

            /// <summary>
            /// Get or set the [song].[Length] column value.
            /// </summary>
            [DataMember]
            [Column("Length")]
            public virtual decimal Length { get; set; }

            /// <summary>
            /// Get or set the [song].[Price] column value.
            /// </summary>
            [DataMember]
            [Column("Price")]
            public virtual decimal Price { get; set; }

            /// <summary>
            /// Get or set the [song].[On_Device] column value.
            /// </summary>
            [DataMember]
            [Column("On_Device")]
            public virtual bool OnDevice { get; set; }
        }

        #endregion Private Members
    }
}