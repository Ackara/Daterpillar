using Acklann.Daterpillar.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using System.IO;
using System.Text;

namespace Acklann.Daterpillar.Tests
{
    [TestClass]
    public class FlywayTest
    {
        [TestMethod]
        public void Should_download_flyway_when_not_installed()
        {
            // Arrange
            string version = "5.2.4";
            string baseDirectory = Path.Combine(Path.GetTempPath(), "dtp-flyway");
            string expectedPath = Path.Combine(baseDirectory, "flyway", version, "flyway");

            // Act
            //if (File.Exists(expectedPath)) Directory.Delete(baseDirectory, recursive: true);
            var flywayPath = Flyway.Install(baseDirectory, version);

            // Assert
            flywayPath.ShouldContain(expectedPath, Case.Insensitive);
            File.Exists(expectedPath).ShouldBeTrue($"{expectedPath} was not downloaded.");
        }

        [TestMethod]
        public void Can_execute_scripts_with_flyway()
        {
            // Arrange
            var baseDirectory = Path.Combine(Path.GetTempPath(), "dtp-flyway-migrate");
            var dbFilePath = Path.Combine(baseDirectory, "test.sqlite");

            // Act
            if (!Directory.Exists(baseDirectory)) Directory.CreateDirectory(baseDirectory);
            File.WriteAllText(Path.Combine(baseDirectory, "V1__init.sql"), "create table user(id int, name varchar(64));", Encoding.UTF8);
            if (!File.Exists(dbFilePath)) File.Create(dbFilePath).Dispose();

            var installationPath = Flyway.Install(baseDirectory);
            var migrate = Flyway.Invoke("migrate", Language.SQLite, $"database={dbFilePath}", baseDirectory, installationPath);

            // Assert
            migrate.ExitCode.ShouldBe(0);
        }
    }
}