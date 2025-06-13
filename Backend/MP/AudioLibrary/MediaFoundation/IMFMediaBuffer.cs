
using MP.ComInterop;
using System.Runtime.InteropServices;

namespace MP.AudioLibrary.MediaFoundation
{
    /// <summary>
    ///     The IMFMediaBuffer interface represent a buffer of multimedia data
    ///     for any possible multimedia type.
    ///     It provides methods for accessing the buffer pointer, the current
    ///     length, and the maximum length of the buffer
    /// </summary>
    [ComImport]
    [Guid(MediaFoundationInterfaceIds.IID_IMFMediaBuffer)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe interface IMFMediaBuffer
    {
        /// <summary>
        ///     The Lock method gives the caller access to the underlying
        ///     buffer pointer and current length of the media buffer.
        /// </summary>
        /// <param name="ppbBuffer">
        ///     Specifies a pointer to a variable where the buffer pointer
        ///     will be stored.
        /// </param>
        /// <param name="pcbMaxLength">
        ///     Pointer to a count of bytes where the maximum length of the
        ///     buffer will be stored.  
        ///     This is the maximum amount of data that can be written to the
        ///     buffer.
        /// </param>
        /// <param name="pcbCurrentLength">
        ///     Pointer to a count of bytes where the current length of the
        ///     buffer will be stored.  
        ///     This is amount of valid data currently in the buffer.
        ///     May be NULL
        /// </param>
        /// <remarks>
        ///     The buffer pointer is guaranteed to be valid for access up to
        ///     the maximum length of the media buffer for the duration of the lock.
        ///      When the caller is finished, the Unlock method should be called.
        ///     Note that Unlock must be called the same number of times that
        ///      Lock has been called in order to signal completion of the use of
        ///     the buffer pointer.
        ///     It is recommended that the caller lock the media buffer only
        ///      for the time necessary to manipulate the buffer contents.
        /// </remarks>
        [PreserveSig]
        public HRESULT Lock(System.Byte** ppbBuffer, System.UInt32* pcbMaxLength, System.UInt32* pcbCurrentLength);

        /// <summary>
        ///     The Unlock method signals completion of the use of the
        ///     buffer pointer acquired via the Lock method.
        /// </summary>
        /// <remarks>
        ///     The buffer pointer acquired via Lock can no longer be used
        ///     once the caller signals completion of that
        ///     usage via the Unlock call.
        ///     Also, note that Unlock must be called the same number of
        ///     times that Lock has been called in order to signal completion
        ///     of the use of the buffer pointer.
        /// </remarks>
        [PreserveSig]
        public HRESULT Unlock();

        /// <summary>
        ///     The GetCurrentLength method returns the current length of the
        ///     media buffer.
        /// </summary>
        /// <param name="pcbCurrentLength">
        ///     Pointer to a count of bytes where the current length of the
        ///     buffer will be stored.
        /// </param>
        /// <remarks>
        ///     A returned value of zero bytes indicates a media buffer with
        ///     no valid data (an empty buffer).
        /// </remarks>
        [PreserveSig]
        public HRESULT GetCurrentLength(System.UInt32* pcbCurrentLength);

        /// <summary>
        ///     The SetCurrentLength method allows the caller to set the current
        ///     length of the media buffer.
        /// </summary>
        /// <param name="cbCurrentLength">
        ///     Current length of the media buffer in bytes.
        /// </param>
        /// <remarks>
        ///     This method should be used anytime a client modifies the content
        ///     of the media buffer in such a way that
        ///     the length of valid data in the media buffer changes.
        /// </remarks>
        [PreserveSig]
        public HRESULT SetCurrentLength(System.UInt32 cbCurrentLength);

        /// <summary>
        ///     The GetMaxLength method allows the caller to retrieve the maximum
        ///     length of the buffer represented by the IMFMediaBuffer object.
        /// </summary>
        /// <param name="pcbMaxLength">
        ///     Pointer to a count of bytes where the maximum length of the
        ///     buffer will be stored.
        /// </param>
        [PreserveSig] 
        public HRESULT GetMaxLength(System.UInt32* pcbMaxLength);
    }
}