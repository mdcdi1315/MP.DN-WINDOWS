using System;

namespace MP.IO
{
    /// <summary>
    /// Specifies the mode under which the data stream is created as.
    /// </summary>
    [Flags]
    public enum DataStreamMode : System.Byte
    {
        /// <summary>Special constant to specify in the <see cref="DataStream"/> class abstraction.</summary>
        Invalid = 0,
        /// <summary>Data stream supports reading.</summary>
        Read = 1 << 0,
        /// <summary>Data stream supports writing.</summary>
        Write = 1 << 1,
        /// <summary>Data stream supports both reading and writing. Check with this flag if you want to enforce having a data stream that is both readable and writable.</summary>
        ReadWrite = Read | Write,
        /// <summary>Data stream supports seeking.</summary>
        Seek = 1 << 2,
    }
}