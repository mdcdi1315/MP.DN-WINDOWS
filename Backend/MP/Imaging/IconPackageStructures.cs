
using System.Runtime.InteropServices;

namespace MP.Imaging
{
    /// <summary>
    /// Defines the header of a Windows Icon package.
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Size = 6, Pack = 2)]
    internal struct PACKAGEHEADER
    {
        [FieldOffset(0)]
        public System.UInt16 RSVD; // Must always be zero.

        [FieldOffset(2)]
        public IconImageType DirEntryType;

        [FieldOffset(4)]
        public System.UInt16 DirEntryCount; // # of PACKAGEENTRY structures
    }

    /// <summary>
    /// Defines a single bitmap entry of a Windows Icon package.
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Size = 16)]
    internal struct PACKAGEENTRY
    {
        [StructLayout(LayoutKind.Explicit, Size = 4, Pack = 2)]
        public struct ICON
        {
            [FieldOffset(0)]
            public System.UInt16 ColorPlanes;

            [FieldOffset(2)]
            public System.UInt16 BitsPerPixel;
        }

        [StructLayout(LayoutKind.Explicit, Size = 4, Pack = 2)]
        public struct CURSOR
        {
            [FieldOffset(0)]
            public System.UInt16 XCoordinate;

            [FieldOffset(2)]
            public System.UInt16 YCoordinate;
        }

        [FieldOffset(0)]
        public System.Byte Width;

        [FieldOffset(1)]
        public System.Byte Height;

        [FieldOffset(2)]
        public System.Byte ColorPalleteColors;

        [FieldOffset(3)]
        public System.Byte RSVD; // Must always be zero.

        // For the two below fields, which is valid is dependent of the value obtained by PACKAGEHEADER.DirEntryType field
        [FieldOffset(4)]
        public ICON IconEntry;

        [FieldOffset(4)]
        public CURSOR CursorEntry;

        [FieldOffset(8)]
        public System.UInt32 ImageSize; // The entire bitmap size

        [FieldOffset(12)]
        public System.UInt32 ImageOffset; // Offset inside the icon file
    }

    internal enum IconImageType : System.UInt16
    {
        ICON = 1,
        CURSOR = 2
    }
}