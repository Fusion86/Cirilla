using Cirilla.Core.Extensions;
using Cirilla.Core.Logging;
using Cirilla.Core.Structs.Native;
using System.Collections.Generic;
using System.IO;

namespace Cirilla.Core.Models
{
    public class ITM : FileTypeBase
    {
        private static readonly ILog Logger = LogProvider.GetCurrentClassLogger();

        public byte[] Header;
        public List<ITM_Item> Items;

        public ITM(string path) : base(path)
        {
            Logger.Info($"Loading '{path}'");

            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (BinaryReader br = new BinaryReader(fs))
            {
                Header = br.ReadBytes(6);
                Items = new List<ITM_Item>(1000);

                while (fs.Position != fs.Length)
                {
                    Items.Add(br.ReadStruct<ITM_Item>());
                }

                Logger.Info($"Loaded {Items.Count} items");
            }
        }
    }
}
