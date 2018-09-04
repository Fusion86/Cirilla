using System.IO;

namespace Cirilla.Core.Test
{
    public static class Utility
    {
        public static string GetFullPath(string path) => Path.Join(Settings.MHWExtractedDataRoot, path);
    }
}
