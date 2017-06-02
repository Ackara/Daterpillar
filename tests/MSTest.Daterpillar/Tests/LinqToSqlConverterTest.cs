using Acklann.Daterpillar;
using Acklann.Daterpillar.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MSTest.Daterpillar.Fake;
using Shouldly;

namespace MSTest.Daterpillar.Tests
{
    [TestClass]
    public class LinqToSqlConverterTest
    {
        [TestMethod]
        public void Convert_should_return_a_string_listing_the_column_names_specified_linq_expression()
        {
            // Act
            var result1 = LinqToSqlConverter.Convert<SimpleTable>(x => x.Sex);
            var result2 = LinqToSqlConverter.Convert<SimpleTable>(x => x.Date);
            var result3 = LinqToSqlConverter.Convert<SimpleTable>(x => x.Name.Length);
            var result4 = LinqToSqlConverter.Convert<SimpleTable>(x => x.Id, x => x.Name);
            var result5 = LinqToSqlConverter.Convert<SimpleTable>(x => new { x.Id, x.Name, x.Date });

            // Assert
            result1.ShouldBe(new string[] { nameof(SimpleTable.Sex) });
            result2.ShouldBe(new string[] { SimpleTable.CreatedOn });
            result3.ShouldBe(new string[] { nameof(string.Length) });
            result4.ShouldBe(new string[] { nameof(SimpleTable.Id), nameof(SimpleTable.Name) });
            result5.ShouldBe(new string[] { nameof(SimpleTable.Id), nameof(SimpleTable.Name), SimpleTable.CreatedOn });
        }

        [TestMethod]
        public void Convert_should_return_a_string_with_the_name_value_pairs_specified_in_linq_expression()
        {
            // Act
            var result1 = LinqToSqlConverter.Convert<SimpleTable>(Syntax.MySQL, x => x.Amount >= 1.25M);
            var result2 = LinqToSqlConverter.Convert<SimpleTable>(Syntax.Generic, x => x.Amount < 100 && x.Sex == "m");
            var result3 = LinqToSqlConverter.Convert<SimpleTable>(Syntax.Generic, x => x.Amount >= 100 && x.Sex == "m" && x.Id != 0);
            var result4 = LinqToSqlConverter.Convert<SimpleTable>(Syntax.Generic, x => (x.Exist == true && x.Sex == "f" && x.Rate > 3) || x.Id == 124);

            var result5 = LinqToSqlConverter.Convert<SimpleTable>(Syntax.Generic, x => x.Amount > (10 * 2));

            // Assert
            result1.ShouldBe($"`amount` >= '1.25'");
            result2.ShouldBe($"amount < '100' AND Sex = 'm'");
            result3.ShouldBe("(amount >= '100' AND Sex = 'm') AND Id <> '0'");
            result4.ShouldBe("((Exist = '1' AND Sex = 'f') AND Rate > '3') OR Id = '124'");
            result5.ShouldBe("amount > '20'");
        }
    }
}