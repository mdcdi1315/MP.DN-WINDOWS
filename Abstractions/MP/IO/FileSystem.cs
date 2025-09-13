
using System;
using MP.Annotations.CodeAnalysis;

namespace MP.IO
{
    /// <summary>
    /// Defines basic methods and is the abstraction layer for the IO namespace.
    /// </summary>
    public static class FileSystem
    {
        /// <summary>
        /// Gets the OS attributes for a file or directory in the file system.
        /// </summary>
        /// <param name="path">The path of the file or directory to get it's attributes.</param>
        /// <returns>The file attributes for the given file system object.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> was <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="path"/> is not a valid file system object at the call time (for example it did not existed when the call was made).</exception>
        [Throws(typeof(ArgumentException) , typeof(ArgumentNullException))]
        public static FileAttributes GetAttributes(System.String path) => 0;

        /// <summary>
        /// Sets a new set of OS attributes for a file or directory in the file system.
        /// </summary>
        /// <param name="path">The path of the file or directory to set the new attributes to.</param>
        /// <param name="attributes">The new file attributes to set.</param>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> was <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="path"/> is not a valid file system object at the call time (for example it did not existed when the call was made).</exception>
        [Throws(typeof(ArgumentException), typeof(ArgumentNullException))]
        public static void SetAttributes(System.String path , FileAttributes attributes) { }

        /// <summary>
        /// Deletes a file in the file system, and returning a value whether the operation succeeded or not. <br />
        /// If the file does not exist, <see langword="false"/> is returned.
        /// </summary>
        /// <param name="path">The path of the file to delete.</param>
        /// <returns>A value whether the specified file was deleted from the file system.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> was <see langword="null"/>.</exception>
        [Throws(typeof(ArgumentNullException))]
        public static System.Boolean RemoveFile(System.String path) => false;

        /// <summary>
        /// Deletes an empty directory in the file system, and returning a value whether the operation succeeded or not. <br />
        /// If the directory does not exist, <see langword="false"/> is returned. <br />
        /// <see langword="false"/> is also returned when the directory exists but is not empty.
        /// </summary>
        /// <param name="path">The path of the directory to delete.</param>
        /// <returns>A value whether the specified directory was deleted from the file system.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> was <see langword="null"/>.</exception>
        [Throws(typeof(ArgumentNullException))]
        public static System.Boolean RemoveDirectory(System.String path) => false;

        /// <summary>
        /// Deletes a directory and all the child objects it has in the file system, and returning a value whether the operation succeeded or not. <br />
        /// If the directory does not exist, <see langword="false"/> is returned. <br />
        /// <see langword="false"/> is also returned when the directory exists but is not empty.
        /// </summary>
        /// <param name="path">The path of the directory to delete.</param>
        /// <returns>A value whether the specified directory was deleted from the file system.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> was <see langword="null"/>.</exception>
        [Throws(typeof(ArgumentNullException))]
        public static System.Boolean RemoveDirectoryRecursive(System.String path) => false;

        /// <summary>
        /// Finds out whether the specified path is an existing file. <br />
        /// <see langword="false"/> is returned in any other case.
        /// </summary>
        /// <param name="path">The path of where it is believed that the file is located to.</param>
        /// <returns>A value whether the specified path is a valid file path and the file that represents it does exist.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> was <see langword="null"/>.</exception>
        [Throws(typeof(ArgumentNullException))]
        public static System.Boolean FileExists(System.String path) => false;

        /// <summary>
        /// Finds out whether the specified path is an existing directory. <br />
        /// <see langword="false"/> is returned in any other case.
        /// </summary>
        /// <param name="path">The path of where it is believed that the directory is located to.</param>
        /// <returns>A value whether the specified path is a valid directory path and the directory that represents it does exist.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> was <see langword="null"/>.</exception>
        [Throws(typeof(ArgumentNullException))]
        public static System.Boolean DirectoryExists(System.String path) => false;

        /// <summary>
        /// Copies a file to another path, failing if the file exists in <paramref name="destpath"/>.
        /// </summary>
        /// <param name="sourcepath">The source path of the file to copy.</param>
        /// <param name="destpath">The destination path of the copied file.</param>
        /// <exception cref="ArgumentNullException"><paramref name="sourcepath"/> and/or <paramref name="destpath"/> were <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="sourcepath"/> is not a valid file system object at the call time (for example it did not existed when the call was made).</exception>
        [Throws(typeof(ArgumentException), typeof(ArgumentNullException))]
        public static void CopyFile(System.String sourcepath, System.String destpath) => CopyFile(sourcepath, destpath, CopyMoveFileOptions.None);

        /// <summary>
        /// Copies a file to another path, with additional options specified by the <paramref name="options"/> parameter.
        /// </summary>
        /// <param name="sourcepath">The source path of the file to copy.</param>
        /// <param name="destpath">The destination path of the copied file.</param>
        /// <param name="options">Additional options that control how the file copy should be done.</param>
        /// <exception cref="ArgumentNullException"><paramref name="sourcepath"/> and/or <paramref name="destpath"/> were <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="sourcepath"/> is not a valid file system object at the call time (for example it did not existed when the call was made).</exception>
        [Throws(typeof(ArgumentException), typeof(ArgumentNullException))]
        public static void CopyFile(System.String sourcepath , System.String destpath, CopyMoveFileOptions options) { }

