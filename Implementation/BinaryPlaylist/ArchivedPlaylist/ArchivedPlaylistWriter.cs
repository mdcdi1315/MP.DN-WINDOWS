

using System;
using MP.BinaryPlaylist.MusicPlayer;

namespace MP.BinaryPlaylist.ArchivedPlaylist
{
    public sealed class ArchivedPlaylistWriter : IDisposable
    {
        private IPlaylist playlist;
        private ArchivedPlaylistAttributeBlobWriter attrwr;
        private PlaylistTrackTagsWriter tagswr;
        private PlaylistStringsWriter stringpool;
        private PlaylistDataWriter arraypool;
        private PlaylistMetadataBlobWriter ltwriter;
        private PlaylistTracksWriter trackwr;
        private BinaryPlaylistWriter writer;
        private ArchivedPlaylistImageBlobWriter imgwr;
        private PlaylistPreferencesWriter prefwr;
        private System.Boolean gen, ltwritten;

        public ArchivedPlaylistWriter(System.IO.Stream target, IPlaylist playlist)
        {
            if (playlist is null) { throw new ArgumentNullException(nameof(playlist)); }
            writer = new(target, 8);
            this.playlist = playlist;
            trackwr = new();
            stringpool = new();
            ltwriter = new();
            arraypool = new();
            prefwr = new();
            attrwr = new();
            imgwr = new();
            tagswr = new(arraypool, this.playlist); // Instead use the referenced field.
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
            track.TrackNameStringBlob = stringpool.WriteString(file.Name);
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
            track.IsLastTrack = file.Name == playlist.CurrentTrack?.Name;
            track.TrackDataTag = tagswr.AddDataTag(track.TrackNameStringBlob, file);
            trackwr.WritePlaylistTrack(track);
        }

        public void ProvidePlaylistImage(System.IO.Stream strm) => imgwr.WriteFromStream(strm);

        public void WriteAttributes(ArchivedPlaylistAttributeCollection attributes) => attrwr.WriteAttributes(attributes);

        public void Generate()
        {
            if (gen) { return; }
            prefwr.WritePreferences(playlist.Preferences);
            prefwr.FinalizeWriter();
            trackwr.FinalizeWriter();
            stringpool.FinalizeWriter();
            arraypool.FinalizeWriter();
            tagswr.FinalizeWriter();
            ltwriter.FinalizeWriter();
            attrwr.FinalizeWriter();
            writer.Write(trackwr);
            writer.Write(prefwr);
            writer.Write(attrwr);
            writer.Write(ltwriter);
            writer.Write(stringpool);
            writer.Write(arraypool);
            writer.Write(imgwr);
            writer.Write(tagswr);
            gen = true;
        }

        public void Dispose()
        {
            if (writer is null) { return; }
            if (gen == false) { throw new InvalidOperationException("The writers have not been saved yet! Call Generate immediately."); }
            writer.Dispose();
            writer = null;
            playlist = null;
            attrwr.Dispose();
            attrwr = null;
            prefwr.Dispose();
            prefwr = null;
            tagswr.Dispose();
            tagswr = null;
            imgwr.Dispose();
            imgwr = null;
            stringpool.Dispose();
            stringpool = null;
            arraypool.Dispose();
            arraypool = null;
            ltwriter.Dispose();
            ltwriter = null;
            trackwr.Dispose();
            trackwr = null;
        }
    }
}