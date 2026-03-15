
using MP.IO.DataStructuring;
using MP.IO.DataStructuring.Generation;

namespace MP.Serialization.MPSBF
{
    [DataStructureGenerator]
    internal partial struct SBF_ENTRY_HEADER : IDataStructure
    {
        public ushort NameLengthInChars;

        public ENTRY_TYPE Type;

        public uint ValueLengthInBytes;

        public uint RSVD_0;

        public uint RSVD_1;

        [VariableLengthString(nameof(NameLengthInChars), StringFieldEncoding.UTF16LE)]
        public string Name;
    }
}