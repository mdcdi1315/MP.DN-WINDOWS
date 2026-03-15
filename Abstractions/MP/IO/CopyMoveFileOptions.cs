
using System;

namespace MP.IO
{
    /// <summary>
    /// Defines flags (options) to use when copying or moving files.
    /// </summary>
    [Flags]
    public enum CopyMoveFileOptions : System.Byte
    {
        /// <summary>No additional behavioral flags are defined.</summary>
        None = 0,
        /// <summary>
        /// By default , <see cref="File.CopyTo(File, CopyMoveFileOptions)"/> fails if the destination path is an existing file. <br />
        /// This flag removes that need and even if the file exists in the destination path, it is overwritten.
        /// </summary>
        OverwriteFileAtDestWithoutFailing = 1 << 0,
        /// <summary>
        /// By default, <see cref="File.CopyTo(File, CopyMoveFileOptions)"/> also copies the attributes of the source file, making both files 1-1 equal. <br />
        /// When this flag is specified, a default set of attributes defined by the OS is defined instead on the destination file. <br />
        /// Note that this option may not be available when not using the <see cref="UseDotNetImplementationIfApplicable"/> flag.
        /// </summary>
        DoNotPreserveAttributes = 1 << 1,
        /// <summary>
        /// By default , <see cref="File.CopyTo(File, CopyMoveFileOptions)"/> uses the native OS routine to do the file copy. <br />
        /// If applicable and this flag is defined, then the copy is done through the OS support for reading and writing data.
        /// </summary>
        UseDotNetImplementationIfApplicable = 1 << 2,
        /// <summary>
        /// By default, <see cref="File.CopyTo(File, CopyMoveFileOptions)"/> copies the file by using buffers. <br />
        /// This option allows the OS to use unbuffered I/O, which is helpful for very large files (> 1GB in size). <br />
        /// This flag is not supported when using the <see cref="UseDotNetImplementationIfApplicable"/> flag.
        /// </summary>
        NoBuffering = 1 << 3,
    }
}