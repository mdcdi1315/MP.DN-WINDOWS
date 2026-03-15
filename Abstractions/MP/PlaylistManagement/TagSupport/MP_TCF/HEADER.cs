
using System.Runtime.InteropServices;

namespace MP.PlaylistManagement.TagSupport.MP_TCF
{
    [StructLayout(
        LayoutKind.Explicit,
        Pack = sizeof(byte) * 4, 
        Size = (sizeof(byte) * 4) + (sizeof(ushort) * 2)
    )]
    internal struct HEADER
    {
        [FieldOffset(0)]
        public byte HDR_0;

        [FieldOffset(sizeof(byte))]
        public byte HDR_1;

        [FieldOffset(sizeof(byte) * 2)]
        public byte HDR_2;

        [FieldOffset(sizeof(byte) * 3)]
        public byte RSVD_0;

        [FieldOffset(sizeof(byte) * 4)]
        public ushort VERSION;

        [FieldOffset((sizeof(byte) * 4) + sizeof(ushort))]
        public ushort RSVD_1;

        public HEADER()
        {
            HDR_0 = (byte)'T';
            HDR_1 = (byte)'C';
            HDR_2 = (byte)'F';
            VERSION = 0;
        }
    }
}