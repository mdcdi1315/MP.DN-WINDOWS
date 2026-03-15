
using MP.Annotations;
using System.Runtime.Versioning;

namespace MP.NativeInterop.Windows.COM
{
    /// <summary>
    /// Provides a data stream that can be only read data from or written data to.
    /// </summary>
    [SupportedOSPlatform(WindowsVersions._WIN32_WINNT_WIN2K)]
    [COMInterfaceGenerator("0c733a30-2a1c-11ce-ade5-00aa0044773d")]
    public unsafe partial interface ISequentialStream
    {
        /// <summary>
        /// Reads the specified number of bytes and places them to <paramref name="buffer"/>.
        /// </summary>
        /// <param name="buffer">The buffer to place data into</param>
        /// <param name="n_bytes">The number of bytes to read</param>
        /// <param name="p_read">The number of bytes that were actually read.</param>
        /// <returns>A value whether the read operation is successfull or not.</returns>
        public HRESULT Read(void* buffer, uint n_bytes, uint* p_read);

        /// <summary>
        /// Writes the specified number of bytes to the stream from <paramref name="buffer"/>.
        /// </summary>
        /// <param name="buffer">The buffer to write data to the stream</param>
        /// <param name="n_bytes">The number of bytes to write</param>
        /// <param name="p_written">The number of bytes that were actually written.</param>
        /// <returns>A value whether the write operation is successfull or not.</returns>
        public HRESULT Write(void* buffer, uint n_bytes, uint* p_written);
    }
}