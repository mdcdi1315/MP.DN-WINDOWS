using System;
using System.Runtime.InteropServices;

// For SetClassLongPtrW there are two other types too:
// LONG_PTR -> System.Int64
// ULONG_PTR -> System.UInt64

partial class Interop
{
    public static unsafe class User32
    {
        [Flags]
        public enum IconCreationFlags : System.UInt32
        {
            /// <summary>
            /// Uses the default color format.
            /// </summary>
            LR_DEFAULTCOLOR = 0x00000000,
            /// <summary>
            /// Uses the width or height specified by the system metric values for cursors or icons, if the cxDesired or cyDesired values are set to zero. 
            /// If this flag is not specified and cxDesired and cyDesired are set to zero, the function uses the actual resource size.
            /// </summary>
            LR_DEFAULTSIZE = 0x00000040,
            /// <summary>
            /// Creates a monochrome icon or cursor.
            /// </summary>
            LR_MONOCHROME = 0x00000001,
            /// <summary>
            /// Shares the icon or cursor handle if the icon or cursor is created multiple times. <br />
            /// If <see cref="LR_SHARED"/> is not set, a second call to <see cref="User32.CreateIconOrCursorExtended"/> for the same resource will create the icon or cursor again and return a different handle. <br />
            /// When you use this flag, the system will destroy the resource when it is no longer needed. <br />
            /// Do not use <see cref="LR_SHARED"/> for icons or cursors that have non-standard sizes, that may change after loading, or that are loaded from a file.
            /// </summary>
            LR_SHARED = 0x00008000
        }

        [Flags]
        public enum ImageCopyFlags : System.UInt32
        {
            /// <summary>
            /// Deletes the original image after creating the copy.
            /// </summary>
            LR_COPYDELETEORG = 0x00000008,
            /// <summary>
            /// Tries to reload an icon or cursor resource from the original resource file rather than simply copying the current image. <br />
            /// This is useful for creating a different-sized copy when the resource file contains multiple sizes of the resource. <br />
            /// Without this flag, CopyImage stretches the original image to the new size. <br />
            /// If this flag is set, CopyImage uses the size in the resource file closest to the desired size. <br />
            /// This will succeed only if hImage was loaded by LoadIcon or LoadCursor, or by LoadImage with the LR_SHARED flag. <br />
            /// </summary>
            LR_COPYFROMRESOURCE = 0x00004000,
            /// <summary>
            /// Returns the original hImage if it satisfies the criteria for the copy—that is, correct dimensions and color depth—in which case the <see cref="LR_COPYDELETEORG"/> flag is ignored. 
            /// If this flag is not specified, a new object is always created.
            /// </summary>
            LR_COPYRETURNORG = 0x00000004,
            /// <summary>
            /// If this is set and a new bitmap is created, the bitmap is created as a DIB section.
            /// Otherwise, the bitmap image is created as a device-dependent bitmap.
            /// This flag is only valid if uType is <see cref="ImageCopyType.IMAGE_BITMAP"/>.
            /// </summary>
            LR_CREATEDIBSECTION = 0x00002000,
            /// <summary>
            /// Uses the default color format.
            /// </summary>
            LR_DEFAULTCOLOR = 0x00000000,
            /// <summary>
            /// Uses the width or height specified by the system metric values for cursors or icons, if the cxDesired or cyDesired values are set to zero.  <br />
            /// If this flag is not specified and cxDesired and cyDesired are set to zero, the function uses the actual resource size. <br />
            /// If the resource contains multiple images, the function uses the size of the first image.
            /// </summary>
            LR_DEFAULTSIZE = 0x00000040,
            /// <summary>
            /// Creates a new monochrome image.
            /// </summary>
            LR_MONOCHROME = 0x00000001
        }

        public enum ImageCopyType : System.UInt32
        {
            IMAGE_BITMAP = 0,
            IMAGE_ICON,
            IMAGE_CURSOR
        }

