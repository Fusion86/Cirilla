using Cirilla.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace Cirilla.Core.Test.Tests
{
    [TestClass]
    public class CMPTests
    {
        [TestMethod]
        public void Load__preset_female_1()
        {
            CMP _ = new CMP(Utility.GetTestAsset(@"game/stage/st407/common/preset/preset_female_1.cmp"));
        }

        [TestMethod]
        public void Load__preset_male_1()
        {
            CMP _ = new CMP(Utility.GetTestAsset(@"game/stage/st407/common/preset/preset_male_1.cmp"));
        }

        [TestMethod]
        public void Rebuild__preset_female_1()
        {
            string origPath = Utility.GetTestAsset(@"game/stage/st407/common/preset/preset_female_1.cmp");
            string rebuildPath = "rebuild__preset_female_1.cmp";

            CMP cmp = new CMP(origPath);
            cmp.Save(rebuildPath);

            if (!Utility.CheckFilesAreSame(origPath, rebuildPath))
                Assert.Fail("Hash doesn't match!");

            File.Delete(rebuildPath);
        }

        [TestMethod]
        public void SaveSlot_to_cmp()
        {
            string cmpPath = "saveslot0.cmp";

            SaveData saveData = new SaveData(Utility.GetTestAsset(@"saves/IceborneSave1"));
            CMP cmp = new CMP(saveData.SaveSlots[0].Native.CharacterAppearance);

            cmp.Save(cmpPath);

            CMP load = new CMP(cmpPath);
            Assert.IsNotNull(load.Appearance);

            File.Delete(cmpPath);
        }
    }
}
