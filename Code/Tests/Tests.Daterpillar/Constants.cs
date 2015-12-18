namespace Tests.Daterpillar
{
    public struct Str
    {
        public const string
            Ackara = "ackara.dev@outlook.com",
            ApprovalsDir = "Approvals";
    }

    public struct Data
    {
        public const string
            Provider = "System.Data.Odbc",
            ConnectionString = "Dsn=Excel Files;dbq=|DataDirectory|\\data.xlsx";
    }

    public struct Filename
    {
        public const string
            DataXLSX = "data.xlsx",
            XddlSpec = "xddl-spec.xsd";
    }
}