using Cirilla.Core.Models;
using Cirilla.Core.Structs.Native;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
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
            Assert.AreEqual(2136768, Marshal.SizeOf<SaveData_SaveSlot>());
        }

        [TestMethod]
        public void SizeOf__SaveData_Appearance()
        {
            Assert.AreEqual(168, Marshal.SizeOf<CharacterAppearance>());
        }

        [TestMethod]
        public void SizeOf__SaveData_GuildCard()
        {
            Assert.AreEqual(7787, Marshal.SizeOf<SaveData_GuildCard>());
        }

        [TestMethod]
        public void SizeOf__SaveData_PalicoAppearance()
        {
            Assert.AreEqual(44, Marshal.SizeOf<PalicoAppearance>());
        }

        [DataTestMethod]
        [DataRow("IceborneSave1")]
        [DataRow("IceborneSaveDec1")]
        [DataRow("IceborneSaveDec2")]
        public void Load__SAVEDATA1000(string filename)
        {
            SaveData save = new SaveData(Utility.GetTestAsset(@"saves/" + filename));
            Assert.AreEqual("Fusion", save.SaveSlots[0].HunterName);
            Assert.AreEqual("Sjonnie Jan", save.SaveSlots[0].PalicoName);
            Assert.AreEqual("Fusion", save.SaveSlots[1].HunterName);
            Assert.AreEqual("Knor", save.SaveSlots[1].PalicoName);
        }

        //[TestMethod]
        //public void Load__SAVEDATA1000_remote()
        //{
        //    SaveData save = new SaveData(@"C:\Steam\userdata\112073240\582010\remote\SAVEDATA1000");
        //    Assert.AreEqual("Fusion", save.SaveSlots[0].HunterName);
        //}

        [DataTestMethod]
        [DataRow("IceborneSave1", true)]
        [DataRow("IceborneSaveDec1", false)]
        [DataRow("IceborneSaveDec2", false)]
        public void Rebuild__SAVEDATA1000(string filename, bool encrypt)
        {
            string origPath = Utility.GetTestAsset(@"saves/" + filename);
            string rebuildPath = "rebuild__" + filename + "_" + (encrypt ? "enc" : "dec");

            SaveData save = new SaveData(origPath);
            save.Save(rebuildPath, encrypt, false);

            if (!Utility.CheckFilesAreSame(origPath, rebuildPath))
                Assert.Fail("Hash doesn't match!");

            File.Delete(rebuildPath);
        }

        //[TestMethod]
        //public void Rebuild__SAVEDATA1000_remote()
        //{
        //    string origPath = @"C:\Steam\userdata\112073240\582010\remote\SAVEDATA1000";
        //    string rebuildPath = "rebuild__SAVEDATA1000_remote";

        //    SaveData save = new SaveData(origPath);
        //    save.Save(rebuildPath);

        //    if (!Utility.CheckFilesAreSame(origPath, rebuildPath))
        //        Assert.Fail("Hash doesn't match!");
        //}
    }
}
