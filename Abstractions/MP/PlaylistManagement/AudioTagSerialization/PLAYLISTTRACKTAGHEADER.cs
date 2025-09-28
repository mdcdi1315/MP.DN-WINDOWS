

using System.Runtime.InteropServices;

namespace MP.PlaylistManagement.AudioTagSerialization
{
    [StructLayout(LayoutKind.Explicit , Size = 6)]
    internal struct PLAYLISTTRACKTAGHEADER
    {
        [FieldOffset(0)]
        public System.Byte Header0;
        [FieldOffset(1)]
        public System.Byte Header1;
        [FieldOffset(2)]
        public System.Byte Header2;
        [FieldOffset(3)]
        public System.Byte Version;
        [FieldOffset(4)]
        public System.UInt16 NumberOfEntries;

        public readonly System.Boolean IsValidHeader => Header0 == 77 && Header1 == 80 && Header2 == 84;

        public PLAYLISTTRACKTAGHEADER() {
            Header0 = 77;
            Header1 = 80;
            Header2 = 84;
            Version = 1;
        }
    }
}