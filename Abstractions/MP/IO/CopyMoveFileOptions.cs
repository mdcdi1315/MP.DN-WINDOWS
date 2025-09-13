

using System;

namespace MP.IO
{
    /// <summary>
    /// Defines flags (options) to use when copying or moving files.
    /// </summary>
    [Flags]
    public enum CopyMoveFileOptions : System.Byte
    {
        /// <summary>
        /// No additional behavioral flags are defined. This is like calling <see cref="FileSystem.CopyFile(string, string)"/>.
        /// </summary>
        None = 0,
        /// <summary>
        /// By default , <see cref="FileSystem.CopyFile(string, string)"/> fails if the destination path is an existing file. <br />
        /// This flag removes that need and even if the file exists in the destination path, it is overwritten.
        /// </summary>
        OverwriteFileAtDestWithoutFailing = 1 << 0,
        /// <summary>
        /// By default , <see cref="FileSystem.CopyFile(string, string)"/> also copies the attributes of the source file, making both files 1-1 equal. <br />
        /// When this flag is specified, a default set of attributes defined by the OS is defined instead on the destination file.
        /// </summary>
        DoNotPreserveAttributes = 1 << 1,
        /// <summary>
        /// By default , <see cref="FileSystem.CopyFile(string, string)"/> uses the native OS routine to do the file copy. <br />
        /// If applicable and this flag is defined, then the copy is done through the OS support for reading and writing data.
        /// </summary>
        UseDotNetImplementationIfApplicable = 1 << 2,
    }
}