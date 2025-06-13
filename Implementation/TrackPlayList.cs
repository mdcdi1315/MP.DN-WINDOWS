using System;
using System.IO;
using MP.Utilities;
using System.Linq;
using System.Collections.Generic;
using MP.BinaryPlaylist.MusicPlayer;

namespace MP
{
    public sealed class TrackPlayList : IPlaylist
    {
        private PlaylistPreferences prefs;
        private List<IPlaylistFile> tracks;
        private Dictionary<System.String, SavedDataTag> readtags;
        private TypedFileInfo lasttrack;
        private PlaylistMetadataItemCollection metadata;
        private System.Int64 lasttracktimepointints;
        private System.String plname;

        private TrackPlayList() 
        {
            prefs = new();
            tracks = new();
            lasttracktimepointints = -1;
            readtags = new();
            lasttrack = null;
            metadata = new();
            plname = "anonymous";
        }

        public TrackPlayList(Microsoft.IO.DirectoryInfo DI) : this()
        {
            Microsoft.IO.FileInfo[] ts = DI.GetFiles("*.m??");
            ts = ts.Union(DI.GetFiles("*.flac")).ToArray();
            ts = ts.Union(DI.GetFiles("*.w??")).ToArray();
            ts = ts.Union(DI.GetFiles("*.ogg")).ToArray();
            tracks.Capacity += ts.Length;
            foreach (var tk in ts) {
                tracks.Add(new TypedFileInfo(tk));
            }
        }

        public TrackPlayList(Microsoft.IO.FileInfo[] tracks , System.String Name) : this()
        {
            this.tracks.Capacity += tracks.Length;
            foreach (var tk in tracks) {
                this.tracks.Add(new TypedFileInfo(tk));
            }
            plname = Name;
        }

        private void ParseBPL(Stream data)
        {
            using (MusicPlayerPlaylistReader rdr = new(data))
            {
                rdr.DisposeAfterUse = false;
                System.Int32 I = 0;
                System.String tmp;
                PlaylistTrackTag ttg;
                TypedFileInfo tfi;
                foreach (var track in rdr.TracksReader.GetAll())
                {
                    tmp = rdr.StringsReader.Get(track.TrackNameStringBlob);
                    if (Microsoft.IO.File.Exists(tmp) == false) { I++; continue; }
                    tfi = new TypedFileInfo(tmp);
                    if (track.TrackDataTag >= 0) {
                        ttg = rdr.TagsReader.GetTagAt(track.TrackDataTag.ToUInt32());
                        readtags.Add(tmp, SavedDataTag.FromRawData(ttg.TagProperties,
                            ttg.TagNative.CoverImagePresent ? 
                            rdr.DataReader.GetByteArray(ttg.TagNative.CoverImageDataBlobOffset) : null));
                    }
                    if (track.IsLastTrack) {
                        System.UInt32 fidx;
                        (fidx, lasttracktimepointints) = rdr.MetadataReader.CriticalMetadata;
                        if (fidx == I) { lasttrack = tfi; } else { lasttracktimepointints = 0; }
                    }
                    tracks.Add(tfi);
                    I++;
                }
                if (rdr.MetadataReader.Header.Version >= 2)
                {
                    rdr.MetadataReader.ReadMetadata();
                    metadata = new(rdr.MetadataReader.Metadata);
                }
                prefs = rdr.PreferencesReader.GetAll();
            }
        }

        public TrackPlayList(Stream data , System.String filename) : this()
        {
            ParseBPL(data);
            if (prefs.Count == 0)
            {
                // We do not have any info for all prefs , explicitly add them now.
                prefs.Add(new("IsReadOnly", false, "Determines whether the playlist is mutable or not."));
                prefs.Add(new("DetermineAudioTagsAtLoad" , false , "Specifies to determine all the tags at every time the player loads this playlist."));
                prefs.Add(new("TrackCoverPath" , "" , "Specifies an image file path to use as a cover for all the files when are played back , even for those that do not have tags.", PreferenceValueType.String, PreferenceBehaviorFlags.FilePath));
                prefs.Add(new("PlaylistCoverPath" , "" , "Specifies a path to a playlist cover image to use for the playlist." , PreferenceValueType.String , PreferenceBehaviorFlags.FilePath));
                prefs.Add(new("PlaylistName", "", "Specifies the playlist name. By default , it is substituted from the file name."));
            }
            if (metadata.Count == 0) 
            {
                // Newly initialized metadata never existed before , init them now
                metadata.Add(new("AudioTracksHaveBeenDeterminedAtLeastOnce" , false)); // A further indication that the Music Player has at least once successfully called the DetermineAudioTags function.
                metadata.Add(new("LifetimeTracksPlayed" , 0UL)); // unsigned long so that it can increment the number of lifetime tracks, without worrying of overflowing.
                metadata.Add(new("NumberOfTracksPlayed" , 0UL)); // As the above , but the functionality of this one is to provide the # of tracks that were requested to be played.
                metadata.Add(new("CurrentTrackFileName", "")); // Extends CurrentTrack behavior by also indicating the file name that was factually played. This will be used to determine the Last Track name faster (if that exists anyway)
                metadata.Add(new("NumberOfContainedAudioTags" , 0)); // Just for statistics...
                // Keep the previous playlist names just for reference...
                // It is a list split with the 65279 character.
                metadata.Add(new("PreviousPlaylistNames" , "")); 
            }
            try { data.Dispose(); } catch { }
            plname = filename ?? System.String.Empty;
        }

