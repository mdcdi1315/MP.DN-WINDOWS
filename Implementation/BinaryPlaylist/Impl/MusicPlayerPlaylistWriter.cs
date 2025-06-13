
using System;

namespace MP.BinaryPlaylist.MusicPlayer
{
    public sealed class MusicPlayerPlaylistWriter : IDisposable
    {
        private IPlaylist playlist;
        private PlaylistTrackTagsWriter tagswr;
        private PlaylistStringsWriter stringpool;
        private PlaylistDataWriter arraypool;
        private PlaylistMetadataBlobWriter ltwriter;
        private PlaylistTracksWriter trackwr;
        private PlaylistPreferencesWriter prefwr;
        private BinaryPlaylistWriter writer;
        private System.Boolean gen , ltwritten;

        public MusicPlayerPlaylistWriter(System.IO.Stream target , IPlaylist playlist)
        {
            if (playlist is null) { throw new ArgumentNullException(nameof(playlist)); }
            this.playlist = playlist;
            writer = new(target, 6);
            trackwr = new();
            stringpool = new();
            ltwriter = new();
            prefwr = new();
            arraypool = new();
            tagswr = new(arraypool , this.playlist); // Instead use the referenced field.
            gen = false;
            ltwritten = false;
        }

        public System.Boolean DisposeAfterUse
        {
            get => writer.DisposeAfterUse;
            set => writer.DisposeAfterUse = value;
        }

        public void AddFile(IPlaylistFile file)
        {
            if (gen) { return; }
            PLAYLISTTRACK track = new();
            track.TrackNameStringBlob = stringpool.WriteString(file.FullName);
            if (playlist.CurrentTrack is null)
            {
                ltwriter.WriteCriticalMetadata(new() { CurrentTrackIndex = trackwr.NextTrackIndex, CurrentTrackStoppedTimeInTicks = 0 });
                ltwritten = true;
            }
            if (ltwritten == false && file.FullName == playlist.CurrentTrack.FullName)
            {
                ltwriter.WriteCriticalMetadata(new() {
                    CurrentTrackIndex = trackwr.NextTrackIndex,
                    CurrentTrackStoppedTimeInTicks = playlist.CurrentTrackProcessedTime.Ticks
                });
                ltwritten = true;
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
            if (writer is null) { return; }
            if (gen) { return; }
            prefwr.WritePreferences(playlist.Preferences);
            trackwr.FinalizeWriter();
            stringpool.FinalizeWriter();
            arraypool.FinalizeWriter();
            tagswr.FinalizeWriter();
            prefwr.FinalizeWriter();
            ltwriter.FinalizeWriter();
            writer.Write(trackwr);
            writer.Write(prefwr);
            writer.Write(ltwriter);
            writer.Write(stringpool);
            writer.Write(arraypool);
            writer.Write(tagswr);
            gen = true;
        }

        public void Dispose()
        {
            if (writer is null) { return; }
            if (gen == false) { throw new InvalidOperationException("The writers have not been saved yet! Call Generate immediately."); }
            writer?.Dispose();
            writer = null;
            playlist = null;
            tagswr?.Dispose();
            tagswr = null;
            stringpool?.Dispose();
            stringpool = null;
            arraypool?.Dispose();
            arraypool = null;
            ltwriter?.Dispose();
            ltwriter = null;
            prefwr?.Dispose();
            prefwr = null;
            trackwr?.Dispose();
            trackwr = null;
        }
    }
}
