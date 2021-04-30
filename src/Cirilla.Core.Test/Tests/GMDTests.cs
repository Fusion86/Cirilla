using System.IO;
using System.Linq;
using Cirilla.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cirilla.Core.Test.Tests
{
    [TestClass]
    public class GMDTests
    {
        [DataTestMethod]
        [DataRow("common/text/em_names_eng.gmd")]
        [DataRow("common/text/quest/q00503_eng.gmd")]
        [DataRow("common/text/item_eng.gmd")]
        [DataRow("common/text/steam/armor_eng.gmd")]
        [DataRow("common/text/cm_facility_eng.gmd")]
        [DataRow("common/text/cm_facility_kor.gmd")]
        [DataRow("common/text/cm_status_eng.gmd")]
        [DataRow("common/text/cm_chat_eng.gmd")]
        [DataRow("common/text/steam/cm_chat_eng.gmd")]
        [DataRow("common/text/vfont/bow_eng.gmd")]
        [DataRow("common/text/trace/tr_em011_01_eng.gmd")]
        public void Load(string str)
        {
            GMD _ = new GMD(Utility.GetTestAsset("game/" + str));
        }

        [DataTestMethod]
        [DataRow("common/text/em_names_eng.gmd")]
        [DataRow("common/text/quest/q00503_eng.gmd")]
        [DataRow("common/text/item_eng.gmd")]
        [DataRow("common/text/steam/wep_series_eng.gmd")]
        [DataRow("common/text/steam/w_sword_eng.gmd")]
        [DataRow("common/text/cm_status_eng.gmd")]
        [DataRow("common/text/cm_chat_eng.gmd")]
        [DataRow("common/text/steam/cm_chat_eng.gmd")]
        [DataRow("common/text/trace/tr_em011_01_eng.gmd")]
        public void Rebuild(string str)
        {
            string origPath = Utility.GetTestAsset("game/" + str);
            string rebuildPath = "rebuild_" + Path.GetFileName(str);

            GMD gmd = new GMD(origPath);
            gmd.Save(rebuildPath);

            if (!Utility.CheckFilesAreSame(origPath, rebuildPath))
                Assert.Fail("Hash doesn't match!");

            File.Delete(rebuildPath);
        }

        [TestMethod]
        public void AddString__q00503_eng()
        {
            string newPath = "addstring__q00503_eng.gmd";

            GMD gmd = new GMD(Utility.GetTestAsset(@"game/common/text/quest/q00503_eng.gmd"));
            gmd.AddString("MY_NEW_STRING", "New string text....");
            gmd.Save(newPath);

            GMD oldGmd = new GMD(Utility.GetTestAsset(@"game/common/text/quest/q00503_eng.gmd"));
            GMD newGmd = new GMD(newPath);

            Assert.IsTrue(oldGmd.Header.KeyCount < newGmd.Header.KeyCount);
            Assert.IsTrue(oldGmd.Header.KeyBlockSize < newGmd.Header.KeyBlockSize);
            Assert.IsTrue(oldGmd.Header.StringCount < newGmd.Header.StringCount);
            Assert.IsTrue(oldGmd.Header.StringBlockSize < newGmd.Header.StringBlockSize);
            Assert.IsNotNull(newGmd.Entries.FirstOrDefault(x => x.Value == "New string text...."));

            File.Delete(newPath);
        }

        [TestMethod]
        public void AddStringAt__armor_eng()
        {
            string newPath = "addstringat__armor_eng.gmd";

            GMD gmd = new GMD(Utility.GetTestAsset(@"game/common/text/steam/armor_eng.gmd"));

            // Find index
            var entryToFind = gmd.Entries.OfType<GMD_Entry>().FirstOrDefault(x => x.Key == "AM_ACCE001_NAME");
            int idx = gmd.Entries.IndexOf(entryToFind);

            gmd.AddString("AM_ACCE001_EXP", "Description for AM_ACCE001_NAME", idx + 1);
            gmd.Save(newPath);

            GMD oldGmd = new GMD(Utility.GetTestAsset(@"game/common/text/quest/q00503_eng.gmd"));
            GMD newGmd = new GMD(newPath);

            Assert.IsTrue(oldGmd.Header.KeyCount < newGmd.Header.KeyCount);
            Assert.IsTrue(oldGmd.Header.KeyBlockSize < newGmd.Header.KeyBlockSize);
            Assert.IsTrue(oldGmd.Header.StringCount < newGmd.Header.StringCount);
            Assert.IsTrue(oldGmd.Header.StringBlockSize < newGmd.Header.StringBlockSize);
            Assert.IsNotNull(newGmd.Entries.FirstOrDefault(x => x.Value == "Description for AM_ACCE001_NAME"));

            File.Delete(newPath);
        }

        [TestMethod]
        public void ReaddString__w_sword_eng()
        {
            // This won't display correctly in game, because the string order DOES matter
            string origPath = Utility.GetTestAsset(@"game/common/text/steam/w_sword_eng.gmd");
            string newPath = "removestring__w_sword_eng.gmd";
            string readdPath = "readdstring__w_sword_eng.gmd";

            GMD gmd = new GMD(origPath);
            gmd.RemoveString("WP_WSWD_044_NAME");
            gmd.Save(newPath);

            GMD newGmd = new GMD(newPath);

            newGmd.AddString("WP_WSWD_044_NAME", "My new string");
            newGmd.Save(readdPath);

            File.Delete(newPath);
            File.Delete(readdPath);
        }
    }
}
