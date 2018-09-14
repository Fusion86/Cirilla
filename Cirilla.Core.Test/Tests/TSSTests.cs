using Cirilla.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cirilla.Core.Test.Tests
{
    [TestClass]
    public class TSSTests
    {
        [TestMethod]
        public void Load__orig_data()
        {
            TSS tss = new TSS(@"orig_data.bin");
        }

        [TestMethod]
        public void Load__data()
        {
            TSS tss = new TSS(Utility.GetGameDirectoryPath(@"tss/data.bin"));
        }
    }
}
