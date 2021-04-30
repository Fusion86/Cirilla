using Cirilla.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;

namespace Cirilla.Core.Test.Tests
{
    [TestClass]
    public class MOD3Tests
    {
        [TestMethod]
        public void Load__hair000()
        {
            MOD3 mod = new MOD3(Utility.GetTestAsset("game/pl/hair/hair000/mod/hair000.mod3"));
            Assert.AreEqual(mod.MeshParts.Count, mod.VertexBuffers.Count, "Probably unrecognized VertexBuffer block type, see output.");
        }

        [TestMethod]
        public void Load__hair103()
        {
            MOD3 mod = new MOD3(Utility.GetTestAsset("game/pl/hair/hair103/mod/hair103.mod3"));
            Assert.AreEqual(mod.MeshParts.Count, mod.VertexBuffers.Count, "Probably unrecognized VertexBuffer block type, see output.");
        }

        [TestMethod]
        public void LoadAllHairStyles()
        {
            int errorCount = 0;
            var hairTypes = new DirectoryInfo(Utility.GetTestAsset("game/pl/hair")).GetDirectories().Select(x => x.Name);

            foreach (var type in hairTypes)
            {
                string fullPath = Path.Combine(Utility.GetTestAsset("game/pl/hair"), type, "mod", type + ".mod3");

                if (!File.Exists(fullPath))
                {
                    Console.WriteLine($"Hairstyle doesn't have a mod3 file, skipping it '{fullPath}'");
                    continue;
                }

                MOD3 mod = new MOD3(fullPath);

                if (mod.MeshParts.Count != mod.VertexBuffers.Count)
                    errorCount++;
            }

            Assert.AreEqual(0, errorCount, "Probably unrecognized VertexBuffer block type, see output.");
        }
    }
}
