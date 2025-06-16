// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text;
using System.Buffers;
using System.Threading;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Microsoft.IO
{
    // Class for creating FileStream objects, and some basic file management
    // routines such as Delete, etc.
    public static class File
    {
        private const int MaxByteArrayLength = 0x7FFFFFC7;
        private static Encoding s_UTF8NoBOM;

        internal const int DefaultBufferSize = 4096;

        public static StreamReader OpenText(string path)
        {
            if (path is null)
                throw new ArgumentNullException(nameof(path));

            return new(path);
        }

        public static System.IO.StreamWriter CreateText(string path)
        {
            if (path is null)
                throw new ArgumentNullException(nameof(path));

            return new(path, append: false);
        }

        public static System.IO.StreamWriter AppendText(string path)
        {
            if (path is null)
                throw new ArgumentNullException(nameof(path));

            return new(path, append: true);
        }

        /// <summary>
        /// Copies an existing file to a new file.
        /// An exception is raised if the destination file already exists.
        /// </summary>
        public static void Copy(string sourceFileName, string destFileName)
            => Copy(sourceFileName, destFileName, overwrite: false);

        /// <summary>
        /// Copies an existing file to a new file.
        /// If <paramref name="overwrite"/> is false, an exception will be
        /// raised if the destination exists. Otherwise it will be overwritten.
        /// </summary>
        public static void Copy(string sourceFileName, string destFileName, bool overwrite)
        {
            if (sourceFileName == null)
                throw new ArgumentNullException(nameof(sourceFileName), SR.ArgumentNull_FileName);
            if (destFileName == null)
                throw new ArgumentNullException(nameof(destFileName), SR.ArgumentNull_FileName);
            if (sourceFileName.Length == 0)
                throw new ArgumentException(SR.Argument_EmptyFileName, nameof(sourceFileName));
            if (destFileName.Length == 0)
                throw new ArgumentException(SR.Argument_EmptyFileName, nameof(destFileName));

            FileSystem.CopyFile(Path.GetFullPath(sourceFileName), Path.GetFullPath(destFileName), overwrite);
        }

        /// <summary>
        /// Creates a file in a particular path. If the file exists, it is replaced. <br />
        /// The file is opened with ReadWrite access and cannot be opened by another application until it has been closed. <br />
        /// An <see cref="System.IO.IOException"/> is thrown if the directory specified doesn't exist.
        /// </summary>
        /// <param name="path">The file path where the file is to be created.</param>
        /// <returns>A <see cref="FileStream"/> object representing the opened file.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> was <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="path"/> was representing the empty string.</exception>
        /// <exception cref="System.IO.IOException">An unexpected I/O exception was occured.</exception>
        /// <exception cref="UnauthorizedAccessException">The application does not have access to the specific path.</exception>
        public static FileStream Create(string path) => Create(path, DefaultBufferSize);

        /// <summary>
        /// Creates a file in a particular path. If the file exists, it is replaced. <br />
        /// The file is opened with ReadWrite access and cannot be opened by another application until it has been closed. <br />
        /// An IOException is thrown if the directory specified doesn't exist.
        /// </summary>
        /// <param name="path">The file path where the file is to be created.</param>
        /// <param name="bufferSize"></param>
        /// <returns>A <see cref="FileStream"/> object representing the opened file.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> was <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="path"/> was representing the empty string.</exception>
        /// <exception cref="System.IO.IOException">An unexpected I/O exception was occured.</exception>
        /// <exception cref="UnauthorizedAccessException">The application does not have access to the specific path.</exception>
        public static FileStream Create(string path, int bufferSize) // bufferSize is currently not used.
            => new(path, FileMode.Create, FileAccess.ReadWrite, FileShare.None);

        /// <summary>
        /// Deletes a file. The file specified by the designated path is deleted. <br />
        /// If the file does not exist, Delete succeeds without throwing an exception. <br /> <br />
        /// 
        /// On Windows, Delete will fail for a file that is open for normal I/O or a file that is memory mapped.
        /// </summary>
        /// <param name="path">The path of the file that is to be deleted.</param>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> was <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="path"/> was representing the empty string.</exception>
        public static void Delete(string path)
        {
            ArgumentException.ThrowIfNullOrEmpty(path);
            FileSystem.DeleteFile(Path.GetFullPath(path));
        }

        /// <summary>
        /// Renames a file. The file specified by the designated path is moved based on the path provided in <paramref name="newname"/> parameter. <br />
        /// The file must exist so that this operation can succeed. <br />
        /// </summary>
        /// <param name="sourcepath"></param>
        /// <param name="newname"></param>
        /// <exception cref="ArgumentNullException"><paramref name="sourcepath"/> and/or <paramref name="newname"/> were <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="sourcepath"/> and/or <paramref name="newname"/> were representing the empty string.</exception>
        /// <exception cref="System.IO.FileNotFoundException">The source file does not exist.</exception>
        /// <exception cref="UnauthorizedAccessException">The application does not have access to the specific path.</exception>
        public static void Rename(System.String sourcepath , System.String newname)
        {
            ArgumentException.ThrowIfNullOrEmpty(sourcepath);
            ArgumentException.ThrowIfNullOrWhiteSpace(newname);
            FileSystem.RenameFile(sourcepath, newname);
        }

        /// <summary>
        /// Tests whether a file exists. <br />
        /// The result is <see langword="true"/> if the file given by the specified path exists; otherwise, the result is <see langword="false"/>. <br />
        /// Note that if <paramref name="path"/> describes a directory, Exists will return <see langword="true"/>.
        /// </summary>
        /// <param name="path">The path to be tested.</param>
        /// <returns>A value whether the file specified in <paramref name="path"/> is present on the file system.</returns>
        public static System.Boolean Exists(string path)
        {
            try
            {
                if (path is null)
                    return false;
                if (path.Length == 0)
                    return false;

                path = Path.GetFullPath(path);

                
                // GetFullPath should never return null
                Debug.Assert(path is not null, "File.Exists: GetFullPath returned null");
                // After normalizing, check whether path ends in directory separator.
                // Otherwise, FillAttributeInfo removes it and we may return a false positive.
                if (path.Length > 0 && System.IO.PathInternal.IsDirectorySeparator(path[path.Length - 1]))
                {
                    return false;
                }

                return FileSystem.FileExists(path);
            }
            catch (ArgumentException) { }
            catch (System.IO.IOException) { }
            catch (UnauthorizedAccessException) { }

            return false;
        }

        public static FileStream Open(string path, FileMode mode) => Open(path, mode, (mode == FileMode.Append ? FileAccess.Write : FileAccess.ReadWrite), FileShare.None);

        public static FileStream Open(string path, FileMode mode, FileAccess access) => Open(path, mode, access, FileShare.None);

        public static FileStream Open(string path, FileMode mode, FileAccess access, FileShare share) => new(path, mode, access, share);

        internal static DateTimeOffset GetUtcDateTimeOffset(DateTime dateTime)
        {
            // File and Directory UTC APIs treat a DateTimeKind.Unspecified as UTC whereas 
            // ToUniversalTime treats this as local.
            if (dateTime.Kind == DateTimeKind.Unspecified)
            {
                return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
            }

            return dateTime.ToUniversalTime();
        }

        public static void SetCreationTime(string path, DateTime creationTime)
        {
            string fullPath = Path.GetFullPath(path);
            FileSystem.SetCreationTime(fullPath, creationTime, asDirectory: false);
        }

        public static void SetCreationTimeUtc(string path, DateTime creationTimeUtc)
        {
            string fullPath = Path.GetFullPath(path);
            FileSystem.SetCreationTime(fullPath, GetUtcDateTimeOffset(creationTimeUtc), asDirectory: false);
        }

        public static DateTime GetCreationTime(string path)
        {
            string fullPath = Path.GetFullPath(path);
            return FileSystem.GetCreationTime(fullPath).LocalDateTime;
        }

        public static DateTime GetCreationTimeUtc(string path)
        {
            string fullPath = Path.GetFullPath(path);
            return FileSystem.GetCreationTime(fullPath).UtcDateTime;
        }

        public static void SetLastAccessTime(string path, DateTime lastAccessTime)
        {
            string fullPath = Path.GetFullPath(path);
            FileSystem.SetLastAccessTime(fullPath, lastAccessTime, asDirectory: false);
        }

        public static void SetLastAccessTimeUtc(string path, DateTime lastAccessTimeUtc)
        {
            string fullPath = Path.GetFullPath(path);
            FileSystem.SetLastAccessTime(fullPath, GetUtcDateTimeOffset(lastAccessTimeUtc), asDirectory: false);
        }

        public static DateTime GetLastAccessTime(string path)
        {
            string fullPath = Path.GetFullPath(path);
            return FileSystem.GetLastAccessTime(fullPath).LocalDateTime;
        }

        public static DateTime GetLastAccessTimeUtc(string path)
        {
            string fullPath = Path.GetFullPath(path);
            return FileSystem.GetLastAccessTime(fullPath).UtcDateTime;
        }

        public static void SetLastWriteTime(string path, DateTime lastWriteTime)
        {
            string fullPath = Path.GetFullPath(path);
            FileSystem.SetLastWriteTime(fullPath, lastWriteTime, asDirectory: false);
        }

        public static void SetLastWriteTimeUtc(string path, DateTime lastWriteTimeUtc)
        {
            string fullPath = Path.GetFullPath(path);
            FileSystem.SetLastWriteTime(fullPath, GetUtcDateTimeOffset(lastWriteTimeUtc), asDirectory: false);
        }

        public static DateTime GetLastWriteTime(string path)
        {
            string fullPath = Path.GetFullPath(path);
            return FileSystem.GetLastWriteTime(fullPath).LocalDateTime;
        }

        public static DateTime GetLastWriteTimeUtc(string path)
        {
            string fullPath = Path.GetFullPath(path);
            return FileSystem.GetLastWriteTime(fullPath).UtcDateTime;
        }

        public static System.IO.FileAttributes GetAttributes(string path)
        {
            string fullPath = Path.GetFullPath(path);
            return FileSystem.GetAttributes(fullPath);
        }

        public static void SetAttributes(string path, System.IO.FileAttributes fileAttributes)
        {
            string fullPath = Path.GetFullPath(path);
            FileSystem.SetAttributes(fullPath, fileAttributes);
        }

        public static FileStream OpenRead(string path) => new(path, FileMode.Open, FileAccess.Read, FileShare.Read);

        public static FileStream OpenWrite(string path) => new(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);

        public static string ReadAllText(string path)
        {
            if (path is null)
                throw new ArgumentNullException(nameof(path));
            if (path.Length == 0)
                throw new ArgumentException(SR.Argument_EmptyPath, nameof(path));

            return InternalReadAllText(path, Encoding.UTF8);
        }

        public static string ReadAllText(string path, Encoding encoding)
        {
            if (path is null)
                throw new ArgumentNullException(nameof(path));
            if (encoding == null)
                throw new ArgumentNullException(nameof(encoding));
            if (path.Length == 0)
                throw new ArgumentException(SR.Argument_EmptyPath, nameof(path));

            return InternalReadAllText(path, encoding);
        }

        private static string InternalReadAllText(string path, Encoding encoding)
        {
            Debug.Assert(path != null);
            Debug.Assert(encoding != null);
            Debug.Assert(path.Length > 0);

            using (var sr = new StreamReader(path, encoding, detectEncodingFromByteOrderMarks: true))
                return sr.ReadToEnd();
        }

        public static void WriteAllText(string path, string contents)
        {
            if (path is null)
                throw new ArgumentNullException(nameof(path));
            if (path.Length == 0)
                throw new ArgumentException(SR.Argument_EmptyPath, nameof(path));

            using (var sw = new System.IO.StreamWriter(path))
            {
                sw.Write(contents);
            }
        }

        public static void WriteAllText(string path, string contents, Encoding encoding)
        {
            if (path is null)
                throw new ArgumentNullException(nameof(path));
            if (encoding == null)
                throw new ArgumentNullException(nameof(encoding));
            if (path.Length == 0)
                throw new ArgumentException(SR.Argument_EmptyPath, nameof(path));

            using (var sw = new System.IO.StreamWriter(path, false, encoding))
            {
                sw.Write(contents);
            }
        }

        public static byte[] ReadAllBytes(string path)
        {
            // bufferSize == 1 used to avoid unnecessary buffer in FileStream
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                long fileLength = fs.Length;
                if (fileLength > int.MaxValue)
                {
                    throw new System.IO.IOException(SR.IO_FileTooLong2GB);
                }
                else if (fileLength == 0)
                {
                    // Some file systems (e.g. procfs on Linux) return 0 for length even when there's content.
                    // Thus we need to assume 0 doesn't mean empty.
                    return ReadAllBytesUnknownLength(fs);
                }

                int index = 0;
                int count = (int)fileLength;
                byte[] bytes = new byte[count];
                while (count > 0)
                {
                    int n = fs.Read(bytes, index, count);
                    if (n == 0)
                        throw System.IO.Error.GetEndOfFile();
                    index += n;
                    count -= n;
                }
                return bytes;
            }
        }

        private static byte[] ReadAllBytesUnknownLength(FileStream fs)
        {
            byte[] rentedArray = null;
            Span<byte> buffer = stackalloc byte[512];
            try
            {
                int bytesRead = 0;
                while (true)
                {
                    if (bytesRead == buffer.Length)
                    {
                        uint newLength = (uint)buffer.Length * 2;
                        if (newLength > MaxByteArrayLength)
                        {
                            newLength = (uint)Math.Max(MaxByteArrayLength, buffer.Length + 1);
                        }

                        byte[] tmp = ArrayPool<byte>.Shared.Rent((int)newLength);
                        buffer.CopyTo(tmp);
                        if (rentedArray != null)
                        {
                            ArrayPool<byte>.Shared.Return(rentedArray);
                        }
                        buffer = rentedArray = tmp;
                    }

                    Debug.Assert(bytesRead < buffer.Length);
                    int n = fs.Read(buffer.Slice(bytesRead));
                    if (n == 0)
                    {
                        return buffer.Slice(0, bytesRead).ToArray();
                    }
                    bytesRead += n;
                }
            }
            finally
            {
                if (rentedArray != null)
                {
                    ArrayPool<byte>.Shared.Return(rentedArray);
                }
            }
        }

        public static void WriteAllBytes(string path, byte[] bytes)
        {
            if (path is null)
                throw new ArgumentNullException(nameof(path), SR.ArgumentNull_Path);
            if (path.Length == 0)
                throw new ArgumentException(SR.Argument_EmptyPath, nameof(path));
            if (bytes == null)
                throw new ArgumentNullException(nameof(bytes));

            InternalWriteAllBytes(path, bytes);
        }

        private static void InternalWriteAllBytes(string path, byte[] bytes)
        {
            Debug.Assert(path != null);
            Debug.Assert(path.Length != 0);
            Debug.Assert(bytes != null);

            using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                fs.Write(bytes, 0, bytes.Length);
            }
        }

        public static string[] ReadAllLines(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));
            if (path.Length == 0)
                throw new ArgumentException(SR.Argument_EmptyPath, nameof(path));

            return InternalReadAllLines(path, Encoding.UTF8);
        }

        public static string[] ReadAllLines(string path, Encoding encoding)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));
            if (encoding == null)
                throw new ArgumentNullException(nameof(encoding));
            if (path.Length == 0)
                throw new ArgumentException(SR.Argument_EmptyPath, nameof(path));

            return InternalReadAllLines(path, encoding);
        }

        private static string[] InternalReadAllLines(string path, Encoding encoding)
        {
            Debug.Assert(path != null);
            Debug.Assert(encoding != null);
            Debug.Assert(path.Length != 0);

            string line;
            List<string> lines = new(10);

            using (var sr = new StreamReader(path, encoding))
                while ((line = sr.ReadLine()) is not null)
                    lines.Add(line);

            return lines.ToArray();
        }

        public static IEnumerable<string> ReadLines(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));
            if (path.Length == 0)
                throw new ArgumentException(SR.Argument_EmptyPath, nameof(path));

            return System.IO.ReadLinesIterator.CreateIterator(path, Encoding.UTF8);
        }

        public static IEnumerable<string> ReadLines(string path, Encoding encoding)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));
            if (encoding == null)
                throw new ArgumentNullException(nameof(encoding));
            if (path.Length == 0)
                throw new ArgumentException(SR.Argument_EmptyPath, nameof(path));

            return System.IO.ReadLinesIterator.CreateIterator(path, encoding);
        }

        public static void WriteAllLines(string path, string[] contents) => WriteAllLines(path, (IEnumerable<string>)contents);

        public static void WriteAllLines(string path, IEnumerable<string> contents)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));
            if (contents == null)
                throw new ArgumentNullException(nameof(contents));
            if (path.Length == 0)
                throw new ArgumentException(SR.Argument_EmptyPath, nameof(path));

            InternalWriteAllLines(new System.IO.StreamWriter(path), contents);
        }

        public static void WriteAllLines(string path, string[] contents, Encoding encoding) => WriteAllLines(path, (IEnumerable<string>)contents, encoding);

        public static void WriteAllLines(string path, IEnumerable<string> contents, Encoding encoding)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));
            if (contents == null)
                throw new ArgumentNullException(nameof(contents));
            if (encoding == null)
                throw new ArgumentNullException(nameof(encoding));
            if (path.Length == 0)
                throw new ArgumentException(SR.Argument_EmptyPath, nameof(path));

            InternalWriteAllLines(new System.IO.StreamWriter(path, false, encoding), contents);
        }

        private static void InternalWriteAllLines(System.IO.TextWriter writer, IEnumerable<string> contents)
        {
            Debug.Assert(writer != null);
            Debug.Assert(contents != null);

            using (writer)
            {
                foreach (string line in contents)
                {
                    writer.WriteLine(line);
                }
            }
        }

        public static void AppendAllText(string path, string contents)
        {
            if (path is null)
                throw new ArgumentNullException(nameof(path));
            if (path.Length == 0)
                throw new ArgumentException(SR.Argument_EmptyPath, nameof(path));

            using (var sw = new System.IO.StreamWriter(path, append: true))
            {
                sw.Write(contents);
            }
        }

        public static void AppendAllText(string path, string contents, Encoding encoding)
        {
            if (path is null)
                throw new ArgumentNullException(nameof(path));
            if (encoding == null)
                throw new ArgumentNullException(nameof(encoding));
            if (path.Length == 0)
                throw new ArgumentException(SR.Argument_EmptyPath, nameof(path));

            using (var sw = new System.IO.StreamWriter(path, true, encoding))
            {
                sw.Write(contents);
            }
        }

        public static void AppendAllLines(string path, IEnumerable<string> contents)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));
            if (contents == null)
                throw new ArgumentNullException(nameof(contents));
            if (path.Length == 0)
                throw new ArgumentException(SR.Argument_EmptyPath, nameof(path));

            InternalWriteAllLines(new System.IO.StreamWriter(path, append: true), contents);
        }

        public static void AppendAllLines(string path, IEnumerable<string> contents, Encoding encoding)
        {
            if (path is null)
                throw new ArgumentNullException(nameof(path));
            if (contents is null)
                throw new ArgumentNullException(nameof(contents));
            if (encoding is null)
                throw new ArgumentNullException(nameof(encoding));
            if (path.Length == 0)
                throw new ArgumentException(SR.Argument_EmptyPath, nameof(path));

            InternalWriteAllLines(new System.IO.StreamWriter(path, true, encoding), contents);
        }

        public static void Replace(string sourceFileName, string destinationFileName, string destinationBackupFileName)
        {
            Replace(sourceFileName, destinationFileName, destinationBackupFileName, ignoreMetadataErrors: false);
        }

        public static void Replace(string sourceFileName, string destinationFileName, string destinationBackupFileName, bool ignoreMetadataErrors)
        {
            if (sourceFileName == null)
                throw new ArgumentNullException(nameof(sourceFileName));
            if (destinationFileName == null)
                throw new ArgumentNullException(nameof(destinationFileName));

            FileSystem.ReplaceFile(
                Path.GetFullPath(sourceFileName), 
                Path.GetFullPath(destinationFileName),
                destinationBackupFileName != null ? Path.GetFullPath(destinationBackupFileName) : null,
                ignoreMetadataErrors);
        }

        // Moves a specified file to a new location and potentially a new file name.
        // This method does work across volumes.
        //
        // The caller must have certain FileIOPermissions.  The caller must
        // have Read and Write permission to 
        // sourceFileName and Write 
        // permissions to destFileName.
        // 
        public static void Move(string sourceFileName, string destFileName)
        {
            Move(sourceFileName, destFileName, false);
        }

        public static void Move(string sourceFileName, string destFileName, bool overwrite)
        {
            if (sourceFileName == null)
                throw new ArgumentNullException(nameof(sourceFileName), SR.ArgumentNull_FileName);
            if (destFileName == null)
                throw new ArgumentNullException(nameof(destFileName), SR.ArgumentNull_FileName);
            if (sourceFileName.Length == 0)
                throw new ArgumentException(SR.Argument_EmptyFileName, nameof(sourceFileName));
            if (destFileName.Length == 0)
                throw new ArgumentException(SR.Argument_EmptyFileName, nameof(destFileName));

            string fullSourceFileName = Path.GetFullPath(sourceFileName);
            string fullDestFileName = Path.GetFullPath(destFileName);

            if (!FileSystem.FileExists(fullSourceFileName))
            {
                throw new System.IO.FileNotFoundException(System.String.Format(SR.IO_FileNotFound_FileName, fullSourceFileName), fullSourceFileName);
            }

            FileSystem.MoveFile(fullSourceFileName, fullDestFileName, overwrite);
        }

        public static void Encrypt(string path)
        {
            FileSystem.Encrypt(path ?? throw new ArgumentNullException(nameof(path)));
        }

        public static void Decrypt(string path)
        {
            FileSystem.Decrypt(path ?? throw new ArgumentNullException(nameof(path)));
        }

        // UTF-8 without BOM and with error detection. Same as the default encoding for StreamWriter.
        private static Encoding UTF8NoBOM => s_UTF8NoBOM ?? (s_UTF8NoBOM = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true));

        // If we use the path-taking constructors we will not have FileOptions.Asynchronous set and
        // we will have asynchronous file access faked by the thread pool. We want the real thing.
        private static StreamReader AsyncStreamReader(string path, Encoding encoding)
        {
            var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);

            return new(stream, encoding, detectEncodingFromByteOrderMarks: true);
        }

        private static System.IO.StreamWriter AsyncStreamWriter(string path, Encoding encoding, bool append)
        {
            var stream = new FileStream(path, append ? FileMode.Append : FileMode.Create, FileAccess.Write, FileShare.Read);

            return new(stream, encoding);
        }

        public static Task<string> ReadAllTextAsync(string path, CancellationToken cancellationToken = default(CancellationToken))
            => ReadAllTextAsync(path, Encoding.UTF8, cancellationToken);

        public static Task<string> ReadAllTextAsync(string path, Encoding encoding, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));
            if (encoding == null)
                throw new ArgumentNullException(nameof(encoding));
            if (path.Length == 0)
                throw new ArgumentException(SR.Argument_EmptyPath, nameof(path));

            return cancellationToken.IsCancellationRequested
                ? Task.FromCanceled<string>(cancellationToken)
                : InternalReadAllTextAsync(path, encoding, cancellationToken);
        }

        private static async Task<string> InternalReadAllTextAsync(string path, Encoding encoding, CancellationToken cancellationToken)
        {
            Debug.Assert(!string.IsNullOrEmpty(path));
            Debug.Assert(encoding != null);

            char[] buffer = null;
            var sr = AsyncStreamReader(path, encoding);
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                buffer = ArrayPool<char>.Shared.Rent(sr.CurrentEncoding.GetMaxCharCount(DefaultBufferSize));
                StringBuilder sb = new StringBuilder();
                for (;;)
                {
                    int read = await sr.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);
                    if (read == 0)
                    {
                        return sb.ToString();
                    }

                    sb.Append(buffer, 0, read);
                }
            }
            finally
            {
                sr.Dispose();
                if (buffer != null)
                {
                    ArrayPool<char>.Shared.Return(buffer);
                }
            }
        }

        public static Task WriteAllTextAsync(string path, string contents, CancellationToken cancellationToken = default(CancellationToken))
            => WriteAllTextAsync(path, contents, UTF8NoBOM, cancellationToken);

        public static Task WriteAllTextAsync(string path, string contents, Encoding encoding, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));
            if (encoding == null)
                throw new ArgumentNullException(nameof(encoding));
            if (path.Length == 0)
                throw new ArgumentException(SR.Argument_EmptyPath, nameof(path));

            if (cancellationToken.IsCancellationRequested)
            {
                return Task.FromCanceled(cancellationToken);
            }

            if (string.IsNullOrEmpty(contents))
            {
                new System.IO.FileStream(path, System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.Read).Dispose();
                return Task.CompletedTask;
            }

            return InternalWriteAllTextAsync(AsyncStreamWriter(path, encoding, append: false), contents, cancellationToken);
        }

        public static Task<byte[]> ReadAllBytesAsync(string path, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return Task.FromCanceled<byte[]>(cancellationToken);
            }

            var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);

            bool returningInternalTask = false;
            try
            {
                long fileLength = fs.Length;
                if (fileLength > int.MaxValue)
                {
                    return Task.FromException<byte[]>(new System.IO.IOException(SR.IO_FileTooLong2GB));
                }

                returningInternalTask = true;
                return fileLength > 0 ?
                    InternalReadAllBytesAsync(fs, (int)fileLength, cancellationToken) :
                    InternalReadAllBytesUnknownLengthAsync(fs, cancellationToken);
            }
            finally
            {
                if (!returningInternalTask)
                {
                    fs.Dispose();
                }
            }
        }

        private static async Task<byte[]> InternalReadAllBytesAsync(FileStream fs, int count, CancellationToken cancellationToken)
        {
            using (fs)
            {
                int index = 0;
                byte[] bytes = new byte[count];
                do {
                    int n = await fs.ReadAsync(bytes, index, count - index, cancellationToken).ConfigureAwait(false);
                    if (n == 0)
                    {
                        throw System.IO.Error.GetEndOfFile();
                    }

                    index += n;
                } while (index < count);
                return bytes;
            }
        }

        private static async Task<byte[]> InternalReadAllBytesUnknownLengthAsync(FileStream fs, CancellationToken cancellationToken)
        {
            byte[] rentedArray = ArrayPool<byte>.Shared.Rent(512);
            try
            {
                int bytesRead = 0;
                while (true)
                {
                    if (bytesRead == rentedArray.Length)
                    {
                        uint newLength = (uint)rentedArray.Length * 2;
                        if (newLength > MaxByteArrayLength)
                        {
                            newLength = (uint)Math.Max(MaxByteArrayLength, rentedArray.Length + 1);
                        }

                        byte[] tmp = ArrayPool<byte>.Shared.Rent((int)newLength);
                        Buffer.BlockCopy(rentedArray, 0, tmp, 0, bytesRead);
                        ArrayPool<byte>.Shared.Return(rentedArray);
                        rentedArray = tmp;
                    }

                    Debug.Assert(bytesRead < rentedArray.Length);
                    int n = await fs.ReadAsync(rentedArray, bytesRead, rentedArray.Length - bytesRead, cancellationToken).ConfigureAwait(false);
                    if (n == 0)
                    {
                        return rentedArray.AsSpan(0, bytesRead).ToArray();
                    }
                    bytesRead += n;
                }
            }
            finally
            {
                fs.Dispose();
                ArrayPool<byte>.Shared.Return(rentedArray);
            }
        }

        public static Task WriteAllBytesAsync(string path, byte[] bytes, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path), SR.ArgumentNull_Path);
            if (path.Length == 0)
                throw new ArgumentException(SR.Argument_EmptyPath, nameof(path));
            if (bytes == null)
                throw new ArgumentNullException(nameof(bytes));

            return cancellationToken.IsCancellationRequested
                ? Task.FromCanceled(cancellationToken)
                : InternalWriteAllBytesAsync(path, bytes, cancellationToken);
        }

        private static async Task InternalWriteAllBytesAsync(string path, byte[] bytes, CancellationToken cancellationToken)
        {
            Debug.Assert(!string.IsNullOrEmpty(path));
            Debug.Assert(bytes != null);

            using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                await fs.WriteAsync(bytes, 0, bytes.Length, cancellationToken).ConfigureAwait(false);
                await fs.FlushAsync(cancellationToken).ConfigureAwait(false);
            }
        }

        public static Task<string[]> ReadAllLinesAsync(string path, CancellationToken cancellationToken = default(CancellationToken))
            => ReadAllLinesAsync(path, Encoding.UTF8, cancellationToken);

        public static Task<string[]> ReadAllLinesAsync(string path, Encoding encoding, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));
            if (encoding == null)
                throw new ArgumentNullException(nameof(encoding));
            if (path.Length == 0)
                throw new ArgumentException(SR.Argument_EmptyPath, nameof(path));

            return cancellationToken.IsCancellationRequested
                ? Task.FromCanceled<string[]>(cancellationToken)
                : InternalReadAllLinesAsync(path, encoding, cancellationToken);
        }

        private static async Task<string[]> InternalReadAllLinesAsync(string path, Encoding encoding, CancellationToken cancellationToken)
        {
            Debug.Assert(!string.IsNullOrEmpty(path));
            Debug.Assert(encoding != null);

            using (var sr = AsyncStreamReader(path, encoding))
            {
                cancellationToken.ThrowIfCancellationRequested();
                string line;
                List<string> lines = new List<string>();
                while ((line = await sr.ReadLineAsync().ConfigureAwait(false)) != null)
                {
                    lines.Add(line);
                    cancellationToken.ThrowIfCancellationRequested();
                }

                return lines.ToArray();
            }
        }

        public static Task WriteAllLinesAsync(string path, IEnumerable<string> contents, CancellationToken cancellationToken = default(CancellationToken))
            => WriteAllLinesAsync(path, contents, UTF8NoBOM, cancellationToken);

        public static Task WriteAllLinesAsync(string path, IEnumerable<string> contents, Encoding encoding, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));
            if (contents == null)
                throw new ArgumentNullException(nameof(contents));
            if (encoding == null)
                throw new ArgumentNullException(nameof(encoding));
            if (path.Length == 0)
                throw new ArgumentException(SR.Argument_EmptyPath, nameof(path));

            return cancellationToken.IsCancellationRequested
                ? Task.FromCanceled(cancellationToken)
                : InternalWriteAllLinesAsync(AsyncStreamWriter(path, encoding, append: false), contents, cancellationToken);
        }

        private static async Task InternalWriteAllLinesAsync(System.IO.TextWriter writer, IEnumerable<string> contents, CancellationToken cancellationToken)
        {
            Debug.Assert(writer != null);
            Debug.Assert(contents != null);

            using (writer)
            {
                foreach (string line in contents)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    // Note that this working depends on the fix to #14563, and cannot be ported without
                    // either also porting that fix, or explicitly checking for line being null.
                    await writer.WriteLineAsync(line).ConfigureAwait(false);
                }

                cancellationToken.ThrowIfCancellationRequested();
                await writer.FlushAsync().ConfigureAwait(false);
            }
        }

        private static async Task InternalWriteAllTextAsync(System.IO.StreamWriter sw, string contents, CancellationToken cancellationToken)
        {
            char[] buffer = null;
            try
            {
                buffer = ArrayPool<char>.Shared.Rent(DefaultBufferSize);
                int count = contents.Length;
                int index = 0;
                while (index < count)
                {
                    int batchSize = Math.Min(DefaultBufferSize, count - index);
                    contents.CopyTo(index, buffer, 0, batchSize);
                    await sw.WriteAsync(buffer, 0, batchSize).ConfigureAwait(false);
                    index += batchSize;
                }

                cancellationToken.ThrowIfCancellationRequested();
                await sw.FlushAsync().ConfigureAwait(false);
            }
            finally
            {
                sw.Dispose();
                if (buffer != null)
                {
                    ArrayPool<char>.Shared.Return(buffer);
                }
            }
        }

        public static Task AppendAllTextAsync(string path, string contents, CancellationToken cancellationToken = default(CancellationToken))
            => AppendAllTextAsync(path, contents, UTF8NoBOM, cancellationToken);

        public static Task AppendAllTextAsync(string path, string contents, Encoding encoding, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (path is null)
                throw new ArgumentNullException(nameof(path));
            if (encoding == null)
                throw new ArgumentNullException(nameof(encoding));
            if (path.Length == 0)
                throw new ArgumentException(SR.Argument_EmptyPath, nameof(path));

            if (cancellationToken.IsCancellationRequested)
            {
                return Task.FromCanceled(cancellationToken);
            }

            if (string.IsNullOrEmpty(contents))
            {
                // Just to throw exception if there is a problem opening the file.
                new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.Read).Dispose();
                return Task.CompletedTask;
            }

            return InternalWriteAllTextAsync(AsyncStreamWriter(path, encoding, append: true), contents, cancellationToken);
        }

        public static Task AppendAllLinesAsync(string path, IEnumerable<string> contents, CancellationToken cancellationToken = default(CancellationToken))
            => AppendAllLinesAsync(path, contents, UTF8NoBOM, cancellationToken);

        public static Task AppendAllLinesAsync(string path, IEnumerable<string> contents, Encoding encoding, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));
            if (contents == null)
                throw new ArgumentNullException(nameof(contents));
            if (encoding == null)
                throw new ArgumentNullException(nameof(encoding));
            if (path.Length == 0)
                throw new ArgumentException(SR.Argument_EmptyPath, nameof(path));

            return cancellationToken.IsCancellationRequested
                ? Task.FromCanceled(cancellationToken)
                : InternalWriteAllLinesAsync(AsyncStreamWriter(path, encoding, append: true), contents, cancellationToken);
        }

        /// <summary>
        /// Gets a value whether the given file path is a file that can be encrypted.
        /// </summary>
        /// <param name="filepath">The file to test.</param>
        /// <returns><see langword="true"/> whether the file can be encrypted; otherwise <see langword="false"/>. <br />
        /// <see langword="false"/> is also returned for already encrypted files.</returns>
        public static System.Boolean IsEncryptableFile(System.String filepath)
        {
            Interop.Advapi32.FileEncryptionConstants fec;
            if (Interop.Advapi32.FileEncryptionStatus(filepath, out fec))
            {
                return fec == Interop.Advapi32.FileEncryptionConstants.FILE_ENCRYPTABLE;
            }
            return false;
        }

        /// <summary>
        /// Gets a value whether the given file path is an encrypted file.
        /// </summary>
        /// <param name="filepath">The file to test.</param>
        /// <returns><see langword="true"/> if the file is encrypted; otherwise <see langword="false"/>.</returns>
        public static System.Boolean IsEncrypted(System.String filepath)
        {
            Interop.Advapi32.FileEncryptionConstants fec;
            if (Interop.Advapi32.FileEncryptionStatus(filepath, out fec))
            {
                return fec == Interop.Advapi32.FileEncryptionConstants.FILE_IS_ENCRYPTED;
            }
            return false;
        }
    }
}
