
using System;
using System.Runtime.InteropServices;

namespace MP.CGISettings
{
    [StructLayout(LayoutKind.Explicit, Size = 28 , Pack = 1)]
    internal unsafe struct CGIHEADER
    {
        // The first three bytes identify the header start.
        [FieldOffset(0)]
        public System.Byte Header0;

        [FieldOffset(1)]
        public System.Byte Header1;

        [FieldOffset(2)]
        public System.Byte Header2;

        [FieldOffset(3)]
        public System.Byte RSVD0; // Reserved for the time being...

        [FieldOffset(4)]
        public System.UInt32 Version;

        /// <summary>
        /// Returns the version up to which the entries are defined. 
        /// Detected entries that are larger to this number are considered breaking and the reader must throw an exception.
        /// </summary>
        [FieldOffset(8)]
        public System.Byte EntriesVersion;

        [FieldOffset(9)]
        public CGISettingType Mask;

        [FieldOffset(11)]
        public System.Byte RSVD1; // Reserved for the time being...

        /// <summary>
        /// An LCID identifying the string encoding of all the setting names and values.
        /// </summary>
        [FieldOffset(12)]
        public System.Int32 StringEncoding; 

        /// <summary>
        /// Contains the number of bytes to skip before reading the application name.
        /// </summary>
        [FieldOffset(16)]
        public System.Int32 Padding;

        [FieldOffset(20)]
        public System.Int32 RSVD2; // Reserved for the time being...

        /// <summary>The application name length in bytes.</summary>
        [FieldOffset(24)]
        public System.Int32 AppNameLength;

        public readonly System.Boolean IsValid => Header0 == 67 && Header1 == 71 && Header2 == 73;

        public CGIHEADER()
        {
            RSVD0 = 0;
            RSVD1 = 0;
            RSVD2 = 0;
            Header0 = 67; // C
            Header1 = 71; // G
            Header2 = 73; // I
        }
    }

    /// <summary>
    /// A structure that does contain the setting's core header. <br />
    /// This structure is standard and must exist in all CGI entry versions.
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Size = 16 , Pack = 1)]
    internal unsafe struct CGICORESETTING
    {
        /// <summary>
        /// Defines the type to return when this setting will be read out.
        /// </summary>
        [FieldOffset(0)]
        public CGISettingType SettingType;

        /// <summary>
        /// Defines the setting's value length in bytes.
        /// </summary>
        [FieldOffset(2)]
        public System.Int64 Length;

        /// <summary>
        /// Specifies the setting name length in bytes.
        /// </summary>
        [FieldOffset(10)]
        public System.Int32 NameLength;

        /// <summary>
        /// Contains the number of pad bytes to skip before reading the setting name.
        /// </summary>
        [FieldOffset(14)]
        public System.UInt16 Padding;
    }

    [StructLayout(LayoutKind.Explicit , Size = 24 , Pack = 4)]
    internal struct CGISETTINGV1
    {
        /// <summary>
        /// Defines the version of the current CGI setting. <br />
        /// This allows to not break the format when multiple versions must coexist in a file.
        /// </summary>
        [FieldOffset(0)]
        public System.Byte Version;

        // V1 has 1..3 bytes kept as reserved.

        [FieldOffset(4)]
        public CGICORESETTING CoreHeader;

        // V1 has the last 4 bytes (ranging from 20..24) as reserved.

        public CGISETTINGV1() { Version = 1; }
    }

    [Flags]
    internal enum SettingFlags : System.Byte
    {
        None = 0x00,
        Array = 0x01
    }

    [StructLayout(LayoutKind.Explicit , Size = 24 , Pack = 4)]
    internal struct CGISETTINGV2
    {
        /// <summary>
        /// Defines the version of the current CGI setting. <br />
        /// This allows to not break the format when multiple versions must coexist in a file. <br />
        /// For this structure , this must be set to 2.
        /// </summary>
        [FieldOffset(0)]
        public System.Byte Version;

        /// <summary>
        /// Defines setting flags that modify it's default behavior. <br />
        /// For now only the array flag is defined.
        /// </summary>
        [FieldOffset(1)]
        public SettingFlags Flags;

        // In V2 , the bytes 2..3 do still remain as reserved.

        [FieldOffset(4)]
        public CGICORESETTING CoreHeader;

        /// <summary>
        /// When <see cref="Flags"/> has the flag <see cref="SettingFlags.Array"/> , then this does contain the number of array elements contained in the setting. <br />
        /// Otherwise , this must be zero.
        /// </summary>
        [FieldOffset(20)]
        public System.UInt16 NumberOfElements;

        // V2 has now the last two bytes (ranging 22..24) reserved.

        public CGISETTINGV2() { Version = 2; }
    }
}
