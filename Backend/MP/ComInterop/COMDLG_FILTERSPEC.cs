

using System.Runtime.InteropServices;

namespace MP.ComInterop
{
    [StructLayout(LayoutKind.Explicit , Pack = 1)]
    public unsafe struct COMDLG_FILTERSPEC
    {
        [FieldOffset(0)]
        public System.Char* FriendlyName;

        [FieldOffset(8)]
        public System.Char* Filter;
    }
}