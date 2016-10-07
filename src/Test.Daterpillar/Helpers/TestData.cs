using System;
using System.IO;
using System.Linq;

namespace Tests.Daterpillar.Helpers
{
    public static class TestData
    {
        public static FileInfo GetFile(string filename)
        {
            filename = Path.GetFileName(filename);
            string ext = "*" + Path.GetExtension(filename);
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            return new DirectoryInfo(baseDirectory).GetFiles(ext, SearchOption.AllDirectories)
                .First(x => x.Name == filename);
        }
    }
}