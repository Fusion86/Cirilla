using Cirilla.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cirilla.Core.Test.Tests
{
    [TestClass]
    public class NBSCTests
    {
        [TestMethod]
        public void Load__npc_basic()
        {
            NBSC nbsc = new NBSC(Utility.GetFullPath(@"chunk0/common/npc/npc_basic.nbsc"));
        }

        [TestMethod]
        public void Rebuild__npc_basic()
        {
            string origPath = Utility.GetFullPath(@"chunk0/common/npc/npc_basic.nbsc");
            string rebuildPath = "rebuild__npc_basic.nbsc";

            NBSC nbsc = new NBSC(origPath);
            nbsc.Save(rebuildPath);

            if (!Utility.CheckFilesAreSame(origPath, rebuildPath))
                Assert.Fail("Hash doesn't match!");
        }
    }
}
