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

            Assert.AreEqual(gmd.Strings[3], "©CAPCOM CO., LTD. ALL RIGHTS RESERVED.");
        }

        [TestMethod]
        public void Load__action_trial_ara()
        {
            // Uses skipInvalidMessages and weird workaround (see Models/GMD.cs)
            GMD gmd = new GMD(Utility.GetFullPath(@"chunk0\common\text\action_trial_ara.gmd"));

            Assert.AreEqual(gmd.Strings[3], "©CAPCOM CO., LTD. ALL RIGHTS RESERVED.");
        }

        [TestMethod]
        public void Rebuild__em_names_eng()
        {
            string origPath = Utility.GetFullPath(@"chunk0\common\text\em_names_eng.gmd");
            string rebuildPath = "rebuild__em_names_eng.gmd";

            GMD gmd = new GMD(origPath);
            gmd.Save(rebuildPath);

            if (!Utility.CheckFilesAreSame(origPath, rebuildPath))
                Assert.Fail("Hash doesn't match!");
        }

        [TestMethod]
        public void Rebuild__item_eng()
        {
            string origPath = Utility.GetFullPath(@"chunk0\common\text\item_eng.gmd");
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
            string origPath = Utility.GetFullPath(@"chunk0\common\text\action_trial_eng.gmd");
            string rebuildPath = "rebuild__action_trial_eng.gmd";

            GMD gmd = new GMD(origPath);
            gmd.Save(rebuildPath);

            if (!Utility.CheckFilesAreSame(origPath, rebuildPath))
                Assert.Fail("Hash doesn't match!");
        }
    }
}
