using System;
using System.IO;
using System.Linq;

namespace Acklann.Daterpillar
{
	internal static partial class TestData
	{
		public const string FOLDER_NAME = "test-data";

		public static string DirectoryName => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, FOLDER_NAME);

		public static FileInfo GetFile(string fileName)
        {
            fileName = Path.GetFileName(fileName);
            string searchPattern = $"*{Path.GetExtension(fileName)}";

            string appDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, FOLDER_NAME);
            return new DirectoryInfo(appDirectory).EnumerateFiles(searchPattern, SearchOption.AllDirectories)
                .First(x => x.Name.Equals(fileName, StringComparison.CurrentCultureIgnoreCase));
        }

		public static FileInfo GetBad_SchemaXML() => GetFile(@"bad_schema.xml");
		public static FileInfo GetMusicSeedXML() => GetFile(@"music-seed.xml");
		public static FileInfo GetMusicXML() => GetFile(@"music.xml");
		public static FileInfo GetSakilaBusinessXML() => GetFile(@"sakila-business.xml");
		public static FileInfo GetSakilaCustomerXML() => GetFile(@"sakila-customer.xml");
		public static FileInfo GetSakilaInventoryXML() => GetFile(@"sakila-inventory.xml");
		public static FileInfo GetSakilaViewsXML() => GetFile(@"sakila-views.xml");

		public class File
		{
			public const string Bad_SchemaXML = @"bad_schema.xml";
			public const string MusicSeedXML = @"music-seed.xml";
			public const string MusicXML = @"music.xml";
			public const string SakilaBusinessXML = @"sakila-business.xml";
			public const string SakilaCustomerXML = @"sakila-customer.xml";
			public const string SakilaInventoryXML = @"sakila-inventory.xml";
			public const string SakilaViewsXML = @"sakila-views.xml";
		}
	}	
}
