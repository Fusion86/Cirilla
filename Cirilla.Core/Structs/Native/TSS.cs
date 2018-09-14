using Cirilla.Core.Helpers;
using System.Runtime.InteropServices;

namespace Cirilla.Core.Structs.Native
{
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct TSS_Header
    {
        [Endian(Endianness.LittleEndian)]
        public int Checksum; // Possibly?
        
        [Endian(Endianness.LittleEndian)]
        public int Unk1;
        
        [Endian(Endianness.LittleEndian)]
        public int DataSize;
        
        [Endian(Endianness.LittleEndian)]
        public int DataOffset;
        
        // 0x10
        
        [Endian(Endianness.LittleEndian)]
        public int BlockCount;
        
        // 0x14
    }
}