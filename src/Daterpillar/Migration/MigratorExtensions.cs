using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Data;
using Acklann.Daterpillar.Configuration;

namespace Acklann.Daterpillar.Migration
{
    public static class MigratorExtensions
    {
        public static void CreateDatabase(this IDbConnection connection, Schema schema, Language kind, bool dropIfExists = false)
        {
            schema.Merge();


        }

        public static void CreateDatabase(this IDbConnection connection, Assembly assembly, Language kind, bool dropIfExists = false)
        {
            CreateDatabase(connection, SchemaFactory.CreateFrom(assembly), kind, dropIfExists);
        }

        public static void CreateDatabase(this IDbConnection connection, string assemblyPath, Language kind, bool dropIfExists = false)
        {
            CreateDatabase(connection, SchemaFactory.CreateFrom(assemblyPath), kind, dropIfExists);
        }
    }
}