using Acklann.Daterpillar;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace MSTest.Daterpillar.Tests
{
    [TestClass]
    public class DataTypeTest
    {
        [TestMethod]
        public void Equals_should_return_true_when_the_specified_dataType_are_the_same()
        {
            // Arrange
            var a = new DataType("decimal", 12, 2);
            var b = new DataType("decimal", 12, 2);

            // Act
            bool valuesAreEqual = (a == b);

            // Assert
            valuesAreEqual.ShouldBeTrue();
            a.GetHashCode().ShouldBe(b.GetHashCode());
        }

        [TestMethod]
        public void Equals_should_return_false_when_the_specified_dataType_are_not_the_same()
        {
            // Arrange
            var a = new DataType("decimal", 12, 2);
            var b = new DataType("int");

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