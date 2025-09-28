

using System.Runtime.InteropServices;

namespace MP.PlaylistManagement.Caching
{
    [StructLayout(LayoutKind.Explicit , Size = 4)]
    internal struct TAGSCACHESSTATICENTRYV1
    {
        [FieldOffset(0)]
        public System.Byte Reserved;
        [FieldOffset(1)]
        public System.Byte CookieCharLength;
        [FieldOffset(2)]
        public System.UInt16 FileNameLength;
    }
}