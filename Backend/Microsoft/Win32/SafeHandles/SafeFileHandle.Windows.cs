// Portions of code are acquired from .NET Runtime:
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

// Modified by mdcdi1315 for use under the Music Player Implementation.

using MP;
using System;
using Microsoft.IO;

namespace Microsoft.Win32.SafeHandles
{
    /// <summary>
    /// Creates and manages file handles for the Music Player app. <br />
    /// For real-world usage of such handles see the <see cref="FileStream"/> class.
    /// </summary>
    public sealed class RedistSafeFileHandle : CriticalHandleZeroOrMinusOneIsInvalid
    {
        internal const FileOptions NoBuffering = (FileOptions)0x20000000;
        private bool owns; // Critical handles cannot implement handle owner logic , so we pseudo-implement it.
        private string path;
        private long _length; // negative means that hasn't been fetched.
        private volatile int _fileType;
        private bool _lengthCanBeCached; // file has been opened for reading and not shared for writing.
        private volatile FileOptions _fileOptions;

        private RedistSafeFileHandle() : base() 
        {
            path = null;
            _length = -1;
            _fileType = -1;
            _fileOptions = (FileOptions)(-1);
            owns = false;
        }

        /// <summary>
        /// Creates a <see cref="RedistSafeFileHandle" /> around a file handle.
        /// </summary>
        /// <param name="preexistingHandle">Handle to wrap</param>
        /// <param name="ownsHandle">Whether to control the handle lifetime</param>
        public RedistSafeFileHandle(IntPtr preexistingHandle, bool ownsHandle) : this()
        {
            owns = ownsHandle;
            SetHandle(preexistingHandle);
        }

        public static unsafe RedistSafeFileHandle Open(string fullPath, FileMode mode, FileAccess access, FileShare share, FileOptions options, long preallocationSize)
        {
            using (DisableMediaInsertionPrompt.Create())
            {
                // we don't use NtCreateFile as there is no public and reliable way
                // of converting DOS to NT file paths (RtlDosPathNameToRelativeNtPathName_U_WithStatus is not documented)
                RedistSafeFileHandle fileHandle = CreateFile(fullPath, mode, access, share, options);

                if (preallocationSize > 0)
                {
                    Preallocate(fullPath, preallocationSize, fileHandle);
                }

                return fileHandle;
            }
        }

        private static unsafe RedistSafeFileHandle CreateFile(string fullPath, FileMode mode, FileAccess access, FileShare share, FileOptions options)
        {
            Interop.Kernel32.SECURITY_ATTRIBUTES secAttrs = default;
            if ((share & FileShare.Inheritable) != 0)
            {
                secAttrs = new Interop.Kernel32.SECURITY_ATTRIBUTES
                {
                    nLength = (uint)sizeof(Interop.Kernel32.SECURITY_ATTRIBUTES),
                    bInheritHandle = Interop.BOOL.TRUE
                };
            }

            int fAccess =
                ((access & FileAccess.Read) == FileAccess.Read ? Interop.Kernel32.GenericOperations.GENERIC_READ : 0) |
                ((access & FileAccess.Write) == FileAccess.Write ? Interop.Kernel32.GenericOperations.GENERIC_WRITE : 0);

            // Our Inheritable bit was stolen from Windows, but should be set in
            // the security attributes class.  Don't leave this bit set.
            share &= ~FileShare.Inheritable;

            // Must use a valid Win32 constant here...
            if (mode == FileMode.Append)
            {
                mode = FileMode.OpenOrCreate;
            }

            int flagsAndAttributes = (int)options;

            // For mitigating local elevation of privilege attack through named pipes
            // make sure we always call CreateFile with SECURITY_ANONYMOUS so that the
            // named pipe server can't impersonate a high privileged client security context
            // (note that this is the effective default on CreateFile2)
            flagsAndAttributes |= Interop.Kernel32.SecurityOptions.SECURITY_SQOS_PRESENT;
            flagsAndAttributes |= Interop.Kernel32.SecurityOptions.SECURITY_ANONYMOUS;

            RedistSafeFileHandle fileHandle = Interop.Kernel32.CreateFile(fullPath, fAccess, share, ref secAttrs, mode, flagsAndAttributes, IntPtr.Zero);
            if (fileHandle.IsInvalid)
            {
                // Return a meaningful exception with the full path.

                // NT5 oddity - when trying to open "C:\" as a Win32FileStream,
                // we usually get ERROR_PATH_NOT_FOUND from the OS.  We should
                // probably be consistent w/ every other directory.
                int errorCode = Interop.Kernel32.GetLastError();

                if (errorCode == Interop.Errors.ERROR_PATH_NOT_FOUND && fullPath!.Length == System.IO.PathInternal.GetRootLength(fullPath))
                {
                    errorCode = Interop.Errors.ERROR_ACCESS_DENIED;
                }

                fileHandle.Dispose();
                throw System.IO.Win32Marshal.GetExceptionForWin32Error(errorCode, fullPath);
            }

            fileHandle.path = fullPath;
            fileHandle._fileOptions = options;
            fileHandle._lengthCanBeCached = (share & FileShare.Write) == 0 && (access & FileAccess.Write) == 0;
            return fileHandle;
        }

