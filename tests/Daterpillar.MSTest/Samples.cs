using System;
using System.IO;
using System.Linq;

namespace Acklann.Daterpillar
{
	public static partial class SampleFile
	{
		public const string FOLDER_NAME = "Samples";

		public static string DirectoryName
        {
            get { return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, FOLDER_NAME); }
        }

		public static FileInfo GetFile(string fileName)
        {
            fileName = Path.GetFileName(fileName);
            string searchPattern = $"*{Path.GetExtension(fileName)}";

            string appDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, FOLDER_NAME);
            return new DirectoryInfo(appDirectory).EnumerateFiles(searchPattern, SearchOption.AllDirectories)
                .First(x => x.Name.Equals(fileName, StringComparison.CurrentCultureIgnoreCase));
        }

        public static string GetContents(this string filePath)
		{
			return File.ReadAllText(filePath);
		}

		public static string GetContents(this FileInfo file)
		{
			return File.ReadAllText(file.FullName);
		}

			}
}
