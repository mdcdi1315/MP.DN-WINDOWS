
using System;

namespace MP.AudioLibrary.Windows.Wave
{
    /// <summary>
    /// Extended and default data structure for RIFF chunks. <br />
    /// Extended to <see cref="long"/> bounds for diverse and derived RIFF formats.
    /// </summary>
    public readonly struct ChunkInfo
    {
        /// <summary>
        /// The ID of this chunk. 
        /// It is four-character code.
        /// </summary>
        public readonly string ID;

        /// <summary>
        /// The length of the chunk.
        /// </summary>
        public readonly System.Int64 Length;

        /// <summary>
        /// Constructs a <see cref="ChunkInfo"/> from the specified <see cref="CHUNK"/>.
        /// </summary>
        /// <param name="ch">The <see cref="CHUNK"/> to construct this <see cref="ChunkInfo"/> from.</param>
        public ChunkInfo(CHUNK ch)
        {
            ID = ch.ID;
            Length = ch.Size;
        }

        /// <summary>
        /// Constructs a <see cref="ChunkInfo"/> from the specified id and length.
        /// </summary>
        /// <param name="id">The ID of the newly created chunk information.</param>
        /// <param name="length">The length of the newly created chunk information.</param>
        public ChunkInfo(string id, long length)
        {
            ArgumentNullException.ThrowIfNull(id);
            ID = id;
            Length = length;
        }
    }
}