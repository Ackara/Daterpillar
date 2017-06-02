using Acklann.Daterpillar;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace MSTest.Daterpillar.Tests
{
    [TestClass]
    public class ColumnNameTest
    {
        [TestMethod]
        public void Equals_should_return_true_when_the_specified_columnName_are_the_same()
        {
            // Arrange
            var a = new ColumnName("name");
            var b = new ColumnName("name");

            // Act
            bool valuesAreEqual = (a == b);

            // Assert
            valuesAreEqual.ShouldBeTrue();
            a.GetHashCode().ShouldBe(b.GetHashCode());
        }

        [TestMethod]
        public void Equals_should_return_false_when_the_specified_columnName_are_not_the_same()
        {
            // Arrange
            var a = new ColumnName("name", Order.Descending);
            var b = new ColumnName("name");

            // Act
            bool valuesAreNotEqual = (a != b);
            bool valuesAreNotTheSame = !(a.Equals(string.Empty));

            // Assert
            valuesAreNotEqual.ShouldBeTrue();
            valuesAreNotTheSame.ShouldBeTrue();
            a.GetHashCode().ShouldNotBe(b.GetHashCode());
        }
    }
}