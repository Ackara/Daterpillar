using Acklann.Daterpillar;
using Acklann.Daterpillar.TextTransformation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.Daterpillar.Constants;

namespace Test.Daterpillar.Tests
{
    [TestClass]
    public class ScriptBuilderFactoryTest
    {
        [TestMethod]
        [Owner(Dev.Ackara)]
        public void CreateInstance_should_return_a_script_builder_object_when_a_valid_name_is_passed()
        {
            // Act
            var instance = new ScriptBuilderFactory().CreateInstance(ConnectionType.TSQL);

            // Assert
            Assert.IsInstanceOfType(instance, typeof(TSQLScriptBuilder));
        }
    }
}