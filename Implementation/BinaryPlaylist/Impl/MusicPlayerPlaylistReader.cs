using System;
using System.Collections.Generic;

namespace MP.BinaryPlaylist.MusicPlayer
{
    public sealed class MusicPlayerPlaylistReader : IDisposable
    {
        private BinaryPlaylistReader reader;
        private Dictionary<BlobTypes, PlaylistBlobReader> readerspooled;

        public static System.Boolean IsBinaryPlaylist(System.IO.Stream stream)
        {
            System.Boolean result = BinaryPlaylistHelpers.VerifyIdentifier(stream);
            stream.Seek(-BinaryPlaylistHelpers.IdentifierLength, System.IO.SeekOrigin.Current);
            return result;
        }

        public MusicPlayerPlaylistReader(System.IO.Stream stream) 
        {
            reader = new BinaryPlaylistReader(stream);
            readerspooled = new((System.Int32)BlobTypes.MAXEMBEDDEDBLOBVAL - 1);
        }

        public System.Boolean DisposeAfterUse
        {
            get => reader.DisposeAfterUse;
            set => reader.DisposeAfterUse = value;
        }

        private T GetReader<T>(BlobTypes type) where T : PlaylistBlobReader, new()
        {
            PlaylistBlobReader inst = null;
            // Determine if this reader is already pooled , and if yes return it.
            if (readerspooled.TryGetValue(type, out inst)) { return inst as T; }
            for (System.Int32 I = 0; I < reader.BlobCount; I++) 
            {
                inst = reader.GetReader<T>(I);
                if (inst.Header.Identifier2 == type) { readerspooled.Add(type, inst); break; }
                // Dispose the reader if it is not the proper one
                inst?.Dispose();
                inst = null;
            }
            return inst as T;
        }

        /// <summary>
        /// Gets a custom playlist blob written by third-party tools , 
        /// casted to a derived instance of <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The derived class of <see cref="PlaylistBlobReader"/> class.</typeparam>
        /// <param name="type">The custom blob type. It must be outside the embedded types range.</param>
        /// <returns>The custom blob reader defined as <typeparamref name="T"/>.</returns>
        public T GetCustomReader<T>(BlobTypes type) 
            where T : PlaylistBlobReader, new()
        {
            if (type <= BlobTypes.MAXEMBEDDEDBLOBVAL) { throw new ArgumentOutOfRangeException(nameof(type) , "The reader type must be outside the embedded types range."); }
            return GetReader<T>(type);
        }

        public PlaylistPreferencesReader PreferencesReader => GetReader<PlaylistPreferencesReader>(BlobTypes.PREFBLOB);

        public PlaylistStringsReader StringsReader => GetReader<PlaylistStringsReader>(BlobTypes.STRINGBLOB);

        public PlaylistTracksReader TracksReader => GetReader<PlaylistTracksReader>(BlobTypes.TRACKBLOB);

        public PlaylistTrackTagsReader TagsReader => GetReader<PlaylistTrackTagsReader>(BlobTypes.TAGBLOB);

        public PlaylistDataReader DataReader => GetReader<PlaylistDataReader>(BlobTypes.DATABLOB);

        public PlaylistMetadataBlobReader MetadataReader => GetReader<PlaylistMetadataBlobReader>(BlobTypes.METADATABLOB);

        public void Dispose()
        {
            if (reader is null) { return; }
            reader?.Dispose();
            reader = null;
            foreach (var rdr in readerspooled.Values) { rdr.Dispose(); }
            readerspooled.Clear();
            readerspooled = null;
        }
    }
}
