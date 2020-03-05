using Cirilla.Core.Models;
using Cirilla.Core.Structs.Native;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Runtime.InteropServices;

namespace Cirilla.Core.Test.Tests
{
    [TestClass]
    public class SaveDataTests
    {
        [TestMethod]
        public void SizeOf__SaveData_Header()
        {
            Assert.AreEqual(64, Marshal.SizeOf<SaveData_Header>());
        }

        [TestMethod]
        public void SizeOf__SaveData_SaveSlot()
        {
            Assert.AreEqual(1007888, Marshal.SizeOf<SaveData_SaveSlot>());
        }

        [TestMethod]
        public void SizeOf__SaveData_Appearance()
        {
            Assert.AreEqual(120, Marshal.SizeOf<CharacterAppearance>());
        }

        [TestMethod]
        public void SizeOf__SaveData_GuildCard()
        {
            Assert.AreEqual(4923, Marshal.SizeOf<SaveData_GuildCard>());
        }

        [TestMethod]
        public void SizeOf__SaveData_PalicoAppearance()
        {
            Assert.AreEqual(44, Marshal.SizeOf<PalicoAppearance>());
        }

        [DataTestMethod]
        [DataRow("SAVEDATA1000_ib_dec")]
        [DataRow("SAVEDATA1000_ib_dec_1")]
        public void Load__SAVEDATA1000(string filename)
        {
            SaveData save = new SaveData(@"L:\Sync\MHW Mods\test_dataset\" + filename);
            Assert.AreEqual("Fusion", save.SaveSlots[0].HunterName);
        }

        [TestMethod]
        public void Rebuild__SAVEDATA1000()
        {
            string origPath = @"C:/Steam/userdata/112073240/582010/remote/SAVEDATA1000";
            string rebuildPath = "rebuild__SAVEDATA1000";

            SaveData save = new SaveData(origPath);
            save.Save(rebuildPath);

            if (!Utility.CheckFilesAreSame(origPath, rebuildPath))
                Assert.Fail("Hash doesn't match!");
        }
    }
}