        /// <summary>
        /// Moves a file to another path, failing if the file exists in <paramref name="destpath"/>.
        /// </summary>
        /// <param name="sourcepath">The source path of the file to move.</param>
        /// <param name="destpath">The destination path where the file will be saved to.</param>
        /// <exception cref="ArgumentNullException"><paramref name="sourcepath"/> and/or <paramref name="destpath"/> were <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="sourcepath"/> is not a valid file system object at the call time (for example it did not existed when the call was made).</exception>
        [Throws(typeof(ArgumentException), typeof(ArgumentNullException))]
        public static void MoveFile(System.String sourcepath, System.String destpath) => MoveFile(sourcepath, destpath, CopyMoveFileOptions.None);

        /// <summary>
        /// Moves a file to another path, failing if the file exists in <paramref name="destpath"/>.
        /// </summary>
        /// <param name="sourcepath">The source path of the file to move.</param>
        /// <param name="destpath">The destination path where the file will be saved to.</param>
        /// <param name="options">Additional options that control how the file move should be done.</param>
        /// <exception cref="ArgumentNullException"><paramref name="sourcepath"/> and/or <paramref name="destpath"/> were <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="sourcepath"/> is not a valid file system object at the call time (for example it did not existed when the call was made).</exception>
        [Throws(typeof(ArgumentException), typeof(ArgumentNullException))]
        public static void MoveFile(System.String sourcepath , System.String destpath, CopyMoveFileOptions options) { }

        /// <summary>
        /// Copies a directory and it's contents to another path, failing if the directory exists in <paramref name="destpath"/>.
        /// </summary>
        /// <param name="sourcepath">The source path of the directory to copy. The directory must exist.</param>
        /// <param name="destpath">The destination path where the copied directory will be saved to.</param>
        /// <exception cref="ArgumentNullException"><paramref name="sourcepath"/> and/or <paramref name="destpath"/> were <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="sourcepath"/> is not a valid file system object at the call time (for example it did not existed when the call was made).</exception>
        [Throws(typeof(ArgumentException), typeof(ArgumentNullException))]
        public static void CopyDirectory(System.String sourcepath, System.String destpath) => CopyDirectory(sourcepath, destpath, CopyMoveDirectoryOptions.None);

        /// <summary>
        /// Copies a directory to another path, with additional options specified by the <paramref name="options"/> parameter.
        /// </summary>
        /// <param name="sourcepath">The source path of the directory to copy. The directory must exist.</param>
        /// <param name="destpath">The destination path where the copied directory will be saved to.</param>
        /// <param name="options">Additional options that control how the file copy should be done.</param>
        /// <exception cref="ArgumentNullException"><paramref name="sourcepath"/> and/or <paramref name="destpath"/> were <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="sourcepath"/> is not a valid file system object at the call time (for example it did not existed when the call was made).</exception>
        [Throws(typeof(ArgumentException), typeof(ArgumentNullException))]
        public static void CopyDirectory(System.String sourcepath, System.String destpath , CopyMoveDirectoryOptions options) { }

        /// <summary>
        /// Moves a directory and it's contents to another path, failing if the directory exists in <paramref name="destpath"/>.
        /// </summary>
        /// <param name="sourcepath">The source path of the directory to copy. The directory must exist.</param>
        /// <param name="destpath">The destination path where the moved directory will be saved to.</param>
        /// <exception cref="ArgumentNullException"><paramref name="sourcepath"/> and/or <paramref name="destpath"/> were <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="sourcepath"/> is not a valid file system object at the call time (for example it did not existed when the call was made).</exception>
        [Throws(typeof(ArgumentException), typeof(ArgumentNullException))]
        public static void MoveDirectory(System.String sourcepath, System.String destpath) => MoveDirectory(sourcepath, destpath, CopyMoveDirectoryOptions.None);

        /// <summary>
        /// Copies a directory to another path, with additional options specified by the <paramref name="options"/> parameter.
        /// </summary>
        /// <param name="sourcepath">The source path of the directory to copy. The directory must exist.</param>
        /// <param name="destpath">The destination path where the moved directory will be saved to.</param>
        /// <param name="options">Additional options that control how the file copy should be done.</param>
        /// <exception cref="ArgumentNullException"><paramref name="sourcepath"/> and/or <paramref name="destpath"/> were <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="sourcepath"/> is not a valid file system object at the call time (for example it did not existed when the call was made).</exception>
        [Throws(typeof(ArgumentException), typeof(ArgumentNullException))]
        public static void MoveDirectory(System.String sourcepath, System.String destpath, CopyMoveDirectoryOptions options) { }

