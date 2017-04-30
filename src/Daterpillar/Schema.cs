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
    public class Schema : ICloneable<Schema>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Schema"/> class.
        /// </summary>
        public Schema()
        {
            _namespace = new XmlSerializerNamespaces(new XmlQualifiedName[]
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
                var schema = (Schema)serializer.Deserialize(inputStream);
                schema.AssignParentNodes();

                return schema;
            }
        }

        /// <summary>
        /// Gets all foreign keys within the schema.
        /// </summary>
        /// <returns>IEnumerable&lt;ForeignKey&gt;.</returns>
        public IEnumerable<ForeignKey> GetForeignKeys()
        {
            foreach (var table in Tables)
            {
                foreach (var constraint in table.ForeignKeys)
                {
                    yield return constraint;
                }
            }
        }

        /// <summary>
        /// Gets all columns within the schema.
        /// </summary>
        /// <returns>IEnumerable&lt;Column&gt;.</returns>
        public IEnumerable<Column> GetColumns()
        {
            foreach (var table in Tables)
            {
                foreach (var column in table.Columns)
                {
                    yield return column;
                }
            }
        }

        /// <summary>
        /// Gets all indexes within the schema.
        /// </summary>
        /// <returns>IEnumerable&lt;Index&gt;.</returns>
        public IEnumerable<Index> GetIndexes()
        {
            foreach (var table in Tables)
            {
                foreach (var index in table.Indexes)
                {
                    yield return index;
                }
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
                serializer.Serialize(writer, this, _namespace);
                outputStream.Seek(0, SeekOrigin.Begin);
            }
        }

        /// <summary>
        /// Concatenate the SQL objects of the specified schemas with this instance.
        /// </summary>
        /// <param name="otherSchemas">The schemas to join.</param>
        public void Join(params Schema[] otherSchemas)
        {
            foreach (var schema in otherSchemas)
            {
                Tables.AddRange(schema.Tables);
                Scripts.AddRange(schema.Scripts);
            }
        }

        /// <summary>
        /// Creates a new <see cref="Schema"/> object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new <see cref="Schema"/> object that is a copy of this instance.</returns>
        public Schema Clone()
        {
            var clone = new Schema()
            {
                Name = this.Name
            };

            foreach (var table in Tables)
            {
                var copy = table.Clone();
                copy.Schema = clone;
                clone.Tables.Add(copy);
            }

            foreach (var script in Scripts) clone.Add(script);

            return clone;
        }

        internal void AssignParentNodes()
        {
            foreach (var table in Tables)
            {
                if (table.Schema == null) table.Schema = this;

                foreach (var column in table.Columns)
                {
                    if (column.Table == null) column.Table = table;
                }

                foreach (var index in table.Indexes)
                {
                    if (index.Table == null) index.Table = table;
                }

                foreach (var constraint in table.ForeignKeys)
                {
                    if (constraint.Table == null) constraint.Table = table;
                }
            }
        }

        #region Private Members

        private readonly XmlSerializerNamespaces _namespace;

        #endregion Private Members
    }
}