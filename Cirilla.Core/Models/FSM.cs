using System;
using Cirilla.Core.Extensions;
using Cirilla.Core.Logging;
using Cirilla.Core.Structs.Native;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Cirilla.Core.Enums;

namespace Cirilla.Core.Models
{
    public class FSM : FileTypeBase
    {
        private static readonly ILog Logger = LogProvider.GetCurrentClassLogger();

        public FSM_Header Header;
        //public List<long> StructOffsets;
        public List<FSM_Struct> Structs;
        //public List<string[]> Objects;

        public FSM(string path) : base(path)
        {
            Logger.Info($"Loading '{path}'");

            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (BinaryReader br = new BinaryReader(fs))
            {
                // Header
                Header = br.ReadStruct<FSM_Header>();

                //long posAfterHeader = fs.Position; // Should be 0x18 (24)

                // Struct offsets - just read offsets sequential, we could also load the full structs but then we'd just jump all over the memory for no reason
                Structs = new List<FSM_Struct>(Header.StructCount);
                for (int i = 0; i < Header.StructCount; i++)
                {
                    Structs.Add(new FSM_Struct { Offset = br.ReadInt64() });
                }

                // Structs - load structs here
                for (int i = 0; i < Header.StructCount; i++)
                {
                    Structs[i].Header = br.ReadStruct<FSM_StructHeader>();
                    Structs[i].VariableEntries = new FSM_VariableEntry[Structs[i].Header.VariableCount];
                    for (int j = 0; j < Structs[i].Header.VariableCount; j++)
                        Structs[i].VariableEntries[j] = br.ReadStruct<FSM_VariableEntry>();
                }

                // Struct variables
                Encoding enc = Encoding.GetEncoding("UTF-8", EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback); // Encoding that throws errors

                for (int i = 0; i < Header.StructCount; i++)
                {
                    Logger.Debug($"Struct {i}: _name_");

                    //fs.Position = 0x18 + Structs[i].VariableEntries[j].NameOffset; // Disabled because we set the fs.Position inside the for(j) loop
                    Structs[i].VariableNames = new string[Structs[i].Header.VariableCount];
                    for (int j = 0; j < Structs[i].Header.VariableCount; j++)
                    {
                        // This is probably not needed because member names usually are sequential in memory
                        // but just to be safe we'll just do it the 100% correct way
                        fs.Position = 0x18 + Structs[i].VariableEntries[j].NameOffset;

                        Structs[i].VariableNames[j] = br.ReadStringZero(Encoding.UTF8);

                        var entry = Structs[i].VariableEntries[j];

                        if (Logger.IsDebugEnabled())
                        {
                            string typeName;
                            if (Enum.IsDefined(typeof(FsmVariableType), entry.Type))
                                typeName = Enum.GetName(typeof(FsmVariableType), entry.Type);
                            else
                                typeName = "type_" + entry.Type;

                            Logger.Debug(
                                $"  {typeName.PadRight(10)} {Structs[i].VariableNames[j].PadRight(40)} size: {entry.Size}");
                        }
                    }
                }
            }
        }

        public class FSM_Struct
        {
            public long Offset; // Offset after FSM_Header
            public FSM_StructHeader Header;
            public FSM_VariableEntry[] VariableEntries;
            public string[] VariableNames;
        }
    }
}
