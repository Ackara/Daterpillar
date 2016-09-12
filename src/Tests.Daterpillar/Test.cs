using System;
using System.Configuration;

namespace Tests.Daterpillar
{
    public class Test
    {
        public struct Dev
        {
            public const string Ackara = "ackara.dev@outlook.com";
        }

        public struct Trait
        {
            public const string Integration = "Integration";
        }

        public struct File
        {
            public const string XDDL = "xddl.xsd";
            public const string x86SQLiteInterop = "x86\\SQLite.Interop.dll";
            public const string x64SQLiteInterop = "x64\\SQLite.Interop.dll";

            public const string SongCSV = "songs.csv";
            public const string DataTypesCSV = "data_types.csv";
            public const string MockSchemaXML = "mock-schema.xml";
            public const string DataSoftXDDL = "datasoft.xddl.xml";
            public const string TextFormatCSV = "text_formats.csv";
        }

        public struct Data
        {
            public const string Samples = "Sample Data";
            public const string Directory = "|DataDirectory|\\";
            public const string Provider = "Microsoft.VisualStudio.TestTools.DataSource.CSV";

            public static readonly Repository Repo = new Repository();
        }

        public struct ConnectionString
        {
            public static readonly string MySQL = ConfigurationManager.ConnectionStrings["mysql"].ConnectionString;
            public static readonly string MSSQL = ConfigurationManager.ConnectionStrings["mssql"].ConnectionString;
            public static readonly string SQLite = string.Concat("Data Source=", System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"temp{DateTime.Now.ToString("yyMddhhmmss")}.db"));
        }
    }
}