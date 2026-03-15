
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MP.TagReading.MP4
{
    [StructLayout(LayoutKind.Explicit , Pack = 1 , Size = 16)]
    internal unsafe struct MP4BOXHEADER
    {
        [FieldOffset(sizeof(System.UInt32))]
        private System.Byte pin;

        [FieldOffset(0)]
        public fixed System.Byte SIZE[sizeof(System.UInt32)];

        [FieldOffset(sizeof(System.UInt32))]
        public fixed System.Byte TYPE[4];

        [FieldOffset(4 + sizeof(System.UInt32))]
        public fixed System.Byte DATALEN[sizeof(System.Int64)];

        public System.String Type
        {
            get {
                System.Boolean IsThreeByte = TYPE[0] == 0xA9;
                fixed (System.Byte* p = &TYPE[IsThreeByte ? 1 : 0])
                {
                    return new((System.SByte*)p, 0, IsThreeByte ? 3 : 4);
                }
            }
        }

        public System.UInt32 Size
        {
            get => ReaderUtils.ReadUInt32BigEndian(ref SIZE[0]);
            set {
                System.Byte[] dt = value.GetBytes();
                if (BitConverter.IsLittleEndian) { dt.Reverse(); }
                Unsafe.CopyBlockUnaligned(ref SIZE[0], ref dt[0], sizeof(System.UInt32));
            }
        }

        public System.Int64 LongSize => ReaderUtils.ReadInt64BigEndian(ref DATALEN[0]);
        
        public static (MP4BOXHEADER , System.Int32 factualsize) ReadHeader(IO.DataStream stream)
        {
            System.UInt32 sizef = stream.ReadBoxUInt32Normal();
            System.Int32 size = sizeof(MP4BOXHEADER) - sizeof(System.UInt32);
            if (sizef != 1)
            {
                size -= sizeof(System.Int64);
            }
            System.Byte[] dt = stream.ReadBytes(size);
            MP4BOXHEADER header = new();
            Unsafe.CopyBlockUnaligned(ref Unsafe.AsRef(in header.pin), ref dt[0], size.ToUInt32());
            header.Size = sizef;
            return (header, size + sizeof(System.UInt32));
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    internal unsafe struct MP4METABOXCOREHEADER
    {
        [FieldOffset(0)]
        public fixed System.Byte SIZE[sizeof(System.UInt32)];

        [FieldOffset(sizeof(System.UInt32))]
        public fixed System.Byte Header[4];

        [FieldOffset(sizeof(System.UInt32) + 4)]
        public System.Byte Version;

        [FieldOffset(sizeof(System.UInt32) + 5)]
        private fixed System.Byte flags[3];

        public AppleDataTagFlags Flags
        {
            get
            {
                System.Byte[] dt = new System.Byte[4];
                dt[0] = 0;
                Unsafe.CopyBlockUnaligned(ref dt[1], ref flags[0], 3);
                if (BitConverter.IsLittleEndian) { dt.Reverse(); }
                return Unsafe.ReadUnaligned<AppleDataTagFlags>(ref dt[0]);
            }
            set
            {
                System.Byte[] dt = ((System.Int32)value).GetBytes();
                if (BitConverter.IsLittleEndian) { dt.Reverse(); }
                Unsafe.CopyBlockUnaligned(ref flags[0], ref dt[1], 3);
            }
        }

        public System.Boolean IsDataHeader => ReaderUtils.ReadASCIIHeaderString(ref Header[0]) == "data";

        public System.String HeaderId => ReaderUtils.ReadASCIIHeaderString(ref Header[0]);

        public System.UInt32 DataSize => ReaderUtils.ReadUInt32BigEndian(ref SIZE[0]) - sizeof(MP4METABOXCOREHEADER).ToUInt32();
    }

    // For doc reasons only.
    [StructLayout(LayoutKind.Explicit)]
    internal unsafe struct MP4METABOXDATAHEADER
    {
        [FieldOffset(0)]
        public MP4METABOXCOREHEADER CoreHeader;

        [FieldOffset(12)]
        public System.UInt32 RSVD;
    }

    // For doc reasons only.
    [StructLayout(LayoutKind.Explicit)]
    internal unsafe struct MP4METABOXOTHERHEADER
    {
        [FieldOffset(0)]
        public MP4METABOXCOREHEADER CoreHeader;
    }
}