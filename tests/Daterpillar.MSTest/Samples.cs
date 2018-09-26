using System;
using System.IO;
using System.Linq;

namespace Acklann.Daterpillar
{
	internal static partial class TestData
	{
		public const string FOLDER_NAME = "Samples";

		public static string DirectoryName => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, FOLDER_NAME);

		public static FileInfo GetFile(string fileName)
        {
            fileName = Path.GetFileName(fileName);
            string searchPattern = $"*{Path.GetExtension(fileName)}";

            string appDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, FOLDER_NAME);
            return new DirectoryInfo(appDirectory).EnumerateFiles(searchPattern, SearchOption.AllDirectories)
                .First(x => x.Name.Equals(fileName, StringComparison.CurrentCultureIgnoreCase));
        }

		public static FileInfo GetSakilaXML() => GetFile(@"Schemas\sakila.xml");

		public class File
		{
			public const string SakilaXML = @"Schemas\sakila.xml";
		}
	}	
}
