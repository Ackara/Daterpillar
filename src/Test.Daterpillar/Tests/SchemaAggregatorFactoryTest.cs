using Gigobyte.Daterpillar;
using Gigobyte.Daterpillar.Migration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;
using Telerik.JustMock;
using Tests.Daterpillar.Constants;

namespace Tests.Daterpillar.Tests
{
    [TestClass]
    public class SchemaAggregatorFactoryTest
    {
        [TestMethod]
        [Owner(Dev.Ackara)]
        public void CreateInstance_should_instantiate_a_schema_aggregator_when_a_db_enum_is_passed()
        {
            var sut = new SchemaAggregatorFactory();
            var type = sut.CreateInstance(SupportedDatabase.MySQL, Mock.Create<IDbConnection>());
            Assert.IsInstanceOfType(type, typeof(ISchemaAggregator));
        }
    }
}