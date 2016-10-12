using System.Configuration;

namespace Tests.Daterpillar.Helpers
{
    public static class ConnectionString
    {
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
            var fileMap = new ExeConfigurationFileMap() { ExeConfigFilename = "user.config" };
            var config = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);

            return config.ConnectionStrings.ConnectionStrings[name].ConnectionString;
        }
    }
}