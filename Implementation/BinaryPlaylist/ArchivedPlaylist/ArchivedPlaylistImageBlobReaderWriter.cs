
using System;

namespace MP.BinaryPlaylist.ArchivedPlaylist
{
    public sealed class ArchivedPlaylistImageBlobWriter : PlaylistBlobWriter
    {
        private System.Boolean imgwritten;

        public ArchivedPlaylistImageBlobWriter() 
        {
            imgwritten = false;
            IsCompleted = true;
            Header = new() { Identifier1 = BlobFlags.Custom , Identifier2 = BlobTypes.MAXEMBEDDEDBLOBVAL + 4 , Version = 1 };
        }

        public void WriteFromStream(System.IO.Stream stream)
        {
            if (imgwritten) { throw new InvalidOperationException("An archived playlist image was already written."); }
            if (stream is null) { throw new ArgumentNullException(nameof(stream)); }
            if (stream.CanRead == false) { throw new ArgumentException("Stream was unreadable." , nameof(stream)); }
            System.Byte[] buf = new System.Byte[4096];
            System.Int32 rb;
            while ((rb = stream.Read(buf , 0 , buf.Length)) > 0)
            {
                Write(buf, 0, rb);
            }
        }

        public System.Boolean ImageWritten => imgwritten;
    }

    public sealed class ArchivedPlaylistImageBlobReader : PlaylistBlobReader 
    {
        public Microsoft.IO.MemoryStream GetStreamImageData()
        {
            Microsoft.IO.MemoryStream ms = new();
            System.Byte[] buf = new System.Byte[4096];
            System.Int32 rb , blen = buf.Length;
            while (true)
            {
                if (blen > Length - Position) {
                    blen = (Length - Position).ToInt32();
                }
                rb = Read(buf , 0 , blen);
                if (rb <= 0) { break; }
                ms.Write(buf, 0, rb);
            }
            ms.Position = 0;
            return ms;
        }
    }
}