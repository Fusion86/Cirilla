using System.IO;

namespace Cirilla.Benchmark
{
    public static class Utility
    {
        public static string GetFullPath(string path) => Path.Join(Settings.MHWExtractedDataRoot, path);
    }
}