        private static unsafe void Preallocate(string fullPath, long preallocationSize, RedistSafeFileHandle fileHandle)
        {
            Interop.Kernel32.FILE_ALLOCATION_INFO allocationInfo = new() {
                AllocationSize = preallocationSize
            };

            // There is not a corresponding FILE_ALLOCATION_INFO in driver mode, so use the Kernel32 API instead.
            if (Interop.Kernel32.SetFileInformationByHandle(fileHandle.handle , allocationInfo) == Interop.BOOL.FALSE)
            {
                int errorCode = Interop.Kernel32.GetLastError();

                // Only throw for errors that indicate there is not enough space.
                if (errorCode == Interop.Errors.ERROR_DISK_FULL ||
                    errorCode == Interop.Errors.ERROR_FILE_TOO_LARGE)
                {
                    fileHandle.Dispose();

                    // Delete the file we've created.
                    Interop.Kernel32.DeleteFile(fullPath);

                    throw new System.IO.IOException(System.String.Format(errorCode == Interop.Errors.ERROR_DISK_FULL
                                                        ? SR.IO_DiskFull_Path_AllocationSize
                                                        : SR.IO_FileTooLarge_Path_AllocationSize,
                                            fullPath, preallocationSize));
                }
            }
        }

        private Interop.NtDll.FILE_ID_INFORMATION GetFileIdInformation()
        {
            EnsureValid();
            Interop.NTSTATUS nts;
            if ((nts = Interop.NtDll.NtQueryInformationFile(handle, out Interop.NtDll.FILE_ID_INFORMATION idi, out _)) != Interop.NTSTATUS.STATUS_SUCCESS)
            {
                // Get a Win32 error for the NTSTATUS , and return that as the exception
                throw System.IO.Win32Marshal.GetExceptionForNtStatus(nts, path);
            }
            return idi;
        }

        private System.Int32 GetFileType()
        {
            if (_fileType == -1)
            {
                EnsureValid();
                var ft = Interop.Kernel32.GetFileType(handle);

                System.Diagnostics.Debug.Assert(
                      ft == Interop.Kernel32.FILE_TYPE.FILE_TYPE_DISK
                    || ft == Interop.Kernel32.FILE_TYPE.FILE_TYPE_PIPE
                    || ft == Interop.Kernel32.FILE_TYPE.FILE_TYPE_CHAR,
                    $"Unknown file type: {ft}");

                _fileType = (System.Int32)ft;
            }

            return _fileType;
        }

        private unsafe FileOptions GetFileOptions()
        {
            // Query the file options only once
            // If have been queried before disposing , continue to return them.
            FileOptions fileOptions = _fileOptions;
            if (fileOptions != (FileOptions)(-1))
            {
                return fileOptions;
            }

            EnsureValid();

            FileOptions result = FileOptions.None;

            
            Interop.NTSTATUS status = Interop.NtDll.NtQueryInformationFile(handle, out Interop.NtDll.FILE_MODE_INFORMATION o, out _);

            if (status != Interop.NTSTATUS.STATUS_SUCCESS)
            {
                throw new MP.ExceptionSystem.NativeWindowsException(status);
            }

            if ((o.Mode & (Interop.NtDll.CreateOptions.FILE_SYNCHRONOUS_IO_ALERT | Interop.NtDll.CreateOptions.FILE_SYNCHRONOUS_IO_NONALERT)) == 0)
            {
                result |= FileOptions.Asynchronous;
            }
            if ((o.Mode & Interop.NtDll.CreateOptions.FILE_WRITE_THROUGH) != 0)
            {
                result |= FileOptions.WriteThrough;
            }
            if ((o.Mode & Interop.NtDll.CreateOptions.FILE_RANDOM_ACCESS) != 0)
            {
                result |= FileOptions.RandomAccess;
            }
            if ((o.Mode & Interop.NtDll.CreateOptions.FILE_SEQUENTIAL_ONLY) != 0)
            {
                result |= FileOptions.SequentialScan;
            }
            if ((o.Mode & Interop.NtDll.CreateOptions.FILE_DELETE_ON_CLOSE) != 0)
            {
                result |= FileOptions.DeleteOnClose;
            }
            if ((o.Mode & Interop.NtDll.CreateOptions.FILE_NO_INTERMEDIATE_BUFFERING) != 0)
            {
                result |= NoBuffering;
            }

            return _fileOptions = result;
        }

