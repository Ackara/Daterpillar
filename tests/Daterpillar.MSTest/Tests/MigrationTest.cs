using Acklann.Daterpillar.Linq;
using Acklann.Daterpillar.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using System;
using System.Linq;

namespace Acklann.Daterpillar.Tests
{
    [TestClass]
    public class MigrationTest
    {
        [TestMethod]
        [DataRow(typeof(System.Data.SqlClient.SqlConnection), Language.TSQL)]
        [DataRow(typeof(System.Data.SQLite.SQLiteConnection), Language.SQLite)]
        [DataRow(typeof(MySql.Data.MySqlClient.MySqlConnection), Language.MySQL)]
        public void Can_determine_language_from_the_connection_type(Type connectionType, Language excepectedValue)
        {
            IDbConnectionExtensions.GetLanguage(connectionType).ShouldBe(excepectedValue);
        }

        // ==================== ENUMERATOR ==================== //

        //[TestMethod]
        [DataRow(0, "")]
        [DataRow(1, "a")]
        [DataRow(2, "a b")]
        [DataRow(9, "a b")]
        [DataRow(3, "b a c d")]
        [DataRow(4, "b a d c")]
        [DataRow(5, "b c a d")]
        [DataRow(6, "a c b d")]
        [DataRow(7, "d a c b")]
        [DataRow(8, "a d c b")]
        public void Can_sort_tables_by_dependencies(int caseNo, string exceptedValue)
        {
            var sut = GetEnumeratorCase(caseNo);
            var results = string.Join(" ", sut.EnumerateTables().Select(x => x.Name));
            results.ShouldBe(exceptedValue);
        }

        #region Backing Members

        private static Schema CreateSchema()
        {
            string assemblyFile = typeof(MigrationTest).Assembly.Location;
            return SchemaFactory.CreateFrom(assemblyFile);
        }

        private static Schema GetEnumeratorCase(int index)
        {
            var a = new Table("a"); var b = new Table("b");
            var c = new Table("c"); var d = new Table("d");
            var e = new Table("e"); var f = new Table("f");

            void join(Table x, Table y) => x.Add(new ForeignKey("", y.Name, ""));

            var s = new Schema();

            switch (index)
            {
                case 0: return s;

                case 1:
                    s.Add(a);
                    return s;

                case 2:
                    s.Add(a, b);
                    return s;

                case 3:
                    s.Add(a, b, c, d);
                    join(a, b);
                    return s;

                case 4:
                    s.Add(a, b, c, d);
                    join(a, b); join(c, d);
                    return s;

                case 5:
                    s.Add(a, b, c, d);
                    join(a, b); join(a, c);
                    return s;

                case 6:
                    s.Add(a, b, c, d);
                    join(b, c); join(c, a);
                    return s;

                case 7:
                    s.Add(a, b, c, d);
                    join(a, d); join(b, c); join(c, a);
                    return s;

                case 8:
                    s.Add(a, b, c, d);
                    join(b, c); join(c, d);
                    join(b, a); join(c, a); join(d, a);
                    return s;

                case 9:
                    s.Add(a, b);
                    join(a, b); join(b, a);
                    return s;
            }

            throw new IndexOutOfRangeException();
        }

        #endregion Backing Members
    }
}