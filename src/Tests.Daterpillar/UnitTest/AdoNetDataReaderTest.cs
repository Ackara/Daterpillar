using Gigobyte.Daterpillar.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Runtime.Serialization;

namespace Tests.Daterpillar.UnitTest
{
    [TestClass]
    [DeploymentItem(Test.Data.Samples)]
    public class AdoNetDataReaderTest
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        [Owner(Test.Dev.Ackara)]
        [DataSource(Test.Data.Provider, (Test.Data.Directory + Test.File.SongCSV), Test.File.SongCSV, DataAccessMethod.Sequential)]
        public void CreateInstance_should_convert_DataRow_object_to_an_EntityBase_object()
        {
            // Arrange
            var name = Convert.ToString(TestContext.DataRow["Name"]);
            var price = Convert.ToDecimal(TestContext.DataRow["Price"]);
            var onDevice = Convert.ToBoolean(TestContext.DataRow["On_Device"]);

            var returnType = typeof(Song);
            var sut = new AdoNetDataReader();

            // Act
            var entity = (Song)sut.CreateInstance(returnType, TestContext.DataRow);

            // Assert
            Assert.AreEqual((name == string.Empty ? null : name), entity.Name);
            Assert.AreEqual(price, entity.Price);
            Assert.AreEqual(onDevice, entity.OnDevice);
        }

        #region Private

        /// <summary>
        /// Represents the [song] table.
        /// </summary>
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
            [Column("Id", IsKey = true, AutoIncrement = true)]
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

        #endregion Private
    }
}