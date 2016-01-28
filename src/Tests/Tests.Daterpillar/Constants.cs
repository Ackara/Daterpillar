namespace Tests.Daterpillar
{
    public struct Dev
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
            XDDL = "xddl.xsd",
            DataXLSX = "data.xlsx",
            SamplesFolder = "Sample Files\\",
            SampleSchema = "music.xddl.xml",
            x86SQLiteInterop = "x86\\SQLite.Interop.dll",
            x64SQLiteInterop = "x64\\SQLite.Interop.dll";
    }
}