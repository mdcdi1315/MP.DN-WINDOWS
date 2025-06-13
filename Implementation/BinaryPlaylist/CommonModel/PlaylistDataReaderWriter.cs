using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MP.BinaryPlaylist
{
    public sealed class PlaylistDataWriter : PlaylistBlobWriter
    {
        private System.UInt32 lastid;

        public PlaylistDataWriter() { lastid = 0; }

        public System.UInt32 WriteByteArray(System.Byte[] bytes , System.Int32 ofs , System.Int32 count)
        {
            Write(count);
            Write(bytes, ofs, count);
            Write(0.ToByte());
            return lastid++;
        }

        public System.UInt32 WriteByteSpan(System.Span<System.Byte> span, System.Int32 ofs, System.Int32 count)
        {
            Write(count);
            Write(span.ToArray(), ofs, count);
            Write(0.ToByte());
            return lastid++;
        }

        public System.UInt32 WriteByteMemory(System.Memory<System.Byte> memory , System.Int32 ofs , System.Int32 count)
        {
            Write(count);
            Write(memory.ToArray(), ofs, count);
            Write(0.ToByte());
            return lastid++;
        }

        /// <summary>
        /// Finalizes the data blob.
        /// </summary>
        public void FinalizeWriter()
        {
            Header = new() { Count = lastid, Identifier1 = BlobFlags.ReadOnly, Identifier2 = BlobTypes.DATABLOB, Version = 1 };
            IsCompleted = true;
        }
    }

    public sealed class PlaylistDataReader : PlaylistBlobReader
    {
        public PlaylistDataReader() : base() { }

        public System.Byte[] GetByteArray(System.UInt32 index)
        {
            if (index >= Header.Count) { throw new ArgumentOutOfRangeException(nameof(index)); }
            System.Int32 marker;
            System.Int32 length;
            Position = 0;
            for (System.UInt32 I = 0; I < Header.Count && I < index; I++)
            {
                length = ReadNumber<System.Int32>();
                Seek(length, System.IO.SeekOrigin.Current);
                marker = ReadByte();
                if (marker != 0) { throw new InvalidByteArrayDataException(); }
            }
            length = ReadNumber<System.Int32>();
            System.Byte[] ret = new System.Byte[length];
            if (Read(ret, 0, length) != length) { throw new System.IO.IOException($"Could not read {length} bytes from the stream."); }
            marker = ReadByte();
            if (marker != 0) { throw new InvalidByteArrayDataException(); }
            return ret;
        }
    }
}
