using System.IO;

namespace Acklann.Daterpillar.Compilation
{
    public class SqlWriter : SchemaWriter
    {
        public SqlWriter(Stream stream) : this(new StreamWriter(stream))
        { }

        public SqlWriter(TextWriter writer) : base(writer)
        { }
    }
}