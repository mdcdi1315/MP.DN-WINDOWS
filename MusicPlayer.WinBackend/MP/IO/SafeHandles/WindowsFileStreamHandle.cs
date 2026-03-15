
using System;
using MP.ExceptionSystem;
using MP.NativeInterop.Windows;
using Microsoft.Win32.SafeHandles;

namespace MP.IO.SafeHandles
{
    public unsafe sealed class WindowsFileStreamHandle : AbstractFileStreamHandle
    {
        private readonly FileMode mode;
        private readonly FileShare share;

        public WindowsFileStreamHandle(System.String file_path, IntPtr root_dir, FileMode mode, FileShare share, FileAccess access)
        {
            Interop.NtDll.DesiredAccess access_native =
                Interop.NtDll.DesiredAccess.SYNCHRONIZE;

            if (mode == FileMode.Append) {
                access_native |= Interop.NtDll.DesiredAccess.FILE_APPEND_DATA;
            }

            if (access.HasFlag(FileAccess.Read)) {
                access_native |= Interop.NtDll.DesiredAccess.FILE_READ_DATA;
            }

            if (access.HasFlag(FileAccess.Write)) {
                access_native |= Interop.NtDll.DesiredAccess.FILE_WRITE_DATA;
            } else if (mode == FileMode.Append) {
                throw new ArgumentException("File appending cannot be specified if a read-only access to the file is desired.");
            }

            Interop.NtDll.CreateFileInfo cfi = Interop.NtDll.NtCreateFile(
                file_path.AsSpan(),
                root_dir,
                mode switch {
                    FileMode.CreateNew => Interop.NtDll.CreateDisposition.FILE_CREATE,
                    FileMode.Create => Interop.NtDll.CreateDisposition.FILE_OVERWRITE_IF,
                    FileMode.Open => Interop.NtDll.CreateDisposition.FILE_OPEN,
                    FileMode.OpenOrCreate => Interop.NtDll.CreateDisposition.FILE_OPEN_IF,
                    FileMode.Truncate => Interop.NtDll.CreateDisposition.FILE_OVERWRITE,
                    FileMode.Append => Interop.NtDll.CreateDisposition.FILE_OPEN_IF,
                    _ => throw new ArgumentException("Invalid file mode: " + mode)
                },
                access_native,
                share
            );

            NativeWindowsException.ThrowIfError(cfi.Status);
            this.mode = mode;
            this.share = share;
            handle = cfi.FileHandle;

            if (mode == FileMode.Append && cfi.StatusBlock.Information == Interop.NtDll.FILE_EXISTS) { Seek(0, SeekDisplacement.End); }
        }

        public override FileMode Mode => mode;

        public override FileShare Share => share;

        public override DataStreamMode StreamMode 
        {
            get {
                // We need to get FILE_ACCESS_INFORMATION for this to work, though.
                Interop.NtDll.IO_STATUS_BLOCK block;
                NativeWindowsException.ThrowIfError(
                    Interop.NtDll.NtQueryInformationFile(handle, out Interop.NtDll.FILE_ACCESS_INFORMATION info, out block)
                );
                DataStreamMode mode = DataStreamMode.Seek; // By default, OS file handles MUST be able to seek.
                if (info.AccessFlags.HasFlag(Interop.NtDll.DesiredAccess.FILE_READ_DATA))
                {
                    mode |= DataStreamMode.Read;
                }
                if (info.AccessFlags.HasFlag(Interop.NtDll.DesiredAccess.FILE_WRITE_DATA))
                {
                    mode |= DataStreamMode.Write;
                }
                return mode;
            }
        }

        public override void Flush()
        {
            // There is the NtFlushFileBuffersEx function, but it is only defined in Windows 8+ systems. 
            // If we are on 7, we simulate the call through Kernel32's FlushFileBuffers.
            NTSTATUS s = Interop.NtDll.NtFlushFileBuffersEx(handle, Interop.NtDll.NtFlushFileBuffersFlags.FLUSH_FLAGS_FILE_DATA_ONLY, out var block);
            // STATUS_ACCESS_DENIED is returned when the data stream mode is read-only.
            // To follow the Flush conventions, we will eat the error and just give up instead.
            if (s == NTSTATUS.STATUS_ACCESS_DENIED && !StreamMode.HasFlag(DataStreamMode.Write)) {
                return;
            } else {
                NativeWindowsException.ThrowIfError(s);
            }
        }

        private long QueryFilePositionInternal()
        {
            Interop.NtDll.IO_STATUS_BLOCK block;
            NativeWindowsException.ThrowIfError(
                Interop.NtDll.NtQueryInformationFile(handle, out Interop.NtDll.FILE_POSITION_INFORMATION fpi, out block)
            );
            return fpi.CurrentByteOffset;
        }

        private long QueryFileLengthInternal()
        {
            Interop.NtDll.IO_STATUS_BLOCK block;
            NativeWindowsException.ThrowIfError(
                Interop.NtDll.NtQueryInformationFile(
                    handle,
                    out Interop.NtDll.FILE_STANDARD_INFORMATION finfo,
                    out block
                )
            );
            return finfo.EndOfFile;
        }

        public System.IntPtr HandleValue => handle;

        public override long Length => QueryFileLengthInternal();

        public override long Position => QueryFilePositionInternal();

        public override long Seek(long offset, SeekDisplacement origin)
        {
            long new_position;
            Interop.NtDll.IO_STATUS_BLOCK block;
            switch (origin)
            {
                case SeekDisplacement.Begin:
                    NativeWindowsException.ThrowIfError(
                        Interop.NtDll.NtSetInformationFile(handle, new Interop.NtDll.FILE_POSITION_INFORMATION() { CurrentByteOffset = offset }, out block)
                    );
                    return offset;
                case SeekDisplacement.Current:
                    new_position = QueryFilePositionInternal() + offset;
                    NativeWindowsException.ThrowIfError(
                        Interop.NtDll.NtSetInformationFile(handle, new Interop.NtDll.FILE_POSITION_INFORMATION() { CurrentByteOffset = new_position }, out block)
                    );
                    return new_position;
                case SeekDisplacement.End:
                    new_position = QueryFileLengthInternal() + offset;
                    NativeWindowsException.ThrowIfError(
                        Interop.NtDll.NtSetInformationFile(handle, new Interop.NtDll.FILE_POSITION_INFORMATION() { CurrentByteOffset = new_position }, out block)
                    );
                    return new_position;
                default:
                    throw new ArgumentException("Invalid seek displacement: " + origin);
            }
        }

        public override void SetLength(long value)
        {
            // This call is equivalent as setting the end-of-file marker of the file.
            Interop.NtDll.IO_STATUS_BLOCK block;
            NativeWindowsException.ThrowIfError(
                Interop.NtDll.NtSetInformationFile(handle, new Interop.NtDll.FILE_END_OF_FILE_INFORMATION() { EndOfFile = value }, out block)
            );
        }

        public string Name
        {
            get {
                // We need to get FILE_NAME_INFORMATION for this to work, though.
                NativeWindowsException.ThrowIfError(
                    Interop.NtDll.NtQueryInformationFile(handle, out string fn)
                );
                return fn;
            }
        }

        protected override bool ReleaseHandle()
        {
            // Closing a handle is done through Kernel32's CloseHandle function. NtClose is superseded by this one.
            BOOL b = Interop.Kernel32.CloseHandle(handle);
            handle = IntPtr.Zero;
            return b != BOOL.FALSE;
        }
    }
}