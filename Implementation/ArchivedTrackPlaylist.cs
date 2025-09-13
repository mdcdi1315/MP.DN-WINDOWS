using System;
using MP.Utilities;
using MP.Archiving;
using System.Text.Json;
using System.IO.ManagedZip.Zip;
using System.Collections.Generic;

namespace MP
{
    public sealed class ArchivedTrackPlaylist : IPlaylist
    {
        private const System.String ExpectedName = "playlist.pljson";
        /// <summary>
        /// Defines the largest buffer size that can exist in a MP memory stream.
        /// </summary>
        private const System.Int32 MaximumSize = 120 * 1048576;

        private ZipInputStream iaold;
        private System.String plname;
        private List<IPlaylistFile> tracks;
        private ArchiveFileInfo lasttrack;
        private PlaylistPreferences prefs;
        private Microsoft.IO.FileStream archive;
        private System.Int64 lasttracktimepointints;
        private MusicPlayerArchiveReader reader;
        private Microsoft.IO.MemoryStream imagestream;
        private ArchivedPlaylistAttributeCollection attributes;
        private Dictionary<System.String, SavedDataTag> readtags;

        public ArchivedTrackPlaylist(Microsoft.IO.FileInfo file)
        {
            archive = file.OpenRead();
            tracks = new();
            readtags = new();
            attributes = null;
            imagestream = null;
            // Try the new format first
            try {
                reader = new MusicPlayerArchiveReader(archive) { IsStreamOwner = false };
                ReadNew();
            } catch (ExceptionSystem.InvalidMPArchiveFormatException) {
                try {
                    archive.Position = 0;
                    // The new format was not present try the old one.
                    iaold = new ZipInputStream(archive) { IsStreamOwner = false };
                    attributes = new(); // On the older format this is not defined, so give it an empty collection
                    ReadOld();
                } catch {
                    archive?.Dispose();
                    tracks = null;
                    attributes = null;
                    readtags = null;
                    throw;
                }
            } catch {
                archive?.Dispose();
                tracks = null;
                readtags = null;
                throw;
            }
        }

        private void ReadOld()
        {
            ZipEntry ent;
            List<IPlaylistFile> temptracks = new();
            Microsoft.IO.MemoryStream jsonstream = null;
            while ((ent = iaold.GetNextEntry()) is not null)
            {
                if (ent.Name == ExpectedName) {
                    plname = ent.Name;
                    jsonstream = new(ent.Size);
                    iaold.CopyToExactly(jsonstream, 2048 , ent.Size);
                } else {
                    temptracks.Add(new ArchiveFileInfo(ent, this));
                }
            }
            if (jsonstream is null) {
                throw new FormatException("The archive is possibly damaged. The central entry index could not be retrieved.");
            }
            ent = null;
            tracks.Capacity += temptracks.Count;
            jsonstream.Position = 0;
            using (JsonDocument doc = JsonDocument.Parse(jsonstream , new() { MaxDepth = 5 , CommentHandling = JsonCommentHandling.Skip }))
            {
                JsonElement tel = doc.RootElement.GetProperty("Tracks");
                foreach (var path in tel.EnumerateArray())
                {
                    foreach (var entry in temptracks) {
                        if (entry.Name == Microsoft.IO.Path.GetFileName(path.GetString())) { 
                            tracks.Add(entry);
                            break;
                        }
                    }
                }
                temptracks.Clear();
                temptracks = null;
                tel = doc.RootElement.GetProperty("LastTrack");
                System.String tmp = tel.GetProperty("TrackPath").GetString();
                tmp = Microsoft.IO.Path.GetFileName(tmp);
                if (System.String.IsNullOrWhiteSpace(tmp) == false) {
                    System.Int32 I = 0;
                    foreach (var entry in tracks) 
                    {
                        if (entry.Name == tmp)
                        {
                            lasttrack = tracks[I] as ArchiveFileInfo;
                            break;
                        }
                        I++;
                    }
                }
                lasttracktimepointints = tel.GetProperty("TrackTimePoint").GetInt64();
                lasttracktimepointints = lasttracktimepointints < 0 ? 0 : lasttracktimepointints;
                tel = doc.RootElement.GetProperty("Tags");
                readtags.EnsureCapacity(tracks.Count);
                foreach (var jsv in tel.EnumerateArray())
                {
                    readtags.Add(jsv.GetProperty("FilePath").GetString(), CommonTagUtils.FromJsonElement(jsv));
                }
                tel = default;
                prefs = PlaylistPrefsExtensions.CreateFrom(doc.RootElement.GetProperty("Preferences"));
            }
            jsonstream.Dispose();
            jsonstream = null;
        }

