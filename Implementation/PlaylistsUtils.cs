
using System;
using System.IO;
using MP.Archiving;
using MP.TagReading;
using System.IO.ManagedZip.Zip;

namespace MP
{
    public sealed class TypedFileInfo : IPlaylistFile
    {
        private Microsoft.IO.FileInfo fi;

        public TypedFileInfo(Microsoft.IO.FileInfo fi)
        {
            if (fi is null) { throw new ArgumentNullException(nameof(fi)); }
            this.fi = fi;
        }

        public TypedFileInfo(System.String path) : this(new Microsoft.IO.FileInfo(path)) { }

        public string FullName => fi is null ? null : fi.FullName;

        public string Name => fi is null ? null : fi.Name;

        public string Extension => fi is null ? null : fi.Extension;

        public DateTime CreationTimeUtc => fi is null ? new(0) : fi.CreationTimeUtc;

        public DateTime LastModificationTimeUtc => fi is null ? new(0) : fi.LastAccessTimeUtc;

        public DateTime LastWriteTimeUtc => fi is null ? new(0) : fi.LastWriteTimeUtc;

        public long Length => fi is null ? 0 : fi.Length;

        public System.Boolean Exists => fi is not null && fi.Exists;

        public MusicPlayerStreamV2 GetStream()
        {
            if (fi is null) { return null; }
            MusicPlayerStreamV2 result = new(fi.OpenRead());
            result.SetBooleanAttribute(MusicPlayerStreamV2.IsStreamOwnerProperty, true);
            result.SetStringAttribute("FileName", fi.Name);
            return result;
        }

        AbstractPropertyStream IPlaylistFile.GetStream() => GetStream();
    }

    public sealed class ArchiveFileInfo : IPlaylistFile
    {
        private ArchivedTrackPlaylist reference;
        private System.String fp;
        private System.Int64 length;
        private System.DateTime writetime , creationtime;

        public ArchiveFileInfo(ZipEntry entry, ArchivedTrackPlaylist plt)
        {
            reference = plt;
            creationtime = SystemInfo.Now;
            fp = entry.Name;
            length = entry.Size;
        }

        public ArchiveFileInfo(ArchiveEntry entry , ArchivedTrackPlaylist plt)
        {
            reference = plt;
            fp = entry.EntryPath;
            creationtime = entry.CreationTime;
            writetime = entry.LastWriteTime;
            length = entry.Length;
        }

        public string FullName => fp;

        public string Name => Path.GetFileName(fp);

        public string Extension => Path.GetExtension(fp);

        public DateTime CreationTimeUtc => creationtime;

        public DateTime LastModificationTimeUtc => writetime;

        public DateTime LastWriteTimeUtc => writetime;

        public long Length => length;

        public bool Exists => true;

        public MusicPlayerStreamV2 GetStream() => reference.GetStream(this);

        AbstractPropertyStream IPlaylistFile.GetStream() => GetStream();
    }

    public enum DownloadedFileType : System.UInt16
    {
        Unknown = 0,
        NormalDownload = 1,
        Youtube = 2,
        NonHttps = 3,
        File = 4,
        YtDlp = 5
    }

    public sealed class DownloadedFileInfo : IPlaylistFile
    {
        private System.String url;
        private Microsoft.IO.FileInfo fi;
        private DownloadedFileType type;

        public DownloadedFileInfo(Microsoft.IO.FileInfo fi , System.String urlsource , DownloadedFileType type)
        {
            if (fi is null) { throw new ArgumentNullException(nameof(fi)); }
            if (System.String.IsNullOrEmpty(urlsource)) { throw new ArgumentNullException(nameof(urlsource)); }
            this.fi = fi;
            url = urlsource;
            this.type = type;
        }

        public string FullName => fi.FullName;

        public string Name => fi.Name;

        public string Extension => fi.Extension;

        public DateTime CreationTimeUtc => fi.CreationTimeUtc;

        public DateTime LastModificationTimeUtc => fi.LastAccessTimeUtc;

        public DateTime LastWriteTimeUtc => fi.LastWriteTimeUtc;

        public long Length => fi.Length;

        public bool Exists => fi.Exists;

        public System.String SourceURL => url;

        public DownloadedFileType FileType => type;

        public MusicPlayerStream GetStream()
        {
            if (fi is null) { return null; }
            MusicPlayerStream result = new(fi.OpenRead());
            result.SetBooleanAttribute(AbstractPropertyStream.IsStreamOwnerProperty, true);
            result.SetStringAttribute("FileName", fi.Name);
            return result;
        }

        AbstractPropertyStream IPlaylistFile.GetStream() => GetStream();
    }

    public static class CommonPlaylistUtilities
    {
        public static ITagReader DetermineAudioTag(IPlaylistFile file)
        {
            ITagReader reader = null;
            System.IO.Stream FDOut = null;
            try
            {
                System.Boolean flag = false;
                System.Threading.Thread TD = new(() => {
                    try
                    {
                        FDOut = file.GetStream();
                        FDOut.Position = 0;
                        try { reader = new ID3V2DataReader(FDOut); } catch { }
                        FDOut.Position = 0;
                        if (reader != null) { goto G_completed; }
                        try { reader = new MP4AudioTagReader(FDOut); } catch { }
                        if (reader != null) { goto G_completed; }
                        FDOut.Position = 0;
                        try { reader = new FlacAudioTagReader(FDOut); } catch { }
                        if (reader != null) { goto G_completed; }
                        FDOut.Position = 0;
                        try { reader = new OggVorbisAudioTagReader(FDOut); } catch { }
                    }
                    catch { }
                    finally { FDOut?.Dispose(); }
                    if (reader is null) { goto G_completed; }
                G_completed:
                    flag = true;
                    return;
                });
                TD.TrySetApartmentState(System.Threading.ApartmentState.STA);
                TD.Start();
                while (flag == false) { System.Threading.Thread.Sleep(10); }
                return reader;
            }
            catch { return null; }
            finally { FDOut?.Dispose(); }
        }
    }

}