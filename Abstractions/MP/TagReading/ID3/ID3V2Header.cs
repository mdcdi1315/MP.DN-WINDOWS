using System.Runtime.InteropServices;

namespace MP.TagReading.ID3
{
    /// <summary>
    /// Defines the starting header of an ID3V2 tag.
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 10)]
    public struct ID3V2HEADER
    {
        /// <summary>The first byte of the header. Identifies that this is an ID3V2 tag.</summary>
        [FieldOffset(0)]
        public System.Byte H0; // = 0x49

        /// <summary>The second byte of the header. Identifies that this is an ID3V2 tag.</summary>
        [FieldOffset(1)]
        public System.Byte H1; // = 0x44 

        /// <summary>The third byte of the header. Identifies that this is an ID3V2 tag.</summary>
        [FieldOffset(2)]
        public System.Byte H2; // = 0x33

        /// <summary>
        /// The minor revision of the tag i.e. for version 2.3 this will be 3.
        /// </summary>
        [FieldOffset(3)]
        public System.Byte VersionMajor;

        /// <summary>
        /// The build revision of the tag i.e. for version 2.3.1 this will be 1. 0 means that this version number is absent.
        /// </summary>
        [FieldOffset(4)]
        public System.Byte VersionMinor;

        /// <summary>
        /// Additional reader and writer behavioral flags.
        /// </summary>
        [FieldOffset(5)]
        public ID3V2Flags Flags;

        /// <summary>
        /// The total length, in bytes, of the tag.
        /// </summary>
        [FieldOffset(6)]
        public SYNCHSAFEINT Length;

        /// <summary>
        /// Gets a value whether the current tag is valid.
        /// </summary>
        public System.Boolean IsValid => H0 == 0x49 && H1 == 0x44 && H2 == 0x33;
    }

    /// <summary>
    /// Extended header of the tag after <see cref="ID3V2HEADER"/> is specified, when <see cref="ID3V2Flags.ExtendedHeader"/> flag is defined.
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 6)]
    public struct ID3V2EXTENDEDHEADER
    {
        /// <summary>
        /// The total length of the extended header, in bytes.
        /// </summary>
        [FieldOffset(0)]
        public SYNCHSAFEINT Length;

        /// <summary></summary>
        [FieldOffset(4)]
        public System.Byte NFlagBytes;

        /// <summary></summary>
        [FieldOffset(5)]
        public System.Byte Flags;
    }
}