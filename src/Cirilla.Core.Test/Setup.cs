using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace Cirilla.Core.Test
{
    [TestClass]
    public class Setup
    {
        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext _)
        {
            Logging.LogProvider.SetCurrentLogProvider(new LogProvider());

            var testDataDir = FindTestDataDir(new DirectoryInfo("."));

            if (testDataDir != null)
                Settings.TestDataDir = testDataDir;
            else
                Assert.Fail("Couldn't find TestDataDir in the current directory tree.");
        }

        public static string? FindTestDataDir(DirectoryInfo currentDir)
        {
            var testDataDir = Path.Combine(currentDir.FullName, "testdata");
            if (Directory.Exists(testDataDir))
                return testDataDir;
            else if (currentDir.Parent != null)
                return FindTestDataDir(currentDir.Parent);
            else
                return null;
        }
    }
}
