using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Daterpillar.IntegrationTest
{
    [TestClass]
    [DeploymentItem(Artifact.MusicSchema)]
    [DeploymentItem(Artifact.x86SQLiteInterop)]
    [DeploymentItem(Artifact.x64SQLiteInterop)]
    public class SqlCommandTest
    {

    }
}
