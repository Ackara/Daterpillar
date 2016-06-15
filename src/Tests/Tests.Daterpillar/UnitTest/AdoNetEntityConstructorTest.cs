using Gigobyte.Daterpillar.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Tests.Daterpillar.UnitTest
{
    [TestClass]
    [DeploymentItem(SampleData.SongsCSV)]
    public class AdoNetEntityConstructorTest
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        [Owner(Dev.Ackara)]
        [DataSource(SampleData.CsvDataProvider, SampleData.SongsConnStr, SampleData.SongsTable, DataAccessMethod.Sequential)]
        public void CreateInstance_should_convert_DataRow_object_to_an_EntityBase_object()
        {
            // Arrange
            var name = Convert.ToString(TestContext.DataRow["Name"]);
            var price = Convert.ToDecimal(TestContext.DataRow["Price"]);
            var onDevice = Convert.ToBoolean(TestContext.DataRow["On_Device"]);

            var returnType = typeof(Sample.Song);
            var sut = new AdoNetEntityConstructor();

            // Act
            var entity = (Sample.Song)sut.CreateInstance(returnType, TestContext.DataRow);

            // Assert
            Assert.AreEqual((name == string.Empty ? null : name), entity.Name);
            Assert.AreEqual(price, entity.Price);
            Assert.AreEqual(onDevice, entity.OnDevice);
        }
    }
}