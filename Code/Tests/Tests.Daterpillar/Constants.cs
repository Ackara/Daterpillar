namespace Tests.Daterpillar
{
    public struct Str
    {
        public const string
            Ackara = "ackara.dev@outlook.com",
            ApprovalsDir = "ApprovalTests";
    }

    public struct Data
    {
        public const string
            ExcelProvider = "System.Data.Odbc",
            ExcelConnStr = "Dsn=Excel Files;dbq=|DataDirectory|\\" + Artifact.DataXLSX;
    }

    public struct Artifact
    {
        public const string
            XDDL = "xsml.xsd",
            DataXLSX = "data.xlsx",
            SamplesFolder = "Sample Files\\",
            MusicSchema = "music-sqlite.sql",
            SampleSchema = "daterpillar_schema.xml",
            x86SQLiteInterop = "x86\\SQLite.Interop.dll",
            x64SQLiteInterop = "x64\\SQLite.Interop.dll";
    }
}