

using MP.ComInterop;
using MP.WindowsInterop;
using System.Runtime.InteropServices;

namespace MP.AudioLibrary.MediaFoundation
{
    [ComImport]
    [Guid(MediaFoundationInterfaceIds.IID_IMFByteStream)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe interface IMFByteStream
    {
        [PreserveSig]
        public HRESULT GetCapabilities(MFBYTESTREAM_CAPABILITIES* pdwCapabilities);

        /// <summary>
        ///     GetLength gets the length of the stream
        /// </summary>
        /// <param name="pqwLength">
        ///     Pointer to a variable that will receive the length of the stream.
        /// </param>
        /// <returns>
        ///     <para>
        ///         MF_E_BYTESTREAM_UNKNOWN_LENGTH:
        ///             The stream length is unknown (can happen in HTTP scenarios)
        ///     </para>
        /// </returns>
        [PreserveSig]
        public HRESULT GetLength(System.UInt64* pqwLength);

        [PreserveSig]
        public HRESULT SetLength(System.UInt64 qwLength);
        
        [PreserveSig]
        public HRESULT GetCurrentPosition(System.UInt64* pqwPosition);

        [PreserveSig]
        public HRESULT SetCurrentPosition(System.UInt64 qwPosition);

        [PreserveSig]
        public HRESULT IsEndOfStream(BOOL* pfEndOfStream);

        /// <summary>
        ///     Read number of bytes from bytestream.
        /// </summary>
        [PreserveSig]
        public HRESULT Read(System.Byte* pb, System.UInt32 cb, System.UInt32* pcbRead);

        /// <summary>
        ///     Begin async read number of bytes from bytestream.
        /// </summary>
        [PreserveSig]
        public HRESULT BeginRead(
            System.Byte* pb,
            System.UInt32 cb,
            /* IMFAsyncCallback* */ void* pCallback,
            /* IUnknown* */ void* punkState);

        /// <summary>
        ///     Complete async read operation.
        /// </summary>
        [PreserveSig]
        public HRESULT EndRead(
            /* IMFAsyncResult* */ void* pResult,
            System.UInt32* pcbRead);

        /// <summary>
        ///     Write number of bytes to bytestream.
        /// </summary>
        [PreserveSig]
        public HRESULT Write(System.Byte* pb, System.UInt32 cb, System.UInt32* pcbWritten);

        /// <summary>
        ///     Begin async write operation.
        /// </summary>
        [PreserveSig]
        public HRESULT BeginWrite(
            System.Byte* pb,
            System.UInt32 cb,
            /* IMFAsyncCallback* */ void* pCallback,
            /* IUnknown* */ void* punkState);

        /// <summary>
        ///     Complete async write operation.
        /// </summary>
        [PreserveSig]
        public HRESULT EndWrite(
            /* IMFAsyncResult* */ void* pResult,
            System.UInt32* pcbWritten);

        /// <summary>
        ///     Seek to location in bytestream.
        /// </summary>
        [PreserveSig]
        public HRESULT Seek(
            MFBYTESTREAM_SEEK_ORIGIN SeekOrigin,
            System.Int64 llSeekOffset,
            MFBYTESTREAM_SEEK_FLAGS dwSeekFlags,
            System.UInt64* pqwCurrentPosition);

        [PreserveSig]
        public HRESULT Flush();

        [PreserveSig]
        public HRESULT Close();
    }
}