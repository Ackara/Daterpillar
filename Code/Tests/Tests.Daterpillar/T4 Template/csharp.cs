using System;
using Gigobyte.Daterpillar.Data;
using System.Runtime.Serialization;
using Gigobyte.Daterpillar.Annotation;

namespace GeneratedCode
{
	/// <summary>
	/// Represents the [artist] table.
	/// </summary>
	[DataContract]
	[Table("artist")]
	public partial class Artist : EntityBase
	{
		#region Constants
	
		/// <summary>
		/// The artist table identifier.
		/// </summary>
		public const string Table = "artist";
	
		/// <summary>
		/// The [artist].[Id] column identifier.
		/// </summary>
		public const string IdColumn = "Id";
	
		/// <summary>
		/// The [artist].[Name] column identifier.
		/// </summary>
		public const string NameColumn = "Name";
	
		/// <summary>
		/// The [artist].[Bio] column identifier.
		/// </summary>
		public const string BioColumn = "Bio";
	
		#endregion Constants
	
		/// <summary>
		/// Get or set the [artist].[Id] column value.
		/// </summary>
		[DataMember]
		[Column("Id", IsKey = true, AutoIncrement = true)]
		public virtual int Id { get; set; }
	
		/// <summary>
		/// Get or set the [artist].[Name] column value.
		/// </summary>
		[DataMember]
		[Column("Name")]
		public virtual string Name { get; set; }
	
		/// <summary>
		/// Get or set the [artist].[Bio] column value.
		/// </summary>
		[DataMember]
		[Column("Bio")]
		public virtual string Bio { get; set; }
	}
	
	/// <summary>
	/// Represents the [genre] table.
	/// </summary>
	[DataContract]
	[Table("genre")]
	public partial class Genre : EntityBase
	{
		#region Constants
	
		/// <summary>
		/// The genre table identifier.
		/// </summary>
		public const string Table = "genre";
	
		/// <summary>
		/// The [genre].[Id] column identifier.
		/// </summary>
		public const string IdColumn = "Id";
	
		/// <summary>
		/// The [genre].[Name] column identifier.
		/// </summary>
		public const string NameColumn = "Name";
	
		#endregion Constants
	
		/// <summary>
		/// Get or set the [genre].[Id] column value.
		/// </summary>
		[DataMember]
		[Column("Id", IsKey = true, AutoIncrement = true)]
		public virtual int Id { get; set; }
	
		/// <summary>
		/// Get or set the [genre].[Name] column value.
		/// </summary>
		[DataMember]
		[Column("Name")]
		public virtual string Name { get; set; }
	}
	
	/// <summary>
	/// Represents the [album] table.
	/// </summary>
	[DataContract]
	[Table("album")]
	public partial class Album : EntityBase
	{
		#region Constants
	
		/// <summary>
		/// The album table identifier.
		/// </summary>
		public const string Table = "album";
	
		/// <summary>
		/// The [album].[Artist_Id] column identifier.
		/// </summary>
		public const string ArtistIdColumn = "Artist_Id";
	
		/// <summary>
		/// The [album].[Name] column identifier.
		/// </summary>
		public const string NameColumn = "Name";
	
		#endregion Constants
	
		/// <summary>
		/// Get or set the [album].[Artist_Id] column value.
		/// </summary>
		[DataMember]
		[Column("Artist_Id", IsKey = true)]
		public virtual int ArtistId { get; set; }
	
		/// <summary>
		/// Get or set the [album].[Name] column value.
		/// </summary>
		[DataMember]
		[Column("Name", IsKey = true)]
		public virtual string Name { get; set; }
	}
	
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
	
		#endregion Constants
	
		/// <summary>
		/// Get or set the [song].[Id] column value.
		/// </summary>
		[DataMember]
		[Column("Id", IsKey = true, AutoIncrement = true)]
		public virtual int Id { get; set; }
	
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
	}
}