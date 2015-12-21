using Ackara.Daterpillar.Transformation;
using Ackara.Daterpillar.Transformation.Template;
using ApprovalTests;
using ApprovalTests.Namers;
using ApprovalTests.Reporters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;
using Telerik.JustMock.Helpers;

namespace Tests.Daterpillar.UnitTest
{
    [TestClass]
    [UseApprovalSubdirectory(Str.ApprovalsDir)]
    [UseReporter(typeof(DiffReporter), typeof(ClipboardReporter))]
    public class MySqlTemplateTest
    {
        [TestMethod]
        [Owner(Str.Ackara)]
        public void GenerateMySqlSchemaWithComments()
        {
            // Arrange
            var schema = Samples.GetSchema();
            var managerTable = Samples.GetTableSchema("Manager");
            schema.Tables.Add(managerTable);

            var mockResolver = Mock.Create<ITypeNameResolver>();
            mockResolver.Arrange(x => x.GetName(Arg.IsAny<DataType>()))
                .Returns("int")
                .OccursAtLeast(1);

            var sut = new MySqlTemplate(mockResolver, addComment: true);

            // Act
            var mysql = sut.Transform(schema);

            // Assert
            mockResolver.Assert();
            Approvals.Verify(mysql);
        }

        [TestMethod]
        [Owner(Str.Ackara)]
        public void GenerateMySqlSchemaWithoutComments()
        {
            // Arrange
            var schema = Samples.GetSchema();
            var managerTable = Samples.GetTableSchema("Manager");
            schema.Tables.Add(managerTable);

            var mockResolver = Mock.Create<ITypeNameResolver>();
            mockResolver.Arrange(x => x.GetName(Arg.IsAny<DataType>()))
                .Returns("int")
                .OccursAtLeast(1);

            var sut = new MySqlTemplate(mockResolver, addComment: false);

            // Act
            var mysql = sut.Transform(schema);

            // Assert
            mockResolver.Assert();
            Approvals.Verify(mysql);
        }
    }
}