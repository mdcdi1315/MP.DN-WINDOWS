

using System.Runtime.InteropServices;

partial class Interop
{
    partial class Kernel32
    {
        [DllImport(Libraries.Kernel32 , ExactSpelling = true)]
        public static extern BOOL IsValidCodePage(System.UInt32 codepage);
    }
}