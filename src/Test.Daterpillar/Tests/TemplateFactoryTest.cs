﻿using Gigobyte.Daterpillar;
using Gigobyte.Daterpillar.TextTransformation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.Daterpillar.Constants;

namespace Tests.Daterpillar.Tests
{
    [TestClass]
    public class TemplateFactoryTest
    {
        [TestMethod]
        [Owner(Dev.Ackara)]
        public void CreateInstance_should_return_a_template_object_when_the_partial_match_flag_is_set()
        {
            var sut = new TemplateFactory();
            var type = sut.CreateInstance(SupportedDatabase.TSQL.ToString(), partialMatch: true);
            Assert.IsInstanceOfType(type, typeof(IScriptBuilder));
        }
    }
}