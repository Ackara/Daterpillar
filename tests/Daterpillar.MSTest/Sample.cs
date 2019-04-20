using System;
using System.IO;
using System.Linq;

namespace Acklann.Daterpillar
{
	internal static partial class Sample
	{
		public const string FOLDER_NAME = "sample-data";

		public static string DirectoryName => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, FOLDER_NAME);

		public static string CreateDirectory(string filePath)
		{
			string dir = Path.GetDirectoryName(filePath);
			if (Directory.Exists(dir) == false) Directory.CreateDirectory(dir);

			return dir;
		}

		public static FileInfo GetFile(string fileName)
        {
            fileName = Path.GetFileName(fileName);
            string searchPattern = $"*{Path.GetExtension(fileName)}";

            string appDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, FOLDER_NAME);
            return new DirectoryInfo(appDirectory).EnumerateFiles(searchPattern, SearchOption.AllDirectories)
                .First(x => x.Name.Equals(fileName, StringComparison.CurrentCultureIgnoreCase));
        }

		public static FileInfo GetBad_SchemaXML() => GetFile(@"bad_schema.xml");
		public static FileInfo GetFlywayOutputTXT() => GetFile(@"flyway-output.txt");
		public static FileInfo GetInitSQL() => GetFile(@"init.sql");
		public static FileInfo GetMusicDataXML() => GetFile(@"music-data.xml");
		public static FileInfo GetMusicRevisionsXML() => GetFile(@"music-revisions.xml");
		public static FileInfo GetMusicXML() => GetFile(@"music.xml");
		public static FileInfo GetNoNsXML() => GetFile(@"no-ns.xml");
		public static FileInfo GetSakilaBusinessXML() => GetFile(@"sakila-business.xml");
		public static FileInfo GetSakilaCustomerXML() => GetFile(@"sakila-customer.xml");
		public static FileInfo GetSakilaInventoryXML() => GetFile(@"sakila-inventory.xml");
		public static FileInfo GetSakilaViewsXML() => GetFile(@"sakila-views.xml");

		public class File
		{
			public const string Bad_SchemaXML = @"bad_schema.xml";
			public const string FlywayOutputTXT = @"flyway-output.txt";
			public const string InitSQL = @"init.sql";
			public const string MusicDataXML = @"music-data.xml";
			public const string MusicRevisionsXML = @"music-revisions.xml";
			public const string MusicXML = @"music.xml";
			public const string NoNsXML = @"no-ns.xml";
			public const string SakilaBusinessXML = @"sakila-business.xml";
			public const string SakilaCustomerXML = @"sakila-customer.xml";
			public const string SakilaInventoryXML = @"sakila-inventory.xml";
			public const string SakilaViewsXML = @"sakila-views.xml";
		}
	}	
}
