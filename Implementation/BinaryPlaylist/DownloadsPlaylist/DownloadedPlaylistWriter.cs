

using System;
using MP.BinaryPlaylist.MusicPlayer;

namespace MP.BinaryPlaylist.DownloadsPlaylist
{
    public sealed class DownloadsPlaylistWriter : IDisposable
    {
        private IPlaylist playlist;
        private PlaylistTrackTagsWriter tagswr;
        private PlaylistStringsWriter stringpool;
        private PlaylistDataWriter arraypool;
        private PlaylistMetadataBlobWriter ltwriter;
        private PlaylistTracksWriter trackwr;
        private PlaylistURLBlobWriter urlwriter;
        private BinaryPlaylistWriter writer;
        private System.Boolean gen, ltwritten;

        public DownloadsPlaylistWriter(System.IO.Stream target, IPlaylist playlist)
        {
            if (playlist is null) { throw new ArgumentNullException(nameof(playlist)); }
            this.playlist = playlist;
            writer = new(target, 6);
            trackwr = new();
            stringpool = new();
            ltwriter = new();
            arraypool = new();
            tagswr = new(arraypool, this.playlist); // Instead use the referenced field.
            urlwriter = new();
            gen = false;
            ltwritten = false;
        }

        public System.Boolean DisposeAfterUse
        {
            get => writer.DisposeAfterUse;
            set => writer.DisposeAfterUse = value;
        }

        public void AddFile(DownloadedFileInfo file)
        {
            if (gen) { return; }
            PLAYLISTTRACK track = new();
            track.TrackNameStringBlob = stringpool.WriteString(file.Name);
            // write it at this point so that the string blob can agree with the URL blob.
            urlwriter.WriteURL(file);
            if (playlist.CurrentTrack is null)
            {
                ltwriter.WriteCriticalMetadata(new() { CurrentTrackIndex = trackwr.NextTrackIndex, CurrentTrackStoppedTimeInTicks = 0 });
                ltwritten = true;
            }
            if (ltwritten == false && file.Name == playlist.CurrentTrack.Name) { 
                ltwritten = true;
                ltwriter.WriteCriticalMetadata(new() { 
                    CurrentTrackIndex = trackwr.NextTrackIndex, 
                    CurrentTrackStoppedTimeInTicks = playlist.CurrentTrackProcessedTime.Ticks
                });
            }
            track.TrackByteLength = file.Length;
            track.TrackLastModTime = file.LastWriteTimeUtc.ToFileTimeUtc();
            track.TrackCreationTime = file.CreationTimeUtc.ToFileTimeUtc();
            track.IsLastTrack = file.FullName == playlist.CurrentTrack?.FullName;
            track.TrackDataTag = tagswr.AddDataTag(track.TrackNameStringBlob, file);
            trackwr.WritePlaylistTrack(track);
        }

        public void AddMetadata(PlaylistMetadataItemCollection metadata) => ltwriter.AddMetadataItems(metadata);

        public void Generate()
        {
            if (gen) { return; }
            trackwr.FinalizeWriter();
            stringpool.FinalizeWriter();
            arraypool.FinalizeWriter();
            urlwriter.FinalizeWriter();
            tagswr.FinalizeWriter();
            ltwriter.FinalizeWriter();
            writer.Write(trackwr);
            writer.Write(ltwriter);
            writer.Write(stringpool);
            writer.Write(arraypool);
            writer.Write(urlwriter);
            writer.Write(tagswr);
            gen = true;
        }

        public void Dispose()
        {
            if (writer is null) { return; }
            if (gen == false) { throw new InvalidOperationException("The writers have not been saved yet! Call Generate immediately."); }
            urlwriter?.Dispose();
            urlwriter = null;
            writer.Dispose();
            writer = null;
            playlist = null;
            tagswr.Dispose();
            tagswr = null;
            stringpool.Dispose();
            stringpool = null;
            arraypool.Dispose();
            arraypool = null;
            ltwriter?.Dispose();
            ltwriter = null;
            trackwr?.Dispose();
            trackwr = null;
        }
    }
}