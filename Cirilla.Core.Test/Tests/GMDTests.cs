using System.Linq;
using Cirilla.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cirilla.Core.Test.Tests
{
    [TestClass]
    public class GMDTests
    {
        [TestMethod]
        public void Load__em_names_eng()
        {
            GMD gmd = new GMD(Utility.GetFullPath(@"chunk0/common/text/em_names_eng.gmd"));
        }

        [TestMethod]
        public void Load__q00503_eng()
        {
            GMD gmd = new GMD(Utility.GetFullPath(@"chunk0/common/text/quest/q00503_eng.gmd"));
        }

        [TestMethod]
        public void Load__item_eng()
        {
            GMD gmd = new GMD(Utility.GetFullPath(@"chunk0/common/text/item_eng.gmd"));
        }

        [TestMethod]
        public void Load__armor_eng()
        {
            // StringCount > actual number of strings
            GMD gmd = new GMD(Utility.GetFullPath(@"chunk0/common/text/steam/armor_eng.gmd"));
        }

        [TestMethod]
        public void Load__action_trial_eng()
        {
            // Uses skipInvalidMessages
            GMD gmd = new GMD(Utility.GetFullPath(@"chunk0/common/text/action_trial_eng.gmd"));

            //Assert.AreEqual(gmd.Entries[3].Value, "©CAPCOM CO., LTD. ALL RIGHTS RESERVED.");
        }

        [TestMethod]
        public void Load__action_trial_ara()
        {
            // Uses skipInvalidMessages and weird workaround (see Models/GMD.cs)
            GMD gmd = new GMD(Utility.GetFullPath(@"chunk0/common/text/action_trial_ara.gmd"));

            //Assert.AreEqual(gmd.Entries[3].Value, "©CAPCOM CO., LTD. ALL RIGHTS RESERVED.");
        }

        [TestMethod]
        public void Rebuild__em_names_eng()
        {
            string origPath = Utility.GetFullPath(@"chunk0/common/text/em_names_eng.gmd");
            string rebuildPath = "rebuild__em_names_eng.gmd";

            GMD gmd = new GMD(origPath);
            gmd.Save(rebuildPath);

            if (!Utility.CheckFilesAreSame(origPath, rebuildPath))
                Assert.Fail("Hash doesn't match!");
        }

        [TestMethod]
        public void Rebuild__q00503_eng()
        {
            string origPath = Utility.GetFullPath(@"chunk0/common/text/quest/q00503_eng.gmd");
            string rebuildPath = "rebuild__q00503_eng";

            GMD gmd = new GMD(origPath);
            gmd.Save(rebuildPath);

            if (!Utility.CheckFilesAreSame(origPath, rebuildPath))
                Assert.Fail("Hash doesn't match!");
        }

        [TestMethod]
        public void Rebuild__item_eng()
        {
            string origPath = Utility.GetFullPath(@"chunk0/common/text/item_eng.gmd");
            string rebuildPath = "rebuild__item_eng.gmd";

            GMD gmd = new GMD(origPath);
            gmd.Save(rebuildPath);

            if (!Utility.CheckFilesAreSame(origPath, rebuildPath))
                Assert.Fail("Hash doesn't match!");
        }

        [TestMethod]
        public void Rebuild__action_trial_eng()
        {
            // Uses skipInvalidMessages, rebuild file will not be the same since it removes the "Invalid Message" strings
            string origPath = Utility.GetFullPath(@"chunk0/common/text/action_trial_eng.gmd");
            string rebuildPath = "rebuild__action_trial_eng.gmd";

            GMD gmd = new GMD(origPath);
            gmd.Save(rebuildPath);

            //if (!Utility.CheckFilesAreSame(origPath, rebuildPath))
            //    Assert.Fail("Hash doesn't match!");
        }

        [TestMethod]
        public void Rebuild__wep_series_eng()
        {
            string origPath = Utility.GetFullPath(@"chunk0/common/text/steam/wep_series_eng.gmd");
            string rebuildPath = "rebuild__wep_series_eng.gmd";

            GMD gmd = new GMD(origPath);
            gmd.Save(rebuildPath);

            //if (!Utility.CheckFilesAreSame(origPath, rebuildPath))
            //    Assert.Fail("Hash doesn't match!");
        }

        [TestMethod]
        public void Rebuild__w_sword_eng()
        {
            string origPath = Utility.GetFullPath(@"chunk0/common/text/steam/w_sword_eng.gmd");
            string rebuildPath = "rebuild__w_sword_eng.gmd";

            GMD gmd = new GMD(origPath);
            gmd.Save(rebuildPath);

            if (!Utility.CheckFilesAreSame(origPath, rebuildPath))
                Assert.Fail("Hash doesn't match!");
        }

        [TestMethod]
        public void AddString__q00503_eng()
        {
            string newPath = "addstring__q00503_eng.gmd";

            GMD gmd = new GMD(Utility.GetFullPath(@"chunk0/common/text/quest/q00503_eng.gmd"));
            gmd.AddString("MY_NEW_STRING", "New string text....");
            gmd.Save(newPath);

            GMD oldGmd = new GMD(Utility.GetFullPath(@"chunk0/common/text/quest/q00503_eng.gmd"));
            GMD newGmd = new GMD(newPath);

            Assert.IsTrue(oldGmd.Header.KeyCount < newGmd.Header.KeyCount);
            Assert.IsTrue(oldGmd.Header.KeyBlockSize < newGmd.Header.KeyBlockSize);
            Assert.IsTrue(oldGmd.Header.StringCount < newGmd.Header.StringCount);
            Assert.IsTrue(oldGmd.Header.StringBlockSize < newGmd.Header.StringBlockSize);
            Assert.IsTrue(newGmd.Entries.ContainsValue("New string text...."));
        }

        [TestMethod]
        public void RemoveString__w_sword_eng()
        {
            string origPath = Utility.GetFullPath(@"chunk0/common/text/steam/w_sword_eng.gmd");
            string newPath = "removestring__w_sword_eng.gmd";
            string readdPath = "readdstring__w_sword_eng.gmd";

            GMD gmd = new GMD(origPath);
            //int idx = gmd.Entries.FindIndex(x => x.Key == "WP_WSWD_044_NAME");
            gmd.RemoveString("WP_WSWD_044_NAME");
            gmd.Save(newPath);

            //GMD newGmd = new GMD(newPath);

            //newGmd.AddString("WP_WSWD_044_NAME", "My new string", idx);
            //newGmd.Save(readdPath);
        }

        [TestMethod]
        public void Shuffle__q00503_eng()
        {
            string newPath = "shuffle__q00503_eng.gmd";
            
            GMD gmd = new GMD(Utility.GetFullPath(@"chunk0/common/text/quest/q00503_eng.gmd"));

            var shuffled = gmd.Entries.OrderBy(x => "A").ToDictionary(item => item.Key, item => item.Value);
            gmd.Entries = shuffled;

            gmd.Save(newPath);
            
            GMD newGmd = new GMD(newPath);
        }
    }
}
