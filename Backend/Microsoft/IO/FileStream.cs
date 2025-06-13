
using MP;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;
using System.Runtime.CompilerServices;

namespace Microsoft.IO
{
    /// <summary>
    /// Defines a thread-safe implementation of a File Stream for Windows , like the FileStream .NET class.
    /// </summary>
    public sealed class FileStream : System.IO.Stream
    {
        private RedistSafeFileHandle sfh;
        private System.Int64 poscurrent;
        private SemaphoreSlim threadaccess;

        public FileStream(RedistSafeFileHandle existing)
        {
            if (existing is null) { throw new ArgumentNullException(nameof(existing)); }
            if (existing.IsAsync) { throw new ArgumentException("Asynchronous file handling is unsupported.", nameof(existing)); }
            sfh = existing;
            poscurrent = sfh.Seek(0, System.IO.SeekOrigin.Current);
            threadaccess = new(1);
        }

        public FileStream(System.String savepath, FileMode mode)
        {
            FileAccess fa = 0;
            switch (mode)
            {
                case FileMode.Open:
                    fa = FileAccess.Read;
                    break;
                case FileMode.Append:
                    fa = FileAccess.Write;
                    break;
                case FileMode.OpenOrCreate:
                    fa = FileAccess.ReadWrite;
                    break;
                case FileMode.Truncate:
                    fa = FileAccess.ReadWrite;
                    break;
                case FileMode.Create:
                case FileMode.CreateNew:
                    fa = FileAccess.ReadWrite;
                    break;
            }
            SharedInitialize(savepath, mode, fa, FileShare.None);
        }

        public FileStream(System.String savepath , FileMode mode , FileAccess access)
        {
            SharedInitialize(savepath, mode, access, FileShare.None);
        }

        public FileStream(System.String savepath, FileMode mode, FileAccess access, FileShare share)
        {
            SharedInitialize(savepath, mode, access, share);
        }

        public static FileStream OpenAsRandomAccess(System.String savepath , FileMode fm , FileAccess fa , FileShare fse)
        {
            if (System.String.IsNullOrEmpty(savepath)) { throw new ArgumentNullException(nameof(savepath)); }
            if (IsOpenMode(fm) && File.Exists(savepath) == false)
            {
                throw new System.IO.FileNotFoundException("The system cannot find the file requested.", savepath);
            }
            return new(RedistSafeFileHandle.Open(savepath, fm, fa, fse, FileOptions.RandomAccess, 100));
        }

