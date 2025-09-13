


using System;

namespace MP.IO
{
    /// <summary>
    /// Defines flags (options) to use when copying or moving directories.
    /// </summary>
    [Flags]
    public enum CopyMoveDirectoryOptions : System.Byte
    {
        /// <summary>
        /// No additional behavioral flags are defined. This is like calling <see cref="FileSystem.CopyDirectory(string, string)"/>.
        /// </summary>
        None,
        /// <summary>
        /// By default , <see cref="FileSystem.CopyDirectory(string, string)"/> also copies the attributes of the source directory and it's files, making both directories 1-1 equal. <br />
        /// When this flag is specified, a default set of attributes defined by the OS is defined instead on the destination directory and it's copied files.
        /// </summary>
        DoNotPreserveAttributes = 1 << 0,
        /// <summary>
        /// By default , <see cref="FileSystem.CopyDirectory(string, string)"/> only copies the files existing in the directory object itself. <br/>
        /// When this flag is specified, all the files and the child directories and their files are copied too.
        /// </summary>
        RecursiveCopy = 1 << 1,
        /// <summary>
        /// By default , <see cref="FileSystem.CopyDirectory(string, string)"/> uses the native OS routine to do the copy of the directories. <br />
        /// If applicable and this flag is defined, then the copy is done through the OS support for reading and writing data, as well as the search mechanism.
        /// </summary>
        UseDotNetImplementationIfApplicable = 1 << 2,
    }
}