        [Flags]
        public enum MessageBoxFlags : System.UInt32
        {
            #region Buttons
                MB_ABORTRETRYIGNORE = 0x00000002,
                MB_CANCELTRYCONTINUE = 0x00000006,
                MB_HELP = 0x00004000,
                MB_OK = 0x00000000,
                MB_OKCANCEL = 0x00000001,
                MB_RETRYCANCEL = 0x00000005,
                MB_YESNO = 0x00000004,
                MB_YESNOCANCEL = 0x00000003,
            #endregion
            #region Icons
                MB_ICONEXCLAMATION = 0x00000030,
                MB_ICONWARNING = 0x00000030,
                MB_ICONINFORMATION = 0x00000040,
                MB_ICONASTERISK = 0x00000040,
                MB_ICONQUESTION = 0x00000020,
                MB_ICONSTOP = 0x00000010,
                MB_ICONHAND = 0x00000010,
            #endregion
            #region Modality
                MB_APPLMODAL = 0x00000000,
                MB_SYSTEMMODAL =0x00001000,
                MB_TASKMODAL = 0x00002000,
            #endregion
            #region Other
                MB_DEFAULT_DESKTOP_ONLY = 0x00020000,
                MB_RIGHT = 0x00080000,
                MB_RTLREADING = 0x00100000,
                MB_TOPMOST = 0x00040000,
                MB_SERVICE_NOTIFICATION = 0x00200000,
            #endregion
        }

        public enum MessageBoxButton : System.Int32
        {
            Error = 0,
            IDABORT = 3,
            IDCANCEL = 2,
            IDCONTINUE = 11,
            IDIGNORE = 5,
            IDNO = 7,
            IDOK = 1,
            IDRETRY = 4,
            IDYES = 6
        }

        public enum SetClassLongIndex : System.Int32
        {
            GCLP_HCURSOR = -12,
            GCLP_HICON = -14,
            GCLP_HICONSM = -34
        }

        [StructLayout(LayoutKind.Explicit , Size = 8 , Pack = 4)]
        public struct POINT
        {
            [FieldOffset(0)]
            public System.Int32 X;

            [FieldOffset(4)]
            public System.Int32 Y;
        }

        [StructLayout(LayoutKind.Explicit , Size = 16 , Pack = 4)]
        public struct RECT
        {
            [FieldOffset(0)]
            public System.Int32 Left;

            [FieldOffset(4)]
            public System.Int32 Top;

            [FieldOffset(8)]
            public System.Int32 Right;

            [FieldOffset(12)]
            public System.Int32 Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ICONINFO
        {
            public BOOL IsIcon;
            public System.UInt32 XHotspot;
            public System.UInt32 YHotspot;
            public System.IntPtr HMaskBitmap;
            public System.IntPtr HColorBitmap;
        }

        // Note that the version parameter must be greater than or equal to 0x00020000 and less or equal than 0x00030000.
        // For iconorcursor parameter , define TRUE to create icons ,
        // or FALSE to create a cursor instead.
        [DllImport(Libraries.User32, EntryPoint = "CreateIconFromResourceEx", ExactSpelling = true, SetLastError = true)]
        public static unsafe extern System.IntPtr CreateIconOrCursorExtended(
            System.Byte* pdata,
            System.UInt32 size,
            BOOL iconorcursor,
            System.UInt32 version,
            System.Int32 cxdesired,
            System.Int32 cydesired,
            IconCreationFlags flags);

        [DllImport(Libraries.User32, EntryPoint = "CopyImage", ExactSpelling = true, SetLastError = true)]
        public static extern System.IntPtr CopyImage(
            System.IntPtr image,
            ImageCopyType type,
            System.Int32 cx,
            System.Int32 cy,
            ImageCopyFlags flags);

        [DllImport(Libraries.User32 , EntryPoint = "SetClassLongPtrW" , ExactSpelling = true, SetLastError = true)]
        public static extern System.UInt64 SetClassLongPtr(System.IntPtr hwnd , SetClassLongIndex index , System.Int64 pnewval);