        private void ReadNew()
        {
            IArchiveEntryReader ae = null;
            List<IPlaylistFile> temptracks = new();
            Microsoft.IO.MemoryStream plbinarystream = null;
            while ((ae = reader.GetNextArchiveEntry()) is not null)
            {
                if (ae.Entry.EntryPath == "playlist.mpbpl")
                {
                    plname = ae.Entry.EntryPath;
                    plbinarystream = new(ae.Entry.Length);
                    ae.EntryStream.CopyTo(plbinarystream);
                    plbinarystream.Position = 0;
                } else {
                    temptracks.Add(new ArchiveFileInfo(ae.Entry, this));
                }
                ae.Dispose();
                ae = null;
            }
            if (plbinarystream is null) {
                throw new FormatException("The archive is possibly damaged. The central entry index could not be retrieved.");
            }
            using (MP.BinaryPlaylist.ArchivedPlaylist.ArchivedPlaylistReader rdr = new(plbinarystream))
            {
                System.String ts;
                System.Int32 I = 0;
                if (rdr.AttributeBlobReader is not null)
                {
                    attributes = rdr.AttributeBlobReader.Attributes;
                }
                var imgr = rdr.ImageReader;
                if (imgr is not null && imgr.Length > 0) { imagestream = imgr.GetStreamImageData(); }
                foreach (var file in rdr.TracksReader.GetAll())
                {
                    ts = rdr.StringsReader.Get(file.TrackNameStringBlob);
                    foreach (var t in temptracks)
                    {
                        if (t.Name == ts) {
                            tracks.Add(t);
                            if (file.TrackDataTag >= 0)
                            {
                                var ttg = rdr.TagsReader.GetTagAt(file.TrackDataTag.ToUInt32());
                                readtags.Add(ts, SavedDataTag.FromRawData(ttg.TagProperties,
                                    ttg.TagNative.CoverImagePresent ?
                                    rdr.DataReader.GetByteArray(ttg.TagNative.CoverImageDataBlobOffset) : null));
                            }
                            if (file.IsLastTrack) {
                                System.UInt32 fidx;
                                (fidx, lasttracktimepointints) = rdr.MetadataReader.CriticalMetadata;
                                if (fidx == I) { lasttrack = t as ArchiveFileInfo; } else { lasttracktimepointints = 0; }
                            }
                            I++;
                            break;
                        }
                    }
                }
                prefs = rdr.PreferencesReader.GetAll();
                temptracks.Clear();
                temptracks = null;
            }
        }

        public MusicPlayerStreamV2 GetStream(IPlaylistFile file)
        {
            if (iaold is not null) {
                return GetStreamOld(file);
            } else if (reader is not null) {
                return GetStreamNew(file);
            } else {
                throw new System.InvalidOperationException("The playlist seems to not have been properly initialized.");  
            }
        }

        private MusicPlayerStreamV2 GetStreamNew(IPlaylistFile file)
        {
            if (file.Length > MaximumSize)
            {
                throw new OutOfMemoryException("Could not load the file into memory.");
            }
            MusicPlayerStreamV2 stream = null;
            IArchiveEntryReader reader = null;
            foreach (var entry in tracks)
            {
                if (entry.Name == file.Name)
                {
                    try {
                        // Try first to load the requested entry after the last read entry on the file;
                        // if not found , try first to re-search the archive and then flag it as not-found.
                        // If the archive has been read to completion , the code will unconditionally 
                        // force to get back to the beginning of the file.
                        System.Boolean exausted = false;
                    g_retry:
                        while ((reader = this.reader.GetNextArchiveEntry()) is not null)
                        {
                            if (reader.Entry.EntryPath == file.FullName)
                            {
                                stream = MusicPlayerStreamV2.CreateMemoryStream(reader.Entry.Length);
                                reader.EntryStream.DirectCopyToStream(stream);
                                stream.SetStringAttribute("FileName", file.Name);
                                stream.Position = 0;
                                // Must dispose the old entry reader
                                reader?.Dispose();
                                reader = null;
                                break;
                            }
                            // Must dispose the old entry reader
                            reader?.Dispose();
                            reader = null;
                        }
                        if (exausted == false)
                        {
                            archive.Position = 0;
                            this.reader?.Dispose();
                            this.reader = new MusicPlayerArchiveReader(archive) { IsStreamOwner = false };
                            exausted = true;
                            goto g_retry;
                        }
                    } catch (System.Exception e) {
                        MusicPlayerHelper.ShowErrorMessage(e.ToString());
                        reader?.Dispose();
                        reader = null;
                        stream?.Dispose();
                        stream = null;
                    }
                    break;
                }
            }
            return stream;
        }

        private MusicPlayerStreamV2 GetStreamOld(IPlaylistFile file)
        {
            if (file.Length > MaximumSize)
            {
                throw new OutOfMemoryException("Could not load the file into memory.");
            }
            MusicPlayerStreamV2 stream = null;
            ZipEntry ze = null;
            foreach (var entry in tracks) 
            {
                if (entry.Name == file.Name) 
                {
                    try {
                        archive.Position = 0;
                        iaold?.Dispose();
                        iaold = new ZipInputStream(archive) { IsStreamOwner = false };
                        while ((ze = iaold.GetNextEntry()) is not null)
                        {
                            if (ze.Name == file.FullName) 
                            {
                                stream = MusicPlayerStreamV2.CreateMemoryStream(ze.Size);
                                iaold.CopyToExactly(stream, 4096, ze.Size);
                                stream.SetStringAttribute("FileName", file.Name);
                                stream.Position = 0;
                                break;
                            }
                        }
                    } catch (System.Exception e) {
                        MusicPlayerHelper.ShowErrorMessage(e.ToString());
                        stream?.Dispose();
                        stream = null;
                    }
                    break;
                }
            }
            return stream;
        }

