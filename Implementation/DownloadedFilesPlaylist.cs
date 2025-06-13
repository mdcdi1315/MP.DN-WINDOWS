

using System;
using Microsoft.IO;
using System.Collections.Generic;
using MP.BinaryPlaylist.MusicPlayer;
using MP.BinaryPlaylist.DownloadsPlaylist;

namespace MP
{
    public sealed class DownloadedFilesPlaylist : IPlaylist
    {
        private FileInfo pltfileinfo;
        private List<IPlaylistFile> tracks;
        private DirectoryInfo pdirfilessave;
        private DownloadedFileInfo lasttrack;
        private System.Int64 lasttracktimepointints;
        private PlaylistMetadataItemCollection metadata;
        private Dictionary<System.String, SavedDataTag> readtags;

        private DownloadedFilesPlaylist()
        {
            tracks = new();
            lasttracktimepointints = -1;
            readtags = new();
            pltfileinfo = null;
            lasttrack = null;
            metadata = new();
        }

        public DownloadedFilesPlaylist(DirectoryInfo baseloadingdir , FileInfo playlistfile) : this()
        {
            if (playlistfile is null) { throw new ArgumentNullException(nameof(playlistfile)); }
            if (baseloadingdir is null) { throw new ArgumentNullException(nameof(baseloadingdir)); }
            pdirfilessave = baseloadingdir;
            pltfileinfo = playlistfile;
            if (pltfileinfo.Exists) { LoadData(); }
        }

        private void LoadData()
        {
            using (DownloadsPlaylistReader pplr = new(pltfileinfo.OpenRead()))
            {
                pplr.DisposeAfterUse = true;
                System.Int32 I = 0;
                System.String t1 , t2;
                PlaylistTrackTag ttg;
                DownloadedFileInfo tfi;
                PLAYLISTTRACKURLHEADER tempurlheader;
                foreach (var track in pplr.TracksReader.GetAll()) 
                {
                    t1 = pplr.StringsReader.Get(track.TrackNameStringBlob);
                    (tempurlheader , t2) = pplr.URLBlobReader.GetURL(track.TrackNameStringBlob);
                    var fe = pdirfilessave.GetFile(t1);
                    t1 = null;
                    if (fe is null) { I++; continue; }
                    tfi = new(fe, t2, tempurlheader.Type);
                    t2 = null;
                    if (track.TrackDataTag >= 0) {
                        ttg = pplr.TagsReader.GetTagAt(track.TrackDataTag.ToUInt32());
                        readtags.Add(tfi.Name, SavedDataTag.FromRawData(ttg.TagProperties,
                            ttg.TagNative.CoverImagePresent ?
                            pplr.DataReader.GetByteArray(ttg.TagNative.CoverImageDataBlobOffset) : null));
                    }
                    if (track.IsLastTrack) {
                        System.UInt32 fidx;
                        (fidx, lasttracktimepointints) = pplr.MetadataReader.CriticalMetadata;
                        if (fidx == I) { lasttrack = tfi; } else { lasttracktimepointints = 0; }
                    }
                    tracks.Add(tfi);
                    I++;
                }
                if (pplr.MetadataReader.Header.Version >= 2)
                {
                    pplr.MetadataReader.ReadMetadata();
                    metadata = new(pplr.MetadataReader.Metadata);
                }
                if (metadata.Count == 0)
                {
                    // Newly initialized metadata never existed before , init them now
                    metadata.Add(new("AudioTracksHaveBeenDeterminedAtLeastOnce", false)); // A further indication that the Music Player has at least once successfully called the DetermineAudioTags function.
                    metadata.Add(new("LifetimeTracksPlayed", 0UL)); // unsigned long so that it can increment the number of lifetime tracks, without worrying of overflowing.
                    metadata.Add(new("NumberOfTracksPlayed", 0UL)); // As the above , but the functionality of this one is to provide the # of tracks that were requested to be played.
                    metadata.Add(new("CurrentTrackFileName", "")); // Extends CurrentTrack behavior by also indicating the file name that was factually played. This will be used to determine the Last Track name faster (if that exists anyway)
                    metadata.Add(new("NumberOfContainedAudioTags", 0)); // Just for statistics...
                    // Previous playlist names not supported , but to keep in line , save nothing.
                    metadata.Add(new("PreviousPlaylistNames", ""));
                }
            }
        }

