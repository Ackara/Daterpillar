using System;
using Gigobyte.Daterpillar.Data;
using System.Runtime.Serialization;
using Gigobyte.Daterpillar.Annotation;

namespace GeneratedCode
{
	/// <summary>
	/// Represents the [TableA] table.
	/// </summary>
	[DataContract]
	[Table("TableA")]
	public partial class TableA : EntityBase
	{
		#region Constants
	
		/// <summary>
		/// The TableA table identifier.
		/// </summary>
		public const string Table = "TableA";
	
		/// <summary>
		/// The [TableA].[Id] column identifier.
		/// </summary>
		public const string IdColumn = "Id";
	
		/// <summary>
		/// The [TableA].[Name] column identifier.
		/// </summary>
		public const string NameColumn = "Name";
	
		#endregion Constants
	
		/// <summary>
		/// Get or set the [TableA].[Id] column value.
		/// </summary>
		[Column("Id", IsKey = true, AutoIncrement = true)]
		[DataMember]
		public virtual int Id { get; set; }
	
		/// <summary>
		/// Get or set the [TableA].[Name] column value.
		/// </summary>
		[Column("Name")]
		[DataMember]
		public virtual string Name { get; set; }
	}
}