        private IEnumerable<IArchiveEntryReader> ActualArchiveStreamsNew()
        {
            archive.Position = 0;
            this.reader?.Dispose();
            this.reader = new MusicPlayerArchiveReader(archive) { IsStreamOwner = false };
            IArchiveEntryReader ae = null;
            System.Boolean dispose;
            while ((ae = reader.GetNextArchiveEntry()) is not null)
            {
                dispose = true;
                System.String nameonly = Microsoft.IO.Path.GetFileName(ae.Entry.EntryPath);
                foreach (var tck in tracks)
                {
                    if (nameonly == tck.Name) { 
                        yield return ae; 
                        dispose = false;
                        break;
                    }
                }
                if (dispose) { ae.Dispose(); }
            }
        }

        public IEnumerable<IArchiveEntryReader> ActualArchiveStreams
        {
            get => ActualArchiveStreamsNew();
        }

        // Not supported.
        public PlaylistMetadataItemCollection Metadata => null;

        /// <summary>
        /// Gets or sets the preference property <c>PlaylistName</c>. <br />
        /// For V1 playlists or for those this field is not supplied , it is substituted from the playlist file on disk.
        /// </summary>
        public System.String PlaylistName
        {
            get
            {
                if (System.String.IsNullOrEmpty(prefs.Get("PlaylistName").Value.ToString()))
                {
                    System.Int32 pos = plname.LastIndexOf('.');
                    prefs.Update("PlaylistName", pos == -1 ? plname : plname.Remove(pos));
                }
                return prefs.Get("PlaylistName").Value.ToString();
            }
            set => prefs.Update("PlaylistName", value);
        }

        public System.String PlaylistNameField
        {
            get
            {
                System.Int32 pos = plname.LastIndexOf('.');
                if (pos == -1) { return plname; }
                return plname.Remove(pos);
            }
        }

        /// <summary>
        /// Gets all the current preferences that comprise this playlist.
        /// </summary>
        public PlaylistPreferences Preferences => prefs;

        public System.IO.Stream PlaylistImageData => imagestream;

        public ArchivedPlaylistAttributeCollection Attributes => attributes;

        public IList<IPlaylistFile> TracksContained => tracks;

        public IPlaylistFile CurrentTrack
        {
            get => lasttrack;
            set
            {
                lasttrack = value as ArchiveFileInfo;
                if (lasttrack is null) { lasttracktimepointints = -1; return; }
                foreach (var track in tracks)
                {
                    if (lasttrack.FullName == track.FullName)
                    {
                        return;
                    }
                }
                throw new InvalidOperationException("The file must exist in TracksContained array.");
            }
        }

        public TimeSpan CurrentTrackProcessedTime
        {
            get => new(lasttracktimepointints < 0 ? 0 : lasttracktimepointints);
            set => lasttracktimepointints = value.Ticks;
        }

        public System.Boolean Remove(IPlaylistFile track) => false;

        public System.Boolean Remove(System.Int32 index) => false;

        public SavedDataTag GetTagFromFile(IPlaylistFile file)
        {
            System.Boolean found = false;
            foreach (var fd in tracks)
            {
                if (fd.Name == file.Name) { found = true; break; }
            }
            if (!found) { return null; }
            foreach (var kvp in readtags)
            {
                if (Microsoft.IO.Path.GetFileName(kvp.Key) == file.Name) { return kvp.Value; }
            }
            return null;
        }

        public ITagReader DetermineAudioTag(IPlaylistFile file) => null;

        public void DetermineAudioTagAndAddToTagList(IPlaylistFile file) { }

        public void DetermineAudioTags() { }

        public System.Boolean AudioTagsDetermined => true;

        public System.Int32 GetCurrentTrackIndex()
        {
            System.Int32 idx = 0;
            if (lasttrack is null) { throw new ArgumentNullException("CurrentTrack", "The detector must know the track so as to return it's index."); }
            foreach (var fd in tracks)
            {
                if (fd.FullName == lasttrack.FullName) { return idx; }
                idx++;
            }
            return idx;
        }

        public IPlaylist Shuffle() => throw new NotSupportedException("Not supported for archived playlists");

        public void Dispose()
        {
            prefs = null;
            readtags?.Clear();
            readtags = null;
            tracks?.Clear();
            tracks = null;
            lasttrack = null;
            plname = null;
            iaold?.Dispose();
            iaold = null;
            reader?.Dispose();
            reader = null;
            archive?.Dispose();
            archive = null;
            imagestream?.Dispose();
            imagestream = null;
            attributes?.Clear();
            attributes = null;
            Settings.Global.TempDirectory.Delete(true);
            Settings.Global.TempDirectory.Create();
            Settings.Global.TempDirectory.Refresh();
        }
    }
}