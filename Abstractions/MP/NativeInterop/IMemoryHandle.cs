
using System;
using System.Collections.Generic;

namespace MP.NativeInterop
{
    /// <summary>
    /// Defines a lightweight abstraction for native memory handles. <br />
    /// It tries to become as most as possible type-safe although that native handles never are in fact.
    /// </summary>
    public unsafe interface IMemoryHandle : IEnumerable<System.Byte> , IDisposable
    {
        /// <summary>
        /// Gets the memory handle's length in bytes.
        /// </summary>
        public System.UInt64 MemoryLength { get; }

        /// <summary>
        /// Gets a pointer to the allocated memory block.
        /// </summary>
        public System.Byte* MemoryPointer { get; }

        /// <summary>
        /// Gets or sets a byte inside the memory, specifying which byte to manipulate in the <paramref name="index"/> parameter.
        /// </summary>
        /// <param name="index">The byte to be either set or got.</param>
        /// <returns>The retrieved byte at <paramref name="index"/>.</returns>
        public System.Byte this[System.UInt64 index] { get; set; }

        /// <summary>
        /// Gets a value whether this memory block is no longer accessible by clients.
        /// </summary>
        public System.Boolean IsClosed { get; }
    }
}
