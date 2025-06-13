
using System.Runtime.InteropServices;

namespace MP.Archiving
{
    /// <summary>
    /// Defines the startup header for Music Player file archives.
    /// </summary>
    [StructLayout(LayoutKind.Explicit , Pack = 4)]
    internal struct MPARCHHEADER
    {
        // ID0..ID5 archive header bytes.
        [FieldOffset(0)]
        public System.Byte ID0;

        [FieldOffset(1)]
        public System.Byte ID1;

        [FieldOffset(2)]
        public System.Byte ID2;
        
        [FieldOffset(3)]
        public System.Byte ID3;

        [FieldOffset(4)]
        public System.Byte ID4;

        [FieldOffset(5)]
        public System.Byte ID5;
        
        // Structural version of this archive file.
        // For V1 , this is set to 1.
        [FieldOffset(6)]
        public System.Byte Version;

        // Defines the byte order under which all byte-order dependent 
        // numbers are defined as. Not used by MPARCHUINT.
        [FieldOffset(7)]
        public ArchiveByteOrder ByteOrder;

        // Reserved field, might be used in the future versions of this structure. 
        [FieldOffset(8)]
        public System.UInt32 RSVD0;

        // Reserved field, might be used in the future versions of this structure.
        [FieldOffset(12)]
        public System.UInt32 RSVD1;

        public MPARCHHEADER()
        {
            RSVD0 = 0;
            RSVD1 = 0;
            Version = 1;
            ByteOrder = ArchiveByteOrder.LittleEndian;
            ID0 = 77;
            ID1 = 80;
            ID2 = 65;
            ID3 = 82;
            ID4 = 67;
            ID5 = 72;
        }

        public System.Boolean IsValid => ID0 == 77 && ID1 == 80 && ID2 == 65 && ID3 == 82 && ID4 == 67 && ID5 == 72;
    }
}