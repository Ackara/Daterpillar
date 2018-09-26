using Acklann.Daterpillar.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Acklann.Daterpillar.Tests
{
    [TestClass]
    public class SchemaTest
    {
        [TestMethod]
        public void Can_deserialize_a_schema()
        {
            // Act
            var schema = Schema.Load(SampleFile.GetSakila().OpenRead());

            // Assert
            
        }
    }
}
