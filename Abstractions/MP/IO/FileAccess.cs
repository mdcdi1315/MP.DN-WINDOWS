// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;

namespace MP.IO
{
    /// <summary>
    /// Contains constants for specifying the access you want for a file. <br />
    /// You can have Read, Write or ReadWrite access.
    /// </summary>
    [Flags]
    public enum FileAccess : System.Byte
    {
        /// <summary>
        /// Specifies read access to the file. <br />
        /// Data can be read from the file and the file pointer can be moved.  <br />
        /// Combine with <see cref="Write"/> for read-write access.
        /// </summary>
        Read = 1 << 0,

        /// <summary>
        /// Specifies write access to the file. 
        /// Data can be written to the file and the file pointer can be moved. 
        /// Combine with <see cref="Read"/> for read-write access.
        /// </summary>
        Write = 1 << 1,

        /// <summary>
        /// Specifies read and write access to the file. <br />
        /// Data can be written to the file and the file pointer can be moved.  <br />
        /// Data can also be read from the file. <br />
        /// This value is in fact the combination of both <see cref="Read"/> and <see cref="Write"/> flags.
        /// </summary>
        ReadWrite = Read | Write,
    }
}
