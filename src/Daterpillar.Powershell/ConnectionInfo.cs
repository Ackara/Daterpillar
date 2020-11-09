namespace Acklann.Daterpillar
{
    public readonly struct ConnectionInfo
    {
        public ConnectionInfo(string connectionString)
        {
            GetValues(connectionString, out string host, out string user, out string password, out string database, out string port);

            Host = host;
            Port = port;
            User = user;
            Password = password;
            Database = database;
        }

        public string Host { get; }

        public string Port { get; }

        public string User { get; }

        public string Password { get; }

        public string Database { get; }

        public string ConnectionString
        {
            get
            {


                return null;
            }
        }

        public string GetJdbcConnectionString(Language dialect = Language.SQL)
        {
            string connectionString = null;

            switch (dialect)
            {
                case Language.MySQL:

                    break;
            }

            return connectionString;
        }

        public static void GetValues(string connectionString, out string host, out string user, out string password, out string database, out string port)
        {
            host = port = user = password = database = null;

            foreach (string pair in connectionString.Split(new char[] { ';' }, System.StringSplitOptions.RemoveEmptyEntries))
            {
                if (string.IsNullOrEmpty(host))
                    if (get(pair, out host, "h", "host", "server", "address", "ip", "data source")) continue;

                if (string.IsNullOrEmpty(port))
                    if (get(pair, out port, "port")) continue;

                if (string.IsNullOrEmpty(user))
                    if (get(pair, out user, "u", "user", "usr", "user id")) continue;

                if (string.IsNullOrEmpty(password))
                    if (get(pair, out password, "p", "pwd", "password")) continue;

                if (string.IsNullOrEmpty(database))
                    if (get(pair, out database, "d", "db", "database", "initial catalog")) continue;
            }

            bool get(string input, out string result, params string[] possibleValues)
            {
                result = null;
                string[] pair = input.Split('=');

                for (int i = 0; i < possibleValues.Length; i++)
                    if (string.Equals(pair[0], possibleValues[i], System.StringComparison.InvariantCultureIgnoreCase))
                    {
                        result = pair[1];
                    }

                return !string.IsNullOrEmpty(result);
            }
        }
    }
}