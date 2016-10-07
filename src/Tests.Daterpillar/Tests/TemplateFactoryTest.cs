using Gigobyte.Daterpillar;
using Gigobyte.Daterpillar.TextTransformation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Daterpillar.Tests
{
    [TestClass]
    public class TemplateFactoryTest
    {
        [TestMethod]
        [Owner(Test.Dev.Ackara)]
        public void CreateInstance_should_return_a_template_object_when_the_partial_match_flag_is_set()
        {
            var sut = new TemplateFactory();
            var type = sut.CreateInstance(SupportedDatabase.MSSQL.ToString(), partialMatch: true);
            Assert.IsInstanceOfType(type, typeof(ITemplate));
        }
    }
}