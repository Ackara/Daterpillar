using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace Ackara.Daterpillar
{
    /// <summary>
    /// An in-memory representation of a database schema.
    /// </summary>
    [XmlRoot("schema", Namespace = XMLNS)]
    [System.Diagnostics.DebuggerDisplay("{AsDebuggerDisplay()}")]
    public class Schema : ICloneable<Schema>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Schema"/> class.
        /// </summary>
        public Schema()
        {
            _namespace = new XmlSerializerNamespaces(new XmlQualifiedName[]
            {
                new XmlQualifiedName(string.Empty, XMLNS)
            });

            Tables = new List<Table>();
            Scripts = new List<Script>();
        }

        /// <summary>
        /// The xml namespace.
        /// </summary>
        public const string XMLNS = "http://static.acklann.com/schema/v2/daterpillar.xsd";

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
        /// Rearrange this instance <see cref="Table"/> objects so referenced tables (aka foreign keys) precede their dependants.
        /// </summary>
        public void Sort()
        {
            if (Tables.IsNotEmpty())
            {
                var nodes = new List<Node>();
                foreach (var table in Tables) nodes.Add(new Node(table));
                foreach (var parent in nodes) parent.AddChildren(nodes);

                int latestRanking = 0;
                foreach (var n in nodes)
                {
                    latestRanking = Rank(n, latestRanking);
                }

                Tables.Clear();
                nodes.Sort(new RankComparer());
                foreach (var item in nodes) Tables.Add(item.Data);
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

        private string AsDebuggerDisplay()
        {
            string name = (string.IsNullOrEmpty(Name) ? "" : "");
            return $"{name}Tables: {Tables.Count}  Scripts: {Scripts.Count}";
        }

        private int Rank(Node node, int? rank = 0)
        {
            Node nonRankedChild;
            if (node.Rank == null)
                do
                {
                    nonRankedChild = node.GetNonRankedChild();
                    if (nonRankedChild == null)
                    {
                        /// Here I am ranking the node only when all of it's children
                        /// has been ranked.
                        node.Rank = rank = (rank + 1);
                        System.Diagnostics.Debug.WriteLine(node);
                    }
                    else
                    {
                        /// Here I am ranking the child node that has not yet been ranked.
                        /// Also I am updating the parent node rank with the rank of its
                        /// then ranked child because the parent rank should always be higher.
                        /// The parent node rank will later be updated when it has no more non ranked child.
                        Rank(nonRankedChild, (node.Rank ?? 0));
                        rank = node.Rank = nonRankedChild.Rank;
                        System.Diagnostics.Debug.WriteLine($"\t\tupdated {node}");
                    }
                }
                while (nonRankedChild != null);

            return rank ?? 0;
        }

        private class Node
        {
            public Node(Table table)
            {
                Data = table;
                Children = new List<Node>();
            }

            public int? Rank;
            public Table Data;
            public IList<Node> Children;

            public string Name
            {
                get { return Data.Name; }
            }

            public void AddChildren(List<Node> nodes)
            {
                foreach (var fKey in Data.ForeignKeys)
                {
                    var child = nodes.FirstOrDefault(x => x.Name == fKey.ForeignTable);
                    if (child != null) Children.Add(child);
                }
            }

            public Node GetNonRankedChild()
            {
                foreach (var item in Children)
                {
                    if (item.Rank == null) return item;
                }

                return null;
            }

            public override string ToString()
            {
                return $"{Name}: {Rank ?? -1}";
            }
        }

        private class RankComparer : IComparer<Node>
        {
            public int Compare(Node x, Node y)
            {
                if (x.Rank > y.Rank) return 1;
                else if (x.Rank < y.Rank) return -1;
                else return 0;
            }
        }

        #endregion Private Members
    }
}