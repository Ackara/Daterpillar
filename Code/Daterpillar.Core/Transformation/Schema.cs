using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace Ackara.Daterpillar.Transformation
{
    [XmlRoot("schema", Namespace = Xmlns)]
    public class Schema
    {
        #region Static Members

        public const string Xmlns = "http://schema.gigobyte.com/v1/xsml.xsd";

        public static Schema Load(Stream stream)
        {
            var serializer = new XmlSerializer(typeof(Schema));
            return (Schema)serializer.Deserialize(stream);
        }

        public static Schema Parse(string text)
        {
            using (var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(text)))
            {
                var serializer = new XmlSerializer(typeof(Schema));
                return (Schema)serializer.Deserialize(stream);
            }
        }

        #endregion Static Members

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("author")]
        public string Author { get; set; }

        [XmlElement("table")]
        public List<Table> Tables
        {
            get
            {
                if (_tables == null)
                {
                    _tables = new List<Table>();
                }

                return _tables;
            }
            set { _tables = value; }
        }

        /// <summary>
        /// Write this <see cref="Schema"/> to the specified <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">The stream to output this <see cref="Schema"/> to.</param>
        public void WriteTo(Stream stream)
        {
            var serializer = new XmlSerializer(typeof(Schema));
            serializer.Serialize(stream, this);
            stream.Position = 0;
        }

        #region Private Member

        private List<Table> _tables;

        #endregion Private Member
    }
}