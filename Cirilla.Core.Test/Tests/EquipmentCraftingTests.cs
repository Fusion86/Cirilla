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
    }
}
