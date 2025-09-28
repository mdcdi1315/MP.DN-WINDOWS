


using System.Runtime.InteropServices;

namespace MP.PlaylistManagement.AudioTagSerialization
{
    [StructLayout(LayoutKind.Explicit , Size = 6)]
    internal struct PLAYLISTTRACKTAGENTRY
    {
        [FieldOffset(0)]
        public PlaylistTrackTagKey Key;
        [FieldOffset(2)]
        public System.UInt32 ValueLength;
    }
}