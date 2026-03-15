
using System.Runtime.InteropServices;

namespace MP.PlaylistManagement.TagSupport.MP_TCF
{
    [StructLayout(
        LayoutKind.Explicit, 
        Pack = sizeof(ushort) + sizeof(STRING_ENCODING), 
        Size = (sizeof(ushort) * 2) + sizeof(ENTRY_FLAGS) + sizeof(STRING_ENCODING)
    )]
    internal struct ENTRY
    {
        // There is also an implicit member at the beginning of this structure named as Version, and it is ushort as well.
        // Currently, the value of that field is 0.

        [FieldOffset(0)]
        public ushort NameLength;

        [FieldOffset(sizeof(ushort))]
        public STRING_ENCODING NameEncoding;

        [FieldOffset(sizeof(ushort) + sizeof(STRING_ENCODING))]
        public uint ValueLength;

        [FieldOffset(sizeof(ushort) + sizeof(STRING_ENCODING))]
        public ENTRY_FLAGS Flags;
    }
}