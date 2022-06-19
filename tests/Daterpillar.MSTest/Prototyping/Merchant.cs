using Acklann.Daterpillar.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Acklann.Daterpillar.Prototyping
{
	//[Table(SQL.TABLE)]
	//public class Merchant
	//{
	//	[Key, Column(SQL.id)]
	//	public string Id { get; set; }

	//	[Column(SQL.name)]
	//	public string Name { get; set; }

	//	[Column(SQL.first_name)]
	//	[Column(SQL.last_name)]
	//	public Usaddress.FullName Agent { get; set; }

	//	/// <summary>
	//	/// Gets or sets the primary email of the company.
	//	/// </summary>
	//	public string Email { get; set; }

	//	/// <summary>
	//	/// Gets or sets the primary mailing address of the company.
	//	/// </summary>
	//	public Usaddress.FullName MailingAddress { get; set; }

	//	/// <summary>
	//	/// Gets or sets the primary telephone number of the company.
	//	/// </summary>
	//	public string Phone { get; set; }

	//	public class SQL
	//	{
	//		public const string TABLE = "merchant";
	//		public const string id = "id";
	//		public const string name = "name";
	//		public const string first_name = "first_name";
	//		public const string last_name = "last_name";
	//		public const string street1 = "street1";
	//		public const string street2 = "street2";
	//		public const string city = "city";
	//		public const string state = "state";
	//		public const string country = "country";
	//		public const string postal_code = "postal_code";
	//	}
	//}
}