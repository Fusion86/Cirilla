using System.IO;
using Cirilla.Core.Extensions;
using Cirilla.Core.Logging;
using Cirilla.Core.Structs.Native;

namespace Cirilla.Core.Models
{
    // See https://pastebin.com/raw/xhAr1YvL
    public class TSS : FileTypeBase
    {
        private static readonly ILog Logger = LogProvider.GetCurrentClassLogger();

        public TSS_Header Header => _header;
        
        private TSS_Header _header;

        public TSS(string path) : base(path)
        {
            Logger.Info($"Loading '{path}'");
            
            // This code is not optimized, it creates a lot of duplicate arrays etc

            byte[] bytes = File.ReadAllBytes(path);
            DecryptData(ref bytes);
            
            // Header
            byte[] tmpBytes = bytes.SubArray(0, 0x14); // 0x0C = sizeof TSS_Header
            _header = tmpBytes.ToStruct<TSS_Header>();
        }
        
        private static void DecryptData(ref byte[] arr)
        {
            byte key = 0x0A; // IV
            for (int p = 0x0C; p < arr.Length; p++)
            {
                byte tmp = arr[p];
                arr[p] ^= key;
                key = tmp;
            }
        }
    }
}