
using System.Drawing;

namespace System.Windows.Forms
{
    public static class IWin32WindowExtensions
    {
        public static Point GetCursorPosition(this IWin32Window window) 
        {
            Interop.BOOL bl = Interop.User32.GetCursorPos(out var pptn);
            if (bl == Interop.BOOL.FALSE) {
                throw new MP.ExceptionSystem.NativeWindowsException();
            }
            bl = Interop.User32.ScreenToClient(window.Handle, pptn, out var fp);
            if (bl == Interop.BOOL.FALSE) {
                throw new MP.ExceptionSystem.NativeWindowsException();
            }
            return new(fp.X , fp.Y);
        }

        public static System.Boolean SetCursorPosition(this IWin32Window window, Point pptn) 
        {
            if (Interop.User32.ClientToScreen(window.Handle, new() { X = pptn.X, Y = pptn.Y }, out var ppt) 
                == Interop.BOOL.FALSE) {
                return false;
            }
            return Interop.User32.SetCursorPos(ppt.X, ppt.Y) != Interop.BOOL.FALSE;
        }

        public static Rectangle GetWindowBounds(this IWin32Window window) 
        {
            if (Interop.User32.GetClientRect(window.Handle , out var rct) == Interop.BOOL.FALSE) {
                throw new MP.ExceptionSystem.NativeWindowsException();
            }
            return new(new Point(rct.Left, rct.Top), new Size(rct.Right - rct.Left, rct.Bottom - rct.Top));
        }
    
        /// <summary>
        /// Applies the specified cursor image defined in <paramref name="cig"/> parameter. <br />
        /// When you do not want to use the specified cursor anymore, you must dispose it.
        /// </summary>
        /// <param name="cf">The <see cref="IWin32Window"/> to apply the cursor to</param>
        /// <param name="cig">The <see cref="CursorImage"/> to be applied as a cursor</param>
        /// <exception cref="ArgumentNullException"><paramref name="cig"/> was <see langword="null"/>.</exception>
        public static void ApplyCustomCursor(this IWin32Window cf , CursorImage cig)
        {
            if (cig is null) {
                throw new ArgumentNullException(nameof(cig));
            }
            // The below works for any class deriving Control:
            if (cf is Control ctl) {
                ctl.Cursor = new(cig.Handle);
                return;
            }
            // For all other cases we need SetClassLongPtr
            Interop.User32.SetClassLongPtr(cf.Handle , Interop.User32.SetClassLongIndex.GCLP_HCURSOR , cig.Handle);
            System.Int32 erc = Interop.Kernel32.GetLastError();
            if (erc != 0) {
                throw new MP.ExceptionSystem.NativeWindowsException(erc);
            }
            // There is a third and more flaky method, the SetCursor function.
            // However, that one sets globally the cursor to the entire screen, 
            // and it would require to call it each time the cursor leaves out of window.
            // So, SetClassLongPtr is becoming here handy.
        }
    }
}