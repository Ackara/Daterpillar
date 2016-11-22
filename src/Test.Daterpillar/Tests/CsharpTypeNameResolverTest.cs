﻿using Acklann.Daterpillar;
using Acklann.Daterpillar.TextTransformation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Tests.Daterpillar.Constants;

namespace Tests.Daterpillar.Tests
{
    [TestClass]
    [DeploymentItem(DDT.Samples)]
    public class CSharpTypeNameResolverTest
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        [Owner(Dev.Ackara)]
        [DataSource(DDT.Provider, (DDT.Directory + KnownFile.DataTypesCSV), KnownFile.DataTypesCSV, DataAccessMethod.Sequential)]
        public void GetName_should_return_a_valid_csharp_type_name_when_data_type_is_passed()
        {
            // Arrange
            var dataType = new DataType(typeName: Convert.ToString(TestContext.DataRow["Type"]));
            var expected = TestContext.DataRow["C#"].ToString();
            var sut = new CSharpTypeNameResolver();

            // Act
            TestContext.WriteLine("Arg: {0}", dataType);
            var result = sut.GetName(dataType);

            // Assert
            Assert.AreEqual(expected, result);
        }
    }
}