

using System;
using System.IO;

namespace MP.BinaryPlaylist
{
    internal sealed class PlaylistBlobReaderAdapterStream : Stream
    {
        private readonly Func<Byte[], Int32, Int32, Int32> ReadFunction;
        private readonly Func<Int64, SeekOrigin, Int64> SeekFunction;
        private readonly Func<Int64> PositionProperty;
        private readonly Func<Int64> LengthProperty;

        public PlaylistBlobReaderAdapterStream(
            Func<Byte[], Int32, Int32, Int32> read,
            Func<Int64, SeekOrigin, Int64> seek,
            Func<Int64> pos,
            Func<Int64> len
            )
        {
            ReadFunction = read;
            SeekFunction = seek;
            PositionProperty = pos;
            LengthProperty = len;
        }

        public override bool CanRead => true;

        public override bool CanSeek => true;

        public override bool CanWrite => false;

        public override long Length => LengthProperty();

        public override long Position 
        { 
            get => PositionProperty();
            set => SeekFunction(value, SeekOrigin.Begin); 
        }

        public override void Flush() { }

        public override int Read(byte[] buffer, int offset, int count) => ReadFunction(buffer, offset, count);

        public override long Seek(long offset, SeekOrigin origin) => SeekFunction(offset, origin);

        public override void SetLength(long value) { }

        public override void Write(byte[] buffer, int offset, int count) { }
    }
}