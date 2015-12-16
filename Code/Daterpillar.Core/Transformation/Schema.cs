using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Ackara.Daterpillar.Transformation
{
    public class Schema : XDocument
    {
        public string Path { get; set; }

        [XmlElement(Table.TagName)]
        public List<Table> Objects { get; set; }
    }
}
