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

            object obj;

            if (testContext.Properties.TryGetValue("mhwExtractedDataRoot", out obj))
            {
                string str = (string)obj;

                if (!Directory.Exists(str))
                    Assert.Fail($"mhwExtractedDataRoot '{str}' doesn't exist!");

                Settings.MHWExtractedDataRoot = str;
            }

            if (testContext.Properties.TryGetValue("mhwInstallDirectory", out obj))
            {
                string str = (string)obj;

                if (!Directory.Exists(str))
                    Assert.Fail($"mhwInstallDirectory '{str}' doesn't exist!");

                Settings.MHWInstallDirectory = str;
            }


            if (Settings.MHWExtractedDataRoot == null || Settings.MHWInstallDirectory == null)
            {
                Assert.Fail("No .runsettings file selected or some values are not set!");
            }
        }
    }
}
