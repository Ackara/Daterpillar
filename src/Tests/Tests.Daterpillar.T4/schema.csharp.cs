using System;
using Gigobyte.Daterpillar.Data;
using System.Runtime.Serialization;
using Gigobyte.Daterpillar.Annotation;

namespace GeneratedCode
{
	/// <summary>
	/// Represents the [tableA] table.
	/// </summary>
	[DataContract]
	[Table("tableA")]
	public partial class TableA : EntityBase
	{
		#region Constants
	
		/// <summary>
		/// The tableA table identifier.
		/// </summary>
		public const string Table = "tableA";
	
		/// <summary>
		/// The [tableA].[Id] column identifier.
		/// </summary>
		public const string IdColumn = "Id";
	
		/// <summary>
		/// The [tableA].[Name] column identifier.
		/// </summary>
		public const string NameColumn = "Name";
	
		#endregion Constants
	
		/// <summary>
		/// Get or set the [tableA].[Id] column value.
		/// </summary>
		[DataMember]
		[Column("Id", IsKey = true, AutoIncrement = true)]
		public int Id { get; set; }
	
		/// <summary>
		/// Get or set the [tableA].[Name] column value.
		/// </summary>
		[DataMember]
		[Column("Name")]
		public string Name { get; set; }
	}
	
	/// <summary>
	/// Represents the [tableB] table.
	/// </summary>
	[DataContract]
	[Table("tableB")]
	public partial class TableB : EntityBase
	{
		#region Constants
	
		/// <summary>
		/// The tableB table identifier.
		/// </summary>
		public const string Table = "tableB";
	
		/// <summary>
		/// The [tableB].[Id] column identifier.
		/// </summary>
		public const string IdColumn = "Id";
	
		/// <summary>
		/// The [tableB].[Date] column identifier.
		/// </summary>
		public const string DateColumn = "Date";
	
		/// <summary>
		/// The [tableB].[TableA_Id] column identifier.
		/// </summary>
		public const string TableAIdColumn = "TableA_Id";
	
		/// <summary>
		/// The [tableB].[Age] column identifier.
		/// </summary>
		public const string AgeColumn = "Age";
	
		#endregion Constants
	
		/// <summary>
		/// Get or set the [tableB].[Id] column value.
		/// </summary>
		[DataMember]
		[Column("Id", IsKey = true)]
		public int Id { get; set; }
	
		/// <summary>
		/// Get or set the [tableB].[Date] column value.
		/// </summary>
		[DataMember]
		[Column("Date")]
		public DateTime Date { get; set; }
	
		/// <summary>
		/// Get or set the [tableB].[TableA_Id] column value.
		/// </summary>
		[DataMember]
		[Column("TableA_Id")]
		public int TableAId { get; set; }
	
		/// <summary>
		/// Get or set the [tableB].[Age] column value.
		/// </summary>
		[DataMember]
		[Column("Age")]
		public int Age { get; set; }
	}
}