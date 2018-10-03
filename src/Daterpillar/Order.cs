
using System.Xml.Serialization;

namespace Acklann.Daterpillar
{
	/// <summary>
	/// A kind of sorting order.
	/// </summary>
	public enum Order
	{
		[XmlEnum("asc")]
		ASC,
		[XmlEnum("desc")]
		DESC,
	}
}