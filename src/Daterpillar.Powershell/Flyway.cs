using SharpCompress.Common;
using SharpCompress.Readers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace Acklann.Daterpillar
{
    public static class Flyway
    {
        public const int DEFAULT_TIMEOUT = (5 * 60/*sec*/);
        private const string DEFAULT_VERSION = "6.5.3", FILESYSTEM = "filesystem:";

        public static ProcessResult Invoke(string verb, string flywayUrl, string user, string password, string migrationsDirectory, string installationPath = null, int timeoutInSeconds = DEFAULT_TIMEOUT)
        {
            if (string.IsNullOrEmpty(flywayUrl)) throw new ArgumentNullException(nameof(flywayUrl));
            if (string.IsNullOrEmpty(migrationsDirectory)) throw new ArgumentNullException(nameof(migrationsDirectory));

            if (string.IsNullOrEmpty(installationPath)) installationPath = GetDefaultInstallationPath();
            if (!migrationsDirectory.StartsWith(FILESYSTEM)) migrationsDirectory = $"{FILESYSTEM}{migrationsDirectory}";

            var args = new ProcessStartInfo(installationPath, $"{verb} -url={flywayUrl.Expand()} -user={user?.Expand()} -password={password.Expand()} -locations={migrationsDirectory.Expand()}")
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                WorkingDirectory = Path.GetDirectoryName(installationPath)
            };

            using (var flyway = new Process { StartInfo = args })
            {
                flyway.Start();
                flyway.WaitForExit(timeoutInSeconds * 1000);

                return new ProcessResult(flyway.ExitCode, flyway.StandardOutput?.ReadToEnd(), flyway.StandardError?.ReadToEnd(), (flyway.ExitTime.Ticks - flyway.StartTime.Ticks));
            }
        }

        public static ProcessResult Invoke(string verb, Language connectionType, string connectionString, string migrationsDirectory, string installationPath = null, int timeoutInSeconds = DEFAULT_TIMEOUT)
        {
            GetCredentials(connectionType, connectionString, out string flywayUrl, out string user, out string password);
            return Invoke(verb, flywayUrl, user, password, migrationsDirectory, installationPath, timeoutInSeconds);
        }

        public static string Install(string baseDirectory = null, string version = DEFAULT_VERSION)
        {
            string installationPath = GetDefaultInstallationPath(ref baseDirectory, version);
            string url = GetPackageUrl(version);

            if (!File.Exists(installationPath))
            {
                string packagePath = Path.Combine(Path.GetTempPath(), $"flyway-{version}{Path.GetExtension(url)}");
                if (!File.Exists(packagePath))
                    using (var web = new System.Net.WebClient())
                    {
                        web.DownloadFile(url, packagePath);
                    }

                string tempFolder = Path.Combine(baseDirectory, "dtp-temp");
                if (Directory.Exists(tempFolder)) Directory.Delete(tempFolder, true);
                Directory.CreateDirectory(tempFolder);

                using (Stream stream = File.OpenRead(packagePath))
                using (IReader reader = ReaderFactory.Open(stream))
                {
                    while (reader.MoveToNextEntry())
                    {
                        if (!reader.Entry.IsDirectory)
                        {
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
                Directory.Move(tempFolder, Path.Combine(Path.GetDirectoryName(tempFolder), "Flyway"));
            }

            return installationPath;
        }

        public static string GetDefaultInstallationPath(string version = DEFAULT_VERSION)
        {
            string temp = null;
            return GetDefaultInstallationPath(ref temp, version);
        }

        internal static void GetCredentials(Language connectionType, string connectionString, out string flywayUrl, out string user, out string password)
        {
            if (string.IsNullOrEmpty(connectionString)) throw new ArgumentNullException(nameof(connectionString));

            bool tryGetValue(out string value, string[] term, params string[] possibleValues) { value = (possibleValues.Contains(term[0].ToLowerInvariant()) ? term[1] : null); return value != null; }
            string host = null, database = null, port = null;
            var optionals = new List<string>();
            string[] pair;

            flywayUrl = user = password = null;
            foreach (string item in connectionString.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
            {
                pair = item.Split('=');
                if (pair.Length != 2) continue;

                if (string.IsNullOrEmpty(host))
                    if (tryGetValue(out host, pair, "h", "host", "s", "server", "address", "data source")) continue;

                if (string.IsNullOrEmpty(user))
                    if (tryGetValue(out user, pair, "u", "usr", "user", "user id")) continue;

                if (string.IsNullOrEmpty(password))
                    if (tryGetValue(out password, pair, "p", "pwd", "pass", "password")) continue;

                if (string.IsNullOrEmpty(database))
                    if (tryGetValue(out database, pair, "d", "db", "database", "initial catalog")) continue;

                if (string.IsNullOrEmpty(port))
                    if (tryGetValue(out port, pair, "port")) continue;

                optionals.Add(item);
            }
            flywayUrl = GetFlywayUrl(connectionType, host, port, database, optionals.ToArray());
        }

        internal static string GetFlywayUrl(Language connectionType, string address, string port, string database, params string[] args)
        {
            if (string.IsNullOrEmpty(address)) throw new ArgumentNullException(nameof(address));

            string addressAndPort() => (string.IsNullOrEmpty(port) ? address : $"{address}:{port}");
            string extra = (args.Length > 0 ? string.Empty : string.Concat("?", string.Join("&", args)));

            string uri = null;
            switch (connectionType)
            {
                default:
                    throw new ArgumentOutOfRangeException(nameof(connectionType), $"A '{connectionType}' connection is not yet supported.");

                case Language.SQLite:
                    uri = $"jdbc:sqlite:{address}";
                    break;

                case Language.TSQL:
                    uri = $"jdbc:sqlserver:////{addressAndPort()};databaseName={database}";
                    break;

                case Language.MySQL:
                    uri = $"jdbc:mysql://{addressAndPort()}/{database}{extra}";
                    break;
            }

            return uri;
        }

        internal static string GetDefaultInstallationPath(ref string baseDirectory, string version = DEFAULT_VERSION)
        {
            if (string.IsNullOrEmpty(baseDirectory)) baseDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string filename = (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "flyway.cmd" : "flyway");
            return Path.Combine(baseDirectory, "Flyway", version, filename);
        }

        internal static string GetPackageUrl(string version)
        {
            const string url = "https://repo1.maven.org/maven2/org/flywaydb/flyway-commandline/{0}/flyway-commandline-{0}-{1}-x64{2}";
            switch (Environment.OSVersion.Platform)
            {
                default:
                case PlatformID.Win32NT:
                    return string.Format(url, version, "windows", ".zip");

                case PlatformID.MacOSX:
                    return string.Format(url, version, "macosx", ".tar.gz");

                case PlatformID.Unix:
                    return string.Format(url, version, "linux", ".tar.gz");
            }
        }
    }
}