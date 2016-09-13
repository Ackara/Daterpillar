using Gigobyte.Daterpillar;
using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Tests.Daterpillar
{
    public class Repository
    {
        public static FileInfo GetFile(string filename)
        {
            filename = Path.GetFileName(filename);
            string ext = "*" + Path.GetExtension(filename);
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            return new DirectoryInfo(baseDirectory).GetFiles(ext, SearchOption.AllDirectories)
                .First(x => x.Name == filename);
        }

        public Schema CreateSchema(string name = nameof(Daterpillar))
        {
            var schema = new Schema();

            return schema;
        }

        public Table CreateTable([CallerMemberName]string name = null)
        {
            return new Schema().CreateTable(name);
        }

        public Column CreateIdColumn(string name)
        {
            throw new System.NotImplementedException();
        }

        public Column CreateNumericColumn()
        {
            throw new System.NotImplementedException();
        }

        public Column CreateStringColumn()
        {
            throw new System.NotImplementedException();
        }

        public Column CreateDateColumn()
        {
            throw new System.NotImplementedException();
        }
    }
}