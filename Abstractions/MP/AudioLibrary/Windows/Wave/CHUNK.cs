using MP.IO.DataStructuring;
using MP.IO.DataStructuring.Generation;

namespace MP.AudioLibrary.Windows.Wave
{
    /// <summary>
    /// Represents a chunk in the wave format data. <br />
    /// See usage in the WaveCodecExtension class.
    /// </summary>
    [DataStructureGenerator]
    public partial struct CHUNK : IDataStructure
    {
        /// <summary>The chunk's ID.</summary>
        [FixedString(4)]
        public System.String ID;

        /// <summary>The size of the chunk without this header.</summary>
        public System.UInt32 Size;
    }
}