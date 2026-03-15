// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;

namespace MP.IO
{
    /// <summary>
    /// Contains constants for controlling file sharing options while opening files.  <br />
    /// You can specify what access other processes trying to open the same file concurrently can have. <br /> <br />
    /// 
    /// Note these values currently match the values for FILE_SHARE_READ, FILE_SHARE_WRITE, and FILE_SHARE_DELETE in winnt.h
    /// </summary>
    [Flags]
    public enum FileShare
    {
        /// <summary>
        /// No sharing. 
        /// Any request to open the file (by this process or another process) will fail until the file is closed.
        /// </summary>
        None = 0,

        /// <summary>
        /// Allows subsequent opening of the file for reading. <br />
        /// If this flag is not specified, any request to open the file for reading (by this process or another process) will fail until the file is closed.
        /// </summary>
        Read = 1,

        /// <summary>
        /// Allows subsequent opening of the file for writing. <br />
        /// If this flag is not specified, any request to open the file for writing (by this process or another process) will fail until the file is closed.
        /// </summary>
        Write = 2,

        /// <summary>
        /// Allows subsequent opening of the file for writing or reading. <br />
        /// If this flag is not specified, any request to open the file for writing or reading (by this process or another process) will fail until the file is closed.
        /// </summary>
        ReadWrite = 3,

        /// <summary>Open the file, but allow someone else to delete the file.</summary>
        Delete = 4,

        /// <summary>
        /// Whether the file handle should be inheritable by child processes. <br />
        /// Note this is not directly supported like this by Win32.
        /// </summary>
        Inheritable = 0x10,
    }
}
