
using System;

namespace MP.Archiving
{
    public sealed class MusicPlayerArchiveWriter : IDisposable
    {
        private System.IO.Stream finalstream , wd;
        private ArchiveByteOrder byteord; // Determined when the header will be written
        private System.Boolean strmown;
        // Tracks current entry written bytes so that it can know how many remaining bytes
        // are left for the current entry.
        private System.Int64 wbs , cbs;
        private ArchiveEntry ce;

        public MusicPlayerArchiveWriter(System.IO.Stream stream)
        {
            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }
            if (stream.CanWrite == false) 
            {
                throw new ArgumentException("Stream was unwriteable." , nameof(stream));
            }
            finalstream = stream;
            strmown = false;
            wbs = -1;
            byteord = ArchiveByteOrder.Invalid;
            Init();
        }

        // Initialization code to be run at the beginning
        private void Init()
        {
            MPARCHHEADER header = new();
            byteord = header.ByteOrder = BitConverter.IsLittleEndian ? ArchiveByteOrder.LittleEndian : ArchiveByteOrder.BigEndian;
            finalstream.WriteStructure(header);
            ce = null;
        }

        public System.Boolean IsStreamOwner
        {
            get => strmown;
            set => strmown = value;
        }

        public void RegisterEntry(ArchiveEntry entry)
        {
            if (entry is null) { throw new ArgumentNullException(nameof(entry)); }
            ce = entry;
            if (wd is null)
            {
                if (ce.Length < System.Array.MaxLength) {
                    wd = new Microsoft.IO.MemoryStream(ce.Length);
                } else {
                    wd = new Microsoft.IO.MemoryStream();
                }
            }
            wbs = 0;
            cbs = 0;
        }

        public void ProvideFileDataFromStream(System.IO.Stream strm)
        {
            if (ce is null) { throw new InvalidOperationException("An archive entry was not registered yet."); }
            if (ce.Length == 0) { throw new InvalidOperationException("The archive entry does not accept any additional data."); }
            if (wbs >= ce.Length) { throw new InvalidOperationException("All the archive entry bytes were written. You cannot write additional bytes."); }
            if (strm is null) { throw new ArgumentNullException(nameof(strm)); }
            if (strm.CanRead == false) { throw new ArgumentException("Stream was unreadable.", nameof(strm)); }
            System.Int64 pc = strm.Position , cpc;
            switch (ce.Compression)
            {
                case ArchiveEntryCompression.Store:
                    strm.DirectCopyToStream(wd);
                    break;
                case ArchiveEntryCompression.GZip:
                    cpc = wd.Position;
                    System.IO.ManagedZip.GZip.GZipOutputStream gzo = new(wd) { IsStreamOwner = false , FileName = ce.EntryPath };
                    gzo.SetLevel(7);
                    try {
                        System.Byte[] buffer = new System.Byte[4096];
                        System.Int32 rb;
                        while ((rb = strm.Read(buffer , 0 , buffer.Length)) > 0)
                        {
                            gzo.Write(buffer, 0, rb);
                        }
                        gzo.Finish();
                    } finally {
                        gzo.Dispose();
                    }
                    cbs = wd.Position - cpc;
                    break;
                case ArchiveEntryCompression.BZip2:
                    cpc = wd.Position;
                    System.IO.ManagedZip.BZip2.BZip2OutputStream bzo = new(wd , 7) { IsStreamOwner = false };
                    try {
                        System.Byte[] buffer = new System.Byte[4096];
                        System.Int32 rb;
                        while ((rb = strm.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            bzo.Write(buffer, 0, rb);
                        }
                        bzo.Flush();
                    } finally {
                        bzo.Dispose();
                    }
                    cbs = wd.Position - cpc;
                    break;
            }
            wbs = strm.Position - pc;
        }

        public void CloseEntry()
        {
            if (ce is null) { throw new InvalidOperationException("An archive entry was not provided."); }
            if (wbs < ce.Length)
            {
                // If bytes are remaining , fill the remaining with zeroes.
                System.Int64 blks = (ce.Length - wbs) / 1024 , rem = (ce.Length - wbs) % 1024;
                System.Byte[] emptybuf = new System.Byte[1024];
                while (blks > 0) {
                    wd.Write(emptybuf , 0 , emptybuf.Length);
                    blks--;
                }
                if (rem > 0) 
                {
                    wd.Write(emptybuf, 0, rem.ToInt32());
                }
            }
            // Write all the found data to the archive stream
            var nh = ce.GetNativeHeader();
            nh.CompressedSize = MPARCHUINT.ToUnsignedSixByteInteger(cbs);
            finalstream.WriteStructure(nh);
            System.Byte[] temp;
            foreach (var attr in ce.Attributes)
            {
                temp = attr.ToArray(byteord);
                finalstream.Write(temp , 0 , temp.Length);
                temp = null;
            }
            wd.Position = 0;
            wd.DirectCopyToStream(finalstream);
            wd.Position = 0;
            wd.SetLength(0);
            finalstream.Flush();
            // Done!
            ce = null;
        }

        /// <summary>
        /// Destroys the <see cref="MusicPlayerArchiveWriter"/> internal state.
        /// </summary>
        public void Dispose()
        {
            if (ce is not null)
            {
                CloseEntry();
                ce = null;
            }
            if (finalstream is not null)
            {
                finalstream.Flush();
                if (strmown) { finalstream.Dispose(); }
                finalstream = null;
            }
            wd?.Dispose();
            wd = null;
        }
    }
}