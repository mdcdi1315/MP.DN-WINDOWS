
using System;
using System.Runtime.InteropServices;

namespace MP.TagReading.ID3
{
    [Flags] // Reversed bit flags.
    public enum ID3V2Flags : System.Byte
    {
        None = 0,
        Unsynchronisation = 1,
        ExtendedHeader = 2,
        Experimental = 4,
        Footer = 8
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 10)]
    public struct ID3V2HEADER
    {
        [FieldOffset(0)]
        public System.Byte H0; // = 0x49

        [FieldOffset(1)]
        public System.Byte H1; // = 0x44 

        [FieldOffset(2)]
        public System.Byte H2; // = 0x33

        [FieldOffset(3)]
        public System.Byte VersionMajor;

        [FieldOffset(4)]
        public System.Byte VersionMinor;

        [FieldOffset(5)]
        public ID3V2Flags Flags;

        [FieldOffset(6)]
        public SYNCHSAFEINT Length;

        public System.Boolean IsValid => H0 == 0x49 && H1 == 0x44 && H2 == 0x33;
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 6)]
    public struct ID3V2EXTENDEDHEADER
    {
        [FieldOffset(0)]
        public SYNCHSAFEINT Length;

        [FieldOffset(4)]
        public System.Byte NFlagBytes;

        [FieldOffset(5)]
        public System.Byte Flags;
    }
}