        public IList<IPlaylistFile> TracksContained => tracks;

        public PlaylistMetadataItemCollection Metadata => metadata;

        public IPlaylistFile CurrentTrack
        {
            get => lasttrack;
            set {
                lasttrack = value as TypedFileInfo;
                if (lasttrack is null) { lasttracktimepointints = -1; return; }
                foreach (var track in tracks)
                {
                    if (lasttrack.FullName == track.FullName)
                    {
                        metadata["CurrentTrackFileName"] = lasttrack.Name;
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

        /// <summary>
        /// Gets or sets the preference property <c>PlaylistName</c>. <br />
        /// For V1 playlists or for those this field is not supplied , it is substituted from the playlist file on disk.
        /// </summary>
        public System.String PlaylistName
        {
            get {
                if (System.String.IsNullOrEmpty(prefs.Get("PlaylistName").Value.ToString())) {
                    System.Int32 pos = plname.LastIndexOf('.');
                    prefs.Update("PlaylistName" , pos == -1 ? plname : plname.Remove(pos));
                }
                return prefs.Get("PlaylistName").Value.ToString();
            }
            set {
                if (metadata.TryGetValue("PreviousPlaylistNames" , out System.Object strobj))
                {
                    System.Text.StringBuilder sb = new(strobj as System.String);
                    if (sb.Length > 0) { sb.Append(65279.ToChar()); }
                    sb.Append(prefs.Get("PlaylistName").Value);
                    metadata["PreviousPlaylistNames"] = sb.ToString();
                    sb = null;
                }
                prefs.Update("PlaylistName", value);
            }
        }

        public System.String PlaylistNameField
        {
            get {
                System.Int32 pos = plname.LastIndexOf('.');
                if (pos == -1) { return plname; }
                return plname.Remove(pos);
            }
        }

        /// <summary>
        /// Gets or sets the preference property <c>IsReadOnly</c>.
        /// </summary>
        public System.Boolean IsReadOnly 
        { 
            get => prefs.Get("IsReadOnly").BooleanValue;
            set => prefs.Update("IsReadOnly" , value);
        }

        /// <summary>
        /// Gets or sets the preference property <c>TrackCoverPath</c>.
        /// </summary>
        public System.String TrackCoverPath
        {
            get => prefs.Get("TrackCoverPath").Value.ToString();
            set {
                if (value.ContainsFileExtension(Settings.Global.Resources.GetStringResource("SupportedCoverImageFormats")) == false)
                {
                    throw new ArgumentException("The cover image for the playlist must be a JPEG , a bitmap or a PNG image.");
                }
                prefs.Update("TrackCoverPath", value);
            }
        }

        /// <summary>
        /// Gets or sets the preference property <c>PlaylistCoverPath</c>.
        /// </summary>
        public System.String PlaylistCoverPath
        {
            get => prefs.Get("PlaylistCoverPath").Value.ToString();
            set {
                if (value.ContainsFileExtension(Settings.Global.Resources.GetStringResource("SupportedCoverImageFormats")) == false)
                {
                    throw new ArgumentException("The cover image for the playlist must be a JPEG , a bitmap or a PNG image.");
                }
                prefs.Update("PlaylistCoverPath", value);
            }
        }

        /// <summary>
        /// Gets or sets the preference property <c>DetermineAudioTagsAtLoad</c>.
        /// </summary>
        public System.Boolean DetermineAudioTagsAtLoad
        {
            get => prefs.Get(nameof(DetermineAudioTagsAtLoad)).BooleanValue;
            set => prefs.Update(nameof(DetermineAudioTagsAtLoad), value);
        }

        /// <summary>
        /// Gets all the current preferences that comprise this playlist.
        /// </summary>
        public PlaylistPreferences Preferences => prefs;

        public void SaveAsPlaylist(Stream stream) 
        {
            MusicPlayerPlaylistWriter wr = null;
            try {
                wr = new(stream, this);
                wr.DisposeAfterUse = false;
                for (System.Int32 I = 0; I < tracks.Count; I++)
                {
                    wr.AddFile(tracks[I]);
                }
                wr.AddMetadata(metadata);
                wr.Generate();
            } finally {
                try { 
                    wr?.Dispose(); 
                } catch (System.InvalidOperationException) {
                    while (wr is not null)
                    {
                        try {
                            wr?.Generate();
                            wr?.Dispose();
                            wr = null;
                        } catch (System.InvalidOperationException) { }
                    }
                }
                wr = null;
            }
        }

        public void CreateArchivedPlaylist(Stream stream , IEnumerable<IPlaylistFile> files , ArchivedPlaylistAttributeCollection attributes , System.String iconcachefp)
        {
            FileStream iconcachefs = null;
            MP.BinaryPlaylist.ArchivedPlaylist.ArchivedPlaylistWriter wr = null;
            try
            {
                wr = new(stream, this);
                wr.DisposeAfterUse = false;
                foreach (var file in files) { wr.AddFile(file); }
                wr.WriteAttributes(attributes);
                if (iconcachefp is not null)
                {
                    iconcachefs = new(iconcachefp, FileMode.Open, FileAccess.Read, FileShare.Read);
                    wr.ProvidePlaylistImage(iconcachefs);
                }
                wr.Generate();
            } catch (System.Exception ex) {
                DebugProvider.WriteLine($"MPArchCreator: Reported exception while creating playlist: {ex}");
            } finally {
                iconcachefs?.Dispose();
                iconcachefs = null;
                wr?.Dispose();
                wr = null;
            }
        }

        public System.Boolean Remove(IPlaylistFile track)
        {
            IPlaylistFile file;
            for (System.Int32 I = 0; I < tracks.Count; I++) 
            {
                file = tracks[I];
                if (file.FullName == track.FullName)
                {
                    return Remove(I);
                }
            }
            return false;
        }

        public System.Boolean Remove(System.Int32 index)
        {
            try {
                if (readtags.Remove(tracks[index].FullName))
                {
                    // Although that remains a statistic, we are ought to update it
                    metadata["NumberOfContainedAudioTags"] = readtags.Count;
                }
                tracks.RemoveAt(index);
                return true;
            } catch { }
            return false;
        }
    
        public void ProvideTagDataForCurrentTrack(ITagReader rdr)
        {
            if (rdr is null)
            {
                readtags[lasttrack.FullName] = SavedDataTag.Empty;
                return;
            }
            readtags[lasttrack.FullName] = new(rdr);
        }

        public SavedDataTag GetTagFromFile(IPlaylistFile file)
        {
            System.Boolean found = false;
            foreach (var fd in tracks)
            {
                if (fd.FullName == file.FullName) { found = true; break; }
            }
            if (!found) { return null; }
            foreach (var kvp in readtags) {
                if (kvp.Key == file.FullName) { return kvp.Value; }
            }
            return null;
        }

        public ITagReader DetermineAudioTag(IPlaylistFile file)
        {
            foreach (var fd in tracks)
            {
                if (file.FullName == fd.FullName) {
                    return CommonPlaylistUtilities.DetermineAudioTag(fd);
                }
            }
            throw new FileNotFoundException($"The file {file.FullName} does not exist in the playlist.");
        }

        public void DetermineAudioTagAndAddToTagList(IPlaylistFile file)
        {
            foreach (var fd in tracks)
            {
                if (file.FullName == fd.FullName)
                {
                    var rdr = CommonPlaylistUtilities.DetermineAudioTag(fd);
                    if (rdr is null) { return; }
                    try {
                        readtags[fd.FullName] = new SavedDataTag(rdr);
                        metadata["NumberOfContainedAudioTags"] = readtags.Count;
                    } finally {
                        rdr.Dispose();
                    }
                    return;
                }
            }
            throw new FileNotFoundException($"The file {file.FullName} does not exist in the playlist.");
        }

        public void DetermineAudioTags()
        {
            ITagReader rd;
            readtags.Clear();
            foreach (var file in tracks)
            {
                if (file is null) { continue; }
                rd = CommonPlaylistUtilities.DetermineAudioTag(file);
                readtags.Add(file.FullName, rd is null ? SavedDataTag.Empty : new SavedDataTag(rd));
            }
            metadata["NumberOfContainedAudioTags"] = readtags.Count;
            metadata["AudioTracksHaveBeenDeterminedAtLeastOnce"] = true;
        }

        public System.Boolean AudioTagsDetermined
        {
            get {
                if (prefs.Get(nameof(DetermineAudioTagsAtLoad)).BooleanValue) {
                    readtags.Clear();
                    return false;
                }
                return (System.Boolean)metadata["AudioTracksHaveBeenDeterminedAtLeastOnce"];
            }
        }

        public System.Int32 GetCurrentTrackIndex()
        {
            System.Int32 idx = 0;
            if (lasttrack is null) { throw new ArgumentNullException("CurrentTrack" ,"The detector must know the track so as to return it's index."); }
            foreach (var fd in tracks)
            {
                if (fd.FullName == lasttrack.FullName) { return idx; }
                idx++;
            }
            return idx;
        }

        public void Dispose()
        {
            if (tracks is not null)
            {
                tracks.Clear();
                tracks = null;
            }
            if (prefs is not null)
            {
                prefs.Clear();
                prefs = null;
            }
            lasttrack = null;
            lasttracktimepointints = 0;
            if (readtags is not null) 
            {
                foreach (var kvp in readtags) { kvp.Value?.Dispose(); }
                readtags.Clear();
                readtags = null;
            }
            if (metadata is not null)
            {
                metadata.Clear();
                metadata = null;
            }
        }
    }
}
