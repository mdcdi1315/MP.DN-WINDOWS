
using System.Runtime.InteropServices;

namespace MP.PlaylistManagement.TagSupport.MP_TCF
{
    [StructLayout(
        LayoutKind.Explicit, 
        Pack = sizeof(ushort), 
        Size = sizeof(STRING_ENCODING) + (sizeof(ushort) * 3) + sizeof(uint)
    )]
    internal struct ID3V2CMTBLOCK
    {
        [FieldOffset(0)]
        public STRING_ENCODING Encoding;

        [FieldOffset(sizeof(STRING_ENCODING))]
        public ushort RSVD;

        [FieldOffset(sizeof(STRING_ENCODING) + sizeof(ushort))]
        public ushort LanguageLength;

        [FieldOffset(sizeof(STRING_ENCODING) + (sizeof(ushort) * 2))]
        public ushort ContentDescriptionLength;

        [FieldOffset(sizeof(STRING_ENCODING) + (sizeof(ushort) * 3))]
        public uint TextLength;
    }
}