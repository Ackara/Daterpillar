using Ackara.Daterpillar.Transformation;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace Tests.Daterpillar
{
    public static class Samples
    {
        public static FileInfo GetFile(string filename)
        {
            throw new System.NotImplementedException();
        }

        public static Schema GetSchema([CallerMemberName]string name = null)
        {
            var schema = new Schema();
            schema.Name = name ?? "SchemaName";
            schema.Author = "johnDoe@example.com";

            var table1 = new Table();
            

            schema.Tables.Add(table1);

            return schema;
        }
    }
}