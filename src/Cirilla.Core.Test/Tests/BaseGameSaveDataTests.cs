using Cirilla.Core.Models;
using Cirilla.Core.Structs.Native;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Runtime.InteropServices;

namespace Cirilla.Core.Test.Tests
{
    [TestClass]
    public class BaseGameSaveDataTests
    {
        [TestMethod]
        public void SizeOf__SaveData_Header()
        {
            Assert.AreEqual(64, Marshal.SizeOf<BaseGameSaveData_Header>());
        }

        [TestMethod]
        public void SizeOf__SaveData_SaveSlot()
        {
            Assert.AreEqual(1007888, Marshal.SizeOf<BaseGameSaveData_SaveSlot>());
        }

        [DataTestMethod]
        [DataRow("SAVEDATA1000")]
        [DataRow("SAVEDATA1000_dec")]
        public void Load__SAVEDATA1000(string filename)
        {
            var save = new BaseGameSaveData(@"L:\Nextcloud\Personal\Modding\MHW Mods\test_dataset\" + filename);
            //Assert.AreEqual("Fusion", save.SaveSlots[0].HunterName);
            //Assert.AreEqual("Sjonnie Jan", save.SaveSlots[0].PalicoName);
            //Assert.AreEqual("Fusion", save.SaveSlots[1].HunterName);
            //Assert.AreEqual("Knor", save.SaveSlots[1].PalicoName);
        }

        [DataTestMethod]
        [DataRow("SAVEDATA1000", true)]
        [DataRow("SAVEDATA1000_dec", false)]
        public void Rebuild__SAVEDATA1000(string filename, bool encrypt)
        {
            string origPath = @"L:\Nextcloud\Personal\Modding\MHW Mods\test_dataset\" + filename;
            string rebuildPath = "rebuild__" + filename + "_" + (encrypt ? "enc" : "dec");

            var save = new BaseGameSaveData(origPath);
            save.Save(rebuildPath, encrypt, false);

            if (!Utility.CheckFilesAreSame(origPath, rebuildPath))
                Assert.Fail("Hash doesn't match!");
        }
    }
}
