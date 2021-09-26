using Cirilla.Core.Extensions;
using Cirilla.Core.Helpers;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;

using ColorDict = System.Collections.Generic.Dictionary<string, System.Drawing.Color>;

namespace GstTool
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct GstHeader
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U1)]
        public char[] Magic;

        public uint Version;
        public ulong FileSize;
        public uint Unk1;
        public ulong Unk2;
        public uint EntryCount;
        public ulong InfoBlockOffset;
        public ulong StringBlockOffset;
    }

    struct GstInfoEntry
    {
        public uint Index;
        public uint Unk1;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U1)]
        public byte[] Color;

        public uint Zero1;
        public uint Offset;
        public uint Zero2;
    }

    class Program
    {
        static void Main()
        {
            var files = Directory.GetFiles(@"L:\Monster Hunter World Assets\ui\font", "*.gst");
            var export = new Dictionary<string, ColorDict>();

            foreach (var file in files)
            {
                using FileStream fs = new(file, FileMode.Open, FileAccess.Read, FileShare.Read);
                using BinaryReader br = new(fs);

                var header = br.ReadStruct<GstHeader>();
                var infos = new GstInfoEntry[header.EntryCount];

                for (int i = 0; i < header.EntryCount; i++)
                    infos[i] = br.ReadStruct<GstInfoEntry>();

                var colors = new ColorDict();

                for (int i = 0; i < header.EntryCount; i++)
                {
                    fs.Position = (long)(header.StringBlockOffset + infos[i].Offset);
                    var colorName = br.ReadStringZero(ExEncoding.UTF8);
                    var color = Color.FromArgb(
                        infos[i].Color[3],
                        infos[i].Color[0],
                        infos[i].Color[1],
                        infos[i].Color[2]);

                    colors.Add(colorName, color);
                }

                export.Add(Path.GetFileNameWithoutExtension(file), colors);
            }

            var json = JsonConvert.SerializeObject(export, Formatting.Indented);
            File.WriteAllText("colors.json", json);
        }
    }
}
