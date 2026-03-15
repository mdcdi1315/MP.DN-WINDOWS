using System;
using MP.Annotations;
using MP.NativeInterop.Windows;
using System.Runtime.InteropServices;

partial class Interop
{
    public static unsafe partial class Kernel32
    {
        [Flags]
        private enum MessageFormatFlags : System.Int32
        {
            FORMAT_MESSAGE_IGNORE_INSERTS = 0x00000200,
            FORMAT_MESSAGE_FROM_HMODULE = 0x00000800,
            FORMAT_MESSAGE_FROM_SYSTEM = 0x00001000,
            FORMAT_MESSAGE_ARGUMENT_ARRAY = 0x00002000,
            FORMAT_MESSAGE_ALLOCATE_BUFFER = 0x00000100,
        }

        public enum ThreadErrorMode : System.UInt32
        {
            None = 0,
            /// <summary>
            /// The system does not display the critical-error-handler message box. <br />
            /// Instead, the system sends the error to the calling thread. <br />
            /// Best practice is that all applications call the process-wide SetErrorMode function with a parameter of <see cref="SEM_FAILCRITICALERRORS"/> at startup. <br />
            /// This is to prevent error mode dialogs from hanging the application.
            /// </summary>
            SEM_FAILCRITICALERRORS = 0x0001,
            /// <summary>
            /// The system does not display the Windows Error Reporting dialog.
            /// </summary>
            SEM_NOGPFAULTERRORBOX = 0x0002,
            /// <summary>
            /// The OpenFile function does not display a message box when it fails to find a file. <br />
            /// Instead, the error is returned to the caller. <br />
            /// This error mode overrides the OF_PROMPT flag.
            /// </summary>
            SEM_NOOPENFILEERRORBOX = 0x8000
        }

        [DllImport(Libraries.Kernel32 , ExactSpelling = true)]
        public static extern System.UInt32 GetLastError();

        [DllImport(Libraries.Kernel32 , ExactSpelling = true)]
        public static extern void SetLastError(System.UInt32 code);

        [AssignsLastError]
        [DllImport(Libraries.Kernel32, EntryPoint = "FormatMessageW", ExactSpelling = true)]
        private static extern System.Int32 FormatMessage(
            MessageFormatFlags dwFlags,
            IntPtr lpSource,
            System.UInt32 dwMessageId,
            System.Int32 dwLanguageId,
            void* lpBuffer,
            System.Int32 nSize,
            IntPtr arguments);

        [AssignsLastError]
        [DllImport(Libraries.Kernel32, EntryPoint = "SetThreadErrorMode", ExactSpelling = true)]
        private static extern BOOL SetThreadErrorMode_Native(ThreadErrorMode dwNewMode, ThreadErrorMode* lpOldMode);

        public static BOOL SetThreadErrorMode(ThreadErrorMode newmode , out ThreadErrorMode oldmode)
        {
            ThreadErrorMode temold;
            BOOL ret = SetThreadErrorMode_Native(newmode, &temold);
            oldmode = temold;
            return ret;
        }

        /// <summary>
        /// Returns a string message for the specified Win32 error code.
        /// </summary>
        public static string GetMessage(System.UInt32 errorCode) => GetMessage(errorCode, IntPtr.Zero);

        public static string GetMessage(System.UInt32 errorCode, IntPtr moduleHandle)
        {
            MessageFormatFlags flags =
                MessageFormatFlags.FORMAT_MESSAGE_IGNORE_INSERTS |
                MessageFormatFlags.FORMAT_MESSAGE_FROM_SYSTEM |
                MessageFormatFlags.FORMAT_MESSAGE_ARGUMENT_ARRAY;
            if (moduleHandle != IntPtr.Zero) {
                flags |= MessageFormatFlags.FORMAT_MESSAGE_FROM_HMODULE;
            }

            // First try to format the message into the stack based buffer.  Most error messages willl fit.
            Span<char> stackBuffer = stackalloc char[256]; // arbitrary stack limit
            fixed (char* bufferPtr = stackBuffer)
            {
                int length = FormatMessage(flags, moduleHandle, errorCode, 0, bufferPtr, stackBuffer.Length, IntPtr.Zero);
                if (length > 0) {
                    return GetAndTrimString(stackBuffer.Slice(0, length));
                }
            }

            // We got back an error.  If the error indicated that there wasn't enough room to store
            // the error message, then call FormatMessage again, but this time rather than passing in
            // a buffer, have the method allocate one, which we then need to free.
            if (GetLastError() == Errors.ERROR_INSUFFICIENT_BUFFER)
            {
                void* nativeMsgPtr = null;
                try {
                    int length = FormatMessage(flags | MessageFormatFlags.FORMAT_MESSAGE_ALLOCATE_BUFFER, moduleHandle, errorCode, 0, &nativeMsgPtr, 0, IntPtr.Zero);
                    if (length > 0) {
                        return GetAndTrimString(new Span<char>((char*)nativeMsgPtr, length));
                    }
                } finally {
                    if (LocalFree(nativeMsgPtr) is not null) {
                        Environment.FailFast($"Windows error code 0x{GetLastError()} occured while trying to free a native error message block with LocalFree.");
                    }
                }
            }

            // Couldn't get a message, so craft one.
            return string.Format("Unknown error (0x{0:x})", errorCode);
        }
    }
}