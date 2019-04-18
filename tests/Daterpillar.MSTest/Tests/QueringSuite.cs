using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Acklann.Daterpillar.Tests
{
	[TestClass]
	public class QueringSuite
	{
        //[TestMethod]
        public void MyTestMethod()
        {
            dynamic sut = null;

            sut.select("", "", "").from("").where("", "");
        }
	}
}
