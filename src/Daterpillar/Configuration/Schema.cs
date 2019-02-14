using Acklann.GlobN;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
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
    [XmlRoot("schema", Namespace = XMLNS)]
    [System.Diagnostics.DebuggerDisplay("{GetDebuggerDisplay()}")]
    public class Schema : ICloneable
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
            _namespace = new XmlSerializerNamespaces(new XmlQualifiedName[] { new XmlQualifiedName(string.Empty, XMLNS) });
            Scripts = new List<Script>();
            Tables = new List<Table>();
            Name = name;
        }

        /// <summary>
        /// The xml namespace.
        /// </summary>
        public const string XMLNS = "https://raw.githubusercontent.com/Ackara/Daterpillar/master/src/daterpillar.xsd";

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
        [XmlElement("documentation")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the path of another schema to merge with this instance.
        /// </summary>
        /// <value>
        /// The include.
        /// </value>
        [XmlAttribute("include")]
        public string Import { get; set; }

        public bool HasChildren
        {
            get => ((Tables?.Count ?? 0) == 0 && (Scripts?.Count ?? 0) == 0);
        }

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
        [XmlIgnore, IgnoreDataMember]
        public string Path { get; set; }

        public static bool Validate(Stream stream, ValidationEventHandler handler = null, string xsdFile = null)
        {
            string xsdName = $"{nameof(Daterpillar)}.xsd".ToLowerInvariant();
            if (string.IsNullOrEmpty(xsdFile)) xsdFile = System.IO.Path.Combine(AppContext.BaseDirectory, xsdName);

            if (File.Exists(xsdFile) == false)
            {
                using (Stream data = Assembly.GetAssembly(typeof(Schema)).GetManifestResourceStream($"{nameof(Acklann)}.{nameof(Daterpillar)}.{xsdName}"))
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
            doc.Validate(sets, (delegate (object sender, ValidationEventArgs e)
            {
                if (e.Severity == XmlSeverityType.Error) isWellFormed = false;
                System.Diagnostics.Debug.WriteLine($"[{e.Severity}] {e.Message}");

                handler?.Invoke(sender, e);
            }));

            stream.Seek(0, SeekOrigin.Begin);
            return isWellFormed;
        }

        public static Schema Load(string filePath)
        {
            if (File.Exists(filePath) == false) throw new FileNotFoundException($"Could not find file at '{filePath}'", filePath);
            using (var stream = File.OpenRead(filePath))
            {
                var serializer = new XmlSerializer(typeof(Schema));
                return (Schema)serializer.Deserialize(stream);
            }
        }

        public static bool TryLoad(string filePath, out Schema schema, out string errorMsg)
        {
            if (File.Exists(filePath) == false)
            {
                errorMsg = $"Could not find file at '{filePath}'.";
                schema = null;
                return false;
            }

            errorMsg = null;
            var err = new StringBuilder();
            void handler(object s, ValidationEventArgs e)
            {
                err.Append($"[{e.Severity}] {e.Message}\r\n");
            }

            bool isWellFormed = TryLoad(filePath, out schema, handler);
            errorMsg = err.ToString();
            return isWellFormed;
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
            if (File.Exists(filePath) == false)
            {
                schema = null;
                return false;
            }

            using (Stream stream = File.OpenRead(filePath))
            {
                if (TryLoad(stream, out schema, handler))
                {
                    schema.Path = filePath;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Deserialize the <see cref="Schema"/> document contained by specified.
        /// </summary>
        /// <param name="stream">The input stream.</param>
        /// <returns><c>true</c> if the xml was well-formed <c>false</c> otherwise.</returns>
        public static bool TryLoad(Stream stream, out Schema schema, ValidationEventHandler handler = null)
        {
            schema = null;
            bool isWellFormed;

            using (stream)
            {
                isWellFormed = Validate(stream, handler);

                if (isWellFormed)
                {
                    var serializer = new XmlSerializer(typeof(Schema));
                    schema = (Schema)serializer.Deserialize(stream);
                    schema.LinkChildNodes();
                }
            }
            return isWellFormed;
        }

        public IEnumerable<Index> GetIndices()
        {
            foreach (Table table in Tables)
                foreach (Index index in table.Indecies)
                {
                    yield return index;
                }
        }

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
        /// Overwrites this instance tables and scripts only with the specified <see cref="Schema" />'s objects that match.
        /// </summary>
        /// <exception cref="ArgumentNullException">Occurs when this instance <see cref="Path"/> is null or empty.</exception>
        public void Merge()
        {
            if (string.IsNullOrEmpty(Import) == false)
            {
                if (string.IsNullOrEmpty(Path)) throw new ArgumentNullException(nameof(Path));
                Merge(((Glob)Import).ResolvePath(System.IO.Path.GetDirectoryName(Path)).ToArray());
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
            void handler(object s, ValidationEventArgs e)
            {
                if (e.Severity == XmlSeverityType.Error)
                {
                    error.AppendLine($"[{e.Severity}] {e.Message}");
                    System.Diagnostics.Debug.WriteLine($"[{e.Severity}] {e.Message}");
                }
            }

            foreach (string file in schemas)
            {
                if (File.Exists(file) == false) throw new FileNotFoundException($"Could not find schema file at '{file}'.", file);
                if (TryLoad(file, out Schema schema, handler))
                {
                    list.Push(schema);
                }
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
            foreach (Schema schema in schemas)
            {
                schema.Merge();
                foreach (Table table in schema.Tables)
                {
                    Table match = FindMatch(table);
                    if (match == null)
                        Tables.Add(table);
                    else
                        match.Merge(table);
                }

                foreach (Script script in schema.Scripts.Where(x => !string.IsNullOrEmpty(x.Content)))
                {
                    Script match = FindMatch(script);
                    if (match == null)
                        Scripts.Add(script);
                    else
                        match.Content = script.Content;
                }

                if (string.IsNullOrEmpty(Import) == false && string.IsNullOrEmpty(schema.Path) == false)
                {
                    if (Glob.IsMatch(schema.Path, Import)) Import = null;
                }
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

            foreach (Script script in Scripts)
                clone.Scripts.Add(script.Clone());

            return clone;
        }

        #endregion ICloneable

        #region Private Members

        private readonly XmlSerializerNamespaces _namespace;

        internal Table FindMatch(Table right)
        {
            foreach (Table left in Tables)
            {
                if (left.Name.Equals(right.Name, StringComparison.OrdinalIgnoreCase))
                {
                    return left;
                }
            }

            return null;
        }

        internal Script FindMatch(Script right)
        {
            foreach (Script left in Scripts)
                if (left.Syntax == right.Syntax && (left?.Name?.Equals(right.Name, StringComparison.OrdinalIgnoreCase) ?? false))
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