        [DllImport(Libraries.User32, EntryPoint = "DestroyCursor", ExactSpelling = true, SetLastError = true)]
        public static extern BOOL DestroyCursor(System.IntPtr handle);

        [DllImport(Libraries.User32, EntryPoint = "ClientToScreen", ExactSpelling = true, SetLastError = true)]
        private static extern BOOL ClientToScreen_Native(System.IntPtr hwnd , POINT* ppoint);

        [DllImport(Libraries.User32, EntryPoint = "ScreenToClient", ExactSpelling = true, SetLastError = true)]
        private static extern BOOL ScreenToClient_Native(System.IntPtr hwnd,  POINT* ppoint);

        [DllImport(Libraries.User32, EntryPoint = "GetCursorPos", ExactSpelling = true, SetLastError = true)]
        private static extern BOOL GetCursorPos_Native(POINT* ppos);

        [DllImport(Libraries.User32, EntryPoint = "GetClientRect", ExactSpelling = true, SetLastError = true)]
        private static extern BOOL GetClientRect_Native(System.IntPtr hwnd , RECT* prect);

        [DllImport(Libraries.User32, EntryPoint = "MessageBoxExW", ExactSpelling = true, SetLastError = true)]
        private static extern MessageBoxButton MessageBox_Native(System.IntPtr hwnd, System.Char* text, System.Char* caption, MessageBoxFlags type, System.UInt16 langid);

        [DllImport(Libraries.User32, EntryPoint = "SetCursorPos", ExactSpelling = true, SetLastError = true)]
        public static extern BOOL SetCursorPos(System.Int32 x, System.Int32 y);

        [DllImport(Libraries.User32 , ExactSpelling = true)]
        public static extern System.IntPtr GetDC(System.IntPtr hwnd);

        // If the DC was released, the ReleaseDC returns 1.
        [DllImport(Libraries.User32 , ExactSpelling = true)]
        public static extern System.Int32 ReleaseDC(System.IntPtr hwnd , System.IntPtr hdcacquired);

        [DllImport(Libraries.User32 , ExactSpelling = true)]
        public static extern System.IntPtr GetForegroundWindow();

        [DllImport(Libraries.User32 , EntryPoint = "CreateIconIndirect", ExactSpelling = true)]
        private static extern System.IntPtr CreateIconIndirect_Native(ICONINFO* piconinfo);

        public static System.IntPtr CreateIconIndirect(ICONINFO iconinfo) => CreateIconIndirect_Native(&iconinfo);

        public static BOOL ClientToScreen(System.IntPtr hwnd , POINT inpoint , out POINT outpoint)
        {
            POINT ppt = inpoint;
            BOOL ret = ClientToScreen_Native(hwnd, &ppt);
            outpoint = ppt;
            return ret;
        }

        public static BOOL ScreenToClient(System.IntPtr hwnd, POINT inpoint, out POINT outpoint)
        {
            POINT ppt = inpoint;
            BOOL ret = ScreenToClient_Native(hwnd , &ppt);
            outpoint = ppt;
            return ret;
        }

        public static BOOL GetCursorPos(out POINT ppt)
        {
            POINT point;
            BOOL ret = GetCursorPos_Native(&point);
            ppt = point;
            return ret;
        }

        public static BOOL GetClientRect(System.IntPtr hwnd, out RECT rct)
        {
            RECT rect;
            BOOL ret = GetClientRect_Native(hwnd, &rect);
            rct = rect;
            return ret;
        }

        public static unsafe MessageBoxButton MessageBox(System.IntPtr hwnd , System.String text , System.String title , MessageBoxFlags flags)
        {
            fixed (System.Char* ptext = text)
            fixed (System.Char* ptitle = title)
            {
                return MessageBox_Native(hwnd, ptext, ptitle, flags, 0);
            }
        }
    }
}