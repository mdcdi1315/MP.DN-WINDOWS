
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Collections.Generic;

namespace MP.BinaryPlaylist.MusicPlayer
{
    [StructLayout(LayoutKind.Explicit , Size = 33)]
    public unsafe struct PLAYLISTTRACK
    {
        [FieldOffset(0)]
        public System.UInt32 TrackNameStringBlob;

        [FieldOffset(4)]
        public System.Int32 TrackDataTag;

        [FieldOffset(8)]
        public System.Boolean IsLastTrack;

        [FieldOffset(9)]
        public System.Int64 TrackByteLength;

        [FieldOffset(17)]
        public System.Int64 TrackCreationTime;

        [FieldOffset(25)]
        public System.Int64 TrackLastModTime;
    }

    public sealed class PlaylistTracksWriter : PlaylistBlobWriter
    {
        private System.UInt32 tracks;

        internal PlaylistTracksWriter() { tracks = 0;  }

        public unsafe void WritePlaylistTrack(PLAYLISTTRACK trck)
        {
            WriteStructure(trck);
            tracks++;
        }

        public System.UInt32 NextTrackIndex => tracks;

        public void FinalizeWriter()
        {
            Header = new() { Count = tracks , Identifier1 = BlobFlags.Normal , Identifier2 = BlobTypes.TRACKBLOB , Version = 1 };
            IsCompleted = true;
        }
    }

    public sealed class PlaylistTracksReader : PlaylistBlobReader
    {
        private System.Byte[] shared;
        private Dictionary<System.UInt32, PLAYLISTTRACK> tracksfast;

        public unsafe PlaylistTracksReader() : base() { tracksfast = new(5); shared = new System.Byte[sizeof(PLAYLISTTRACK)]; }

        private unsafe PLAYLISTTRACK GetTrackPrivate(System.UInt32 id)
        {
            if (id >= Header.Count) { throw new System.ArgumentOutOfRangeException(nameof(id)); }
            Position = 0;
            for (System.UInt32 I = 0; I < Header.Count && I < id; I++)
            {
                Seek(sizeof(PLAYLISTTRACK) , System.IO.SeekOrigin.Current);
            }
            return ReadStructure<PLAYLISTTRACK>();
        }

        public PLAYLISTTRACK GetTrackAt(System.UInt32 index)
        {
            if (tracksfast.TryGetValue(index, out PLAYLISTTRACK track)) { return track; }
            if (tracksfast.Count > 5) { tracksfast.Clear(); }
            PLAYLISTTRACK ret = GetTrackPrivate(index);
            tracksfast.Add(index, ret);
            return ret;
        }

        public IEnumerable<PLAYLISTTRACK> GetAll()
        {
            PLAYLISTTRACK temp;
            Position = 0;
            for (System.UInt32 I = 0; I < Header.Count; I++)
            {
                temp = ReadStructure<PLAYLISTTRACK>();
                if (tracksfast.Count < 6) { tracksfast.Add(I, temp); }
                yield return temp;
            }
        }

        protected override void Dispose(bool disposing)
        {
            shared = null;
            tracksfast?.Clear();
            tracksfast = null;
            base.Dispose(disposing);
        }
    }
}
