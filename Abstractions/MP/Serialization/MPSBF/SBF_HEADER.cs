

using System.Runtime.InteropServices;

namespace MP.Serialization.MPSBF
{
    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 6)]
    internal struct SBF_HEADER
    {
        [FieldOffset(0)]
        public byte Header_0;

        [FieldOffset(1)]
        public byte Header_1;

        [FieldOffset(2)]
        public byte Header_2;

        [FieldOffset(3)]
        public byte RSVD_0;

        [FieldOffset(4)]
        public ushort VersionCode;

        public readonly System.Boolean IsValid => Header_0 == 'S' && Header_1 == 'B' && Header_2 == 'F';

        public SBF_HEADER()
        {
            Header_0 = (byte)'S';
            Header_1 = (byte)'B';
            Header_2 = (byte)'F';
            VersionCode = 0;
        }
    }
}