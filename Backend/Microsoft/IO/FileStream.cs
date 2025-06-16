
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

        /// <summary>
        /// Creates a new <see cref="FileStream"/> class instance from a preexisting <see cref="RedistSafeFileHandle"/>.
        /// </summary>
        /// <param name="existing">The pre-existing <see cref="RedistSafeFileHandle"/> to use.</param>
        /// <exception cref="ArgumentNullException"><paramref name="existing"/> was <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="existing"/> was an asyncronous handle, or the passed handle was already closed.</exception>
        public FileStream(RedistSafeFileHandle existing)
        {
            if (existing is null) { throw new ArgumentNullException(nameof(existing)); }
            if (existing.IsClosed) { throw new ArgumentException("The passed handle is closed." , nameof(existing)); }
            if (existing.IsAsync) { throw new ArgumentException("Asynchronous file handling is unsupported.", nameof(existing)); }
            sfh = existing;
            poscurrent = sfh.Seek(0, System.IO.SeekOrigin.Current);
            threadaccess = new(1);
        }

        /// <summary>
        /// Creates a new <see cref="FileStream"/> class instance by specifying a file path, and with the specified file mode for this file to be opened.
        /// </summary>
        /// <param name="savepath">The path where the file is to be saved to, or read from.</param>
        /// <param name="mode">The file mode to use to open this file.</param>
        /// <exception cref="ArgumentNullException"><paramref name="savepath"/> was <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="savepath"/> was represented the empty string.</exception>
        /// <exception cref="System.IO.FileNotFoundException">Valid only when opening a file. The file does not exist.</exception>
        /// <exception cref="System.IO.IOException">An unexpected I/O exception was occured.</exception>
        /// <exception cref="UnauthorizedAccessException">The application does not have access to the specific path.</exception>
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

        /// <summary>
        /// Creates a new <see cref="FileStream"/> class instance by specifying a file path, the file mode for this file to be opened,
        /// and the desired data access to have on the file.
        /// </summary>
        /// <param name="savepath">The path where the file is to be saved to, or read from.</param>
        /// <param name="mode">The file mode to use to open this file.</param>
        /// <param name="access">The desired file access that the system and you will have access on the creating file object.</param>
        /// <exception cref="ArgumentNullException"><paramref name="savepath"/> was <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="savepath"/> was represented the empty string.</exception>
        /// <exception cref="System.IO.FileNotFoundException">Valid only when opening a file. The file does not exist.</exception>
        /// <exception cref="System.IO.IOException">An unexpected I/O exception was occured.</exception>
        /// <exception cref="UnauthorizedAccessException">The application does not have access to the specific path.</exception>
        public FileStream(System.String savepath , FileMode mode , FileAccess access)
            => SharedInitialize(savepath, mode, access, FileShare.None);

        /// <summary>
        /// Creates a new <see cref="FileStream"/> class instance by specifying a file path, the file mode for this file to be opened,
        /// the desired data access to have on the file, and with a value whether the file can be opened by other applications too, 
        /// and how it should be opened by such applications.
        /// </summary>
        /// <param name="savepath">The path where the file is to be saved to, or read from.</param>
        /// <param name="mode">The file mode to use to open this file.</param>
        /// <param name="access">The desired file access that the system and you will have access on the creating file object.</param>
        /// <param name="share">The desired file access that other applications can have on the file.</param>
        /// <exception cref="ArgumentNullException"><paramref name="savepath"/> was <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="savepath"/> was represented the empty string.</exception>
        /// <exception cref="System.IO.FileNotFoundException">Valid only when opening a file. The file does not exist.</exception>
        /// <exception cref="System.IO.IOException">An unexpected I/O exception was occured.</exception>
        /// <exception cref="UnauthorizedAccessException">The application does not have access to the specific path.</exception>
        public FileStream(System.String savepath, FileMode mode, FileAccess access, FileShare share)
            => SharedInitialize(savepath, mode, access, share);

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
            ArgumentException.ThrowIfNullOrEmpty(savepath);
            if (IsOpenMode(mode) && File.Exists(savepath) == false)
            {
                throw new System.IO.FileNotFoundException("The system cannot find the file requested.", savepath);
            }
            sfh = RedistSafeFileHandle.Open(savepath, mode, access, share, FileOptions.None , 100);
            poscurrent = sfh.GetFileLength();
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
        
        // This is just readonly so the caller can call it many consecutive times
        public override bool CanSeek => sfh.CanSeek; 

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

        public override string ToString() => $"Microsoft.IO.FileStream {{ CanRead = {CanRead}, CanWrite = {CanWrite}, Position = {poscurrent}, Length = {Length}, Handle = {sfh?.Handle} }}";

        protected override void Dispose(bool disposing) 
        {
            if (disposing)
            {
                threadaccess?.Wait();
                try {
                    if (sfh is not null)
                    {
                        // May fail if the handle is invalid
                        if (CanWrite) { sfh.FlushToDisk(); }
                        sfh.Dispose();
                        sfh = null;
                    }
                } catch { 
                    
                } finally {
                    if (threadaccess is not null)
                    {
                        threadaccess.Release();
                        threadaccess.Dispose();
                        threadaccess = null;
                    }
                }
            }
        }
    }
}