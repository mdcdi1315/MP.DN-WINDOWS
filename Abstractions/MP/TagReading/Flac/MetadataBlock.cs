

namespace MP.TagReading.Flac
{
    /// <summary>
    /// Represents a single FLAC metadata block. <br />
    /// Metadata blocks are the way for FLAC files to store their information.
    /// </summary>
    public struct MetadataBlock
    {
        /// <summary>
        /// Reads a new <see cref="MetadataBlock"/> structure from four bytes.
        /// </summary>
        /// <param name="data">The four bytes to read from the array and construct a new <see cref="MetadataBlock"/> structure.</param>
        /// <returns>A new instance of the <see cref="MetadataBlock"/> structure.</returns>
        public static MetadataBlock ReadFromBytes(byte[] data)
        {
            MetadataBlock block = new();
            if (data is null) { return block; }
            if (data.Length < SharedConstants.MetadataHeaderBytes) { return block; }
            // The data in the first byte do contain the Type and IsLast members...
            int I = SharedConstants.TypeMemberBits;
            block.Type = (MetadataType)data.ReadBitLevelNumber(0, 7);
            block.IsLast = data[0].GetBit(7);
            I += SharedConstants.IsLastMemberBits;
            bool bit;
            int start = I;
            byte[] lentemp = new byte[sizeof(uint)];
            for (; I < start + SharedConstants.LengthMemberBits; I++)
            {
                int shift = I >> 3, index = I % 8;
                bit = data[shift].GetBit(index);
                if (bit) { lentemp[shift].SetBit(index, true); }
            }
            if (System.BitConverter.IsLittleEndian) { lentemp.Reverse(); }
            block.Length = lentemp.ToUInt32(0);
            return block;
        }

        /// <summary>
        /// Reads four bytes from the stream, then it passes these to the <see cref="ReadFromBytes"/> method to read a <see cref="MetadataBlock"/> structure.
        /// </summary>
        /// <param name="stream">The stream to read the metadata block from.</param>
        /// <returns>A new instance of the <see cref="MetadataBlock"/> structure.</returns>
        /// <exception cref="System.IO.IOException">Cannot read four bytes from the stream.</exception>
        public static MetadataBlock ReadFromStream(System.IO.Stream stream)
        {
            byte[] data = new byte[SharedConstants.MetadataHeaderBytes];
            if (stream.Read(data, 0, SharedConstants.MetadataHeaderBytes) < SharedConstants.MetadataHeaderBytes)
            { throw new System.IO.IOException($"Could not read {SharedConstants.MetadataHeaderBytes} bytes from the stream."); }
            return ReadFromBytes(data);
        }

        /// <summary>
        /// Gets the type of this metadata block. <br />
        /// Valid values for this member are defined in the <see cref="MetadataType"/> enumeration.
        /// </summary>
        public MetadataType Type;
        /// <summary>
        /// Gets a value whether this metadata block was the last block read from the FLAC stream.
        /// </summary>
        public bool IsLast;
        /// <summary>
        /// Gets the length of this metadata block. <br />
        /// Note, however, that the block length cannot reach up to 4 GB in length; <br />
        /// In fact, this value is stored as three bytes, and thus it can reach up to 16777215 bytes which is around 15 MB only??
        /// </summary>
        public uint Length;
        /// <summary>
        /// This member does not exist in the native structure. <br />
        /// It is just used to track the stream offset where the data begin.
        /// </summary>
        public long StreamOffset;
    }
}