/*
 * Copyright © 2000-2018 SharpZipLib Contributors

Permission is hereby granted, free of charge, to any person obtaining a copy of this
software and associated documentation files (the "Software"), to deal in the Software
without restriction, including without limitation the rights to use, copy, modify, merge,
publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or
substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
DEALINGS IN THE SOFTWARE.

*/

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
    #define VECTORIZE_MEMORY_MOVE
#endif

using MP;
using System;
using System.Linq;
using System.Text;
using System.Buffers;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.IO.ManagedZip.Zip.Compression;
using CT = System.Threading.CancellationToken;

namespace System.IO.ManagedZip.Core
{
    internal static class ByteOrderStreamExtensions
    {
        internal static byte[] SwappedBytes(ushort value) => value.GetBytes();
        internal static byte[] SwappedBytes(short value) => value.GetBytes();
        internal static byte[] SwappedBytes(uint value) => value.GetBytes();
        internal static byte[] SwappedBytes(int value) => value.GetBytes();

        internal static byte[] SwappedBytes(long value) => value.GetBytes();

        internal static byte[] SwappedBytes(ulong value) => value.GetBytes();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static long SwappedS64(byte[] bytes) => bytes.ToInt64(0);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static ulong SwappedU64(byte[] bytes) => bytes.ToUInt64(0);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int SwappedS32(byte[] bytes) => bytes.ToInt32(0);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static uint SwappedU32(byte[] bytes) => bytes.ToUInt32(0);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static short SwappedS16(byte[] bytes) => bytes.ToInt16(0);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static ushort SwappedU16(byte[] bytes) => bytes.ToUInt16(0);

        internal static byte[] ReadBytes(this Stream stream, int count)
        {
            var bytes = new byte[count];
            var remaining = count;
            while (remaining > 0)
            {
                var bytesRead = stream.Read(bytes, count - remaining, remaining);
                if (bytesRead < 1) throw new EndOfStreamException();
                remaining -= bytesRead;
            }

            return bytes;
        }

        /// <summary> Read an unsigned short in little endian byte order. </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReadLEShort(this Stream stream) => stream.ReadInt16();

        /// <summary> Read an int in little endian byte order. </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReadLEInt(this Stream stream) => stream.ReadInt32();

        /// <summary> Read a long in little endian byte order. </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long ReadLELong(this Stream stream) => stream.ReadInt64();

        /// <summary> Write an unsigned short in little endian byte order. </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteLEShort(this Stream stream, int value) => stream.WriteInt16((System.Int16)value);

        /// <inheritdoc cref="WriteLEShort"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task WriteLEShortAsync(this Stream stream, int value, CT ct)
            => await stream.WriteAsync(SwappedBytes(value), 0, 2, ct).ConfigureAwait(false);

        /// <summary> Write a ushort in little endian byte order. </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteLEUshort(this Stream stream, ushort value) => stream.WriteUInt16(value);

        /// <inheritdoc cref="WriteLEUshort"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task WriteLEUshortAsync(this Stream stream, ushort value, CT ct)
            => await stream.WriteAsync(SwappedBytes(value), 0, 2, ct).ConfigureAwait(false);

        /// <summary> Write an int in little endian byte order. </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteLEInt(this Stream stream, int value) => stream.WriteInt32(value);

        /// <inheritdoc cref="WriteLEInt"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task WriteLEIntAsync(this Stream stream, int value, CT ct)
            => await stream.WriteAsync(SwappedBytes(value), 0, 4, ct).ConfigureAwait(false);

        /// <summary> Write a uint in little endian byte order. </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteLEUint(this Stream stream, uint value) => stream.WriteUInt32(value);

        /// <inheritdoc cref="WriteLEUint"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task WriteLEUintAsync(this Stream stream, uint value, CT ct)
            => await stream.WriteAsync(SwappedBytes(value), 0, 4, ct).ConfigureAwait(false);

        /// <summary> Write a long in little endian byte order. </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteLELong(this Stream stream, long value) => stream.WriteInt64(value);

        /// <inheritdoc cref="WriteLELong"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task WriteLELongAsync(this Stream stream, long value, CT ct)
            => await stream.WriteAsync(SwappedBytes(value), 0, 8, ct).ConfigureAwait(false);

        /// <summary> Write a ulong in little endian byte order. </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteLEUlong(this Stream stream, ulong value) => stream.WriteUInt64(value);

        /// <inheritdoc cref="WriteLEUlong"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task WriteLEUlongAsync(this Stream stream, ulong value, CT ct)
            => await stream.WriteAsync(SwappedBytes(value), 0, 8, ct).ConfigureAwait(false);
    }

    internal static class Empty
    {
#if NET45
		internal static class EmptyArray<T>
		{
			public static readonly T[] Value = new T[0];
		}
		public static T[] Array<T>() => EmptyArray<T>.Value;
#else
        public static T[] Array<T>() => System.Array.Empty<T>();
#endif
    }

    /// <summary>
    /// A MemoryPool that will return a Memory which is exactly the length asked for using the bufferSize parameter.
    /// This is in contrast to the default ArrayMemoryPool which will return a Memory of equal size to the underlying
    /// array which at least as long as the minBufferSize parameter.
    /// Note: The underlying array may be larger than the slice of Memory
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal sealed class ExactMemoryPool<T> : MemoryPool<T>
    {
        public new static readonly MemoryPool<T> Shared = new ExactMemoryPool<T>();

        public override IMemoryOwner<T> Rent(int bufferSize = -1)
        {
            if ((uint)bufferSize > int.MaxValue || bufferSize < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(bufferSize));
            }

            return new ExactMemoryPoolBuffer(bufferSize);
        }

        protected override void Dispose(bool disposing)
        {
        }

        public override int MaxBufferSize => int.MaxValue;