        private unsafe System.Int64 GetFileLengthCore()
        {
            Interop.NtDll.FILE_STANDARD_INFORMATION stdi;
            Interop.NTSTATUS nts;
            if ((nts = Interop.NtDll.NtQueryInformationFile(handle, out stdi, out _)) == Interop.NTSTATUS.STATUS_SUCCESS)
            {
                return stdi.EndOfFile;
            }

            // In theory when NtQueryInformation fails, then
            // a) IsDevice can modify last error (not true today, but can be in the future),
            // b) DeviceIoControl can succeed (last error set to ERROR_SUCCESS) but return fewer bytes than requested.
            // The error is stored and in such cases exception for the first failure is going to be thrown.

            if (path is null || System.IO.PathInternal.IsDevice(path) == false)
            {
                throw System.IO.Win32Marshal.GetExceptionForNtStatus(nts, path is null ? System.String.Empty : path);
            }

            Interop.Kernel32.STORAGE_READ_CAPACITY storageReadCapacity;
            System.UInt32 bytesReturned;
            Interop.BOOL success = Interop.Kernel32.DeviceIoControl(
                handle,
                Interop.IOCTL.IOCTL_STORAGE_READ_CAPACITY,
                null,
                0,
                &storageReadCapacity,
                sizeof(Interop.Kernel32.STORAGE_READ_CAPACITY).ToUInt32(),
                &bytesReturned,
                null);

            if (success == Interop.BOOL.FALSE)
            {
                throw System.IO.Win32Marshal.GetExceptionForLastWin32Error(path);
            }
            else if (bytesReturned != sizeof(Interop.Kernel32.STORAGE_READ_CAPACITY))
            {
                throw System.IO.Win32Marshal.GetExceptionForNtStatus(nts, path);
            }

            return storageReadCapacity.DiskLength;
        }

        /// <summary>
        /// Gets the Volume ID where this file is located to. <br />
        /// By using the <see cref="MP.UnsafeMethods.ToUInt32(long)"/> to this member 
        /// you get the volume ID for the id returned by <see cref="DriveInfo.SoftwareSerialNumber"/>.
        /// </summary>
        /// <exception cref="ObjectDisposedException">The current <see cref="RedistSafeFileHandle"/> is disposed.</exception>
        public System.UInt64 FileVolumeID => GetFileIdInformation().VolumeID;

        /// <summary>
        /// Gets a 128-bit ID that uniquely identifies this file. <br />
        /// Note that it might not exist for all cases. <br />
        /// Returned as a Guid for flexibility.
        /// </summary>
        /// <exception cref="ObjectDisposedException">The current <see cref="RedistSafeFileHandle"/> is disposed.</exception>
        public unsafe Guid FileId
        {
            get {
                var inf = GetFileIdInformation();
                return Interop.GUID.From16BytePointer(inf.FileID).GetGuid();
            }
        }

        /// <summary>
        /// The opened file path.
        /// </summary>
        public System.String Path => path;

