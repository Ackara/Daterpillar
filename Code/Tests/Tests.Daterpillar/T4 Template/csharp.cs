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
		/// The [employee].[birth_date] column identifier.
		/// </summary>
		public const string BirthDateColumn = "birth_date";
	
		/// <summary>
		/// The [employee].[first_name] column identifier.
		/// </summary>
		public const string FirstNameColumn = "first_name";
	
		/// <summary>
		/// The [employee].[last_name] column identifier.
		/// </summary>
		public const string LastNameColumn = "last_name";
	
		/// <summary>
		/// The [employee].[gender] column identifier.
		/// </summary>
		public const string GenderColumn = "gender";
	
		/// <summary>
		/// The [employee].[hire_date] column identifier.
		/// </summary>
		public const string HireDateColumn = "hire_date";
	
		#endregion Constants
	
		/// <summary>
		/// Get or set the [employee].[Id] column value.
		/// </summary>
		[DataMember]
		[Column("Id", IsKey = true)]
		public virtual int Id { get; set; }
	
		/// <summary>
		/// Get or set the [employee].[birth_date] column value.
		/// </summary>
		[DataMember]
		[Column("birth_date")]
		public virtual DateTime BirthDate { get; set; }
	
		/// <summary>
		/// Get or set the [employee].[first_name] column value.
		/// </summary>
		[DataMember]
		[Column("first_name")]
		public virtual string FirstName { get; set; }
	
		/// <summary>
		/// Get or set the [employee].[last_name] column value.
		/// </summary>
		[DataMember]
		[Column("last_name")]
		public virtual string LastName { get; set; }
	
		/// <summary>
		/// Get or set the [employee].[gender] column value.
		/// </summary>
		[DataMember]
		[Column("gender")]
		public virtual int Gender { get; set; }
	
		/// <summary>
		/// Get or set the [employee].[hire_date] column value.
		/// </summary>
		[DataMember]
		[Column("hire_date")]
		public virtual DateTime HireDate { get; set; }
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
		/// The [salary].[amount] column identifier.
		/// </summary>
		public const string AmountColumn = "amount";
	
		/// <summary>
		/// The [salary].[from_date] column identifier.
		/// </summary>
		public const string FromDateColumn = "from_date";
	
		/// <summary>
		/// The [salary].[to_date] column identifier.
		/// </summary>
		public const string ToDateColumn = "to_date";
	
		#endregion Constants
	
		/// <summary>
		/// Get or set the [salary].[Employee_Id] column value.
		/// </summary>
		[DataMember]
		[Column("Employee_Id", IsKey = true)]
		public virtual int EmployeeId { get; set; }
	
		/// <summary>
		/// Get or set the [salary].[amount] column value.
		/// </summary>
		[DataMember]
		[Column("amount")]
		public virtual decimal Amount { get; set; }
	
		/// <summary>
		/// Get or set the [salary].[from_date] column value.
		/// </summary>
		[DataMember]
		[Column("from_date", IsKey = true)]
		public virtual DateTime FromDate { get; set; }
	
		/// <summary>
		/// Get or set the [salary].[to_date] column value.
		/// </summary>
		[DataMember]
		[Column("to_date")]
		public virtual DateTime ToDate { get; set; }
	}
}