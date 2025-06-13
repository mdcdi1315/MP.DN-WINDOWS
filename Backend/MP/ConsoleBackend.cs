using System;
using Microsoft.IO;

namespace MP
{
    public static class ConsoleBackend
    {
        private static System.Boolean Created = false;
        private static ConsoleStream stream; 

        [System.Security.SecurityCritical]
        public static System.Boolean Create()
        {
            if (Created) { return true; }
            Created = Interop.Kernel32.AllocConsole() != Interop.BOOL.FALSE;
            if (Created) { stream = new(); }
            return Created;
        }

        [System.Security.SecurityCritical]
        public static System.Boolean Destroy()
        {
            if (Created == false) { return true; }
            if (Interop.Kernel32.FreeConsole() != Interop.BOOL.FALSE)
            {
                stream?.Dispose();
                stream = null;
                Created = false;
                return true;
            }
            return false;
        }

        [System.Security.SecurityCritical]
        public static System.Boolean WriteConsole(System.String text)
        {
            if (Created == false) { return true; }
            stream.WriteText(text);
            return true;
        }
        
        [System.Security.SecurityCritical]
        public static System.Boolean WriteConsoleLine(System.String text) => WriteConsole(text + "\r\n");

        [System.Security.SecurityCritical]
        public static System.String ReadConsole()
        {
            if (Created == false) { return System.String.Empty; }
            return stream.ReadLine();
        }

        public static System.String Title
        {
            [System.Security.SecurityCritical]
            get {
                if (Created == false) { return System.String.Empty; }
                return Interop.Kernel32.GetConsoleTitle();
            }
            [System.Security.SecurityCritical]
            set {
                if (Created == false) { return; }
                Interop.Kernel32.SetConsoleTitle(value);
            }
        }

        public static System.String OriginalTitle
        {
            [System.Security.SecurityCritical]
            get
            {
                if (Created == false) { return System.String.Empty; }
                return Interop.Kernel32.GetConsoleOriginalTitle();
            }
        }

        public static System.IntPtr WindowHandle
        {
            [System.Security.SecurityCritical]
            get => Interop.Kernel32.GetConsoleWindow();
        }

        [System.Security.SecurityCritical]
        [System.Diagnostics.DebuggerHidden]
        private static Interop.Kernel32.CONSOLE_SCREEN_BUFFER_INFO GetCSBI()
        {
            if (Created == false) { return default; }
            Interop.BOOL succ = Interop.Kernel32.GetConsoleScreenBufferInfo(stream.SafeOutputFileHandle.Handle, out Interop.Kernel32.CONSOLE_SCREEN_BUFFER_INFO csbi);
            if (succ == Interop.BOOL.FALSE) {
                succ = Interop.Kernel32.GetConsoleScreenBufferInfo(stream.SafeOutputFileHandle.Handle, out csbi);
            }
            if (succ == Interop.BOOL.FALSE) {
                throw new ExceptionSystem.NativeWindowsException();
            }
            return csbi;
        }

        public static ConsoleColor BackgroundColor
        {
            [System.Security.SecurityCritical]
            [System.Diagnostics.DebuggerHidden]
            get => ColorAttributeToConsoleColor(GetCSBI().Attributes);
            [System.Security.SecurityCritical]
            [System.Diagnostics.DebuggerHidden]
            set => SetColor(value, true);
        }

        public static ConsoleColor ForegroundColor
        {
            [System.Security.SecurityCritical]
            [System.Diagnostics.DebuggerHidden]
            get => ColorAttributeToConsoleColor(GetCSBI().Attributes);
            [System.Security.SecurityCritical]
            [System.Diagnostics.DebuggerHidden]
            set => SetColor(value, false);
        }

        [System.Security.SecurityCritical]
        [System.Diagnostics.DebuggerHidden]
        private static void SetColor(ConsoleColor c , System.Boolean background)
        {
            var csbi = GetCSBI();
            System.UInt16 attrs = csbi.Attributes;
            if (background)
            {
                attrs = (ushort)(attrs & -241);
                attrs = (ushort)(attrs | (ushort)ConsoleColorToColorAttribute(c, true));
            } else {
                attrs = (ushort)(attrs & -16);
                attrs = (ushort)(attrs | (ushort)ConsoleColorToColorAttribute(c, false));
            }
            Interop.Kernel32.SetConsoleTextAttribute(stream.SafeOutputFileHandle.Handle, attrs);
        }

        private static ConsoleControlChars ConsoleColorToColorAttribute(ConsoleColor color, bool isBackground)
        {
            if (((uint)color & 0xFFFFFFF0u) != 0)
            {
                throw new ArgumentException($"The Console Color specified , {color} , is invalid.");
            }
            ConsoleControlChars color2 = (ConsoleControlChars)color;
            if (isBackground)
            {
                color2 = (ConsoleControlChars)((System.Int32)color2 << 4);
            }
            return color2;
        }

        private static ConsoleColor ColorAttributeToConsoleColor(System.UInt16 c)
        {
            if ((c & (System.Int16)ConsoleControlChars.BackgroundMask) != 0)
            {
                c = ((System.Int32)c >> 4).ToUInt16();
            }
            return (ConsoleColor)c;
        }
    }
}
