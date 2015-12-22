using System;
using System.Xml.Serialization;

namespace Gigobyte.Daterpillar.Transformation
{
    public class ForeignKey
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("localColumn")]
        public string LocalColumn { get; set; }

        [XmlAttribute("foreignTable")]
        public string ForeignTable { get; set; }

        [XmlAttribute("foreignColumn")]
        public string ForeignColumn { get; set; }

        [XmlElement("onUpdate")]
        public string OnUpdate { get; set; }

        [XmlElement("onDelete")]
        public string OnDelete { get; set; }

        [XmlIgnore]
        public ForeignKeyRule OnUpdateRule
        {
            get { return (ForeignKeyRule)Enum.Parse(typeof(ForeignKeyRule), OnUpdate.Replace(" ", "_")); }
            set { OnUpdate = value.ToText(); }
        }

        [XmlIgnore]
        public ForeignKeyRule OnDeleteRule
        {
            get { return (ForeignKeyRule)Enum.Parse(typeof(ForeignKeyRule), OnDelete.Replace(" ", "_")); }
            set { OnDelete = value.ToText(); }
        }
    }
}