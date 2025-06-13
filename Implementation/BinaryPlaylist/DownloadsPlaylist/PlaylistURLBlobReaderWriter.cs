
using System;
using System.Runtime.InteropServices;

namespace MP.BinaryPlaylist.DownloadsPlaylist
{
    [StructLayout(LayoutKind.Explicit , Pack = 1 , Size = 8)]
    public struct PLAYLISTTRACKURLHEADER
    {
        [FieldOffset(0)]
        public DownloadedFileType Type;

        [FieldOffset(2)]
        public System.Int32 Length;

        [FieldOffset(6)]
        public System.UInt16 Padding;
    }

    public sealed class PlaylistURLBlobWriter : PlaylistBlobWriter
    {
        private System.UInt32 lastid;

        public PlaylistURLBlobWriter() : base() { }

        public System.UInt32 WriteURL(DownloadedFileInfo url)
        {
            if (url is null) { throw new ArgumentNullException(nameof(url)); }
            if (System.String.IsNullOrEmpty(url.SourceURL)) { throw new ArgumentException("URL field must not be null or empty." , nameof(url)); }
            PLAYLISTTRACKURLHEADER hdr = new();
            hdr.Length = url.SourceURL.Length;
            hdr.Padding = ((hdr.Length * sizeof(System.Char)) % 4).ToUInt16();
            hdr.Type = url.FileType;
            WriteStructure(hdr);
            Write(url.SourceURL, false);
            WritePadString("PD" , hdr.Padding);
            return lastid++;
        }

        /// <summary>
        /// Finalizes the URL blob writer.
        /// </summary>
        public void FinalizeWriter()
        {
            Header = new() { Count = lastid, Identifier1 = BlobFlags.ReadOnly | BlobFlags.Custom, Identifier2 = BlobTypes.MAXEMBEDDEDBLOBVAL + 1, Version = 1 };
            IsCompleted = true;
        }
    }

    public sealed class PlaylistURLBlobReader : PlaylistBlobReader
    {
        public PlaylistURLBlobReader() { }

        public (PLAYLISTTRACKURLHEADER , System.String) GetURL(System.UInt32 index)
        {
            if (index >= Header.Count) { throw new ArgumentOutOfRangeException(nameof(index)); }
            PLAYLISTTRACKURLHEADER hdr;
            Position = 0;
            for (System.UInt32 I = 0; I < Header.Count && I < index; I++)
            {
                hdr = ReadStructure<PLAYLISTTRACKURLHEADER>();
                Seek(hdr.Length * sizeof(System.Char), System.IO.SeekOrigin.Current);
                if (hdr.Padding > 0) {
                    Seek(hdr.Padding, System.IO.SeekOrigin.Current);
                }
            }
            hdr = ReadStructure<PLAYLISTTRACKURLHEADER>();
            return (hdr , ReadString(hdr.Length, false));
        }
    }

}