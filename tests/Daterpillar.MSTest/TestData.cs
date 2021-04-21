using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq;

namespace Acklann.Daterpillar
{
    internal static class TestData
    {
        static TestData()
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();
        }

        public static readonly IConfiguration Configuration;

        public static readonly string Directory = Path.Combine(AppContext.BaseDirectory, "test-cases");

        public static string GetFilePath(string pattern)
        {
            return System.IO.Directory.EnumerateFiles(Directory, pattern, SearchOption.AllDirectories).First();
        }

        public static string GetValue(string key)
            => GetValue<string>(key) ?? throw new ArgumentNullException(key);

        public static string GetValue(string key, string defaultValue)
            => GetValue<string>(key) ?? defaultValue;

        public static T GetValue<T>(string key)
        {
            return Configuration.GetValue<T>(key, default);
        }
    }
}