using Acklann.Daterpillar.Compilation;
using Acklann.Daterpillar.Configuration;
using Acklann.Daterpillar.Samples;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Acklann.Daterpillar.Tests
{
    [TestClass]
    public class ScriptingTest
    {
        [TestCleanup]
        public void Cleanup()
        {
            System.Diagnostics.Debug.WriteLine("clen");
        }

        [DataTestMethod]
        [DataRow(Syntax.SQLite)]
        public void Can_generate_scripts_to_create_sql_objects(Syntax syntax)
        {
            throw new System.NotImplementedException();
        }

        [DataTestMethod]
        [DataRow(Syntax.SQLite)]
        public void Can_generate_scripts_to_drop_sql_objects(Syntax syntax)
        {
            throw new System.NotImplementedException();
        }

        [DataTestMethod]
        [DataRow(Syntax.SQLite)]
        public void Can_generate_scripts_to_alter_sql_objects(Syntax syntax)
        {
            throw new System.NotImplementedException();
        }

        private static bool IsWellFormed(string file, Syntax syntax)
        {
            return false;
        }
    }
}