        public void SaveCurrentPlaylistState()
        {
            using (DownloadsPlaylistWriter ppw = new(pltfileinfo.OpenWrite() , this))
            {
                ppw.DisposeAfterUse = true;
                foreach (var t in tracks) { ppw.AddFile(t as DownloadedFileInfo); }
                ppw.AddMetadata(metadata);
                ppw.Generate();
            }
        }

        public void AddDownloadedFile(DownloadedFileInfo downloadedfile)
        {
            if (downloadedfile is null) { throw new ArgumentNullException(nameof(downloadedfile)); }
            if (downloadedfile.Exists == false) { throw new ArgumentException("Source File does not exist." , nameof(downloadedfile)); }
            tracks.Add(downloadedfile);
            DetermineAudioTagAndAddToTagList(downloadedfile);
        }

        public PlaylistMetadataItemCollection Metadata => metadata;

        public DirectoryInfo BaseResolvingDirectory => pdirfilessave;

        public System.String PlaylistName => "Downloaded";

        public PlaylistPreferences Preferences => null;

        public System.Boolean AudioTagsDetermined => true;

        public IList<IPlaylistFile> TracksContained => tracks;

        public IPlaylistFile CurrentTrack 
        { 
            get => lasttrack; 
            set {
                lasttrack = value as DownloadedFileInfo;
                if (lasttrack is null) { lasttracktimepointints = -1; return; }
                foreach (var track in tracks)
                {
                    if (lasttrack.Name == track.Name) {
                        metadata["CurrentTrackFileName"] = lasttrack.Name;
                        return; 
                    }
                }
                throw new InvalidOperationException("The file must exist in TracksContained array.");
            }
        }

        public TimeSpan CurrentTrackProcessedTime 
        { 
            get => new(lasttracktimepointints); 
            set => lasttracktimepointints = value.Ticks; 
        }

        public ITagReader DetermineAudioTag(IPlaylistFile file)
        {
            foreach (var fd in tracks)
            {
                if (file.FullName == fd.FullName)
                {
                    return CommonPlaylistUtilities.DetermineAudioTag(fd);
                }
            }
            throw new System.IO.FileNotFoundException($"The file {file.FullName} does not exist in the playlist.");
        }

        public void DetermineAudioTagAndAddToTagList(IPlaylistFile file)
        {
            foreach (var fd in tracks)
            {
                if (file.Name == fd.Name)
                {
                    var rdr = CommonPlaylistUtilities.DetermineAudioTag(fd);
                    if (rdr is null) { return; }
                    try {
                        // Anything can happen in the constructor so make sure that we will not fail indefinitely.
                        readtags[fd.Name] = new SavedDataTag(rdr);
                        metadata["NumberOfContainedAudioTags"] = readtags.Count;
                    } finally {
                        rdr.Dispose();
                    }
                    return;
                }
            }
            throw new System.IO.FileNotFoundException($"The file {file.FullName} does not exist in the playlist.");
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

        public System.Int32 GetCurrentTrackIndex()
        {
            System.Int32 idx = 0;
            if (lasttrack is null) { throw new ArgumentNullException("CurrentTrack", "The detector must know the track so as to return it's index."); }
            foreach (var fd in tracks)
            {
                if (fd.Name == lasttrack.Name) { return idx; }
                idx++;
            }
            return idx;
        }

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
                if (kvp.Key == file.Name) { return kvp.Value; }
            }
            return null;
        }

        public System.Boolean Remove(IPlaylistFile track)
        {
            IPlaylistFile file;
            for (System.Int32 I = 0; I < tracks.Count; I++)
            {
                file = tracks[I];
                if (file.Name == track.Name) { return Remove(I); }
            }
            return false;
        }

        public System.Boolean Remove(System.Int32 index)
        {
            try {
                // First , attempt to delete the file
                File.Delete(Path.Join(pdirfilessave.FullName , tracks[index].Name));
                // Then , remove the track's tag if any 
                if (readtags.Remove(tracks[index].Name))
                {
                    // Although that remains a statistic, we are ought to update it
                    metadata["NumberOfContainedAudioTags"] = readtags.Count;
                }
                // Finally , remove it from the track list and we're done.
                tracks.RemoveAt(index);
                return true;
            } catch { }
            return false;
        }

        public void Dispose()
        {
            if (tracks is not null)
            {
                tracks.Clear();
                tracks = null;
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
            pdirfilessave = null;
            pltfileinfo = null;
        }
    }
}