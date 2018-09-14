using Cirilla.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cirilla.Core.Test.Tests
{
    [TestClass]
    public class TSSTests
    {
        [TestMethod]
        public void Load__itemData()
        {
            TSS tss = new TSS(Utility.GetFullPath(@"orig_data.bin"));
        }
    }
}
