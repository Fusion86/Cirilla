using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace Cirilla.Core.Test
{
    public static class Utility
    {
        public static string GetFullPath(string path) => Path.Join(Settings.MHWExtractedDataRoot, path);

        public static bool CheckFilesAreSame(string path1, string path2)
        {
            using (HashAlgorithm hashAlgorithm = SHA256.Create())
            using (FileStream origFs = new FileStream(path1, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (FileStream rebuildFs = new FileStream(path2, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                byte[] origHash = hashAlgorithm.ComputeHash(origFs);
                byte[] rebuildHash = hashAlgorithm.ComputeHash(rebuildFs);

                return origHash.SequenceEqual(rebuildHash);
            }
        }
    }
}
