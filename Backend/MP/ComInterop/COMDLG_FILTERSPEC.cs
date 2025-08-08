

using System.Runtime.InteropServices;

namespace MP.ComInterop
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct COMDLG_FILTERSPEC
    {
        public System.Char* FriendlyName;

        public System.Char* Filter;
    }
}