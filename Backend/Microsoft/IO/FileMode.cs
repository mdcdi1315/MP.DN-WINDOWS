// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Microsoft.IO
{
    /// <summary>
    /// Contains constants for specifying how the OS should open a file. <br />
    /// These will control whether you overwrite a file, open an existing
    /// file, or some combination thereof. <br /> <br />
    /// 
    /// To append to a file, use <see cref="Append"/> (which maps to <see cref="OpenOrCreate"/> then we seek to the end of the file). <br />
    /// To truncate a file or create it if it doesn't exist, use <see cref="Create"/>.
    /// </summary>
    public enum FileMode
    {
        /// <summary>
        /// Creates a new file. <br />
        /// An exception is raised if the file already exists.
        /// </summary>
        CreateNew = 1,

        /// <summary>
        /// Creates a new file. <br />
        /// If the file already exists, it is overwritten.
        /// </summary>
        Create = 2,

        /// <summary>
        /// Opens an existing file. <br />
        /// An exception is raised if the file does not exist.
        /// </summary>
        Open = 3,

        /// <summary>
        /// Opens the file if it exists. <br />
        /// Otherwise, creates a new file.
        /// </summary>
        OpenOrCreate = 4,

        /// <summary>
        /// Opens an existing file. <br />
        /// Once opened, the file is truncated so that its size is zero bytes. <br />
        /// The calling process must open the file with at least WRITE access. <br />
        /// An exception is raised if the file does not exist.
        /// </summary>
        Truncate = 5,

        /// <summary>
        /// Opens the file if it exists and seeks to the end. <br />
        /// Otherwise, creates a new file.
        /// </summary>
        Append = 6,
    }
}
