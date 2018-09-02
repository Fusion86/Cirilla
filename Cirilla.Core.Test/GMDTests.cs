using Cirilla.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cirilla.Core.Test
{
    [TestClass]
    public class GMDTests
    {
        [TestMethod]
        public void Load__em_names_eng()
        {
            GMD gmd = GMD.Load(@"C:\Steam\steamapps\common\Monster Hunter World\nativePC\common\text\em_names_eng.gmd");
        }

        [TestMethod]
        public void Load__q00503_eng()
        {
            GMD gmd = GMD.Load(@"C:\Steam\steamapps\common\Monster Hunter World\nativePC\common\text\quest\q00503_eng.gmd");
        }

        [TestMethod]
        public void Load__item_eng()
        {
            GMD gmd = GMD.Load(@"C:\Steam\steamapps\common\Monster Hunter World\nativePC\common\text\item_eng.gmd");
        }
    }
}
