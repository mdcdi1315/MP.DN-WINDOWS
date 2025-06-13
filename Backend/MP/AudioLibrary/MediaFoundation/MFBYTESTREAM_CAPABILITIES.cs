
using System;
using MP.WindowsInterop;
using System.Runtime.Versioning;

namespace MP.AudioLibrary.MediaFoundation
{
    [Flags]
    public enum MFBYTESTREAM_CAPABILITIES : System.UInt32
    {
        /// <summary>
        ///     This bit indicates that the byte stream can be read from.
        /// </summary>
        IS_READABLE = 0x00000001,
        /// <summary>
        ///     This bit indicates that the byte stream can be written to.
        /// </summary>
        IS_WRITABLE = 0x00000002,
        /// <summary>
        ///     This bit indicates that the byte stream can be sought.
        /// </summary>
        IS_SEEKABLE = 0x00000004,
        /// <summary>
        ///     This bit indicates that the byte stream is based on a remote (network)
        ///     drive.
        /// </summary>
        IS_REMOTE = 0x00000008,
        /// <summary>
        ///     This bit indicates that the byte stream is a directory.
        /// </summary>
        IS_DIRECTORY = 0x00000080,
        /// <summary>
        ///     This bit indicates that a read operation following a seek operation
        ///     may take a long time to complete depending on whether the byte stream
        ///     has to wait for the data to become available from a remote server or not.
        ///     This capability is exposed by byte streams that pre-cache the data
        ///     sequentially from a remote server. This bit goes away when the data is
        ///     fully cached.
        /// </summary>
        HAS_SLOW_SEEK = 0x00000100,
        /// <summary>
        ///     This bit indicates that the byte stream is downloading the data
        ///     in the background to a local cache. In this case a read operation may
        ///     take longer to complete depending on whether the byte stream has the
        ///     data in cache or not. If MFBYTESTREAM_HAS_SLOW_SEEK is not present,
        ///     then the byte stream can pre-cache the data sparsely instead of
        ///     sequentially, and a read operation that misses the local cache
        ///     will cause a reconnection to the remote server instead of waiting
        ///     for the sequential download to catch up. This bit goes away when the
        ///     data is fully cached.
        /// </summary>
        IS_PARTIALLY_DOWNLOADED = 0x00000200,
        /// <summary>
        ///     This bit indicates that the byte stream data is opened for write by
        ///     another thread or process.  This means that the length of the
        ///     bytestream could change and special care must be taken to handle
        ///     situations where only part of a file may be written.  Only byte
        ///     stream plugins with the attribute MF_BYTESTREAMPLUGIN_ACCEPTS_SHARE_WRITE
        ///     will be considered by the source resolver for byte streams that have this
        ///     characteristic.
        /// </summary>
        [SupportedOSPlatform(WindowsVersions.NTDDI_WIN7)] // Windows 7
        SHARE_WRITE = 0x00000400,
        /// <summary>
        ///     This bit should be set if the byte stream is not currently
        ///     using the network to receive the content.  Networking hardware
        ///     may enter a power saving state when this bit is set.
        /// </summary>
        [SupportedOSPlatform(WindowsVersions.NTDDI_WINBLUE)] // Windows 8.1
        DOES_NOT_USE_NETWORK = 0x00000800,
    }
}