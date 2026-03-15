
using System;
using MP.ExceptionSystem;
using MP.NativeInterop.Windows;

namespace MP.IO
{
    internal static class WindowsIOHelpers
    {
        public static void ThrowIfErrorAsIOException(NTSTATUS status)
        {
            if (status != NTSTATUS.STATUS_SUCCESS) {
                throw new IOException("An OS I/O error condition was encountered.", new NativeWindowsException(status));
            }
        }

        public static void CloseHandleOrThrow(IntPtr handle)
        {
            if (Interop.Kernel32.CloseHandle(handle) == BOOL.FALSE) {
                NativeWindowsException.ThrowFromLastError();
            }
        }
    }
}