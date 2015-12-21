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
            DataXLSX = "data.xlsx",
            x86SQLiteInterop = "x86\\SQLite.Interop.dll",
            x64SQLiteInterop = "x64\\SQLite.Interop.dll",
            SamplesFolder = "Sample Files\\",
            EmployeeSchema = "employee_schema.xml",
            XSML = "xsml.xsd";
    }
}