using Cirilla.Core.Models;
using Cirilla.Core.Structs.Native;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
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
        [DataRow("VanillaSave1")]
        [DataRow("VanillaSaveDec1")]
        public void Load__SAVEDATA1000(string filename)
        {
            var save = new BaseGameSaveData(Utility.GetTestAsset(@"saves/" + filename));
            Assert.AreEqual("Fusion", save.SaveSlots[0].HunterName);
            Assert.AreEqual("Sjonnie Jan", save.SaveSlots[0].PalicoName);
        }

        [DataTestMethod]
        [DataRow("VanillaSave1", true)]
        [DataRow("VanillaSaveDec1", false)]
        public void Rebuild__SAVEDATA1000(string filename, bool encrypt)
        {
            string origPath = Utility.GetTestAsset(@"saves/" + filename);
            string rebuildPath = "rebuild__" + filename + "_" + (encrypt ? "enc" : "dec");

            var save = new BaseGameSaveData(origPath);
            save.Save(rebuildPath, encrypt, false);

            if (!Utility.CheckFilesAreSame(origPath, rebuildPath))
                Assert.Fail("Hash doesn't match!");

            File.Delete(rebuildPath);
        }
    }
}
