using Cirilla.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cirilla.Core.Test.Tests
{
    [TestClass]
    public class EquipmentCraftingTests
    {
        [TestMethod]
        public void Load__weapon()
        {
            EquipmentCrafting eqcrt = new EquipmentCrafting(Utility.GetFullPath(@"chunk0/common/equip/weapon.eq_crt"));
        }

        [TestMethod]
        public void Load__armor()
        {
            EquipmentCrafting eqcrt = new EquipmentCrafting(Utility.GetFullPath(@"chunk0/common/equip/armor.eq_crt"));
        }

        [TestMethod]
        public void Load__ot_equip()
        {
            EquipmentCrafting eqcrt = new EquipmentCrafting(Utility.GetFullPath(@"chunk0/common/equip/ot_equip.eq_crt"));
        }

        [TestMethod]
        public void Rebuild__weapon()
        {
            string origPath = Utility.GetFullPath(@"chunk0/common/equip/weapon.eq_crt");
            string rebuildPath = "rebuild__weapon.eq_crt";

            EquipmentCrafting eqcrt = new EquipmentCrafting(origPath);
            eqcrt.Save(rebuildPath);

            if (!Utility.CheckFilesAreSame(origPath, rebuildPath))
                Assert.Fail("Hash doesn't match!");
        }

        [TestMethod]
        public void Rebuild__armor()
        {
            string origPath = Utility.GetFullPath(@"chunk0/common/equip/armor.eq_crt");
            string rebuildPath = "rebuild__armor.eq_crt";

            EquipmentCrafting eqcrt = new EquipmentCrafting(origPath);
            eqcrt.Save(rebuildPath);

            if (!Utility.CheckFilesAreSame(origPath, rebuildPath))
                Assert.Fail("Hash doesn't match!");
        }

        [TestMethod]
        public void Rebuild__ot_equip()
        {
            string origPath = Utility.GetFullPath(@"chunk0/common/equip/ot_equip.eq_crt");
            string rebuildPath = "rebuild__ot_equip.eq_crt";

            EquipmentCrafting eqcrt = new EquipmentCrafting(origPath);
            eqcrt.Save(rebuildPath);

            if (!Utility.CheckFilesAreSame(origPath, rebuildPath))
                Assert.Fail("Hash doesn't match!");
        }
    }
}