        [System.Diagnostics.StackTraceHidden]
        private void SharedInitialize(System.String savepath, FileMode mode, FileAccess access , FileShare share)
        {
            if (System.String.IsNullOrEmpty(savepath)) { throw new ArgumentNullException(nameof(savepath)); }
            if (IsOpenMode(mode) && File.Exists(savepath) == false)
            {
                throw new System.IO.FileNotFoundException("The system cannot find the file requested.", savepath);
            }
            sfh = RedistSafeFileHandle.Open(savepath, mode, access, share, FileOptions.None , 100);
            poscurrent = sfh.Seek(0, System.IO.SeekOrigin.Current);
            threadaccess = new(1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static System.Boolean IsOpenMode(FileMode fm)
            => fm == FileMode.Open || fm == FileMode.Truncate || fm == FileMode.Append;

        /// <summary>
        /// Gets the full path of the opened file.
        /// </summary>
        public System.String Name => sfh.LogicalPath;

        /// <summary>
        /// Gets the underlying native handle that does the heavy lifting of reading and writing to the underlying device.
        /// </summary>
        public RedistSafeFileHandle SafeFileHandle => sfh;

        public override bool CanSeek => sfh.CanSeek; // This is just readonly so the caller can call it many consecutive times

        public override System.Int64 Seek(long offset, System.IO.SeekOrigin origin)
        {
            // Because we are modifying seek position, we have to enforce that a single thread will access it each time.
            threadaccess.Wait();
            try {
                poscurrent = sfh.Seek(offset, origin);
            } finally {
                threadaccess.Release();
            }
            return poscurrent;
        }

        // SetLength just sets the end of file so no problem to be called from multiple threads.
        public override void SetLength(long value) => sfh.SetFileLength(value);

        public override long Position
        { 
            get => poscurrent;
            set => Seek(value , System.IO.SeekOrigin.Begin);  // poscurrent is set by Seek
        }

        // Length is also a read-only value.
        public override long Length
        {
            get {
                System.Int64 len;
                if (sfh.TryGetCachedLength(out len) == false)
                {
                    // The length cannot be cached so perform the heavy native call.
                    len = sfh.GetFileLength();
                }
                return len;
            }
        }

        public override bool CanRead
        {
            get {
                FileAccess detaccess = sfh.GetFileAccess();
                if (detaccess == FileAccess.Read) { return true; }
                return detaccess == FileAccess.ReadWrite;
            }
        }

        public override bool CanWrite
        {
            get {
                FileAccess detaccess = sfh.GetFileAccess();
                if (detaccess == FileAccess.Write) { return true; }
                return detaccess == FileAccess.ReadWrite;
            }
        }

        public override void Flush()
        {
            if (CanWrite)
            {
                // Flushing is also safe.
                // Flushing is meaningless if we cannot write to the file.
                sfh.FlushToDisk(); 
            }
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            ValidateBufferArguments(buffer, offset, count);
            System.Int32 error;
            threadaccess.Wait();
            System.Int32 wb = FileOperations.WriteFileNativeUseOverlapped(sfh, new System.Span<System.Byte>(buffer, offset, count), FileOperations.GetNativeOverlappedForSyncHandle(sfh, poscurrent), out error);
            if (error == 0) { 
                // No errors reported? check for buffer validity write and 
                // update the position appropriately.
                System.Diagnostics.Debug.Assert(wb == count);
                poscurrent += wb;
            }
            threadaccess.Release();
            if (error != 0) { throw System.IO.Win32Marshal.GetExceptionForWin32Error(error); }
        }

        public override void Write(ReadOnlySpan<byte> buffer)
        {
            System.Int32 error;
            threadaccess.Wait();
            System.Int32 wb = FileOperations.WriteFileNativeUseOverlapped(sfh, buffer, FileOperations.GetNativeOverlappedForSyncHandle(sfh, poscurrent), out error);
            if (error == 0) {
                // No errors reported? check for buffer validity write and 
                // update the position appropriately.
                System.Diagnostics.Debug.Assert(wb == buffer.Length);
                poscurrent += wb;
            }
            threadaccess.Release();
            if (error != 0) { throw System.IO.Win32Marshal.GetExceptionForWin32Error(error); }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            ValidateBufferArguments(buffer, offset, count);
            System.Int32 error;
            threadaccess.Wait();
            System.Int32 rb = FileOperations.ReadFileNativeUseOverlapped(sfh, new System.Span<System.Byte>(buffer, offset, count), FileOperations.GetNativeOverlappedForSyncHandle(sfh, poscurrent), out error);
            if (error == 0) { poscurrent += rb; }
            threadaccess.Release();
            if (error != 0) { throw System.IO.Win32Marshal.GetExceptionForWin32Error(error); }
            return rb;
        }

        public override int Read(Span<byte> buffer)
        {
            System.Int32 error;
            threadaccess.Wait();
            System.Int32 rb = FileOperations.ReadFileNativeUseOverlapped(sfh, buffer, FileOperations.GetNativeOverlappedForSyncHandle(sfh, poscurrent), out error);
            if (error == 0) { poscurrent += rb; }
            threadaccess.Release();
            if (error != 0) { throw System.IO.Win32Marshal.GetExceptionForWin32Error(error); }
            return rb;
        }

        public unsafe override void CopyTo(System.IO.Stream destination, int bufferSize)
        {
            if (destination is null) {
                throw new ArgumentNullException(nameof(destination));
            }
            if (destination.CanWrite == false) {
                throw new ArgumentException("Target stream must be writeable." , nameof(destination));
            }
            if (CanRead == false) {
                throw new InvalidOperationException("Cannot copy to another stream when this stream is unreadable!!");
            }

            if (destination is FileStream other)
            {
                // Instead call an even more optimized method for copying between file streams.
                OptimizedFileStreamCopyTo(other , bufferSize);
                return;
            }

            // Create native memory to be referenced by the buffer span
            SafeLibcMemoryHandle underlyinghandle = new(bufferSize);
            System.Boolean locked = false;
            System.Int32 error;
            try {
                System.Span<System.Byte> buffer = new(underlyinghandle.MemoryPointer, underlyinghandle.MemoryLength);
                System.Int32 rb;
                // Use lock only once.
                threadaccess.Wait();
                locked = true;
                while ((rb = FileOperations.ReadFileNativeUseOverlapped(sfh , buffer , FileOperations.GetNativeOverlappedForSyncHandle(sfh , poscurrent) , out error)) > 0)
                {
                    // Slice required so that only the truly read bytes should be copied out.
                    destination.Write(buffer.Slice(0 , rb));
                    poscurrent += rb;
                }
            } finally {
                underlyinghandle.Dispose();
                if (locked) { threadaccess.Release(); }
            }
            // If an error occured (In which case the ReadFileNativeUseOverlapped will return with -1 and populated error code)
            // report it here. Not reporting before to leave time for the semaphore and the native buffer to be reliably freed.
            if (error != 0) { throw System.IO.Win32Marshal.GetExceptionForWin32Error(error); }
        }

        private unsafe void OptimizedFileStreamCopyTo(FileStream destination, int bufferSize)
        {
            // Create native memory to be referenced by the buffer span
            SafeLibcMemoryHandle underlyinghandle = new(bufferSize);
            System.Boolean locked = false;
            System.Int32 error;
            try {
                System.Span<System.Byte> buffer = new(underlyinghandle.MemoryPointer, underlyinghandle.MemoryLength);
                System.Int32 rb , writeopcode;
                // Use lock only once.
                // Now that we know that our destination is another file stream,
                // do the same for that too for increased performance.
                threadaccess.Wait();
                destination.threadaccess.Wait();
                locked = true;
                while ((rb = FileOperations.ReadFileNativeUseOverlapped(sfh, buffer, FileOperations.GetNativeOverlappedForSyncHandle(sfh, poscurrent), out error)) > 0)
                {
                    // Slice required so that only the truly read bytes should be copied out.
                    writeopcode = FileOperations.WriteFileNativeUseOverlapped(destination.sfh, buffer.Slice(0, rb), FileOperations.GetNativeOverlappedForSyncHandle(destination.sfh, poscurrent), out error);
                    if (writeopcode == -1) { break; } // We have an error. The error has been saved and will be thrown once we fully unload the operation.
                    // Update both stream positions respectively
                    poscurrent += rb;
                    destination.poscurrent += writeopcode;
                }
            } finally {
                // Dispose all sync data and the temporary native buffer
                underlyinghandle.Dispose();
                if (locked) { 
                    destination.threadaccess.Release();
                    threadaccess.Release(); 
                }
            }
            // If an error occured (In which case the ReadFileNativeUseOverlapped or WriteFileNativeUseOverlapped will return with -1 and populated error code)
            // report it here. Not reporting before to leave time for the semaphores and the native buffer to be reliably freed.
            if (error != 0) { throw System.IO.Win32Marshal.GetExceptionForWin32Error(error); }
        }

        public override Task CopyToAsync(System.IO.Stream destination, int bufferSize, CancellationToken cancellationToken) => Task.Run(() => { CopyTo(destination, bufferSize); } , cancellationToken);

        public override Task FlushAsync(CancellationToken cancellationToken) => Task.Run(Flush, cancellationToken);

        public override ValueTask DisposeAsync() => new(Task.Run(Dispose));

        public override string ToString() => $"Microsoft.IO.FileStream {{ CanRead = {CanRead}, CanWrite = {CanWrite}, Position = {poscurrent}, Length = {Length}, Handle = {sfh.Handle} }}";

        protected override void Dispose(bool disposing) 
        {
            if (sfh is not null)
            {
                if (CanWrite) { sfh.FlushToDisk(); }
                sfh.Dispose();
                sfh = null;
            }
            if (threadaccess is not null)
            {
                // The thread that called Dispose must wait until all the semaphore operations have been finished.
                while (threadaccess.CurrentCount == 0) { Thread.Sleep(10); }
                threadaccess.Dispose();
                threadaccess = null;
            }
        }
    }
}