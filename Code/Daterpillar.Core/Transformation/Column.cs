using System.Collections.Generic;
using System.Xml.Serialization;

namespace Gigobyte.Daterpillar.Transformation
{
    public class Column
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("autoIncrement")]
        public bool AutoIncrement { get; set; }

        [XmlElement("comment")]
        public string Comment { get; set; }

        [XmlElement("dataType")]
        public DataType DataType { get; set; }

        [XmlElement("modifier")]
        public List<string> Modifiers
        {
            get
            {
                if (_modifiers == null)
                {
                    _modifiers = new List<string>();
                }

                return _modifiers;
            }
            set { _modifiers = value; }
        }

        #region Private Members

        private List<string> _modifiers;

        #endregion Private Members
    }
}