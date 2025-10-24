
using System.Runtime.InteropServices;

namespace MP.Serialization.MPSBF
{
    [StructLayout(LayoutKind.Explicit, Pack = 1 , Size = 16)]
    internal struct SBF_ENTRY_HEADER
    {
        [FieldOffset(0)]
        public ushort NameLengthInChars;

        [FieldOffset(2)]
        public ENTRY_TYPE Type;

        [FieldOffset(4)]
        public uint ValueLengthInBytes;

        [FieldOffset(8)]
        public uint RSVD_0;

        [FieldOffset(12)]
        public uint RSVD_1;
    }
}