        /// <summary>
        /// Gets the full path of the opened file that NtDll thinks it to be.
        /// </summary>
        public System.String FileName
        {
            get
            {
                EnsureValid();
                System.String fn;
                // Verify that we can get the file name first.
                var s = Interop.NtDll.NtQueryInformationFile(handle , out fn);
                if (s != Interop.NTSTATUS.STATUS_SUCCESS) { return null; }
                // The file name returned has the drive letter stripped out.
                // So, we need to get the file's volume ID and use that to find out the drive from which the file was opened from.
                // Now, try to enumerate the drives to find from which drive the file was opened from.
                try
                {
                    var idi = GetFileIdInformation();
                    System.UInt32 volid = idi.VolumeID.ToUInt32(); // The volume ID for a drive is this value but getting only it's UInt32 portion.
                    if (volid == 0)
                    {
                        // If this happens , then we have to abort.
                        return null;
                    }
                    // Now enumerate the drives and find the matching drive.
                    foreach (var drive in DriveInfo.GetDrives())
                    {
                        if (drive.SoftwareSerialNumber == volid)
                        {
                            // Join the path and return the result.
                            System.String ret = drive.RootDirectory.FullName;
                            if (fn.StartsWith('\\'))
                            {
                                ret += fn.Substring(1);
                            }
                            else
                            {
                                ret += fn;
                            }
                            return ret;
                        }
                    }
                    // If we failed to find the drive, return the known name.
                    return fn;
                }
                catch
                {
                    // If we can't get the FILE_ID_INFORMATION for the file , we can just simply return the already known path
                    return fn;
                }
            }
        }

        /// <summary>
        /// Computes the full path where this file has been opened from. <br />
        /// It pulls the data by using two properties: the <see cref="Path"/> and the <see cref="FileName"/> property.
        /// </summary>
        public System.String LogicalPath
        {
            get
            {
                if (handle == IntPtr.Zero) {
                    return path;
                }
                try
                {
                    System.String fn = FileName;
                    if (fn is null || (fn.Length > 2 && fn[1] != ':'))
                    {
                        // We do not have a full path variant or NTDLL call failed so we must return the already known path
                        return path;
                    }
                    return fn; // The full path has been determined , return this instead
                }
                catch
                {
                    // FileName can fail for a number of reasons , so return the already known path
                    return path;
                }
            }
        }

        /// <summary>Gets a value whether the file was opened with native asyncronous semantics.</summary>
        /// <exception cref="ObjectDisposedException">The current <see cref="RedistSafeFileHandle"/> was disposed before the file options were retrieved.</exception>
        public System.Boolean IsAsync => (GetFileOptions() & FileOptions.Asynchronous) != 0;

        /// <summary>Gets a value whether the OS does not perform any buffering to the file.</summary>
        /// <exception cref="ObjectDisposedException">The current <see cref="RedistSafeFileHandle"/> was disposed before the file options were retrieved.</exception>
        public System.Boolean IsNoBuffering => (GetFileOptions() & NoBuffering) != 0;

        /// <summary>Gets a value whether the handle can seek into the file.</summary>
        /// <exception cref="ObjectDisposedException">The current <see cref="RedistSafeFileHandle"/> was disposed before the file options were retrieved.</exception>
        public System.Boolean CanSeek => !IsClosed && GetFileType() == (System.Int32)Interop.Kernel32.FILE_TYPE.FILE_TYPE_DISK;

        /// <summary>
        /// Gets a value whether this class manages the supplied handle; otherwise it returns false.
        /// </summary>
        public System.Boolean OwnsHandle => owns;

        /// <summary>
        /// Gets the underlying OS handle. Once disposed, this property will return an OS invalid handle.
        /// </summary>
        public System.IntPtr Handle => handle;

        /// <summary>
        /// Gets the options used to create this handle.
        /// </summary>
        /// <exception cref="ObjectDisposedException">The current <see cref="RedistSafeFileHandle"/> was disposed before the file options were retrieved.</exception>
        public FileOptions Options => GetFileOptions();

        /// <summary>
        /// Flushes all the data to the file that the connected handle represents.
        /// </summary>
        /// <exception cref="ObjectDisposedException">The current <see cref="RedistSafeFileHandle"/> is disposed.</exception>
        public void FlushToDisk()
        {
            EnsureValid();
            if (Interop.Kernel32.FlushFileBuffers(handle) == Interop.BOOL.FALSE)
            {
                System.Int32 errorCode = Interop.Kernel32.GetLastError();

                // NOTE: unlike fsync() on Unix, the FlushFileBuffers() function on Windows doesn't
                // support flushing handles opened for read-only access and will return an error. We
                // ignore this error to harmonize the two platforms: i.e. users can flush handles
                // opened for read-only access on BOTH platforms and no exception will be thrown.
                if (errorCode != Interop.Errors.ERROR_ACCESS_DENIED)
                {
                    throw System.IO.Win32Marshal.GetExceptionForWin32Error(errorCode, path);
                }
            }
        }

