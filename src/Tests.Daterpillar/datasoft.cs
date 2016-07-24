using System;
using Gigobyte.Daterpillar.Data;
using System.Runtime.Serialization;
using Gigobyte.Daterpillar.Annotation;

namespace GeneratedCode
{
	/// <summary>
	/// Represents the [employee] table.
	/// </summary>
	[DataContract]
	[Table("employee")]
	public partial class Employee : EntityBase
	{
		#region Constants
	
		/// <summary>
		/// The employee table identifier.
		/// </summary>
		public const string Table = "employee";
	
		/// <summary>
		/// The [employee].[Id] column identifier.
		/// </summary>
		public const string IdColumn = "Id";
	
		/// <summary>
		/// The [employee].[Dob] column identifier.
		/// </summary>
		public const string DobColumn = "Dob";
	
		/// <summary>
		/// The [employee].[First_Name] column identifier.
		/// </summary>
		public const string FirstNameColumn = "First_Name";
	
		/// <summary>
		/// The [employee].[Last_Name] column identifier.
		/// </summary>
		public const string LastNameColumn = "Last_Name";
	
		/// <summary>
		/// The [employee].[Gender] column identifier.
		/// </summary>
		public const string GenderColumn = "Gender";
	
		#endregion Constants
	
		/// <summary>
		/// Get or set the [employee].[Id] column value.
		/// </summary>
		[Column("Id", IsKey = true, AutoIncrement = true)]
		[DataMember]
		public virtual int Id { get; set; }
	
		/// <summary>
		/// Get or set the [employee].[Dob] column value.
		/// </summary>
		[Column("Dob")]
		[DataMember]
		public virtual DateTime Dob { get; set; }
	
		/// <summary>
		/// Get or set the [employee].[First_Name] column value.
		/// </summary>
		[Column("First_Name")]
		[DataMember]
		public virtual string FirstName { get; set; }
	
		/// <summary>
		/// Get or set the [employee].[Last_Name] column value.
		/// </summary>
		[Column("Last_Name")]
		[DataMember]
		public virtual string LastName { get; set; }
	
		/// <summary>
		/// Get or set the [employee].[Gender] column value.
		/// </summary>
		[Column("Gender")]
		[DataMember]
		public virtual int Gender { get; set; }
	}
	
	/// <summary>
	/// Represents the [department] table.
	/// </summary>
	[DataContract]
	[Table("department")]
	public partial class Department : EntityBase
	{
		#region Constants
	
		/// <summary>
		/// The department table identifier.
		/// </summary>
		public const string Table = "department";
	
		/// <summary>
		/// The [department].[Id] column identifier.
		/// </summary>
		public const string IdColumn = "Id";
	
		/// <summary>
		/// The [department].[Name] column identifier.
		/// </summary>
		public const string NameColumn = "Name";
	
		#endregion Constants
	
		/// <summary>
		/// Get or set the [department].[Id] column value.
		/// </summary>
		[Column("Id", IsKey = true, AutoIncrement = true)]
		[DataMember]
		public virtual int Id { get; set; }
	
		/// <summary>
		/// Get or set the [department].[Name] column value.
		/// </summary>
		[Column("Name")]
		[DataMember]
		public virtual string Name { get; set; }
	}
	
	/// <summary>
	/// Represents the [salary] table.
	/// </summary>
	[DataContract]
	[Table("salary")]
	public partial class Salary : EntityBase
	{
		#region Constants
	
		/// <summary>
		/// The salary table identifier.
		/// </summary>
		public const string Table = "salary";
	
		/// <summary>
		/// The [salary].[Employee_Id] column identifier.
		/// </summary>
		public const string EmployeeIdColumn = "Employee_Id";
	
		/// <summary>
		/// The [salary].[Start] column identifier.
		/// </summary>
		public const string StartColumn = "Start";
	
		/// <summary>
		/// The [salary].[End] column identifier.
		/// </summary>
		public const string EndColumn = "End";
	
		/// <summary>
		/// The [salary].[Amount] column identifier.
		/// </summary>
		public const string AmountColumn = "Amount";
	
		#endregion Constants
	
		/// <summary>
		/// Get or set the [salary].[Employee_Id] column value.
		/// </summary>
		[Column("Employee_Id", IsKey = true)]
		[DataMember]
		public virtual int EmployeeId { get; set; }
	
		/// <summary>
		/// Get or set the [salary].[Start] column value.
		/// </summary>
		[Column("Start", IsKey = true)]
		[DataMember]
		public virtual DateTime Start { get; set; }
	
		/// <summary>
		/// Get or set the [salary].[End] column value.
		/// </summary>
		[Column("End")]
		[DataMember]
		public virtual DateTime End { get; set; }
	
		/// <summary>
		/// Get or set the [salary].[Amount] column value.
		/// </summary>
		[Column("Amount")]
		[DataMember]
		public virtual decimal Amount { get; set; }
	}
	
	/// <summary>
	/// Represents the [gender] table.
	/// </summary>
	[DataContract]
	[Table("gender")]
	public partial class Gender : EntityBase
	{
		#region Constants
	
		/// <summary>
		/// The gender table identifier.
		/// </summary>
		public const string Table = "gender";
	
		/// <summary>
		/// The [gender].[Id] column identifier.
		/// </summary>
		public const string IdColumn = "Id";
	
		/// <summary>
		/// The [gender].[Name] column identifier.
		/// </summary>
		public const string NameColumn = "Name";
	
		#endregion Constants
	
		/// <summary>
		/// Get or set the [gender].[Id] column value.
		/// </summary>
		[Column("Id", IsKey = true, AutoIncrement = true)]
		[DataMember]
		public virtual int Id { get; set; }
	
		/// <summary>
		/// Get or set the [gender].[Name] column value.
		/// </summary>
		[Column("Name")]
		[DataMember]
		public virtual string Name { get; set; }
	}
}
