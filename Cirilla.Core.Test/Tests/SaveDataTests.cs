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
        public void Load__SAVEDATA1000()
        {
            SaveData save = new SaveData(@"C:/Steam/userdata/112073240/582010/remote/SAVEDATA1000");
        }
    }
}
