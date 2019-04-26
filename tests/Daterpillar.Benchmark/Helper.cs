using System;
using System.Data;
using System.IO;

namespace Acklann.Daterpillar
{
    public static class Helper
    {
        public static IDbConnection CreateDatabase()
        {
            string databasePath = Path.Combine(AppContext.BaseDirectory, "sample.db");
            if (!File.Exists(databasePath)) throw new FileNotFoundException($"Could not find file at '{databasePath}'.");

            return new System.Data.SQLite.SQLiteConnection($"Data Source={databasePath}");
        }
    }
}