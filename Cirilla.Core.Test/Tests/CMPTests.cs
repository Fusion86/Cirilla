using Cirilla.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cirilla.Core.Test.Tests
{
    [TestClass]
    public class CMPTests
    {
        [TestMethod]
        public void Load__preset_female_1()
        {
            CMP cmp = new CMP(Utility.GetFullPath(@"chunk0/stage/st407/common/preset/preset_female_1.cmp"));
        }

        [TestMethod]
        public void Load__preset_male_1()
        {
            CMP cmp = new CMP(Utility.GetFullPath(@"chunk0/stage/st407/common/preset/preset_male_1.cmp"));
        }
    }
}
