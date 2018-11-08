using Cirilla.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cirilla.Core.Test.Tests
{
    [TestClass]
    public class CMPTests
    {
        [TestMethod]
        public void Load__preset_female_1()
        {
            CMP cmp = new CMP(Utility.GetFullPath(@"chunk0/stage/st407/common/preset/preset_female_1.cmp"));
        }

        [TestMethod]
        public void Load__preset_male_1()
        {
            CMP cmp = new CMP(Utility.GetFullPath(@"chunk0/stage/st407/common/preset/preset_male_1.cmp"));
        }

        [TestMethod]
        public void Rebuild__preset_female_1()
        {
            string origPath = Utility.GetFullPath(@"chunk0/stage/st407/common/preset/preset_female_1.cmp");
            string rebuildPath = "rebuild__preset_female_1.cmp";

            CMP cmp = new CMP(origPath);
            cmp.Save(rebuildPath);

            if (!Utility.CheckFilesAreSame(origPath, rebuildPath))
                Assert.Fail("Hash doesn't match!");
        }

        [TestMethod]
        public void SaveSlot_to_cmp()
        {
            string cmpPath = "saveslot0.cmp";

            SaveData saveData = new SaveData(@"C:/Steam/userdata/112073240/582010/remote/SAVEDATA1000");
            CMP cmp = new CMP(saveData.SaveSlots[0].Native.Appearance);

            cmp.Save(cmpPath);

            CMP load = new CMP(cmpPath);
            Assert.IsNotNull(load.Appearance);
        }
    }
}
