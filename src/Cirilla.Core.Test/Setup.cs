using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace Cirilla.Core.Test
{
    [TestClass]
    public class Setup
    {
        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext testContext)
        {
            Logging.LogProvider.SetCurrentLogProvider(new LogProvider());

            var mhwExtractedDataRoot = testContext.Properties["mhwExtractedDataRoot"];
            if (mhwExtractedDataRoot is string str)
            {
                if (!Directory.Exists(str))
                    Assert.Fail($"mhwExtractedDataRoot '{str}' doesn't exist!");

                Settings.MHWExtractedDataRoot = str;
            }
            else
            {
                Assert.Fail("No .runsettings file selected or mhwExtractedDataRoot is not set!");
            }
        }
    }
}
