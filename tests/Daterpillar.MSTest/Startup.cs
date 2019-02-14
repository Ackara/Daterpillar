
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class Startup
{
	public static void Initialize(TestContext context)
	{
        Acklann.Diffa.Resolution.TestContext.ProjectDirectory = @"C:\Users\Ackeem\Projects\Acklann\Daterpillar\tests\Daterpillar.MSTest";
	}
}