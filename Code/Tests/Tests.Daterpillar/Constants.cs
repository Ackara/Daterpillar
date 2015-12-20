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
            ExcelConnStr = "Dsn=Excel Files;dbq=|DataDirectory|\\" + Filename.DataXLSX;
    }

    public struct Filename
    {
        public const string
            DataXLSX = "data.xlsx",
            SamplesFolder = "Sample Files",
            EmployeeSchema = "employee_schema.xml",
            XSML = "xsml.xsd";
    }
}