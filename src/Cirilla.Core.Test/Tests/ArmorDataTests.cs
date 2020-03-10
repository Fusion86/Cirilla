using Cirilla.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cirilla.Core.Test.Tests
{
    [TestClass]
    public class ArmorDataTests
    {
        [TestMethod]
        public void Load__armordata()
        {
            ArmorData amdat = new ArmorData(Utility.GetFullPath(@"chunk0/common/equip/armor.am_dat"));
        }
    }
}
