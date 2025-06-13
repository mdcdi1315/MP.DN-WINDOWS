
using System.Runtime.InteropServices;

partial class Interop
{

    public static class Console
    {
        [StructLayout(LayoutKind.Explicit, Size = 4 , Pack = 2)]
        public struct Coord
        {
            [FieldOffset(0)]
            public System.Int16 X;

            [FieldOffset(2)]
            public System.Int16 Y;
        }

        [StructLayout(LayoutKind.Explicit, Size = 8 , Pack = 2)]
        public struct Rectangle
        {
            [FieldOffset(0)]
            public System.Int16 Left;

            [FieldOffset(2)]
            public System.Int16 Top;

            [FieldOffset(4)]
            public System.Int16 Right;

            [FieldOffset(6)]
            public System.Int16 Bottom;
        }

        public enum ConsoleHandleOptions : System.UInt32 
        { 
            Input = 0xFFFFFFF6, 
            Output = 0xFFFFFFF5, 
            Error = 0xFFFFFFF4 
        }

        public enum DisplayModeFlags : System.Int32
        {
            FullScreen = 1,
            Windowed = 2,
        }
    }

    public static unsafe partial class Kernel32
    {
        public enum InputRecordType : System.UInt16
        {
            /// <summary>
            /// The Event member contains a FOCUS_EVENT_RECORD structure. These events are used internally and should be ignored.
            /// </summary>
            FOCUS_EVENT = 0x0010,
            /// <summary>
            /// The Event member contains a KEY_EVENT_RECORD structure with information about a keyboard event.
            /// </summary>
            KEY_EVENT = 0x0001,
            /// <summary>
            /// The Event member contains a MENU_EVENT_RECORD structure. These events are used internally and should be ignored.
            /// </summary>
            MENU_EVENT = 0x0008,
            /// <summary>
            /// The Event member contains a MOUSE_EVENT_RECORD structure with information about a mouse movement or button press event.
            /// </summary>
            MOUSE_EVENT = 0x0002,
            /// <summary>
            /// The Event member contains a WINDOW_BUFFER_SIZE_RECORD structure with information about the new size of the console screen buffer.
            /// </summary>
            WINDOW_BUFFER_SIZE_EVENT = 0x0004,
        }

        [StructLayout(LayoutKind.Explicit)]
        public unsafe struct INPUT_RECORD
        {
            [FieldOffset(0)]
            public InputRecordType Type;

            [FieldOffset(2)]
            public KeyRecord KeyEvent;

            [FieldOffset(2)]
            public FocusRecord FocusEvent;

            [FieldOffset(2)]
            public MenuRecord MenuEvent;

            [FieldOffset(2)]
            public MouseEventRecord MouseEvent;

            [FieldOffset(2)]
            public WindowBufferSizeRecord WindowSizeEvent;

            [StructLayout(LayoutKind.Explicit , Size = 16)]
            public struct KeyRecord
            {
                [FieldOffset(0)]
                public BOOL KeyDown;

                [FieldOffset(4)]
                public System.UInt16 RepeatCount;

                [FieldOffset(6)]
                public System.UInt16 VirtualKeyCode;

                [FieldOffset(8)]
                public System.UInt16 VirtualScanCode;

                [FieldOffset(10)]
                public System.Char UnicodeChar;

                [FieldOffset(10)]
                public System.Byte AsciiChar;

                [FieldOffset(12)]
                public System.UInt32 ControlKeyState;
            }

            [StructLayout(LayoutKind.Explicit , Size = 4)]
            public struct FocusRecord
            {
                [FieldOffset(0)]
                public BOOL SetFocus;
            }

            [StructLayout(LayoutKind.Explicit , Size = 4)]
            public struct MenuRecord
            {
                [FieldOffset(0)]
                public System.UInt32 CommandId;
            }

            [StructLayout(LayoutKind.Explicit , Size = 4)]
            public struct MouseEventRecord
            {
                [FieldOffset(0)]
                public Console.Coord MouseCoordinates;

                [FieldOffset(4)]
                public System.UInt32 ButtonState;

                [FieldOffset(8)]
                public System.UInt32 ControlKeyState;

                [FieldOffset(12)]
                public System.UInt32 EventFlags;
            }

            [StructLayout(LayoutKind.Explicit , Size = 4)]
            public struct WindowBufferSizeRecord
            {
                [FieldOffset(0)]
                public Console.Coord BufferSize;
            }
        }

        [StructLayout(LayoutKind.Explicit , Size = 22)]
        public unsafe struct CONSOLE_SCREEN_BUFFER_INFO
        {
            [FieldOffset(0)]
            public Console.Coord Size;

