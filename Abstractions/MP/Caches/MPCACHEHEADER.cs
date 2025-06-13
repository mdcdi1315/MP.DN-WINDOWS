
using System.Runtime.InteropServices;

namespace MP.Caches
{
    [StructLayout(LayoutKind.Explicit , Pack = 1)]
    internal struct MPCACHEHEADER
    {
        // Header fields
        [FieldOffset(0)]
        public System.Byte ID0;

        [FieldOffset(1)]
        public System.Byte ID1;

        [FieldOffset(2)]
        public System.Byte ID2;

        [FieldOffset(3)]
        public System.Byte RSVD; // Set to zero.

        [FieldOffset(4)] // MPCACHEHEADER structure version
        public System.UInt16 CacheHeaderVersion;

        // Format-specific fields
        [FieldOffset(6)] 
        public System.UInt16 FormatVersion;

        [FieldOffset(8)]
        public System.Int64 FormatIdentifier;

        // ID Header forms the string 'MPC'.
        public MPCACHEHEADER() {
            ID0 = 77;
            ID1 = 80;
            ID2 = 67;
            RSVD = 0;
            CacheHeaderVersion = 1;
        }

        public readonly System.Boolean IsValid => ID0 == 77 && ID1 == 80 && ID2 == 67;
    }
}