        /// <summary>
        /// Gets the creation time of any file system object.
        /// </summary>
        /// <param name="path">The path of the file system object whose creation time is to be retrieved.</param>
        /// <returns>The creation date and time of the file system object.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> was <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="path"/> is not a valid file system object at the call time (for example it did not existed when the call was made).</exception>
        [Throws(typeof(ArgumentException), typeof(ArgumentNullException))]
        public static DateTime GetFileSystemObjectCreationTime(System.String path) => GetFileSystemObjectTimes(path).CreationTime;

        /// <summary>
        /// Gets the modification time of any file system object.
        /// </summary>
        /// <param name="path">The path of the file system object whose modification time is to be retrieved.</param>
        /// <returns>The modification date and time of the file system object.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> was <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="path"/> is not a valid file system object at the call time (for example it did not existed when the call was made).</exception>
        [Throws(typeof(ArgumentException), typeof(ArgumentNullException))]
        public static DateTime GetFileSystemObjectModificationTime(System.String path) => GetFileSystemObjectTimes(path).ModificationTime;

        /// <summary>
        /// Gets the last access time of any file system object.
        /// </summary>
        /// <param name="path">The path of the file system object whose last access time is to be retrieved.</param>
        /// <returns>The last access date and time of the file system object.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> was <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="path"/> is not a valid file system object at the call time (for example it did not existed when the call was made).</exception>
        [Throws(typeof(ArgumentException), typeof(ArgumentNullException))]
        public static DateTime GetFileSystemObjectLastAccessTime(System.String path) => GetFileSystemObjectTimes(path).LastAccessTime;

        /// <summary>
        /// Gets the times of any file system object.
        /// </summary>
        /// <param name="path">The path of the file system object whose times are to be retrieved.</param>
        /// <returns>The times of the file system object.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> was <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="path"/> is not a valid file system object at the call time (for example it did not existed when the call was made).</exception>
        [Throws(typeof(ArgumentException), typeof(ArgumentNullException))]
        public static FileSystemObjectTimes GetFileSystemObjectTimes(System.String path) => new();

        /// <summary>
        /// Sets all the times of any file system object.
        /// </summary>
        /// <param name="path">The path of the file system object whose times are to be retrieved.</param>
        /// <param name="times">The new time values to be set on the specified file system object.</param>
        /// <returns>The older times set on the file system object, before those where changed by this call.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> was <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="path"/> is not a valid file system object at the call time (for example it did not existed when the call was made).</exception>
        [Throws(typeof(ArgumentException), typeof(ArgumentNullException))]
        public static FileSystemObjectTimes SetFileSystemObjectTimes(System.String path, FileSystemObjectTimes times) => new();

        /// <summary>
        /// Sets the creation time of any file system object.
        /// </summary>
        /// <param name="path">The path of the file system object whose creation time is to be set.</param>
        /// <param name="creationtime">The exact creation time that is to be set to the file system object at <paramref name="path"/>.</param>
        /// <returns>The creation date and time of the current file system object, before the date and time were changed by the current call.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> was <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="path"/> is not a valid file system object at the call time (for example it did not existed when the call was made).</exception>
        [Throws(typeof(ArgumentException), typeof(ArgumentNullException))]
        public static DateTime SetFileSystemObjectCreationTime(System.String path , DateTime creationtime) 
        {
            FileSystemObjectTimes times = GetFileSystemObjectTimes(path);
            SetFileSystemObjectTimes(path , times.WithNewCreationTime(creationtime));
            return times.CreationTime;
        }

        /// <summary>
        /// Sets the modification time of any file system object.
        /// </summary>
        /// <param name="path">The path of the file system object whose creation time is to be set.</param>
        /// <param name="modtime">The exact modification time that is to be set to the file system object at <paramref name="path"/>.</param>
        /// <returns>The modification date and time of the current file system object, before the date and time were changed by the current call.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> was <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="path"/> is not a valid file system object at the call time (for example it did not existed when the call was made).</exception>
        [Throws(typeof(ArgumentException), typeof(ArgumentNullException))]
        public static DateTime SetFileSystemObjectModificationTime(System.String path , DateTime modtime)
        {
            FileSystemObjectTimes times = GetFileSystemObjectTimes(path);
            SetFileSystemObjectTimes(path, times.WithNewModificationTime(modtime));
            return times.ModificationTime;
        }

        /// <summary>
        /// Sets the last access time of any file system object.
        /// </summary>
        /// <param name="path">The path of the file system object whose creation time is to be set.</param>
        /// <param name="lastaccesstime">The exact last access time that is to be set to the file system object at <paramref name="path"/>.</param>
        /// <returns>The last access date and time of the current file system object, before the date and time were changed by the current call.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> was <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="path"/> is not a valid file system object at the call time (for example it did not existed when the call was made).</exception>
        [Throws(typeof(ArgumentException), typeof(ArgumentNullException))]
        public static DateTime SetFileSystemObjectLastAccessTime(System.String path , DateTime lastaccesstime)
        {
            FileSystemObjectTimes times = GetFileSystemObjectTimes(path);
            SetFileSystemObjectTimes(path , times.WithNewLastAccessTime(lastaccesstime));
            return times.LastAccessTime;
        }


    }
}