            [FieldOffset(4)]
            public Console.Coord CursorPosition;

            [FieldOffset(8)]
            public System.UInt16 Attributes;

            [FieldOffset(10)]
            public Console.Rectangle Window;

            [FieldOffset(18)]
            public Console.Coord MaximumWindowSize;
        }

        /// <summary>Allocates a new console for the calling process.</summary>
        [DllImport(Libraries.Kernel32 , ExactSpelling = true, SetLastError = true)]
        public static extern BOOL AllocConsole();

        [DllImport(Libraries.Kernel32 , ExactSpelling = true, SetLastError = true)]
        public static extern BOOL FreeConsole();

        [DllImport(Libraries.Kernel32 , EntryPoint = "PeekConsoleInputW", ExactSpelling = true, SetLastError = true)]
        private static extern BOOL PeekConsoleInput_Native(System.IntPtr consolein , INPUT_RECORD* records , System.UInt32 nrecords , System.UInt32* recordsread);

        public static BOOL PeekConsoleInputOneOnly(System.IntPtr consolein , out System.Boolean hasrecords , out INPUT_RECORD record)
        {
            System.UInt32 rrd;
            INPUT_RECORD rcd;
            BOOL ret = PeekConsoleInput_Native(consolein, &rcd, 1, &rrd);
            if (hasrecords = (rrd >= 1)) { record = rcd; } else { record = default; }
            return ret;
        }

        [DllImport(Libraries.Kernel32 , EntryPoint = "ReadConsoleInputW" , ExactSpelling = true, SetLastError = true)]
        private static extern BOOL ReadConsoleInput_Native(System.IntPtr consolein, INPUT_RECORD* records, System.UInt32 nrecords, System.UInt32* recordsread);

        public static BOOL ReadConsoleInputOneOnly(System.IntPtr consolein, out System.Boolean hasrecords, out INPUT_RECORD record)
        {
            System.UInt32 rrd;
            INPUT_RECORD rcd;
            BOOL ret = ReadConsoleInput_Native(consolein, &rcd, 1, &rrd);
            if (hasrecords = (rrd >= 1)) { record = rcd; } else { record = default; }
            return ret;
        }

        [DllImport(Libraries.Kernel32 , EntryPoint = "GetConsoleScreenBufferInfo" , ExactSpelling = true , SetLastError = true)]
        private static extern BOOL GetConsoleScreenBufferInfo_Native(System.IntPtr consoleoutput , CONSOLE_SCREEN_BUFFER_INFO* csbi);

        [DllImport(Libraries.Kernel32 , EntryPoint = "SetConsoleWindowInfo" , ExactSpelling = true , SetLastError = true)]
        private static extern BOOL SetConsoleWindowInfo_Native(System.IntPtr consoleoutput, BOOL Absolute, Console.Rectangle* NewData);

        [DllImport(Libraries.Kernel32 , EntryPoint = "GetConsoleTitleW" , ExactSpelling = true , SetLastError = true)]
        private static extern System.UInt32 GetConsoleTitle_Native(System.Char* buffer, System.Int32 size);

        [DllImport(Libraries.Kernel32 , EntryPoint = "GetConsoleOriginalTitleW" , ExactSpelling = true , SetLastError = true)]
        private static extern System.UInt32 GetConsoleOriginalTitle_Native(System.Char* buffer, System.Int32 size);

        [DllImport(Libraries.Kernel32 , EntryPoint = "SetConsoleTitleW" , ExactSpelling = true , SetLastError = true)]
        private static extern BOOL SetConsoleTitle_Native(System.Char* buffer);

        [DllImport(Libraries.Kernel32, ExactSpelling = true , SetLastError = true)]
        public static extern BOOL SetConsoleCursorPosition(System.IntPtr consoleout, Console.Coord position);

        [DllImport(Libraries.Kernel32 , ExactSpelling = true , SetLastError = true)]
        public static extern BOOL SetConsoleOutputCP(System.UInt32 codepage);

        [DllImport(Libraries.Kernel32 , ExactSpelling = true , SetLastError = true)]
        public static extern System.UInt32 GetConsoleOutputCP();

        [DllImport(Libraries.Kernel32 , ExactSpelling = true , SetLastError = true)]
        public static extern System.UInt32 GetConsoleCP();

        [DllImport(Libraries.Kernel32 , ExactSpelling = true , SetLastError = true)]
        public static extern BOOL SetConsoleCP(System.UInt32 codepage);

        [DllImport(Libraries.Kernel32 , EntryPoint = "SetConsoleDisplayMode" , ExactSpelling = true , SetLastError = true)]
        private static extern BOOL SetConsoleDisplayMode_Native(System.IntPtr consoleout , Console.DisplayModeFlags flags , Console.Coord* screenbufferdimensions);

