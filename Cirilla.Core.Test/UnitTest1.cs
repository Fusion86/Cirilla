using Cirilla.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cirilla.Core.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestInitialize]
        public void TestInitialize()
        {
            Logging.LogProvider.SetCurrentLogProvider(new LogProvider());
        }

        [TestMethod]
        public void TestMethod1()
        {
            GMD gmd = GMD.Load(@"C:\Steam\steamapps\common\Monster Hunter World\nativePC\common\text\quest\q00503_eng.gmd");
        }
    }
}
