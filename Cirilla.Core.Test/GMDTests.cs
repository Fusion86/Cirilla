using Cirilla.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Security.Cryptography;

namespace Cirilla.Core.Test
{
    [TestClass]
    public class GMDTests
    {
        [TestMethod]
        public void Load__em_names_eng()
        {
            GMD gmd = new GMD(@"C:\Steam\steamapps\common\Monster Hunter World\nativePC\common\text\em_names_eng.gmd");
        }

        [TestMethod]
        public void Load__q00503_eng()
        {
            GMD gmd = new GMD(@"C:\Steam\steamapps\common\Monster Hunter World\nativePC\common\text\quest\q00503_eng.gmd");
        }

        [TestMethod]
        public void Load__item_eng()
        {
            GMD gmd = new GMD(@"C:\Steam\steamapps\common\Monster Hunter World\nativePC\common\text\item_eng.gmd");
        }

        [TestMethod]
        public void Load__armor_eng()
        {
            // KeyCount != StringCount
            GMD gmd = new GMD(@"L:\MHWMods\chunk0\common\text\steam\armor_eng.gmd");
        }

        [TestMethod]
        public void Rebuild__action_trial_eng()
        {
            string origPath = @"C:\Steam\steamapps\common\Monster Hunter World\nativePC\common\text\em_names_eng.gmd";
            string rebuildPath = "rebuild__em_names_eng.gmd";

            GMD gmd = new GMD(origPath);
            gmd.Save(rebuildPath);

            using (HashAlgorithm hashAlgorithm = SHA256.Create())
            using (FileStream origFs = new FileStream(origPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (FileStream rebuildFs = new FileStream(rebuildPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                byte[] origHash = hashAlgorithm.ComputeHash(origFs);
                byte[] rebuildHash = hashAlgorithm.ComputeHash(rebuildFs);

                CollectionAssert.AreEqual(origHash, rebuildHash);
            }
        }

        [TestMethod]
        public void Rebuild__item_eng()
        {
            string origPath = @"C:\Steam\steamapps\common\Monster Hunter World\nativePC\common\text\item_eng.gmd";
            string rebuildPath = "rebuild__item_eng.gmd";

            GMD gmd = new GMD(origPath);
            gmd.Save(rebuildPath);

            using (HashAlgorithm hashAlgorithm = SHA256.Create())
            using (FileStream origFs = new FileStream(origPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (FileStream rebuildFs = new FileStream(rebuildPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                byte[] origHash = hashAlgorithm.ComputeHash(origFs);
                byte[] rebuildHash = hashAlgorithm.ComputeHash(rebuildFs);

                CollectionAssert.AreEqual(origHash, rebuildHash);
            }
        }
    }
}
