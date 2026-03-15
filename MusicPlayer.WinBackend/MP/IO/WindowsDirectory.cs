
using System;
using MP.NativeInterop.Windows;
using System.Collections.Generic;

namespace MP.IO
{
    public sealed partial class WindowsDirectory : Directory
    {
        private IntPtr h_file;
        private readonly WindowsDirectory parent;

        private bool exists;
        private System.String name;
        private System.Int64 length;
        private FileAttributes attributes;
        private FileSystemObjectTimes times;

        private WindowsDirectory()
        {
            exists = false;
            name = null;
            length = 0L;
            attributes = FileAttributes.Normal;
            times = new();
            parent = null;
            h_file = IntPtr.Zero;
        }

        public WindowsDirectory(string root_dir_name) : this()
        {
            name = root_dir_name;
            Refresh();
        }

        public unsafe WindowsDirectory(string dir_name, WindowsDirectory parent) : this()
        {
            name = dir_name;
            this.parent = parent;
            Interop.NtDll.CreateFileInfo cfi = Interop.NtDll.NtCreateFile(
                (parent is null) ? "\\??\\" + name : name,
                (parent is null) ? IntPtr.Zero : parent.h_file,
                Interop.NtDll.CreateDisposition.FILE_OPEN_IF,
                Interop.NtDll.DesiredAccess.FILE_READ_EA | Interop.NtDll.DesiredAccess.FILE_READ_ATTRIBUTES | Interop.NtDll.DesiredAccess.FILE_WRITE_ATTRIBUTES | Interop.NtDll.DesiredAccess.FILE_LIST_DIRECTORY | Interop.NtDll.DesiredAccess.FILE_ADD_SUBDIRECTORY | Interop.NtDll.DesiredAccess.FILE_TRAVERSE | Interop.NtDll.DesiredAccess.DELETE | Interop.NtDll.DesiredAccess.FILE_DELETE_CHILD,
                FileShare.Delete | FileShare.ReadWrite,
                0,
                Interop.NtDll.CreateOptions.FILE_DIRECTORY_FILE
            );
            Init(cfi);
        }

        private WindowsDirectory(IntPtr handle) :this()
        {
            h_file = handle;
            Refresh();
        }

        public override string Name => name;

        public override bool Exists => exists;

        public override long Length => length;

        public override Directory Parent => parent;

        public override FileSystemObjectTimes Times
        {
            get => times;
            set
            {
                times = value;
                WindowsIOHelpers.ThrowIfErrorAsIOException(
                    Interop.NtDll.NtSetInformationFile(h_file, new Interop.NtDll.FILE_BASIC_INFORMATION()
                    {
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
            set
            {
                attributes = value;
                WindowsIOHelpers.ThrowIfErrorAsIOException(
                    Interop.NtDll.NtSetInformationFile(h_file, new Interop.NtDll.FILE_BASIC_INFORMATION()
                    {
                        CreationTime = Interop.LongFileTime.MinusOne,
                        ChangeTime = Interop.LongFileTime.MinusOne,
                        LastAccessTime = Interop.LongFileTime.MinusOne,
                        LastWriteTime = Interop.LongFileTime.MinusOne,
                        FileAttributes = (Interop.FileAttributes)attributes
                    }, out _)
                );
            }
        }

        public override void CopyTo(Directory target, CopyMoveDirectoryOptions options)
        {
            ArgumentNullException.ThrowIfNull(target);

        }

        public override bool Delete(bool recursive)
        {
            if (recursive) {
                // We need to have the delete subdirectories right to do that:
                WindowsIOHelpers.ThrowIfErrorAsIOException(Interop.NtDll.NtQueryInformationFile(h_file, out Interop.NtDll.FILE_ACCESS_INFORMATION f, out _));
                if (!f.AccessFlags.HasFlag(Interop.NtDll.DesiredAccess.FILE_DELETE_CHILD)) {
                    throw new UnauthorizedAccessException("The current object does not have access to delete the directory.");
                }
            }
            exists = false;
            return Interop.NtDll.NtSetInformationFile(h_file, new Interop.NtDll.FILE_DISPOSITION_INFORMATION() { DeleteFile = BOOLEAN.TRUE }, out _) == NTSTATUS.STATUS_SUCCESS;
        }

        public override void MoveTo(Directory target, CopyMoveDirectoryOptions options)
        {
            CopyTo(target, options);
            Delete(options.HasFlag(CopyMoveDirectoryOptions.RecursiveCopy));
        }

        public override IEnumerable<FileSystemObject> GetObjects(bool recursive = false)
        {
            throw new NotImplementedException();
        }

        public IntPtr Handle => h_file;

        public override unsafe void Refresh()
        {
            if (h_file != IntPtr.Zero) { WindowsIOHelpers.CloseHandleOrThrow(h_file); }

            Interop.NtDll.CreateFileInfo cfi = Interop.NtDll.NtCreateFile(
                (parent is null) ? "\\??\\" + name : name,
                (parent is null) ? IntPtr.Zero : parent.h_file,
                Interop.NtDll.CreateDisposition.FILE_OPEN,
                Interop.NtDll.DesiredAccess.FILE_READ_EA | Interop.NtDll.DesiredAccess.FILE_READ_ATTRIBUTES | Interop.NtDll.DesiredAccess.FILE_WRITE_ATTRIBUTES | Interop.NtDll.DesiredAccess.FILE_LIST_DIRECTORY | Interop.NtDll.DesiredAccess.FILE_ADD_SUBDIRECTORY | Interop.NtDll.DesiredAccess.FILE_TRAVERSE | Interop.NtDll.DesiredAccess.DELETE,
                FileShare.Delete | FileShare.ReadWrite,
                0,
                Interop.NtDll.CreateOptions.FILE_DIRECTORY_FILE
            );

            Init(cfi);
        }

        private void Init(Interop.NtDll.CreateFileInfo cfi)
        {
            if (cfi.Status == NTSTATUS.STATUS_SUCCESS) {
                h_file = cfi.FileHandle;

                WindowsIOHelpers.ThrowIfErrorAsIOException(Interop.NtDll.NtQueryInformationFile(h_file, out name));
                WindowsIOHelpers.ThrowIfErrorAsIOException(Interop.NtDll.NtQueryInformationFile(h_file, out Interop.NtDll.FILE_BASIC_INFORMATION basic, out _));
                attributes = (FileAttributes)basic.FileAttributes;
                times = new(basic.CreationTime.ToUtcDateTime(), basic.ChangeTime.ToUtcDateTime(), basic.LastAccessTime.ToUtcDateTime());
                WindowsIOHelpers.ThrowIfErrorAsIOException(Interop.NtDll.NtQueryInformationFile(h_file, out Interop.NtDll.FILE_STANDARD_INFORMATION fstd, out _));
                exists = fstd.Directory != BOOLEAN.FALSE;
                length = fstd.EndOfFile;
                if (!exists)
                {
                    WindowsIOHelpers.CloseHandleOrThrow(h_file);
                    h_file = IntPtr.Zero;
                }
            } else {
                exists = false;
                h_file = IntPtr.Zero;
            }
        }

        public override WindowsDirectory Subdirectory(string name)
        {
            ArgumentNullException.ThrowIfNull(name);
            return new WindowsDirectory(name, this);
        }

        ~WindowsDirectory()
        {
            if (h_file != IntPtr.Zero) {
                DebugProvider.WriteLine($"WINDOWS_FILE: Disposing file handle {h_file}...");
                WindowsIOHelpers.CloseHandleOrThrow(h_file);
                h_file = IntPtr.Zero;
            }
        }
    }
}