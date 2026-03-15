using System.Runtime.InteropServices;

namespace MP.NativeInterop.Windows.COM
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct COMDLG_FILTERSPEC
    {
        public char* FriendlyName;

        public char* Filter;
    }
}