        [DllImport(Libraries.Kernel32 , EntryPoint = "WriteConsoleOutputAttribute" ,  ExactSpelling = true , SetLastError = true)]
        private static extern BOOL WriteConsoleOutputAttribute_Native(System.IntPtr consoleout, System.Int32* attribute, System.Int32 length, Console.Coord firstcoord, System.UInt32* attributeswritten);

        [DllImport(Libraries.Kernel32 , ExactSpelling = true , SetLastError = true)]
        public static extern System.IntPtr GetConsoleWindow(); // HWND is returned from here

        [DllImport(Libraries.Kernel32 , ExactSpelling = true , SetLastError = true)]
        public static extern void SetConsoleTextAttribute(System.IntPtr consoleout, System.UInt16 attribute);

        [DllImport(Libraries.Kernel32, ExactSpelling = true, SetLastError = true)]
        public static extern System.IntPtr GetStdHandle(Console.ConsoleHandleOptions options);

        [DllImport(Libraries.Kernel32 , ExactSpelling = true , SetLastError = true)]
        private static extern BOOL GetNumberOfConsoleInputEvents_Native(System.IntPtr consolein , System.UInt32* nevents);

        public static BOOL GetNumberOfConsoleInputEvents(System.IntPtr consolein , out System.UInt32 nevents)
        {
            System.UInt32 ret;
            BOOL bret = GetNumberOfConsoleInputEvents_Native(consolein, &ret);
            nevents = ret;
            return bret;
        }

        public static BOOL WriteConsoleOutputAttribute(System.IntPtr consoleout , System.Int32 attribute , System.Int32 length , Console.Coord firstcoord , out System.UInt32 attributeswritten)
        {
            fixed (System.UInt32* pattrwr = &attributeswritten)
            {
                return WriteConsoleOutputAttribute_Native(consoleout, &attribute, length, firstcoord, pattrwr);
            }
        }

        public static BOOL SetConsoleDisplayMode(System.IntPtr consoleout, Console.DisplayModeFlags flags, out Console.Coord screenbufferdimensions) 
        {
            fixed (Console.Coord* dst = &screenbufferdimensions) 
            {
                return SetConsoleDisplayMode_Native(consoleout ,flags, dst);
            }
        }

        public static BOOL GetConsoleScreenBufferInfo(System.IntPtr consoleout, out CONSOLE_SCREEN_BUFFER_INFO csbi)
        {
            csbi = default;
            fixed (CONSOLE_SCREEN_BUFFER_INFO* ptr = &csbi) {
                return GetConsoleScreenBufferInfo_Native(consoleout, ptr);
            }
        }

        public static BOOL SetConsoleWindowInfo(System.IntPtr consoleout, BOOL Absolute, Console.Rectangle consolewindow)
            => SetConsoleWindowInfo_Native(consoleout, Absolute, &consolewindow);

        public static System.String GetConsoleTitle()
        {
            System.Char[] chr = new System.Char[256];
            System.UInt32 length;
            fixed (System.Char* ptr = chr) 
            {
                length = GetConsoleTitle_Native(ptr, chr.Length);
            }
            if (length == 0) {
                throw new MP.ExceptionSystem.NativeWindowsException();
            }
            System.Int32 len = GetNativeStringBounds(chr);
            return new(chr , 0 , len);
        }

        public static System.String GetConsoleOriginalTitle()
        {
            System.Char[] chr = new System.Char[256];
            System.UInt32 length;
            fixed (System.Char* ptr = chr)
            {
                length = GetConsoleOriginalTitle_Native(ptr, chr.Length);
            }
            if (length == 0)
            {
                throw new MP.ExceptionSystem.NativeWindowsException();
            }
            System.Int32 len = GetNativeStringBounds(chr);
            return new(chr, 0, len);
        }

        public static BOOL SetConsoleTitle(System.String title) 
        {
            if (title is null) { return BOOL.TRUE; }
            if (title.Length == 0) { return BOOL.TRUE; }
            if (title[title.Length - 1] != '\0') { title += "\0"; }
            fixed (System.Char* src = title) {
                return SetConsoleTitle_Native(src);
            }
        }

        private static System.Int32 GetNativeStringBounds(System.Char[] chars)
        {
            System.Int32 result = -1;
            for (System.Int32 I = chars.Length-1; I >= 0; I--) 
            {
                if (chars[I] == '\0') { result = I; break; }
            }
            return (result == -1) ? chars.Length : chars.Length - result;
        }
    }
}