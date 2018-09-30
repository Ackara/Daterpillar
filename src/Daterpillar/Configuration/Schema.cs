using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Acklann.Daterpillar.Configuration
{
    /// <summary>
    /// An in-memory representation of a database schema.
    /// </summary>
    [XmlRoot("schema", Namespace = XMLNS)]
    [System.Diagnostics.DebuggerDisplay("{ToDebuggerDisplay()}")]
    public partial class Schema : ICloneable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Schema"/> class.
        /// </summary>
        public Schema() : this(string.Empty, Syntax.Generic)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Schema"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="syntax">The syntax.</param>
        public Schema(string name, Syntax syntax)
        {
            Name = name;
            Syntax = syntax;

            _namespace = new XmlSerializerNamespaces(new XmlQualifiedName[] { new XmlQualifiedName(string.Empty, XMLNS) });
            Tables = new List<Table>();
            Scripts = new List<Script>();
        }

        /// <summary>
        /// Gets or sets the name of the schema.
        /// </summary>
        /// <value>The name.</value>
        [XmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the schema description.
        /// </summary>
        /// <value> The comments.</value>
        [XmlElement("description")]
        public string Description { get; set; }

        [XmlAttribute("syntax")]
        public Syntax Syntax { get; set; }

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
        /// <param name="stream">The input stream.</param>
        /// <returns><c>true</c> if the xml was well-formed <c>false</c> otherwise.</returns>
        public static bool TryLoad(Stream stream, out Schema schema, ValidationEventHandler handler = null)
        {
            schema = null;

            try
            {
                using (stream)
                {
                    string name = $"{nameof(Daterpillar)}.xsd".ToLowerInvariant();
                    string xsdFile = Path.Combine(AppContext.BaseDirectory, name);
                    if (File.Exists(xsdFile) == false)
                    {
                        using (Stream data = Assembly.GetAssembly(typeof(Schema)).GetManifestResourceStream($"{nameof(Acklann)}.{nameof(Daterpillar)}.{name}"))
                        using (var file = new FileStream(xsdFile, FileMode.Create, FileAccess.Write, FileShare.Read))
                        {
                            data.CopyTo(file);
                            file.Flush();
                        }
                    }

                    var doc = XDocument.Load(stream);
                    var sets = new XmlSchemaSet();
                    sets.Add(XMLNS, xsdFile);
                    bool isWellFormed = true;
                    doc.Validate(sets, (handler ?? delegate (object o, ValidationEventArgs e)
                    {
                        if (e.Severity == XmlSeverityType.Error) isWellFormed = false;
                    }));

                    stream.Seek(0, SeekOrigin.Begin);
                    var serializer = new XmlSerializer(typeof(Schema));
                    schema = (Schema)serializer.Deserialize(stream);
                    schema.LinkChildNodes();

                    return isWellFormed;
                }
            }
            catch { }
            return false;
        }

        /// <summary>
        /// Deserialize the <see cref="Schema"/> document contained by specified.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="schema">The schema.</param>
        /// <param name="handler">The handler.</param>
        /// <returns><c>true</c> if the xml was well-formed <c>false</c> otherwise.</returns>
        /// <exception cref="FileNotFoundException"></exception>
        public static bool TryLoad(string filePath, out Schema schema, ValidationEventHandler handler = null)
        {
            if (File.Exists(filePath) == false) throw new FileNotFoundException($"Could not find file at '{filePath}'.", filePath);

            using (Stream stream = File.OpenRead(filePath))
            {
                return TryLoad(stream, out schema, handler);
            }
        }

        /// <summary>
        /// Adds the specified <see cref="Table"/> objects as children of this <see cref="Schema"/>.
        /// </summary>
        /// <param name="tables">The SQL objects.</param>
        public void Add(params Table[] tables)
        {
            foreach (Table item in tables)
            {
                item.Schema = this;
                Tables.Add(item);
            }
        }

        /// <summary>
        /// Adds the specified <see cref="Script"/>  objects as children of this <see cref="Schema"/>.
        /// </summary>
        /// <param name="scripts">The scripts.</param>
        public void Add(params Script[] scripts)
        {
            foreach (Script item in scripts)
            {
                Scripts.Add(item);
            }
        }

        /// <summary>
        /// Serialize this instance and writes it to the specified output stream.
        /// </summary>
        /// <param name="stream">The output stream.</param>
        public void Save(Stream stream)
        {
            var settings = new XmlWriterSettings()
            {
                Indent = true,
                OmitXmlDeclaration = false,
                Encoding = new UTF8Encoding(false)
            };

            using (var writer = XmlWriter.Create(stream, settings))
            {
                var serializer = new XmlSerializer(typeof(Schema));
                serializer.Serialize(writer, this, _namespace);
            }
        }

        /// <summary>
        /// Serialize this instance and writes it to the specified output stream.
        /// </summary>
        /// <param name="filePath">The output file path.</param>
        public void Save(string filePath)
        {
            using (Stream outStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                Save(outStream);
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var settings = new XmlWriterSettings()
            {
                Indent = true,
                OmitXmlDeclaration = true,
                Encoding = new UTF8Encoding()
            };

            using (var stream = new MemoryStream())
            using (var writer = XmlWriter.Create(stream, settings))
            {
                var serializer = new XmlSerializer(typeof(Schema));
                serializer.Serialize(writer, this, _namespace);
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }

        #region ICloneable

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        object ICloneable.Clone() => Clone();

        /// <summary>
        /// Creates a new <see cref="Schema"/> object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new <see cref="Schema"/> object that is a copy of this instance.</returns>
        public Schema Clone()
        {
            var clone = new Schema() { Name = Name };

            foreach (Table table in Tables)
            {
                Table copy = table.Clone();
                copy.Schema = clone;
                clone.Tables.Add(copy);
            }

            foreach (var script in Scripts) clone.Add(script);

            return clone;
        }

        #endregion ICloneable

        #region Private Members

        private readonly XmlSerializerNamespaces _namespace;

        private void LinkChildNodes()
        {
            foreach (Table table in Tables)
            {
                if (table.Schema == null) table.Schema = this;

                foreach (Column column in table.Columns)
                {
                    if (column.Table == null) column.Table = table;
                }

                foreach (Index index in table.Indexes)
                {
                    if (index.Table == null) index.Table = table;
                }

                foreach (ForeignKey constraint in table.ForeignKeys)
                {
                    if (constraint.Table == null) constraint.Table = table;
                }
            }
        }

        private string ToDebuggerDisplay()
        {
            string name = (string.IsNullOrEmpty(Name) ? string.Empty : $"{Name} | ");
            return $"{name}Tables: {Tables.Count} Scripts: {Scripts.Count}";
        }

        #endregion Private Members
    }
}