using Newtonsoft.Json;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FaceEditPalette
{
    class Program
    {
        static void Main()
        {
            List<Rgba32> colors = new();

            using (var fs = File.OpenRead(@"L:\Sync\MHW Mods\chunks_v0\chunk\common\face_edit\face_edit_palette.pal"))
            using (BinaryReader br = new(fs))
            {
                char[] magic = br.ReadChars(3);
                fs.Position += 5; // Padding

                while (fs.Position < fs.Length)
                {
                    byte[] b = br.ReadBytes(4);
                    colors.Add(new Rgba32(b[0], b[1], b[2], b[3]));
                }
            }

            int width = 500;
            int rowHeight = 100;
            int height = rowHeight * colors.Count;
            using (var img = new Image<Rgba32>(width, height))
            {
                var font = SystemFonts.CreateFont("Arial", 24);

                img.Mutate(gfx =>
                {
                    for (int i = 0; i < colors.Count; i++)
                    {
                        int y = i * rowHeight;
                        gfx.Fill(colors[i], new Rectangle(0, y, width, rowHeight));
                        gfx.DrawText(colors[i].ToHex(), font, Rgba32.White, new PointF(10, y + 20));
                    }
                });

                img.Save("face_edit_palette.png");
            }

            var jsonColors = colors.Select(x => x.ToHex());
            var json = JsonConvert.SerializeObject(jsonColors);
            File.WriteAllText("palette.json", json);
        }
    }
}
