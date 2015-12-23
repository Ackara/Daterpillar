using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Tests.Daterpillar.UnitTest
{
    [TestClass]
    public abstract class TypeNameResolverTestBase
    {
        /// <summary>
        /// This method ensures that the test data in alignment with the allowed <see
        /// cref="Artifact.XDDL"/> xml schema types.
        /// </summary>
        public static void AssertTestDataIsValid()
        {
            var officialTypeNameList = GetXsdTypeNames().ToList();
            var testDataTypeNameList = GetSpreadSheetTypeNames().ToList();

            Assert.AreEqual(officialTypeNameList.Count, testDataTypeNameList.Count);

            // Verify both list have the same type names
            foreach (var officialName in officialTypeNameList)
            {
                if (testDataTypeNameList.Contains(officialName) == false)
                {
                    Assert.Fail($"{Artifact.DataXLSX} does not contain \"{officialName}\" as a type name.");
                }
            }
        }

        private static IEnumerable<string> GetXsdTypeNames()
        {
            string schemaPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Artifact.XDDL);

            var xsdDoc = XDocument.Load(schemaPath);
            string xmlns = "http://www.w3.org/2001/XMLSchema";
            XElement typeNameElement = (from x in xsdDoc.Descendants(XName.Get("simpleType", xmlns))
                                        where x.Attribute("name") != null && x.Attribute("name").Value == "TypeName"
                                        select x).FirstOrDefault();

            var enumValues = from x in typeNameElement.Descendants(XName.Get("enumeration", xmlns))
                             select x.Attribute("value").Value;

            return enumValues;
        }

        private static IEnumerable<string> GetSpreadSheetTypeNames()
        {
            using (var connection = new System.Data.Odbc.OdbcConnection(Data.ExcelConnStr))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT Type FROM [DataTypes$];";
                    using (var results = new DataTable())
                    {
                        results.Load(command.ExecuteReader());
                        foreach (DataRow row in results.Rows)
                        {
                            yield return Convert.ToString(row[0]);
                        }
                    }
                }
            }
        }
    }
}