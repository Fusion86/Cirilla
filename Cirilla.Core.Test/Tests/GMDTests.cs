using Cirilla.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Security.Cryptography;

namespace Cirilla.Core.Test.Tests
{
    [TestClass]
    public class GMDTests
    {
        [TestMethod]
        public void Load__em_names_eng()
        {
            GMD gmd = new GMD(Utility.GetFullPath(@"chunk0\common\text\em_names_eng.gmd"));
        }

        [TestMethod]
        public void Load__q00503_eng()
        {
            GMD gmd = new GMD(Utility.GetFullPath(@"chunk0\common\text\quest\q00503_eng.gmd"));
        }

        [TestMethod]
        public void Load__item_eng()
        {
            GMD gmd = new GMD(Utility.GetFullPath(@"chunk0\common\text\item_eng.gmd"));
        }

        [TestMethod]
        public void Load__armor_eng()
        {
            // StringCount > actual number of strings
            GMD gmd = new GMD(Utility.GetFullPath(@"chunk0\common\text\steam\armor_eng.gmd"));
        }

        [TestMethod]
        public void Load__action_trial_eng()
        {
            // Uses skipInvalidMessages
            GMD gmd = new GMD(Utility.GetFullPath(@"chunk0\common\text\action_trial_eng.gmd"));
        }

        [TestMethod]
        public void Rebuild__em_names_eng()
        {
            string origPath = Utility.GetFullPath(@"chunk0\common\text\em_names_eng.gmd");
            string rebuildPath = "rebuild__em_names_eng.gmd";

            GMD gmd = new GMD(origPath);
            gmd.Save(rebuildPath);

            using (HashAlgorithm hashAlgorithm = SHA256.Create())
            using (FileStream origFs = new FileStream(origPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (FileStream rebuildFs = new FileStream(rebuildPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                byte[] origHash = hashAlgorithm.ComputeHash(origFs);
                byte[] rebuildHash = hashAlgorithm.ComputeHash(rebuildFs);

                CollectionAssert.AreEqual(origHash, rebuildHash, "Hash doesn't match!");
            }
        }

        [TestMethod]
        public void Rebuild__item_eng()
        {
            string origPath = Utility.GetFullPath(@"chunk0\common\text\item_eng.gmd");
            string rebuildPath = "rebuild__item_eng.gmd";

            GMD gmd = new GMD(origPath);
            gmd.Save(rebuildPath);

            using (HashAlgorithm hashAlgorithm = SHA256.Create())
            using (FileStream origFs = new FileStream(origPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (FileStream rebuildFs = new FileStream(rebuildPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                byte[] origHash = hashAlgorithm.ComputeHash(origFs);
                byte[] rebuildHash = hashAlgorithm.ComputeHash(rebuildFs);

                CollectionAssert.AreEqual(origHash, rebuildHash, "Hash doesn't match!");
            }
        }

        [TestMethod]
        public void Rebuild__action_trial_eng()
        {
            // Uses skipInvalidMessages, rebuild file will not be the same since it removes the "Invalid Message" strings
            string origPath = Utility.GetFullPath(@"chunk0\common\text\action_trial_eng.gmd");
            string rebuildPath = "rebuild__action_trial_eng.gmd";

            GMD gmd = new GMD(origPath);
            gmd.Save(rebuildPath);

            using (HashAlgorithm hashAlgorithm = SHA256.Create())
            using (FileStream origFs = new FileStream(origPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (FileStream rebuildFs = new FileStream(rebuildPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                byte[] origHash = hashAlgorithm.ComputeHash(origFs);
                byte[] rebuildHash = hashAlgorithm.ComputeHash(rebuildFs);

                //CollectionAssert.AreEqual(origHash, rebuildHash, "Hash doesn't match!");
            }
        }
    }
}
