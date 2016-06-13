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
            DataTypesSheet = "DataTypes$",
            ExcelProvider = "System.Data.Odbc",
            ExcelConnStr = "Dsn=Excel Files;dbq=|DataDirectory|\\data.xlsx";
    }

    public struct Artifact
    {
        public const string
            XDDL = "xddl.xsd",
            SampleDataDir = "Sample Data",
            DataXLSX = SampleDataDir + "\\data.xlsx",
            x86SQLiteInterop = "x86\\SQLite.Interop.dll",
            x64SQLiteInterop = "x64\\SQLite.Interop.dll",
            SampleSchema = SampleDataDir + "\\music.xddl.xml",
            TSqlSampleSchema = SampleDataDir + "\\music.tsql.xddl.xml";
    }
}