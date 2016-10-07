using System.Linq;
using System.Xml.Linq;

namespace Tests.Daterpillar.Helpers
{
    public static class ConnectionString
    {
        private static readonly string _configFile = "database.config.xml";

        public static string GetMySQLServerConnectionString()
        {
            return GetConnectionString("mysql");
        }

        public static string GetSQLServerConnectionString()
        {
            return GetConnectionString("mssql");
        }

        internal static string GetConnectionString(string name)
        {
            var doc = XDocument.Load(_configFile);
            var connectionStrings = doc.Element("connectionStrings");

            var record = (from element in connectionStrings.Descendants("add")
                          where element.Attribute("name").Value == name
                          select element)
                          .First();

            var connStr = new System.Data.Common.DbConnectionStringBuilder();
            connStr.Add("server", record.Attribute("server").Value);
            connStr.Add("user", record.Attribute("user").Value);
            connStr.Add("password", record.Attribute("password").Value);

            return connStr.ConnectionString;
        }
    }
}