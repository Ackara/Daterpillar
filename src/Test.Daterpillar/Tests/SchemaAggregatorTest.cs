using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.Daterpillar.Helpers;
using Tests.Daterpillar.Constants;
using System.Data;
using Gigobyte.Daterpillar.Migration;

namespace Test.Daterpillar.Tests
{
    [TestClass]
    [DeploymentItem(SampleData.Folder)]
    [DeploymentItem(KnownFile.DbConfig)]
    [DeploymentItem(KnownFile.x86SQLiteInterop)]
    public class SchemaAggregatorTest
    {
        void RunFetchSchemaTest(ISchemaAggregator sut)
        {
            sut.FetchSchema();
        }
    }
}
