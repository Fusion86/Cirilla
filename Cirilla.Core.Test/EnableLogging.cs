using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cirilla.Core.Test
{
    [TestClass]
    public class EnableLogging
    {
        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext testContext)
        {
            Logging.LogProvider.SetCurrentLogProvider(new LogProvider());
        }
    }
}