        private sealed class ExactMemoryPoolBuffer : IMemoryOwner<T>, IDisposable
        {
            private T[] array;
            private readonly int size;

            public ExactMemoryPoolBuffer(int size)
            {
                this.size = size;
                this.array = ArrayPool<T>.Shared.Rent(size);
            }

            public Memory<T> Memory
            {
                get
                {
                    T[] array = this.array;
                    if (array == null)
                    {
                        throw new ObjectDisposedException(nameof(ExactMemoryPoolBuffer));
                    }

                    return new Memory<T>(array).Slice(0, size);
                }
            }

            public void Dispose()
            {
                T[] array = this.array;
                if (array == null)
                {
                    return;
                }

                this.array = null;
                ArrayPool<T>.Shared.Return(array);
            }
        }
    }

    #region EventArgs

    /// <summary>
    /// Event arguments for scanning.
    /// </summary>
    public class ScanEventArgs : EventArgs
    {
        #region Constructors

        /// <summary>
        /// Initialise a new instance of <see cref="ScanEventArgs"/>
        /// </summary>
        /// <param name="name">The file or directory name.</param>
        public ScanEventArgs(string name)
        {
            name_ = name;
        }

        #endregion Constructors

        /// <summary>
        /// The file or directory name for this event.
        /// </summary>
        public string Name
        {
            get { return name_; }
        }

        /// <summary>
        /// Get set a value indicating if scanning should continue or not.
        /// </summary>
        public bool ContinueRunning
        {
            get { return continueRunning_; }
            set { continueRunning_ = value; }
        }

        #region Instance Fields

        private string name_;
        private bool continueRunning_ = true;

        #endregion Instance Fields
    }

    /// <summary>
    /// Event arguments during processing of a single file or directory.
    /// </summary>
    public class ProgressEventArgs : EventArgs
    {
        #region Constructors

        /// <summary>
        /// Initialise a new instance of <see cref="ScanEventArgs"/>
        /// </summary>
        /// <param name="name">The file or directory name if known.</param>
        /// <param name="processed">The number of bytes processed so far</param>
        /// <param name="target">The total number of bytes to process, 0 if not known</param>
        public ProgressEventArgs(string name, long processed, long target)
        {
            name_ = name;
            processed_ = processed;
            target_ = target;
        }

        #endregion Constructors

        /// <summary>
        /// The name for this event if known.
        /// </summary>
        public string Name
        {
            get { return name_; }
        }

        /// <summary>
        /// Get set a value indicating whether scanning should continue or not.
        /// </summary>
        public bool ContinueRunning
        {
            get { return continueRunning_; }
            set { continueRunning_ = value; }
        }

        /// <summary>
        /// Get a percentage representing how much of the <see cref="Target"></see> has been processed
        /// </summary>
        /// <value>0.0 to 100.0 percent; 0 if target is not known.</value>
        public float PercentComplete
        {
            get
            {
                float result;
                if (target_ <= 0)
                {
                    result = 0;
                }
                else
                {
                    result = ((float)processed_ / (float)target_) * 100.0f;
                }
                return result;
            }
        }

        /// <summary>
        /// The number of bytes processed so far
        /// </summary>
        public long Processed
        {
            get { return processed_; }
        }

        /// <summary>
        /// The number of bytes to process.
        /// </summary>
        /// <remarks>Target may be 0 or negative if the value isnt known.</remarks>
        public long Target
        {
            get { return target_; }
        }

        #region Instance Fields

        private string name_;
        private long processed_;
        private long target_;
        private bool continueRunning_ = true;

        #endregion Instance Fields
    }

    /// <summary>
    /// Event arguments for directories.
    /// </summary>
    public class DirectoryEventArgs : ScanEventArgs
    {
        #region Constructors

        /// <summary>
        /// Initialize an instance of <see cref="DirectoryEventArgs"></see>.
        /// </summary>
        /// <param name="name">The name for this directory.</param>
        /// <param name="hasMatchingFiles">Flag value indicating if any matching files are contained in this directory.</param>
        public DirectoryEventArgs(string name, bool hasMatchingFiles)
            : base(name)
        {
            hasMatchingFiles_ = hasMatchingFiles;
        }

        #endregion Constructors

        /// <summary>
        /// Get a value indicating if the directory contains any matching files or not.
        /// </summary>
        public bool HasMatchingFiles
        {
            get { return hasMatchingFiles_; }
        }

        private readonly

        #region Instance Fields

        bool hasMatchingFiles_;

        #endregion Instance Fields
    }

    /// <summary>
    /// Arguments passed when scan failures are detected.
    /// </summary>
    public class ScanFailureEventArgs : EventArgs
    {
        #region Constructors

        /// <summary>
        /// Initialise a new instance of <see cref="ScanFailureEventArgs"></see>
        /// </summary>
        /// <param name="name">The name to apply.</param>
        /// <param name="e">The exception to use.</param>
        public ScanFailureEventArgs(string name, Exception e)
        {
            name_ = name;
            exception_ = e;
            continueRunning_ = true;
        }

        #endregion Constructors

        /// <summary>
        /// The applicable name.
        /// </summary>
        public string Name
        {
            get { return name_; }
        }

        /// <summary>
        /// The applicable exception.
        /// </summary>
        public Exception Exception
        {
            get { return exception_; }
        }

        /// <summary>
        /// Get / set a value indicating whether scanning should continue.
        /// </summary>
        public bool ContinueRunning
        {
            get { return continueRunning_; }
            set { continueRunning_ = value; }
        }

        #region Instance Fields

        private string name_;
        private Exception exception_;
        private bool continueRunning_;

        #endregion Instance Fields
    }

    #endregion EventArgs

    #region Delegates

    /// <summary>
    /// Delegate invoked before starting to process a file.
    /// </summary>
    /// <param name="sender">The source of the event</param>
    /// <param name="e">The event arguments.</param>
    public delegate void ProcessFileHandler(object sender, ScanEventArgs e);

    /// <summary>
    /// Delegate invoked during processing of a file or directory
    /// </summary>
    /// <param name="sender">The source of the event</param>
    /// <param name="e">The event arguments.</param>
    public delegate void ProgressHandler(object sender, ProgressEventArgs e);

    /// <summary>
    /// Delegate invoked when a file has been completely processed.
    /// </summary>
    /// <param name="sender">The source of the event</param>
    /// <param name="e">The event arguments.</param>
    public delegate void CompletedFileHandler(object sender, ScanEventArgs e);

    /// <summary>
    /// Delegate invoked when a directory failure is detected.
    /// </summary>
    /// <param name="sender">The source of the event</param>
    /// <param name="e">The event arguments.</param>
    public delegate void DirectoryFailureHandler(object sender, ScanFailureEventArgs e);

    /// <summary>
    /// Delegate invoked when a file failure is detected.
    /// </summary>
    /// <param name="sender">The source of the event</param>
    /// <param name="e">The event arguments.</param>
    public delegate void FileFailureHandler(object sender, ScanFailureEventArgs e);

    #endregion Delegates

    /// <summary>
    /// FileSystemScanner provides facilities scanning of files and directories.
    /// </summary>
    public class FileSystemScanner
    {
        #region Constructors

        /// <summary>
        /// Initialise a new instance of <see cref="FileSystemScanner"></see>
        /// </summary>
        /// <param name="filter">The <see cref="PathFilter">file filter</see> to apply when scanning.</param>
        public FileSystemScanner(string filter)
        {
            fileFilter_ = new PathFilter(filter);
        }

        /// <summary>
        /// Initialise a new instance of <see cref="FileSystemScanner"></see>
        /// </summary>
        /// <param name="fileFilter">The <see cref="PathFilter">file filter</see> to apply.</param>
        /// <param name="directoryFilter">The <see cref="PathFilter"> directory filter</see> to apply.</param>
        public FileSystemScanner(string fileFilter, string directoryFilter)
        {
            fileFilter_ = new PathFilter(fileFilter);
            directoryFilter_ = new PathFilter(directoryFilter);
        }

        /// <summary>
        /// Initialise a new instance of <see cref="FileSystemScanner"></see>
        /// </summary>
        /// <param name="fileFilter">The file <see cref="IScanFilter">filter</see> to apply.</param>
        public FileSystemScanner(IScanFilter fileFilter)
        {
            fileFilter_ = fileFilter;
        }

        /// <summary>
        /// Initialise a new instance of <see cref="FileSystemScanner"></see>
        /// </summary>
        /// <param name="fileFilter">The file <see cref="IScanFilter">filter</see>  to apply.</param>
        /// <param name="directoryFilter">The directory <see cref="IScanFilter">filter</see>  to apply.</param>
        public FileSystemScanner(IScanFilter fileFilter, IScanFilter directoryFilter)
        {
            fileFilter_ = fileFilter;
            directoryFilter_ = directoryFilter;
        }

        #endregion Constructors

        #region Delegates

        /// <summary>
        /// Delegate to invoke when a directory is processed.
        /// </summary>
        public event EventHandler<DirectoryEventArgs> ProcessDirectory;

        /// <summary>
        /// Delegate to invoke when a file is processed.
        /// </summary>
        public ProcessFileHandler ProcessFile;

        /// <summary>
        /// Delegate to invoke when processing for a file has finished.
        /// </summary>
        public CompletedFileHandler CompletedFile;

        /// <summary>
        /// Delegate to invoke when a directory failure is detected.
        /// </summary>
        public DirectoryFailureHandler DirectoryFailure;

        /// <summary>
        /// Delegate to invoke when a file failure is detected.
        /// </summary>
        public FileFailureHandler FileFailure;

        #endregion Delegates

        /// <summary>
        /// Raise the DirectoryFailure event.
        /// </summary>
        /// <param name="directory">The directory name.</param>
        /// <param name="e">The exception detected.</param>
        private bool OnDirectoryFailure(string directory, Exception e)
        {
            DirectoryFailureHandler handler = DirectoryFailure;
            bool result = (handler != null);
            if (result)
            {
                var args = new ScanFailureEventArgs(directory, e);
                handler(this, args);
                alive_ = args.ContinueRunning;
            }
            return result;
        }

        /// <summary>
        /// Raise the FileFailure event.
        /// </summary>
        /// <param name="file">The file name.</param>
        /// <param name="e">The exception detected.</param>
        private bool OnFileFailure(string file, Exception e)
        {
            FileFailureHandler handler = FileFailure;

            bool result = (handler != null);

            if (result)
            {
                var args = new ScanFailureEventArgs(file, e);
                FileFailure(this, args);
                alive_ = args.ContinueRunning;
            }
            return result;
        }

        /// <summary>
        /// Raise the ProcessFile event.
        /// </summary>
        /// <param name="file">The file name.</param>
        private void OnProcessFile(string file)
        {
            ProcessFileHandler handler = ProcessFile;

            if (handler != null)
            {
                var args = new ScanEventArgs(file);
                handler(this, args);
                alive_ = args.ContinueRunning;
            }
        }

        /// <summary>
        /// Raise the complete file event
        /// </summary>
        /// <param name="file">The file name</param>
        private void OnCompleteFile(string file)
        {
            CompletedFileHandler handler = CompletedFile;

            if (handler != null)
            {
                var args = new ScanEventArgs(file);
                handler(this, args);
                alive_ = args.ContinueRunning;
            }
        }

        /// <summary>
        /// Raise the ProcessDirectory event.
        /// </summary>
        /// <param name="directory">The directory name.</param>
        /// <param name="hasMatchingFiles">Flag indicating if the directory has matching files.</param>
        private void OnProcessDirectory(string directory, bool hasMatchingFiles)
        {
            EventHandler<DirectoryEventArgs> handler = ProcessDirectory;

            if (handler != null)
            {
                var args = new DirectoryEventArgs(directory, hasMatchingFiles);
                handler(this, args);
                alive_ = args.ContinueRunning;
            }
        }

        /// <summary>
        /// Scan a directory.
        /// </summary>
        /// <param name="directory">The base directory to scan.</param>
        /// <param name="recurse">True to recurse subdirectories, false to scan a single directory.</param>
        public void Scan(string directory, bool recurse)
        {
            alive_ = true;
            ScanDir(directory, recurse);
        }

        private void ScanDir(string directory, bool recurse)
        {
            try
            {
                string[] names = System.IO.Directory.GetFiles(directory);
                bool hasMatch = false;
                for (int fileIndex = 0; fileIndex < names.Length; ++fileIndex)
                {
                    if (!fileFilter_.IsMatch(names[fileIndex]))
                    {
                        names[fileIndex] = null;
                    }
                    else
                    {
                        hasMatch = true;
                    }
                }

                OnProcessDirectory(directory, hasMatch);

                if (alive_ && hasMatch)
                {
                    foreach (string fileName in names)
                    {
                        try
                        {
                            if (fileName != null)
                            {
                                OnProcessFile(fileName);
                                if (!alive_)
                                {
                                    break;
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            if (!OnFileFailure(fileName, e))
                            {
                                throw;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                if (!OnDirectoryFailure(directory, e))
                {
                    throw;
                }
            }

            if (alive_ && recurse)
            {
                try
                {
                    string[] names = System.IO.Directory.GetDirectories(directory);
                    foreach (string fulldir in names)
                    {
                        if ((directoryFilter_ == null) || (directoryFilter_.IsMatch(fulldir)))
                        {
                            ScanDir(fulldir, true);
                            if (!alive_)
                            {
                                break;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    if (!OnDirectoryFailure(directory, e))
                    {
                        throw;
                    }
                }
            }
        }

        #region Instance Fields

        /// <summary>
        /// The file filter currently in use.
        /// </summary>
        private IScanFilter fileFilter_;

        /// <summary>
        /// The directory filter currently in use.
        /// </summary>
        private IScanFilter directoryFilter_;

        /// <summary>
        /// Flag indicating if scanning should continue running.
        /// </summary>
        private bool alive_;

        #endregion Instance Fields
    }

    /// <summary>
    /// INameTransform defines how file system names are transformed for use with archives, or vice versa.
    /// </summary>
    public interface INameTransform
    {
        /// <summary>
        /// Given a file name determine the transformed value.
        /// </summary>
        /// <param name="name">The name to transform.</param>
        /// <returns>The transformed file name.</returns>
        string TransformFile(string name);

        /// <summary>
        /// Given a directory name determine the transformed value.
        /// </summary>
        /// <param name="name">The name to transform.</param>
        /// <returns>The transformed directory name</returns>
        string TransformDirectory(string name);
    }

    /// <summary>
    /// Pool for <see cref="Inflater"/> instances as they can be costly due to byte array allocations.
    /// </summary>
    internal sealed class InflaterPool
    {
        private readonly ConcurrentQueue<PooledInflater> noHeaderPool = new ConcurrentQueue<PooledInflater>();
        private readonly ConcurrentQueue<PooledInflater> headerPool = new ConcurrentQueue<PooledInflater>();

        internal static InflaterPool Instance { get; } = new InflaterPool();

        private InflaterPool()
        {
        }

        internal Inflater Rent(bool noHeader = false)
        {
            if (SharpZipLibOptions.InflaterPoolSize <= 0)
            {
                return new Inflater(noHeader);
            }

            var pool = GetPool(noHeader);

            PooledInflater inf;
            if (pool.TryDequeue(out var inflater))
            {
                inf = inflater;
                inf.Reset();
            }
            else
            {
                inf = new PooledInflater(noHeader);
            }

            return inf;
        }

        internal void Return(Inflater inflater)
        {
            if (SharpZipLibOptions.InflaterPoolSize <= 0)
            {
                return;
            }

            if (!(inflater is PooledInflater pooledInflater))
            {
                throw new ArgumentException("Returned inflater was not a pooled one");
            }

            var pool = GetPool(inflater.noHeader);
            if (pool.Count < SharpZipLibOptions.InflaterPoolSize)
            {
                pooledInflater.Reset();
                pool.Enqueue(pooledInflater);
            }
        }

        private ConcurrentQueue<PooledInflater> GetPool(bool noHeader) => noHeader ? noHeaderPool : headerPool;
    }

    /// <summary>
    /// InvalidNameException is thrown for invalid names such as directory traversal paths and names with invalid characters
    /// </summary>
    [Serializable]
    public class InvalidNameException : SharpZipBaseException
    {
        /// <summary>
        /// Initializes a new instance of the InvalidNameException class with a default error message.
        /// </summary>
        public InvalidNameException() : base("An invalid name was specified")
        {
        }

        /// <summary>
        /// Initializes a new instance of the InvalidNameException class with a specified error message.
        /// </summary>
        /// <param name="message">A message describing the exception.</param>
        public InvalidNameException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the InvalidNameException class with a specified
        /// error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">A message describing the exception.</param>
        /// <param name="innerException">The inner exception</param>
        public InvalidNameException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// Scanning filters support filtering of names.
    /// </summary>
    public interface IScanFilter
    {
        /// <summary>
        /// Test a name to see if it 'matches' the filter.
        /// </summary>
        /// <param name="name">The name to test.</param>
        /// <returns>Returns true if the name matches the filter, false if it does not match.</returns>
        bool IsMatch(string name);
    }

    /// <summary>
    /// NameFilter is a string matching class which allows for both positive and negative
    /// matching.
    /// A filter is a sequence of independant <see cref="Regex">regular expressions</see> separated by semi-colons ';'.
    /// To include a semi-colon it may be quoted as in \;. Each expression can be prefixed by a plus '+' sign or
    /// a minus '-' sign to denote the expression is intended to include or exclude names.
    /// If neither a plus or minus sign is found include is the default.
    /// A given name is tested for inclusion before checking exclusions.  Only names matching an include spec
    /// and not matching an exclude spec are deemed to match the filter.
    /// An empty filter matches any name.
    /// </summary>
    /// <example>The following expression includes all name ending in '.dat' with the exception of 'dummy.dat'
    /// "+\.dat$;-^dummy\.dat$"
    /// </example>
    public class NameFilter : IScanFilter
    {
        #region Constructors

        /// <summary>
        /// Construct an instance based on the filter expression passed
        /// </summary>
        /// <param name="filter">The filter expression.</param>
        public NameFilter(string filter)
        {
            filter_ = filter;
            inclusions_ = new List<Regex>();
            exclusions_ = new List<Regex>();
            Compile();
        }

        #endregion Constructors

        /// <summary>
        /// Test a string to see if it is a valid regular expression.
        /// </summary>
        /// <param name="expression">The expression to test.</param>
        /// <returns>True if expression is a valid <see cref="System.Text.RegularExpressions.Regex"/> false otherwise.</returns>
        public static bool IsValidExpression(string expression)
        {
            bool result = true;
            try
            {
                var exp = new Regex(expression, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            }
            catch (ArgumentException)
            {
                result = false;
            }
            return result;
        }

        /// <summary>
        /// Test an expression to see if it is valid as a filter.
        /// </summary>
        /// <param name="toTest">The filter expression to test.</param>
        /// <returns>True if the expression is valid, false otherwise.</returns>
        public static bool IsValidFilterExpression(string toTest)
        {
            bool result = true;

            try
            {
                if (toTest != null)
                {
                    string[] items = SplitQuoted(toTest);
                    for (int i = 0; i < items.Length; ++i)
                    {
                        if ((items[i] != null) && (items[i].Length > 0))
                        {
                            string toCompile;

                            if (items[i][0] == '+')
                            {
                                toCompile = items[i].Substring(1, items[i].Length - 1);
                            }
                            else if (items[i][0] == '-')
                            {
                                toCompile = items[i].Substring(1, items[i].Length - 1);
                            }
                            else
                            {
                                toCompile = items[i];
                            }

                            var testRegex = new Regex(toCompile, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                        }
                    }
                }
            }
            catch (ArgumentException)
            {
                result = false;
            }

            return result;
        }

        /// <summary>
        /// Split a string into its component pieces
        /// </summary>
        /// <param name="original">The original string</param>
        /// <returns>Returns an array of <see cref="System.String"/> values containing the individual filter elements.</returns>
        public static string[] SplitQuoted(string original)
        {
            char escape = '\\';
            char[] separators = { ';' };

            var result = new List<string>();

            if (!string.IsNullOrEmpty(original))
            {
                int endIndex = -1;
                var b = new StringBuilder();

                while (endIndex < original.Length)
                {
                    endIndex += 1;
                    if (endIndex >= original.Length)
                    {
                        result.Add(b.ToString());
                    }
                    else if (original[endIndex] == escape)
                    {
                        endIndex += 1;
                        if (endIndex >= original.Length)
                        {
                            throw new ArgumentException("Missing terminating escape character", nameof(original));
                        }
                        // include escape if this is not an escaped separator
                        if (Array.IndexOf(separators, original[endIndex]) < 0)
                            b.Append(escape);

                        b.Append(original[endIndex]);
                    }
                    else
                    {
                        if (Array.IndexOf(separators, original[endIndex]) >= 0)
                        {
                            result.Add(b.ToString());
                            b.Length = 0;
                        }
                        else
                        {
                            b.Append(original[endIndex]);
                        }
                    }
                }
            }

            return result.ToArray();
        }

        /// <summary>
        /// Convert this filter to its string equivalent.
        /// </summary>
        /// <returns>The string equivalent for this filter.</returns>
        public override string ToString()
        {
            return filter_;
        }

        /// <summary>
        /// Test a value to see if it is included by the filter.
        /// </summary>
        /// <param name="name">The value to test.</param>
        /// <returns>True if the value is included, false otherwise.</returns>
        public bool IsIncluded(string name)
        {
            bool result = false;
            if (inclusions_.Count == 0)
            {
                result = true;
            }
            else
            {
                foreach (Regex r in inclusions_)
                {
                    if (r.IsMatch(name))
                    {
                        result = true;
                        break;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Test a value to see if it is excluded by the filter.
        /// </summary>
        /// <param name="name">The value to test.</param>
        /// <returns>True if the value is excluded, false otherwise.</returns>
        public bool IsExcluded(string name)
        {
            bool result = false;
            foreach (Regex r in exclusions_)
            {
                if (r.IsMatch(name))
                {
                    result = true;
                    break;
                }
            }
            return result;
        }

        #region IScanFilter Members

        /// <summary>
        /// Test a value to see if it matches the filter.
        /// </summary>
        /// <param name="name">The value to test.</param>
        /// <returns>True if the value matches, false otherwise.</returns>
        public bool IsMatch(string name)
        {
            return (IsIncluded(name) && !IsExcluded(name));
        }

        #endregion IScanFilter Members

        /// <summary>
        /// Compile this filter.
        /// </summary>
        private void Compile()
        {
            // TODO: Check to see if combining RE's makes it faster/smaller.
            // simple scheme would be to have one RE for inclusion and one for exclusion.
            if (filter_ == null)
            {
                return;
            }

            string[] items = SplitQuoted(filter_);
            for (int i = 0; i < items.Length; ++i)
            {
                if ((items[i] != null) && (items[i].Length > 0))
                {
                    bool include = (items[i][0] != '-');
                    string toCompile;

                    if (items[i][0] == '+')
                    {
                        toCompile = items[i].Substring(1, items[i].Length - 1);
                    }
                    else if (items[i][0] == '-')
                    {
                        toCompile = items[i].Substring(1, items[i].Length - 1);
                    }
                    else
                    {
                        toCompile = items[i];
                    }

                    // NOTE: Regular expressions can fail to compile here for a number of reasons that cause an exception
                    // these are left unhandled here as the caller is responsible for ensuring all is valid.
                    // several functions IsValidFilterExpression and IsValidExpression are provided for such checking
                    if (include)
                    {
                        inclusions_.Add(new Regex(toCompile, RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline));
                    }
                    else
                    {
                        exclusions_.Add(new Regex(toCompile, RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline));
                    }
                }
            }
        }

        #region Instance Fields

        private string filter_;
        private List<Regex> inclusions_;
        private List<Regex> exclusions_;

        #endregion Instance Fields
    }

    /// <summary>
    /// PathFilter filters directories and files using a form of <see cref="System.Text.RegularExpressions.Regex">regular expressions</see>
    /// by full path name.
    /// See <see cref="NameFilter">NameFilter</see> for more detail on filtering.
    /// </summary>
    public class PathFilter : IScanFilter
    {
        #region Constructors

        /// <summary>
        /// Initialise a new instance of <see cref="PathFilter"></see>.
        /// </summary>
        /// <param name="filter">The <see cref="NameFilter">filter</see> expression to apply.</param>
        public PathFilter(string filter)
        {
            nameFilter_ = new NameFilter(filter);
        }

        #endregion Constructors

        #region IScanFilter Members

        /// <summary>
        /// Test a name to see if it matches the filter.
        /// </summary>
        /// <param name="name">The name to test.</param>
        /// <returns>True if the name matches, false otherwise.</returns>
        /// <remarks><see cref="Path.GetFullPath(string)"/> is used to get the full path before matching.</remarks>
        public virtual bool IsMatch(string name)
        {
            bool result = false;

            if (name != null)
            {
                string cooked = (name.Length > 0) ? Path.GetFullPath(name) : "";
                result = nameFilter_.IsMatch(cooked);
            }
            return result;
        }

        private readonly

        #endregion IScanFilter Members

        #region Instance Fields

        NameFilter nameFilter_;

        #endregion Instance Fields
    }

    /// <summary>
    /// ExtendedPathFilter filters based on name, file size, and the last write time of the file.
    /// </summary>
    /// <remarks>Provides an example of how to customise filtering.</remarks>
    public class ExtendedPathFilter : PathFilter
    {
        #region Constructors

        /// <summary>
        /// Initialise a new instance of ExtendedPathFilter.
        /// </summary>
        /// <param name="filter">The filter to apply.</param>
        /// <param name="minSize">The minimum file size to include.</param>
        /// <param name="maxSize">The maximum file size to include.</param>
        public ExtendedPathFilter(string filter,
            long minSize, long maxSize)
            : base(filter)
        {
            MinSize = minSize;
            MaxSize = maxSize;
        }

        /// <summary>
        /// Initialise a new instance of ExtendedPathFilter.
        /// </summary>
        /// <param name="filter">The filter to apply.</param>
        /// <param name="minDate">The minimum <see cref="DateTime"/> to include.</param>
        /// <param name="maxDate">The maximum <see cref="DateTime"/> to include.</param>
        public ExtendedPathFilter(string filter,
            DateTime minDate, DateTime maxDate)
            : base(filter)
        {
            MinDate = minDate;
            MaxDate = maxDate;
        }

        /// <summary>
        /// Initialise a new instance of ExtendedPathFilter.
        /// </summary>
        /// <param name="filter">The filter to apply.</param>
        /// <param name="minSize">The minimum file size to include.</param>
        /// <param name="maxSize">The maximum file size to include.</param>
        /// <param name="minDate">The minimum <see cref="DateTime"/> to include.</param>
        /// <param name="maxDate">The maximum <see cref="DateTime"/> to include.</param>
        public ExtendedPathFilter(string filter,
            long minSize, long maxSize,
            DateTime minDate, DateTime maxDate)
            : base(filter)
        {
            MinSize = minSize;
            MaxSize = maxSize;
            MinDate = minDate;
            MaxDate = maxDate;
        }

        #endregion Constructors

        #region IScanFilter Members

        /// <summary>
        /// Test a filename to see if it matches the filter.
        /// </summary>
        /// <param name="name">The filename to test.</param>
        /// <returns>True if the filter matches, false otherwise.</returns>
        /// <exception cref="System.IO.FileNotFoundException">The <see paramref="fileName"/> doesnt exist</exception>
        public override bool IsMatch(string name)
        {
            bool result = base.IsMatch(name);

            if (result)
            {
                var fileInfo = new FileInfo(name);
                result =
                    (MinSize <= fileInfo.Length) &&
                    (MaxSize >= fileInfo.Length) &&
                    (MinDate <= fileInfo.LastWriteTime) &&
                    (MaxDate >= fileInfo.LastWriteTime)
                    ;
            }
            return result;
        }

        #endregion IScanFilter Members

        #region Properties

        /// <summary>
        /// Get/set the minimum size/length for a file that will match this filter.
        /// </summary>
        /// <remarks>The default value is zero.</remarks>
        /// <exception cref="ArgumentOutOfRangeException">value is less than zero; greater than <see cref="MaxSize"/></exception>
        public long MinSize
        {
            get { return minSize_; }
            set
            {
                if ((value < 0) || (maxSize_ < value))
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                minSize_ = value;
            }
        }

        /// <summary>
        /// Get/set the maximum size/length for a file that will match this filter.
        /// </summary>
        /// <remarks>The default value is <see cref="System.Int64.MaxValue"/></remarks>
        /// <exception cref="ArgumentOutOfRangeException">value is less than zero or less than <see cref="MinSize"/></exception>
        public long MaxSize
        {
            get { return maxSize_; }
            set
            {
                if ((value < 0) || (minSize_ > value))
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                maxSize_ = value;
            }
        }

        /// <summary>
        /// Get/set the minimum <see cref="DateTime"/> value that will match for this filter.
        /// </summary>
        /// <remarks>Files with a LastWrite time less than this value are excluded by the filter.</remarks>
        public DateTime MinDate
        {
            get
            {
                return minDate_;
            }

            set
            {
                if (value > maxDate_)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Exceeds MaxDate");
                }

                minDate_ = value;
            }
        }

        /// <summary>
        /// Get/set the maximum <see cref="DateTime"/> value that will match for this filter.
        /// </summary>
        /// <remarks>Files with a LastWrite time greater than this value are excluded by the filter.</remarks>
        public DateTime MaxDate
        {
            get
            {
                return maxDate_;
            }

            set
            {
                if (minDate_ > value)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Exceeds MinDate");
                }

                maxDate_ = value;
            }
        }

        #endregion Properties

        #region Instance Fields

        private long minSize_;
        private long maxSize_ = long.MaxValue;
        private DateTime minDate_ = DateTime.MinValue;
        private DateTime maxDate_ = DateTime.MaxValue;

        #endregion Instance Fields
    }

    /// <summary>
    /// PathUtils provides simple utilities for handling paths.
    /// </summary>
    public static class PathUtils
    {
        /// <summary>
        /// Remove any path root present in the path
        /// </summary>
        /// <param name="path">A <see cref="string"/> containing path information.</param>
        /// <returns>The path with the root removed if it was present; path otherwise.</returns>
        public static string DropPathRoot(string path)
        {
            // No need to drop anything
            if (path == string.Empty) return path;

            var invalidChars = Path.GetInvalidPathChars();
            // If the first character after the root is a ':', .NET < 4.6.2 throws
            var cleanRootSep = path.Length >= 3 && path[1] == ':' && path[2] == ':';

            // Replace any invalid path characters with '_' to prevent Path.GetPathRoot from throwing.
            // Only pass the first 258 (should be 260, but that still throws for some reason) characters
            // as .NET < 4.6.2 throws on longer paths
            var cleanPath = new string(path.Take(258)
                .Select((c, i) => invalidChars.Contains(c) || (i == 2 && cleanRootSep) ? '_' : c).ToArray());

            var stripLength = Path.GetPathRoot(cleanPath)?.Length ?? 0;
            while (path.Length > stripLength && (path[stripLength] == '/' || path[stripLength] == '\\')) stripLength++;
            return path.Substring(stripLength);
        }

        /// <summary>
        /// Returns a random file name in the users temporary directory, or in directory of <paramref name="original"/> if specified
        /// </summary>
        /// <param name="original">If specified, used as the base file name for the temporary file</param>
        /// <returns>Returns a temporary file name</returns>
        public static string GetTempFileName(string original = null)
        {
            string fileName;
            var tempPath = Path.GetTempPath();

            do
            {
                fileName = original == null
                    ? Path.Combine(tempPath, Path.GetRandomFileName())
                    : $"{original}.{Path.GetRandomFileName()}";
            } while (File.Exists(fileName));

            return fileName;
        }
    }

    /// <summary>
    /// Provides simple <see cref="Stream"/> utilities.
    /// </summary>
    public static class StreamUtils
    {
        /// <summary>
        /// Read from a <see cref="Stream"/> ensuring all the required data is read.
        /// </summary>
        /// <param name="stream">The stream to read.</param>
        /// <param name="buffer">The buffer to fill.</param>
        /// <seealso cref="ReadFully(Stream,byte[],int,int)"/>
        public static void ReadFully(Stream stream, byte[] buffer)
        {
            ReadFully(stream, buffer, 0, buffer.Length);
        }

        /// <summary>
        /// Read from a <see cref="Stream"/> ensuring all the required data is read.
        /// </summary>
        /// <param name="stream">The stream to read data from.</param>
        /// <param name="buffer">The buffer to store data in.</param>
        /// <param name="offset">The offset at which to begin storing data.</param>
        /// <param name="count">The number of bytes of data to store.</param>
        /// <exception cref="ArgumentNullException">Required parameter is null</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="offset"/> and or <paramref name="count"/> are invalid.</exception>
        /// <exception cref="EndOfStreamException">End of stream is encountered before all the data has been read.</exception>
        public static void ReadFully(Stream stream, byte[] buffer, int offset, int count)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            // Offset can equal length when buffer and count are 0.
            if ((offset < 0) || (offset > buffer.Length))
            {
                throw new ArgumentOutOfRangeException(nameof(offset));
            }

            if ((count < 0) || (offset + count > buffer.Length))
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            while (count > 0)
            {
                int readCount = stream.Read(buffer, offset, count);
                if (readCount <= 0)
                {
                    throw new EndOfStreamException();
                }
                offset += readCount;
                count -= readCount;
            }
        }

        /// <summary>
        /// Read as much data as possible from a <see cref="Stream"/>, up to the requested number of bytes
        /// </summary>
        /// <param name="stream">The stream to read data from.</param>
        /// <param name="buffer">The buffer to store data in.</param>
        /// <param name="offset">The offset at which to begin storing data.</param>
        /// <param name="count">The number of bytes of data to store.</param>
        /// <exception cref="ArgumentNullException">Required parameter is null</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="offset"/> and or <paramref name="count"/> are invalid.</exception>
        public static int ReadRequestedBytes(Stream stream, byte[] buffer, int offset, int count)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            // Offset can equal length when buffer and count are 0.
            if ((offset < 0) || (offset > buffer.Length))
            {
                throw new ArgumentOutOfRangeException(nameof(offset));
            }

            if ((count < 0) || (offset + count > buffer.Length))
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            int totalReadCount = 0;
            while (count > 0)
            {
                int readCount = stream.Read(buffer, offset, count);
                if (readCount <= 0)
                {
                    break;
                }
                offset += readCount;
                count -= readCount;
                totalReadCount += readCount;
            }

            return totalReadCount;
        }

        /// <summary>
        /// Copy the contents of one <see cref="Stream"/> to another.
        /// </summary>
        /// <param name="source">The stream to source data from.</param>
        /// <param name="destination">The stream to write data to.</param>
        /// <param name="buffer">The buffer to use during copying.</param>
        public static void Copy(Stream source, Stream destination, byte[] buffer)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (destination == null)
            {
                throw new ArgumentNullException(nameof(destination));
            }

            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            // Ensure a reasonable size of buffer is used without being prohibitive.
            if (buffer.Length < 128)
            {
                throw new ArgumentException("Buffer is too small", nameof(buffer));
            }

            bool copying = true;

            while (copying)
            {
                int bytesRead = source.Read(buffer, 0, buffer.Length);
                if (bytesRead > 0) {
                    destination.Write(buffer, 0, bytesRead);
                } else {
                    destination.Flush();
                    copying = false;
                }
            }
        }

        /// <summary>
        /// Copy the contents of one <see cref="Stream"/> to another.
        /// </summary>
        /// <param name="source">The stream to source data from.</param>
        /// <param name="destination">The stream to write data to.</param>
        /// <param name="buffer">The buffer to use during copying.</param>
        /// <param name="progressHandler">The <see cref="ProgressHandler">progress handler delegate</see> to use.</param>
        /// <param name="updateInterval">The minimum <see cref="TimeSpan"/> between progress updates.</param>
        /// <param name="sender">The source for this event.</param>
        /// <param name="name">The name to use with the event.</param>
        /// <remarks>This form is specialised for use within #Zip to support events during archive operations.</remarks>
        public static void Copy(Stream source, Stream destination,
            byte[] buffer, ProgressHandler progressHandler, TimeSpan updateInterval, object sender, string name)
        {
            Copy(source, destination, buffer, progressHandler, updateInterval, sender, name, -1);
        }

        /// <summary>
        /// Copy the contents of one <see cref="Stream"/> to another.
        /// </summary>
        /// <param name="source">The stream to source data from.</param>
        /// <param name="destination">The stream to write data to.</param>
        /// <param name="buffer">The buffer to use during copying.</param>
        /// <param name="progressHandler">The <see cref="ProgressHandler">progress handler delegate</see> to use.</param>
        /// <param name="updateInterval">The minimum <see cref="TimeSpan"/> between progress updates.</param>
        /// <param name="sender">The source for this event.</param>
        /// <param name="name">The name to use with the event.</param>
        /// <param name="fixedTarget">A predetermined fixed target value to use with progress updates.
        /// If the value is negative the target is calculated by looking at the stream.</param>
        /// <remarks>This form is specialised for use within #Zip to support events during archive operations.</remarks>
        public static void Copy(Stream source, Stream destination,
            byte[] buffer,
            ProgressHandler progressHandler, TimeSpan updateInterval,
            object sender, string name, long fixedTarget)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (destination == null)
            {
                throw new ArgumentNullException(nameof(destination));
            }

            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            // Ensure a reasonable size of buffer is used without being prohibitive.
            if (buffer.Length < 128)
            {
                throw new ArgumentException("Buffer is too small", nameof(buffer));
            }

            if (progressHandler == null)
            {
                throw new ArgumentNullException(nameof(progressHandler));
            }

            bool copying = true;

            DateTime marker = DateTime.Now;
            long processed = 0;
            long target = 0;

            if (fixedTarget >= 0)
            {
                target = fixedTarget;
            }
            else if (source.CanSeek)
            {
                target = source.Length - source.Position;
            }

            // Always fire 0% progress..
            var args = new ProgressEventArgs(name, processed, target);
            progressHandler(sender, args);

            bool progressFired = true;

            while (copying)
            {
                int bytesRead = source.Read(buffer, 0, buffer.Length);
                if (bytesRead > 0)
                {
                    processed += bytesRead;
                    progressFired = false;
                    destination.Write(buffer, 0, bytesRead);
                }
                else
                {
                    destination.Flush();
                    copying = false;
                }

                if (DateTime.Now - marker > updateInterval)
                {
                    progressFired = true;
                    marker = DateTime.Now;
                    args = new ProgressEventArgs(name, processed, target);
                    progressHandler(sender, args);

                    copying = args.ContinueRunning;
                }
            }

            if (!progressFired)
            {
                args = new ProgressEventArgs(name, processed, target);
                progressHandler(sender, args);
            }
        }

        internal static async Task WriteProcToStreamAsync(this Stream targetStream, Microsoft.IO.MemoryStream bufferStream, Action<Stream> writeProc, CancellationToken ct)
        {
            bufferStream.SetLength(0);
            writeProc(bufferStream);
            bufferStream.Position = 0;
            await bufferStream.CopyToAsync(targetStream, 81920, ct).ConfigureAwait(false);
            bufferStream.SetLength(0);
        }

        internal static async Task WriteProcToStreamAsync(this Stream targetStream, Action<Stream> writeProc, CancellationToken ct)
        {
            using (var ms = new Microsoft.IO.MemoryStream())
            {
                await WriteProcToStreamAsync(targetStream, ms, writeProc, ct).ConfigureAwait(false);
            }
        }
    }
}
