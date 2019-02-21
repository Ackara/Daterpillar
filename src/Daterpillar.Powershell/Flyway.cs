using Acklann.Daterpillar.Configuration;
using SharpCompress.Common;
using SharpCompress.Readers;
using System;
using System.IO;
using System.Linq;

namespace Acklann.Daterpillar
{
    public static class Flyway
    {
        private const string Version = "5.2.4";

        public static string Install(string version = Version, string baseDirectory = null)
        {
            string installationPath = GetDefaultInstallationPath(version, baseDirectory);
            string url = "https://repo1.maven.org/maven2/org/flywaydb/flyway-commandline/{0}/flyway-commandline-{0}-{1}-x64{2}";
            switch (Environment.OSVersion.Platform)
            {
                default:
                case PlatformID.Win32NT:
                    installationPath = $"{installationPath}.cmd";
                    url = string.Format(url, version, "windows", ".zip");
                    break;

                case PlatformID.MacOSX:
                    url = string.Format(url, version, "macosx", "tar.gz");
                    break;

                case PlatformID.Unix:
                    url = string.Format(url, version, "linux", "tar.gz");
                    break;
            }

            if (!File.Exists(installationPath))
            {
                string packagePath = Path.Combine(Path.GetTempPath(), $"flyway-{version}{Path.GetExtension(url)}");
                if (!File.Exists(packagePath))
                    using (var web = new System.Net.WebClient())
                    {
                        web.DownloadFile(url, packagePath);
                    }

                string tempFolder = Path.Combine(baseDirectory, "dtp-temp");
                if (!Directory.Exists(tempFolder)) Directory.CreateDirectory(tempFolder);

                using (Stream stream = File.OpenRead(packagePath))
                using (IReader reader = ReaderFactory.Open(stream))
                {
                    while (reader.MoveToNextEntry())
                    {
                        if (!reader.Entry.IsDirectory)
                        {
                            System.Diagnostics.Debug.WriteLine(reader.Entry.Key);
                            reader.WriteEntryToDirectory(tempFolder, new ExtractionOptions()
                            {
                                Overwrite = true,
                                ExtractFullPath = true,
                            });
                        }
                    }
                }

                // Renaming the folders to match the installation path.
                string ver = Directory.EnumerateDirectories(tempFolder).First();
                Directory.Move(ver, Path.Combine(Path.GetDirectoryName(ver), version));
                Directory.Move(tempFolder, Path.Combine(Path.GetDirectoryName(tempFolder), "flyway"));
            }

            return installationPath;
        }

        public static void Migrate(string flywayUrl, string user, string password, string migrationsDirectory, string installationPath)
        {
        }

        public static void Migrate(Syntax language, string connectionString, string migrationsDirectory, string installationPath = null)
        {
        }

        public static void GetCredentials(Syntax language, string connectionString, out string flywayUrl, out string user, out string password)
        {
            if (string.IsNullOrEmpty(connectionString)) throw new ArgumentNullException(nameof(connectionString));

            string getValue(string[] prospect, params string[] names) => names.Contains(prospect[0].ToLowerInvariant()) ? prospect[1] : null;
            string host, database;
            string[] pair;

            flywayUrl = user = password = null;
            foreach (string item in connectionString.Split(';'))
            {
                pair = item.Split('=');
                user = (pair.Length == 2 ? getValue(pair, "u", "usr", "user") : null);
                database = (pair.Length == 2 ? getValue(pair, "d", "db", "database") : null);
                password = (pair.Length == 2 ? getValue(pair, "p", "pwd", "pass", "password") : null);
                host = (pair.Length == 2 ? getValue(pair, "h", "host", "s", "server", "address") : null);

                flywayUrl = GetFlywayUrl(language, database, host);
            }
        }

        public static string GetFlywayUrl(Syntax language, string database, string host)
        {
            if (string.IsNullOrEmpty(database)) throw new ArgumentNullException(nameof(database));

            string url = null;
            switch (language)
            {
                case Syntax.TSQL:
                    break;

                case Syntax.MySQL:
                    break;

                case Syntax.SQLite:
                    break;
            }

            return url;
        }

        public static string GetDefaultInstallationPath(string version = Version, string baseDirectory = null) => Path.Combine((baseDirectory ?? Path.GetTempPath()), "flyway", version, "flyway");
    }
}