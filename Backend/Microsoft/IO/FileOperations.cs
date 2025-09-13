
using MP;
using System;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.IO
{
    // File operation functions, used for MPSafeFileHandle
    internal static unsafe class FileOperations
    {
        public static Interop.OVERLAPPED GetNativeOverlappedForSyncHandle(RedistSafeFileHandle handle, long fileOffset)
        {
            System.Diagnostics.Debug.Assert(!handle.IsAsync);

            Interop.OVERLAPPED result = new();
            if (handle.CanSeek)
            {
                result.Union.Offset = fileOffset.ToUInt32();
                result.Union.OffsetHigh = (fileOffset >> 32).ToUInt32();
            }
            return result;
        }

        private static bool IsEndOfFile(int errorCode, RedistSafeFileHandle handle, long fileOffset)
        {
            switch (errorCode)
            {
                case Interop.Errors.ERROR_HANDLE_EOF: // logically success with 0 bytes read (read at end of file)
                case Interop.Errors.ERROR_BROKEN_PIPE: // For pipes, ERROR_BROKEN_PIPE is the normal end of the pipe.
                case Interop.Errors.ERROR_PIPE_NOT_CONNECTED: // Named pipe server has disconnected, return 0 to match NamedPipeClientStream behaviour
                case Interop.Errors.ERROR_INVALID_PARAMETER when IsEndOfFileForNoBuffering(handle, fileOffset):
                    return true;
                default:
                    return false;
            }
        }

        // From https://docs.microsoft.com/en-us/windows/win32/fileio/file-buffering:
        // "File access sizes, including the optional file offset in the OVERLAPPED structure,
        // if specified, must be for a number of bytes that is an integer multiple of the volume sector size."
        // So if buffer and physical sector size is 4096 and the file size is 4097:
        // the read from offset=0 reads 4096 bytes
        // the read from offset=4096 reads 1 byte
        // the read from offset=4097 fails with ERROR_INVALID_PARAMETER (the offset is not a multiple of sector size)
        // Based on feedback received from customers (https://github.com/dotnet/runtime/issues/62851),
        // it was decided to not throw, but just return 0.
        private static bool IsEndOfFileForNoBuffering(RedistSafeFileHandle fileHandle, long fileOffset)
            => fileHandle.IsNoBuffering && fileHandle.CanSeek && fileOffset >= fileHandle.GetFileLength();

        public static System.Int32 ReadFileNativeUseOverlapped(RedistSafeFileHandle handle, Span<byte> bytes, Interop.OVERLAPPED overlapped, out int errorCode)
        {
            System.Diagnostics.Debug.Assert(handle is not null, "Check failed: handle != null");

            Interop.BOOL error;
            System.UInt32 numBytesRead = 0;
            fixed (byte* p = &System.Runtime.InteropServices.MemoryMarshal.GetReference(bytes))
            {
                error = Interop.Kernel32.ReadFile(handle.Handle, p, bytes.Length.ToUInt32(), overlapped, out numBytesRead);
            }

            if (error == Interop.BOOL.FALSE)
            {
                System.Int64 ofs = new System.IntPtr(overlapped.Union.Pointer).ToInt64();
                errorCode = Interop.Kernel32.GetLastError();
                // End-Of-File. Treat this as successfull call with the returned number of bytes to zero.
                if (IsEndOfFile(errorCode, handle, ofs))
                {
                    errorCode = 0;
                    return 0;
                }
                if (errorCode == Interop.Errors.ERROR_INVALID_HANDLE)
                {
                    handle.Dispose();
                }
                return -1;
            }
            else
            {
                errorCode = 0;
                return numBytesRead.ToInt32();
            }
        }

        public static System.Int32 ReadFileNative(RedistSafeFileHandle handle , Span<System.Byte> bytes , out System.Int32 errorcode)
        {
            System.Diagnostics.Debug.Assert(handle is not null, "Check failed: handle != null");

            Interop.BOOL error;
            Interop.OVERLAPPED overlapped = new();
            System.UInt32 numBytesRead = 0;
            fixed (byte* p = &System.Runtime.InteropServices.MemoryMarshal.GetReference(bytes))
            {
                error = Interop.Kernel32.ReadFile(handle.Handle, p, bytes.Length.ToUInt32(), &overlapped, out numBytesRead);
            }

            if (error == Interop.BOOL.FALSE)
            {
                System.Int64 ofs = new System.IntPtr(overlapped.Union.Pointer).ToInt64();
                errorcode = Interop.Kernel32.GetLastError();
                // End-Of-File. Treat this as successfull call with the returned number of bytes to zero.
                if (IsEndOfFile(errorcode, handle, ofs))
                {
                    errorcode = 0;
                    return 0;
                }
                if (errorcode == Interop.Errors.ERROR_INVALID_HANDLE)
                {
                    handle.Dispose();
                }
                return -1;
            }
            else
            {
                errorcode = 0;
                return numBytesRead.ToInt32();
            }
        }

        public static System.Int32 WriteFileNativeUseOverlapped(RedistSafeFileHandle handle, ReadOnlySpan<System.Byte> bytes, Interop.OVERLAPPED overlapped, out int error)
        {
            System.Diagnostics.Debug.Assert(handle is not null, "handle != null");

            Interop.BOOL err;
            System.UInt32 numBytesWritten = 0;
            fixed (byte* p = &System.Runtime.InteropServices.MemoryMarshal.GetReference(bytes))
            {
                err = Interop.Kernel32.WriteFile(handle.Handle, p, bytes.Length.ToUInt32(), overlapped, out numBytesWritten);
            }

            if (err == Interop.BOOL.FALSE)
            {
                error = Interop.Kernel32.GetLastError();
                if (error == Interop.Errors.ERROR_INVALID_HANDLE)
                {
                    handle.Dispose();
                }
                return -1;
            }
            else
            {
                error = 0;
                return numBytesWritten.ToInt32();
            }
        }

        public static System.Int32 WriteFileNative(RedistSafeFileHandle handle , ReadOnlySpan<System.Byte> bytes , out System.Int32 error)
        {
            System.Diagnostics.Debug.Assert(handle is not null, "handle != null");

            Interop.BOOL err;
            System.UInt32 numBytesWritten = 0;
            fixed (byte* p = &System.Runtime.InteropServices.MemoryMarshal.GetReference(bytes))
            {
                err = Interop.Kernel32.WriteFile(handle.Handle, p, bytes.Length.ToUInt32(), null, out numBytesWritten);
            }

            if (err == Interop.BOOL.FALSE)
            {
                error = Interop.Kernel32.GetLastError();
                if (error == Interop.Errors.ERROR_INVALID_HANDLE)
                {
                    handle.Dispose();
                }
                return -1;
            }
            else
            {
                error = 0;
                return numBytesWritten.ToInt32();
            }
        }
    
        public static System.Int32 ReadFileNativeUseNtDll(RedistSafeFileHandle handle, Span<System.Byte> bytes, out System.Int32 errorcode)
        {
            System.Diagnostics.Debug.Assert(handle is not null, "Check failed: handle != null");

            Interop.NtDll.IO_STATUS_BLOCK blk;
            fixed (System.Byte* pd = bytes)
            {
                errorcode = Interop.NtDll.RtlNtStatusToDosError(Interop.NtDll.NtReadFile(handle.Handle, IntPtr.Zero, pd, bytes.Length.ToUInt32(), out blk)).ToInt32();
            }
            return blk.Information.ToInt32();
        }

        public static System.Int32 WriteFileNativeUseNtDll(RedistSafeFileHandle handle, ReadOnlySpan<System.Byte> bytes, out System.Int32 errorcode)
        {
            System.Diagnostics.Debug.Assert(handle is not null, "Check failed: handle != null");

            Interop.NtDll.IO_STATUS_BLOCK blk;
            fixed (System.Byte* pd = bytes)
            {
                errorcode = Interop.NtDll.RtlNtStatusToDosError(Interop.NtDll.NtWriteFile(handle.Handle, IntPtr.Zero, pd, bytes.Length.ToUInt32(), out blk)).ToInt32();
            }
            return blk.Information.ToInt32();
        }
    }
}