        /// <summary>
        /// Seeks to a given file offset.
        /// </summary>
        /// <param name="offset">The offset to seek.</param>
        /// <param name="origin">The seek origin to perform seeking.</param>
        /// <param name="closeInvalidHandle">If the command fails with invalid handle, it does then dispose the handle.</param>
        /// <returns>The seeked position inside the file.</returns>
        /// <exception cref="ObjectDisposedException">The current <see cref="RedistSafeFileHandle"/> is disposed.</exception>
        public System.Int64 Seek(System.Int64 offset, System.IO.SeekOrigin origin, System.Boolean closeInvalidHandle = false)
        {
            EnsureValid();
            System.Diagnostics.Debug.Assert(origin >= System.IO.SeekOrigin.Begin && origin <= System.IO.SeekOrigin.End, "origin >= SeekOrigin.Begin && origin <= SeekOrigin.End");

            Interop.NTSTATUS nts;
            Interop.NtDll.FILE_POSITION_INFORMATION pinf = new();

            switch (origin)
            {
                case System.IO.SeekOrigin.Begin:
                    // When we specify at the beginning, we can just directly apply the new seeked value
                    pinf.CurrentByteOffset = offset;
                    break;
                case System.IO.SeekOrigin.Current:
                    // When we need offset relative to the current, we must call NtQueryInformationFile to retrieve current offset, and add to that the offset parameter
                    nts = Interop.NtDll.NtQueryInformationFile(handle, out pinf, out _);
                    if (nts != Interop.NTSTATUS.STATUS_SUCCESS) {
                        throw System.IO.Win32Marshal.GetExceptionForNtStatus(nts , path);
                    }
                    pinf.CurrentByteOffset += offset;
                    break;
                case System.IO.SeekOrigin.End:
                    // When we need offset relative to the end, we must get the standard information structure.
                    // However, here we will use GetFileLength to also cache the result if needed.
                    if (TryGetCachedLength(out pinf.CurrentByteOffset) == false) {
                        pinf.CurrentByteOffset = GetFileLength();
                    }
                    pinf.CurrentByteOffset += offset;
                    break;
            }

            if ((nts = Interop.NtDll.NtSetInformationFile(handle, pinf, out _)) != Interop.NTSTATUS.STATUS_SUCCESS)
            {
                if (closeInvalidHandle && nts == Interop.NTSTATUS.STATUS_INVALID_HANDLE) { Dispose(); }
                throw System.IO.Win32Marshal.GetExceptionForNtStatus(nts, path);
            }

            return pinf.CurrentByteOffset;
        }

        /// <summary>Implements <see cref="System.IO.Stream.SetLength(long)"/> logic.</summary>
        /// <param name="length">The absolute length of the file.</param>
        /// <exception cref="ObjectDisposedException">The current <see cref="RedistSafeFileHandle"/> is disposed.</exception>
        public unsafe void SetFileLength(long length)
        {
            EnsureValid();

            var eofi = new Interop.NtDll.FILE_END_OF_FILE_INFORMATION() { EndOfFile = length };

            Interop.NTSTATUS nts;

            if ((nts = Interop.NtDll.NtSetInformationFile(handle, eofi, out _)) != Interop.NTSTATUS.STATUS_SUCCESS)
            {
                throw nts == Interop.NTSTATUS.STATUS_INVALID_PARAMETER ?
                    new ArgumentOutOfRangeException(nameof(length), SR.ArgumentOutOfRange_FileLengthTooBig) :
                    System.IO.Win32Marshal.GetExceptionForNtStatus(nts, path);
            }
        }

        /// <summary>
        /// Attempts to get the cached length value for this handle. <br />
        /// If not supported, the caller must still use the <see cref="GetFileLength"/> method
        /// to retrieve the file object length.
        /// </summary>
        /// <param name="cachedLength">The cached length value.</param>
        /// <returns>A value whether the length is cached.</returns>
        public System.Boolean TryGetCachedLength(out long cachedLength) => _lengthCanBeCached & (cachedLength = _length) > -1;

