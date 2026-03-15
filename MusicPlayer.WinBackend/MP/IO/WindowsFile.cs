
using System;
using MP.ExceptionSystem;
using MP.NativeInterop.Windows;
using System.Diagnostics.CodeAnalysis;

namespace MP.IO
{
    public sealed class WindowsFile : File
    {
        private System.IntPtr h_file;
        private readonly WindowsDirectory parent;

        private bool exists;
        private System.String name;
        private System.Int64 length;
        private FileAttributes attributes;
        private FileSystemObjectTimes times;

        public WindowsFile(WindowsDirectory parent, System.String file_name)
        {
            name = file_name;
            this.parent = parent;
            exists = false;
            length = 0L;
            attributes = FileAttributes.Normal;
            times = new();
            Refresh();
        }

        public override WindowsDirectory Parent => parent;

        public override bool Exists => exists;

        public override long Length => length;

        public override string Name => name;

        public override FileSystemObjectTimes Times 
        {
            get => times; 
            set {
                times = value;
                WindowsIOHelpers.ThrowIfErrorAsIOException(
                    Interop.NtDll.NtSetInformationFile(h_file, new Interop.NtDll.FILE_BASIC_INFORMATION() {
                        CreationTime = new(times.CreationTime),
                        ChangeTime = new(times.ModificationTime),
                        LastAccessTime = new(times.LastAccessTime),
                        LastWriteTime = Interop.LongFileTime.MinusOne,
                        FileAttributes = (Interop.FileAttributes)attributes
                    }, out _)
                );
            }
        }

        public override FileAttributes Attributes 
        { 
            get => attributes; 
            set {
                attributes = value;
                WindowsIOHelpers.ThrowIfErrorAsIOException(
                    Interop.NtDll.NtSetInformationFile(h_file, new Interop.NtDll.FILE_BASIC_INFORMATION() {
                        CreationTime = Interop.LongFileTime.MinusOne,
                        ChangeTime = Interop.LongFileTime.MinusOne,
                        LastAccessTime = Interop.LongFileTime.MinusOne,
                        LastWriteTime = Interop.LongFileTime.MinusOne,
                        FileAttributes = (Interop.FileAttributes)attributes
                    }, out _)
                );
            }
        }

        public unsafe override void CopyTo(File target, CopyMoveFileOptions options)
        {
            ArgumentNullException.ThrowIfNull(target);
            if (options.HasFlag(CopyMoveFileOptions.UseDotNetImplementationIfApplicable)) {
                using (var src = OpenRead())
                using (var dst = target.OpenWrite())
                {
                    src.DirectCopyToStream(dst, 4096);
                }
            } else {
                Interop.Kernel32.CopyFileExFlags flags = Interop.Kernel32.CopyFileExFlags.None;
                if (options.HasFlag(CopyMoveFileOptions.OverwriteFileAtDestWithoutFailing) == false)  {
                    flags |= Interop.Kernel32.CopyFileExFlags.COPY_FILE_FAIL_IF_EXISTS;
                }
                if (options.HasFlag(CopyMoveFileOptions.NoBuffering)) {
                    flags |= Interop.Kernel32.CopyFileExFlags.COPY_FILE_NO_BUFFERING;
                }
                if (Interop.Kernel32.CopyFileEx(WindowsFileSystem.CreateFullPath_Static(this), WindowsFileSystem.CreateFullPath_Static(target), flags) == BOOL.FALSE)
                {
                    uint err = Interop.Kernel32.GetLastError();
                    if (err == Interop.Errors.ERROR_FILE_EXISTS) {
                        throw new IOException("The target file is already existing.", new NativeWindowsException(err));
                    } else {
                        throw new IOException("An I/O error was occurred.", new NativeWindowsException(err));
                    }
                }
            }
        }

        public override bool Delete()
        {
            if (Interop.NtDll.NtSetInformationFile(h_file, new Interop.NtDll.FILE_DISPOSITION_INFORMATION() { DeleteFile = BOOLEAN.TRUE }, out _) == NTSTATUS.STATUS_SUCCESS) {
                exists = false;
                return true;
            } else {
                return false;
            }
        }

        public override void MoveTo(File target, CopyMoveFileOptions options)
        {
            CopyTo(target, options);
            Delete();
        }

        [return: NotNull]
        public override AbstractFileStream Open(FileMode mode, FileAccess access, FileShare share) => new WindowsFileStream(name, parent is null ? IntPtr.Zero : parent.Handle, mode, access, share);

        public override unsafe void Refresh()
        {
            if (h_file != IntPtr.Zero) { WindowsIOHelpers.CloseHandleOrThrow(h_file); }

            Interop.NtDll.CreateFileInfo cfi = Interop.NtDll.NtCreateFile(
                parent is null ? "\\??\\" + name : name,
                parent is null ? IntPtr.Zero : parent.Handle,
                Interop.NtDll.CreateDisposition.FILE_OPEN,
                Interop.NtDll.DesiredAccess.FILE_READ_EA | Interop.NtDll.DesiredAccess.FILE_READ_ATTRIBUTES | Interop.NtDll.DesiredAccess.FILE_WRITE_ATTRIBUTES,
                FileShare.Delete | FileShare.ReadWrite,
                0,
                Interop.NtDll.CreateOptions.FILE_NON_DIRECTORY_FILE
            );

            if (cfi.Status == NTSTATUS.STATUS_SUCCESS) {
                h_file = cfi.FileHandle;

                WindowsIOHelpers.ThrowIfErrorAsIOException(Interop.NtDll.NtQueryInformationFile(h_file, out Interop.NtDll.FILE_BASIC_INFORMATION basic, out _));
                attributes = (FileAttributes)basic.FileAttributes;
                times = new(basic.CreationTime.ToUtcDateTime(), basic.ChangeTime.ToUtcDateTime(), basic.LastAccessTime.ToUtcDateTime());
                WindowsIOHelpers.ThrowIfErrorAsIOException(Interop.NtDll.NtQueryInformationFile(h_file, out Interop.NtDll.FILE_STANDARD_INFORMATION fstd, out _));
                exists = fstd.Directory == BOOLEAN.FALSE;
                length = fstd.EndOfFile;
                if (!exists) {
                    WindowsIOHelpers.CloseHandleOrThrow(h_file);
                    h_file = IntPtr.Zero;
                }
            } else {
                exists = false;
                h_file = IntPtr.Zero;
            }
        }

        ~WindowsFile()
        {
            if (h_file != IntPtr.Zero) {
                DebugProvider.WriteLine($"WINDOWS_FILE: Disposing file handle {h_file}...");
                WindowsIOHelpers.CloseHandleOrThrow(h_file); 
                h_file = IntPtr.Zero;
            }
        }
    }
}