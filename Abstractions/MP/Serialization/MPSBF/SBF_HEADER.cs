using MP.IO.DataStructuring;
using MP.IO.DataStructuring.Generation;

namespace MP.Serialization.MPSBF
{
    [DataStructureGenerator]
    internal partial struct SBF_HEADER : IDataStructure
    {
        public byte Header_0;

        public byte Header_1;

        public byte Header_2;

        public byte RSVD_0;

        public ushort VersionCode;

        public readonly System.Boolean IsValid => Header_0 == 'S' && Header_1 == 'B' && Header_2 == 'F';

        public SBF_HEADER()
        {
            Header_0 = (byte)'S';
            Header_1 = (byte)'B';
            Header_2 = (byte)'F';
            VersionCode = 0;
        }
    }
}