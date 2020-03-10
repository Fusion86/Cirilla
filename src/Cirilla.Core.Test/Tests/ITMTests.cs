using Cirilla.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cirilla.Core.Test.Tests
{
    [TestClass]
    public class ITMTests
    {
        [TestMethod]
        public void Load__itemData()
        {
            ITM itm = new ITM(Utility.GetFullPath(@"chunk0/common/item/itemData.itm"));
        }
    }
}
