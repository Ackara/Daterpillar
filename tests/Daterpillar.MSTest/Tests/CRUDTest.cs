using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Acklann.Daterpillar.Prototyping;
using AutoBogus;
using Acklann.Daterpillar.Scripting;

namespace Acklann.Daterpillar.Tests
{
	[TestClass]
	public class CRUDTest
	{
		[TestMethod]
		public void Can_generate_insert_statment_for_data_object()
		{
			// Arrange

			var record = AutoFaker.Generate<Song>();

			// Act

			var sql = SqlWriter.Create(record, Language.SQL);
			

            // Assert


        }
	}
}
