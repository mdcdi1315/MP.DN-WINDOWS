
using System.Runtime.InteropServices;

namespace MP.TagReading.MP4
{
    [StructLayout(LayoutKind.Explicit)]
    internal unsafe struct MP4HEADER
    {
        [FieldOffset(0)]
        public fixed System.Byte SIZE[sizeof(System.UInt32)];

        [FieldOffset(sizeof(System.UInt32))]
        public fixed System.Byte FTYPHEADER[sizeof(System.UInt32)];

        [FieldOffset(sizeof(System.UInt32) * 2)]
        public fixed System.Byte BRAND[sizeof(System.UInt32)];

        [FieldOffset(sizeof(System.UInt32) * 3)]
        public fixed System.Byte REVISION[sizeof(System.UInt32)];

        public System.Boolean IsCorrectHeader => ReaderUtils.ReadASCIIHeaderString(ref FTYPHEADER[0]) == "ftyp";

        public System.String Brand => ReaderUtils.ReadASCIIHeaderString(ref BRAND[0]);

        public System.UInt32 Revision => ReaderUtils.ReadUInt32BigEndian(ref REVISION[0]);

        public System.UInt32 HeaderSize => ReaderUtils.ReadUInt32BigEndian(ref SIZE[0]);

        public System.UInt32 RestHeaderSize => ReaderUtils.ReadUInt32BigEndian(ref SIZE[0]) - sizeof(MP4HEADER).ToUInt32();
    }
}