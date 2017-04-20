using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Ackara.Daterpillar
{
    /// <summary>
    /// An in-memory representation of a database schema.
    /// </summary>
    [XmlRoot("schema", Namespace = Namespace)]
    public class Schema
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Schema"/> class.
        /// </summary>
        public Schema()
        {
            _namespaces = new XmlSerializerNamespaces(new XmlQualifiedName[]
            {
                new XmlQualifiedName(string.Empty, Namespace)
            });

            Tables = new List<Table>();
            Scripts = new List<Script>();
        }

        /// <summary>
        /// The xml namespace.
        /// </summary>
        public const string Namespace = "http://static.acklann.com/schema/v2/daterpillar.xsd";

        /// <summary>
        /// Gets or sets the name of the schema.
        /// </summary>
        /// <value>The name.</value>
        [XmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the tables belonging to this instance.
        /// </summary>
        /// <value>The tables.</value>
        [XmlElement("table")]
        public List<Table> Tables { get; set; }

        /// <summary>
        /// Gets or sets the scripts.
        /// </summary>
        /// <value>The scripts.</value>
        [XmlElement("script")]
        public List<Script> Scripts { get; set; }

        /// <summary>
        /// Deserialize the <see cref="Schema"/> document contained by specified.
        /// </summary>
        /// <param name="inputStream">The input stream.</param>
        /// <returns>Schema.</returns>
        public static Schema Load(Stream inputStream)
        {
            using (inputStream)
            {
                var serializer = new XmlSerializer(typeof(Schema));
                return (Schema)serializer.Deserialize(inputStream);
            }
        }

        /// <summary>
        /// Deserialize the specified file to a <see cref="Schema"/> object.
        /// </summary>
        /// <param name="pathToFiles">The path to file.</param>
        /// <returns>Schema.</returns>
        public static Schema Load(params string[] pathToFiles)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Serialize this instance and writes it to the specified output stream.
        /// </summary>
        /// <param name="outputStream">The output stream.</param>
        public void Save(Stream outputStream)
        {
            var settings = new XmlWriterSettings()
            {
                Indent = true
            };

            using (var writer = XmlWriter.Create(outputStream, settings))
            {
                var serializer = new XmlSerializer(typeof(Schema));
                serializer.Serialize(writer, this, _namespaces);
                outputStream.Seek(0, SeekOrigin.Begin);
            }
        }

        /// <summary>
        /// Adds the specified SQL objects as children of this <see cref="Schema"/>.
        /// </summary>
        /// <param name="sqlObjects">The SQL objects.</param>
        public void Add(params object[] sqlObjects)
        {
            foreach (var item in sqlObjects)
            {
                if (item is Table table)
                {
                    table.Schema = this;
                    Tables.Add(table);
                }
                else if (item is Script script)
                {
                    Scripts.Add(script);
                }
            }
        }

        public void Join(Schema schema2)
        {
            throw new NotImplementedException();
        }

        #region Private Members

        private readonly XmlSerializerNamespaces _namespaces;

        #endregion Private Members
    }
}