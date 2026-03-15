
using MP.IO.DataStructuring;
using MP.IO.DataStructuring.Generation;

namespace MP.AudioLibrary.Windows.Wave
{
    [DataStructureGenerator]
    internal partial struct RF64_DS64 : IDataStructure
    {
        public System.UInt64 RIFFSize;
        public System.UInt64 DataSize;
        public System.UInt64 FactSize;
        public System.UInt32 AdditionalChunksLength;
    }

    [DataStructureGenerator]
    internal partial struct RF64_CHUNK64 : IDataStructure
    {
        [FixedString(4)]
        public System.String ID;

        public System.UInt64 Length;
    }
}