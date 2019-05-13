using Acklann.GlobN;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Acklann.Daterpillar.Configuration
{
    /// <summary>
    /// A database schema.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "()}")]
    [XmlRoot("schema", Namespace = XMLNS)]
    public partial class Schema : ICloneable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Schema"/> class.
        /// </summary>
        public Schema() : this(null)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Schema"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        ///
        public Schema(string name)
        {
            Scripts = new List<Script>();
            Tables = new List<Table>();
            Name = name;
        }

        /// <summary>
        /// The xml namespace.
        /// </summary>
        public const string XMLNS = "https://raw.githubusercontent.com/Ackara/Daterpillar/master/src/Daterpillar/daterpillar.xsd";

        /// <summary>
        /// Gets or sets the name of the schema.
        /// </summary>
        /// <value>The name.</value>
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("version")]
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the schema description.
        /// </summary>
        /// <value> The comments.</value>
        [XmlElement("documentation")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the path of another schema to merge with this instance.
        /// </summary>
        /// <value>
        /// The include.
        /// </value>
        [XmlElement("include")]
        public List<string> Imports { get; set; }

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
        /// Gets or sets the absolute path.
        /// </summary>
        /// <value>
        /// The path.
        /// </value>
        [XmlIgnore]
        public string Path { get; set; }

        public static Schema Load(string filePath)
        {
            if (File.Exists(filePath) == false) throw new FileNotFoundException($"Could not find file at '{filePath}'", filePath);
            using (var stream = File.OpenRead(filePath))
            {
                var serializer = new XmlSerializer(typeof(Schema));
                var schema = (Schema)serializer.Deserialize(stream);
                schema.Path = filePath;
                return schema;
            }
        }

        public static bool TryLoad(string filePath, out Schema schema, out string errorMsg)
        {
            schema = null; errorMsg = null;
            var err = new StringBuilder();
            void handler(XmlSeverityType severity, XmlSchemaException e)
            {
                err.Append($"[{severity}] {e.Message} at line:{e.LineNumber},{e.LinePosition}\r\n");
            }

            if (TryLoad(filePath, out schema, handler)) return true;

            errorMsg = err.ToString();
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
        public static bool TryLoad(string filePath, out Schema schema, Action<XmlSeverityType, XmlSchemaException> handler = null)
        {
            schema = null;
            if (!File.Exists(filePath))
            {
                handler?.Invoke(XmlSeverityType.Error, new XmlSchemaException($"Could not find xml document at '{filePath}'."));
                return false;
            }

            using (Stream stream = File.OpenRead(filePath))
            {
                if (Validate(stream, handler))
                {
                    var serializer = new XmlSerializer(typeof(Schema));
                    schema = (Schema)serializer.Deserialize(stream);

                    schema.LinkChildNodes();
                    schema.Path = filePath;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Validates a xml document.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="handler">The handler.</param>
        /// <returns></returns>
        public static bool Validate(Stream stream, Action<XmlSeverityType, XmlSchemaException> handler = null)
        {
            if (stream == null) return false;

            var schemas = new XmlSchemas();
            var exporter = new XmlSchemaExporter(schemas);
            exporter.ExportTypeMapping(new XmlReflectionImporter().ImportTypeMapping(typeof(Schema)));
            var set = new XmlSchemaSet();
            foreach (XmlSchema xsd in schemas.ToArray()) set.Add(xsd);

            try
            {
                var doc = XDocument.Load(stream, LoadOptions.SetLineInfo);
                bool noErrorFound = true;
                doc.Validate(set, delegate (object sender, ValidationEventArgs e)
                {
                    handler?.Invoke(e.Severity, e.Exception);
                    if (e.Severity == XmlSeverityType.Error) noErrorFound = false;
                    System.Diagnostics.Debug.WriteLine($"[{e.Severity}] {e.Message}");
                });

                return noErrorFound;
            }
            catch (XmlException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                handler?.Invoke(XmlSeverityType.Error, new XmlSchemaException(ex.Message, ex, ex.LineNumber, ex.LinePosition));
            }
            finally { stream.Seek(0, SeekOrigin.Begin); }

            return false;
        }

        /// <summary>
        /// Enumerate all <see cref="ForeignKey"/> for this instance.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ForeignKey> GetForeignKeys()
        {
            foreach (Table table in Tables)
                foreach (var fk in table.ForeignKeys)
                {
                    yield return fk;
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
        public void WriteTo(Stream stream)
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
                serializer.Serialize(writer, this, _namespaces);
            }
        }

        /// <summary>
        /// Serialize this instance and writes it to the specified output stream.
        /// </summary>
        /// <param name="filePath">The output file path.</param>
        public void Save(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) throw new ArgumentNullException(nameof(filePath));

            string folder = System.IO.Path.GetDirectoryName(filePath);
            if (Directory.Exists(folder) == false) Directory.CreateDirectory(folder);
            using (Stream outStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                WriteTo(outStream);
            }
        }

        /// <summary>
        /// Overwrites this instance tables and scripts only with the specified <see cref="Schema" />'s objects that match.
        /// </summary>
        /// <exception cref="ArgumentNullException">Occurs when this instance <see cref="Path"/> is null or empty.</exception>
        public void Merge()
        {
            if (Imports != null && File.Exists(Path))
            {
                foreach (Glob pattern in Imports)
                    if (!pattern.IsMatch(Path))
                        Merge(pattern.ResolvePath(System.IO.Path.GetDirectoryName(Path), SearchOption.TopDirectoryOnly, true).ToArray());
            }
        }

        /// <summary>
        /// Overwrites this instance tables and scripts only with the specified <see cref="Schema" />'s objects that match.
        /// </summary>
        /// <param name="schemas">The schemas to merge.</param>
        /// <exception cref="FileNotFoundException">Occurs when on of the files is not found.</exception>
        /// <exception cref="XmlSchemaValidationException">Occurs when one of the files is not well-formed.</exception>
        public void Merge(params string[] schemas)
        {
            if (schemas.Length == 0) return;

            var list = new Stack<Schema>();
            var error = new StringBuilder();

            foreach (string filePath in schemas)
            {
                if (!File.Exists(filePath)) throw new FileNotFoundException($"Could not find schema file at '{filePath}'.", filePath);
                if (TryLoad(filePath, out Schema schema, out string message))
                    list.Push(schema);
                else
                    error.AppendLine(message);
            }

            if (error.Length > 0) throw new XmlSchemaValidationException(error.ToString());
            Merge(list.ToArray());
        }

        /// <summary>
        /// Overwrites this instance tables and scripts only with the specified <see cref="Schema"/>'s objects that match.
        /// </summary>
        /// <param name="schemas">The schemas to merge.</param>
        public void Merge(params Schema[] schemas)
        {
            foreach (Schema foreignSchema in schemas)
            {
                foreignSchema.Merge();
                foreach (Table foreignTable in foreignSchema.Tables)
                {
                    Table match = FindMatch(foreignTable);
                    if (match == null)
                        Tables.Add(foreignTable.Clone());
                    else
                        match.Merge(foreignTable);
                }

                foreach (Script foreignScript in foreignSchema.Scripts.Where(x => !string.IsNullOrWhiteSpace(x.Content)))
                {
                    Script match = FindMatch(foreignScript);
                    if (match == null)
                        Scripts.Add(foreignScript);
                    else
                        match.Content = foreignScript.Content;
                }
            }
        }

        /// <summary>
        /// Resolves the name.
        /// </summary>
        /// <returns></returns>
        public string ResolveName()
        {
            return (string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Path) ? System.Text.RegularExpressions.Regex.Replace(System.IO.Path.GetFileNameWithoutExtension(Path), @"(?i)\.schema", string.Empty) : Name);
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
                serializer.Serialize(writer, this, _namespaces);
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

            foreach (Script script in Scripts)
                clone.Scripts.Add(script.Clone());

            return clone;
        }

        #endregion ICloneable

        #region Private Members

        private static readonly XmlSerializerNamespaces _namespaces = new XmlSerializerNamespaces(new XmlQualifiedName[]
        {
            new XmlQualifiedName(string.Empty, XMLNS)
        });

        internal Table FindMatch(Table right)
        {
            foreach (Table left in Tables)
            {
                if (left.IsIdentical(right)) return left;
            }

            return null;
        }

        internal Script FindMatch(Script right)
        {
            foreach (Script left in Scripts)
                if (left?.IsIdentical(right) ?? false)
                {
                    return left;
                }

            return null;
        }

        private void LinkChildNodes()
        {
            foreach (Table table in Tables)
            {
                if (table.Schema == null) table.Schema = this;

                foreach (Column column in table.Columns)
                {
                    if (column.Table == null) column.Table = table;
                }

                foreach (Index index in table.Indecies)
                {
                    if (index.Table == null) index.Table = table;
                    index.SetName();
                }

                foreach (ForeignKey constraint in table.ForeignKeys)
                {
                    if (constraint.Table == null) constraint.Table = table;
                    constraint.SetName();
                }
            }
        }

        private string GetDebuggerDisplay()
        {
            string name = (string.IsNullOrEmpty(Name) ? System.IO.Path.GetFileName(Path) : Name) ?? "N/A";
            return $"{name} | Tables: {Tables.Count} Scripts: {Scripts.Count}";
        }

        #endregion Private Members
    }
}