
using MP.Imaging;
using System.Runtime.InteropServices;

partial class Interop
{
    public static unsafe class Gdi32
    {
        public enum DIBSECTION_USAGE : System.UInt32
        {
            DIB_RGB_COLORS = 0,
            DIB_PAL_COLORS = 1
        }

        [DllImport(Libraries.Gdi32, SetLastError = true , ExactSpelling = true)]
        public static extern /* HBITMAP */ System.IntPtr CreateDIBSection(
                /* HDC */ System.IntPtr hdc,
                BITMAPINFOHEADER* pheader,
                DIBSECTION_USAGE usage,
                void** ppbitmapbits,
                System.IntPtr hsectionnotused,
                System.UInt32 hsecofsnotused
        );

        [DllImport(Libraries.Gdi32 , ExactSpelling = true)]
        public static extern BOOL DeleteObject(/* HGDIOBJ */ System.IntPtr hObject);

        [DllImport(Libraries.Gdi32 , ExactSpelling = true , SetLastError = true)]
        public static extern /* HBITMAP */ System.IntPtr CreateBitmap(
            System.Int32 width , 
            System.Int32 height , 
            System.UInt32 planes ,
            System.UInt32 bitcount , 
            void* ppbits
        );
    }
}