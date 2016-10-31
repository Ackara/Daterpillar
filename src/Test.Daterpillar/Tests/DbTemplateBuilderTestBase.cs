using Gigobyte.Daterpillar;
using Gigobyte.Daterpillar.TextTransformation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;
using Telerik.JustMock;
using Telerik.JustMock.Helpers;

namespace Test.Daterpillar.Tests
{
    internal abstract class DbTemplateBuilderTestBase
    {
        public void RunSchemaTest(IDbConnection connection, TemplateBuilderSettings settings)
        {
            // Arrange
            var schema = new Schema();

            var mockTypeResolver = Mock.Create<ITypeNameResolver>();
            mockTypeResolver.Arrange(x => x.GetName(Arg.IsAny<DataType>()))
                .Returns("string")
                .OccursAtLeast(2);
            

            // Act

            // Assert
            Assert.Fail();
        }

        public void RunSchemaTestWithSettingsDisabled()
        {
            throw new System.NotImplementedException();
        }

        public void RunIndexTest()
        {
            
            throw new System.NotImplementedException();
        }

        public void RunForeignKeyTest()
        {
            throw new System.NotImplementedException();
        }

        public void RunSchemaDropTest()
        {
            throw new System.NotImplementedException();
        }

        public void RunTableDropTest()
        {
            throw new System.NotImplementedException();
        }

        public void RunIndexDropTest()
        {
            throw new System.NotImplementedException();
        }

        public void RunForeignKeyDropTest()
        {
            throw new System.NotImplementedException();
        }
    }
}