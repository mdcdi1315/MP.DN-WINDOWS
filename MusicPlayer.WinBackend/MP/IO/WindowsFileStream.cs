
using System;
using MP.NativeInterop.Windows;
using Microsoft.Win32.SafeHandles;

namespace MP.IO
{
    public unsafe sealed class WindowsFileStream : AbstractFileStream
    {
        private readonly SafeHandles.WindowsFileStreamHandle handle;

        public WindowsFileStream(System.String file_name, FileMode mode, FileAccess access) : this(file_name, IntPtr.Zero, mode, access, FileShare.None) {}

        public WindowsFileStream(System.String file_name, FileMode mode, FileAccess access, FileShare share) : this(file_name, IntPtr.Zero, mode, access, share) { }

        internal WindowsFileStream(System.String file_name, IntPtr root_dir, FileMode mode, FileAccess access, FileShare share)
        {
            ArgumentNullException.ThrowIfNull(file_name);
            handle = new(file_name, root_dir, mode, share, access);
        }

        public override AbstractFileStreamHandle SafeFileHandle => handle;

        public override int Read(Span<byte> buffer)
        {
            NTSTATUS s;
            Interop.NtDll.IO_STATUS_BLOCK block;
            fixed (System.Byte* p = buffer)
            {
                s = Interop.NtDll.NtReadFile(
                    handle.HandleValue,
                    IntPtr.Zero,
                    p,
                    buffer.Length.ToUInt32(),
                    out block
                );
            }
            if (s == NTSTATUS.STATUS_END_OF_FILE) {
                return -1;
            } else {
                WindowsIOHelpers.ThrowIfErrorAsIOException(s);
                return block.Information.ToUInt32().ToInt32();
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            ValidateBufferArguments(buffer, offset, count);
            NTSTATUS s;
            Interop.NtDll.IO_STATUS_BLOCK block;
            fixed (System.Byte* p = &buffer[offset])
            {
                s = Interop.NtDll.NtReadFile(
                    handle.HandleValue,
                    IntPtr.Zero,
                    p,
                    count.ToUInt32(),
                    out block
                );
            }
            if (s == NTSTATUS.STATUS_END_OF_FILE) {
                return -1;
            } else {
                WindowsIOHelpers.ThrowIfErrorAsIOException(s);
                return block.Information.ToUInt32().ToInt32();
            }
        }

        public override void Write(ReadOnlySpan<byte> buffer)
        {
            NTSTATUS s;
            Interop.NtDll.IO_STATUS_BLOCK block;
            fixed (System.Byte* p = buffer)
            {
                s = Interop.NtDll.NtWriteFile(
                    handle.HandleValue,
                    IntPtr.Zero,
                    p,
                    buffer.Length.ToUInt32(),
                    out block
                );
            }
            WindowsIOHelpers.ThrowIfErrorAsIOException(s);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            ValidateBufferArguments(buffer, offset, count);
            NTSTATUS s;
            Interop.NtDll.IO_STATUS_BLOCK block;
            fixed (System.Byte* p = &buffer[offset])
            {
                s = Interop.NtDll.NtWriteFile(
                    handle.HandleValue,
                    IntPtr.Zero,
                    p,
                    count.ToUInt32(),
                    out block
                );
            }
            WindowsIOHelpers.ThrowIfErrorAsIOException(s);
        }

        public override short ReadByte()
        {
            byte pb;
            Interop.NtDll.IO_STATUS_BLOCK block;
            NTSTATUS s = Interop.NtDll.NtReadFile(handle.HandleValue, IntPtr.Zero, &pb, 1U, out block);
            if (s == NTSTATUS.STATUS_END_OF_FILE) {
                return -1;
            } else {
                WindowsIOHelpers.ThrowIfErrorAsIOException(s);
                return pb;
            }
        }

        public override void WriteByte(byte value) => WindowsIOHelpers.ThrowIfErrorAsIOException(Interop.NtDll.NtWriteFile(handle.HandleValue, IntPtr.Zero, &value, 1U, out _));
    }
}
