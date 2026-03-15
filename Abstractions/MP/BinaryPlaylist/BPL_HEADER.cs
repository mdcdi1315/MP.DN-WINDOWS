
using MP.IO.DataStructuring;
using MP.IO.DataStructuring.Generation;

namespace MP.BinaryPlaylist
{
    [DataStructureGenerator]
    internal partial struct BPL_HEADER : IDataStructure
    {
        [FixedString(3, StringFieldEncoding.ASCII)]
        public System.String ID;

        public System.Byte RSVD_0;

        public System.UInt16 Version;

        /// <summary>
        /// Gets a value whether this structure instance validly represents a MPBPL Format Header.
        /// </summary>
        public readonly System.Boolean IsValidHeader => ID == "BPL";

        public BPL_HEADER()
        {
            ID = "BPL";
            Version = 2;
        }
    }
}