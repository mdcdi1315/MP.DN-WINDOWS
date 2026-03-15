
using MP.NativeInterop.Windows;
using System.Runtime.InteropServices;

partial class Interop
{
    public static unsafe class OleAut32
    {
        // BSTR API

        [DllImport(Libraries.OleAut32, ExactSpelling = true)]
        public static extern /* BSTR */ System.Char* SysAllocStringLen(/* OLECHAR* */ System.Char* pstring, System.UInt32 length);

        [DllImport(Libraries.OleAut32, ExactSpelling = true)]
        public static extern void SysFreeString(/* BSTR */ System.Char* pstring);

        [DllImport(Libraries.OleAut32, ExactSpelling = true)]
        public static extern System.UInt32 SysStringLen(/* BSTR */ System.Char* pstring);

        [DllImport(Libraries.OleAut32, ExactSpelling = true)]
        public static extern BOOL SysReAllocStringLen(/* BSTR* */ System.Char** pinstring, /* OLECHAR* */ System.Char* olestringcpy, System.UInt32 length);

        // End BSTR API
    }
}