        /// <summary>
        /// Gets the total length of the current file, in bytes.
        /// </summary>
        /// <returns>The total length of the file object in bytes.</returns>
        /// <exception cref="ObjectDisposedException">The current <see cref="RedistSafeFileHandle"/> is disposed.</exception>
        public System.Int64 GetFileLength()
        {
            if (!_lengthCanBeCached)
            {
                // Check for handle validity only when we need to call GetFileLengthCore.
                EnsureValid();
                return GetFileLengthCore();
            }

            // On Windows, when the file is locked for writes we can cache file length
            // in memory and avoid subsequent native calls which are expensive.
            if (_length < 0)
            {
                // Check for handle validity only when we need to call GetFileLengthCore.
                EnsureValid();
                _length = GetFileLengthCore();
            }

            return _length;
        }

        /// <summary>
        /// Gets the access that the handle has to the underlying file.
        /// </summary>
        /// <exception cref="ObjectDisposedException">The current <see cref="RedistSafeFileHandle"/> is disposed.</exception>
        public FileAccess GetFileAccess()
        {
            EnsureValid();
            Interop.NtDll.FILE_ACCESS_INFORMATION acc;
            Interop.NTSTATUS nts = Interop.NtDll.NtQueryInformationFile(handle, out acc, out _);

            if (nts == Interop.NTSTATUS.STATUS_SUCCESS)
            {
                System.Boolean rd = false , wr = false;
                if (acc.AccessFlags.HasFlag(Interop.NtDll.DesiredAccess.FILE_READ_DATA))
                {
                    rd = true;
                }
                if (acc.AccessFlags.HasFlag(Interop.NtDll.DesiredAccess.FILE_WRITE_DATA))
                {
                    wr = true;
                }
                if (rd && wr) { return FileAccess.ReadWrite; }
                if (rd) { return FileAccess.Read; }
                if (wr) { return FileAccess.Write; }
                // This case will never be reached but C# compiler complains about unreturned code paths, so just return a plain zero.
                // Additionally assert on debugging to detect and troubleshoot this.
                System.Diagnostics.Debug.Fail("Invalid code path: Native interop call failed although STATUS_SUCCESS was retrieved.");
                return 0; 
            } else {
                // Get a Win32 error for the NTSTATUS , and return that as the exception
                throw System.IO.Win32Marshal.GetExceptionForNtStatus(nts , path);
            }
        }

        /// <summary>
        /// Gets the file's attributes.
        /// </summary>
        /// <returns>A combination of flags specifying the current file's attributes.</returns>
        /// <exception cref="MP.ExceptionSystem.NativeWindowsException">A native unexpected exception occured.</exception>
        /// <exception cref="ObjectDisposedException">The current <see cref="RedistSafeFileHandle"/> is disposed.</exception>
        public System.UInt32 GetAttributes()
        {
            EnsureValid();
            Interop.NtDll.FILE_BASIC_INFORMATION basic;
            Interop.NTSTATUS nts = Interop.NtDll.NtQueryInformationFile(handle , out basic , out _);
            if (nts != Interop.NTSTATUS.STATUS_SUCCESS) {
                throw new MP.ExceptionSystem.NativeWindowsException(nts);
            }
            return (System.UInt32)basic.FileAttributes;
        }

        /// <summary>
        /// Gets the file's seek pointer position, in offset of bytes from the beginning of the file.
        /// </summary>
        /// <returns>The file object's seek pointer position.</returns>
        /// <exception cref="ObjectDisposedException">The current <see cref="RedistSafeFileHandle"/> is disposed.</exception>
        public System.Int64 GetFilePosition()
        {
            EnsureValid();
            Interop.NTSTATUS nts = Interop.NtDll.NtQueryInformationFile(handle, out Interop.NtDll.FILE_POSITION_INFORMATION p, out _);
            if (nts != Interop.NTSTATUS.STATUS_SUCCESS) {
                throw System.IO.Win32Marshal.GetExceptionForNtStatus(nts , path);
            }
            return p.CurrentByteOffset;
        }

        private void CommonDisposeCode()
        {
            path = null;
            handle = System.IntPtr.Zero; // For public members to throw ObjectDisposedException
        }

        private void EnsureValid() 
            => ObjectDisposedException.ThrowIf(handle == IntPtr.Zero, this);

        protected override System.Boolean ReleaseHandle()
        {
            // We do not own the handle; emulate the call so that the handle was 'freed' successfully.
            if (owns == false) {
                CommonDisposeCode();
                return true; 
            }
            // Try to close the handle , then destroy it to show disposeness to the exposed members.
            System.Boolean val = Interop.Kernel32.CloseHandle(handle) != Interop.BOOL.FALSE;
            CommonDisposeCode();
            return val;
        }
    }
}
