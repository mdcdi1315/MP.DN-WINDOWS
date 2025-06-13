
using MP.ComInterop;
using MP.Annotations;
using MP.WindowsInterop;
using System.Runtime.InteropServices;

namespace MP.AudioLibrary.MediaFoundation
{
    /// <summary>
    ///     The IMFSample interface represents a multimedia sample for a 
    ///     multimedia stream type.  An typical example is a video frame,
    ///     and a notable exception is audio where an IMFSample does not usually 
    ///     represent a single sample of audio, but rather a chunk
    ///     of audio samples.  Note that this allows us to reduce the overhead for 
    ///     representing audio in the pipeline.
    ///
    ///     A sample may consist of a multiple buffers as is the case of compressed
    ///     audio or video samples received from the network or being sent to an 
    ///     ASF media sink or ASF multiplexer.  
    ///
    ///     The IMFAttributes interface can be used to tag the sample with extra
    ///     information that is not represented by the IMFSample methods.
    ///     The MFSampleExtension_xxx GUIDs in mfapi.h define some standard 
    ///     attributes, but custom attributes are allowed and are preserved
    ///     through the Media Foundation pipeline as well as possible.
    /// </summary>
    /// <remarks>
    ///     MediaFoundation does not provide a synchronized (thread-safe) way to 
    ///     call the IMFSample methods.
    ///     To guarantee thread-safety, the caller should obtain a lock while 
    ///     working with sample object to prevent access by other threads.
    /// </remarks>
    [ComImport]
    [Guid(MediaFoundationInterfaceIds.IID_IMFSample)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe interface IMFSample : IMFAttributes
    {
        [PreserveSig]
        public new HRESULT GetItem(GUID* guidKey, PROPVARIANT* pValue);

        [PreserveSig]
        public new HRESULT GetItemType(GUID* guidKey, MF_ATTRIBUTE_TYPE* pType);

        [PreserveSig]
        public new HRESULT CompareItem(GUID* guidKey, PROPVARIANT* Value, BOOL* pbResult);

        [PreserveSig]
        public new HRESULT Compare([IsPointerToCOMInterfaceType(typeof(IMFAttributes))] void* pTheirs, MF_ATTRIBUTES_MATCH_TYPE MatchType, BOOL* pbResult);

        [PreserveSig]
        public new HRESULT GetUINT32(GUID* guidKey, System.UInt32* punValue);

        [PreserveSig]
        public new HRESULT GetUINT64(GUID* guidKey, System.UInt64* punValue);

        [PreserveSig]
        public new HRESULT GetDouble(GUID* guidKey, System.Double* pfValue);

        [PreserveSig]
        public new HRESULT GetGUID(GUID* guidKey, GUID* pguidValue);

        [PreserveSig]
        public new HRESULT GetStringLength(GUID* guidKey, System.UInt32* pcchLength);

        [PreserveSig]
        public new HRESULT GetString(GUID* guidKey, System.Char* pwszValue, System.UInt32 cchBufSize, System.UInt32* pcchLength);

        [PreserveSig]
        public new HRESULT GetAllocatedString(GUID* guidKey, System.Char* ppwszValue, System.UInt32* pcchLength);

        [PreserveSig]
        public new HRESULT GetBlobSize(GUID* guidKey, System.UInt32* pcbBlobSize);

        [PreserveSig]
        public new HRESULT GetBlob(GUID* guidKey, System.Byte* pBuf, System.UInt32 cbBufSize, System.UInt32* pcbBlobSize);

        [PreserveSig]
        public new HRESULT GetAllocatedBlob(GUID* guidKey, System.Byte** ppBuf, System.UInt32* pcbSize);

        [PreserveSig]
        public new HRESULT GetUnknown(GUID* guidKey, GUID* riid, void** ppv);

        [PreserveSig]
        public new HRESULT SetItem(GUID* guidKey, PROPVARIANT* Value);

        [PreserveSig]
        public new HRESULT DeleteItem(GUID* guidKey);

        [PreserveSig]
        public new HRESULT DeleteAllItems();

        [PreserveSig]
        public new HRESULT SetUINT32(GUID* guidKey, System.UInt32 unValue);

        [PreserveSig]
        public new HRESULT SetUINT64(GUID* guidKey, System.UInt64 unValue);

        [PreserveSig]
        public new HRESULT SetDouble(GUID* guidKey, double fValue);

        [PreserveSig]
        public new  HRESULT SetGUID(GUID* guidKey, GUID* guidValue);

        [PreserveSig]
        public new HRESULT SetString(GUID* guidKey, System.Char* wszValue);

        [PreserveSig]
        public new HRESULT SetBlob(GUID* guidKey, System.Byte* pBuf, System.UInt32 cbBufSize);

        [PreserveSig]
        public new HRESULT SetUnknown(GUID* guidKey, void* pUnknown);

        [PreserveSig]
        public new HRESULT LockStore();

        [PreserveSig]
        public new HRESULT UnlockStore();

        [PreserveSig]
        public new HRESULT GetCount(System.UInt32* pcItems);

        [PreserveSig]
        public new HRESULT GetItemByIndex(System.UInt32 unIndex, GUID* pguidKey, PROPVARIANT* pValue);

        [PreserveSig]
        public new HRESULT CopyAllItems([IsPointerToCOMInterfaceType(typeof(IMFAttributes))] void* pDest);

        /// <summary>
        ///     The GetSampleFlags method returns a set of bitwise flags associated with the sample.  The flags are defined in the Remarks section.
        /// </summary>
        /// <param name="pdwSampleFlags">
        ///     Pointer to a 32 bit value where the sample flags will be stored.
        /// </param>
        /// <remarks>
        ///     No flags are defined in this version.  *pdwSampleFlags must be 
        ///     set to 0.
        /// </remarks>
        [PreserveSig]
        public HRESULT GetSampleFlags(System.UInt32* pdwSampleFlags);

        /// <summary>
        ///     The SetSampleFlags method allows the caller to set flags associated with the sample.
        ///     See the Remarks section of the GetSampleFlags method for the definition of the flags.
        /// </summary>
        /// <param name="dwSampleFlags">
        ///     32 bit value specifying the sample flags.
        /// </param>
        [PreserveSig]
        public HRESULT SetSampleFlags(System.UInt32 dwSampleFlags);

        /// <summary>
        ///     The GetSampleTime method returns the presentation time associated with the sample.
        /// </summary>
        /// <param name="phnsSampleTime">
        ///     Specifies a pointer to a 64 bit variable where the presentation time will be stored.
        /// </param>
        /// <returns>
        ///     If the method succeeds, it returns S_OK.
        ///     If no the sample does not have a sample time MF_E_NO_SAMPLE_TIMESTAMP is returned.
        ///     If this method fails otherwise it returns an error code.
        /// </returns>
        /// <remarks>
        ///     The presentation time is stored in 100ns interval units.
        /// </remarks>
        [PreserveSig]
        public HRESULT GetSampleTime(System.Int64* phnsSampleTime);

        /// <summary>
        ///     The SetSampleTime method allows the caller to set the presentation time associated with the sample.
        /// </summary>
        /// <param name="hnsSampleTime">
        ///     64 bit value specifying the presentation time.
        /// </param>
        /// <remarks>
        ///     The presentation time is stored in 100ns interval units.
        /// </remarks>
        [PreserveSig] 
        public HRESULT SetSampleTime(System.Int64 hnsSampleTime);

        /// <summary>
        ///     The GetSampleDuration method returns the duration of the sample.
        /// </summary>
        /// <param name="phnsSampleDuration">
        ///     Specifies a pointer to a 64 bit variable where duration will be stored.
        /// </param>
        /// <returns>
        ///     If the method succeeds, it returns S_OK.
        ///     If no the sample does not have a duration MF_E_NO_SAMPLE_DURATION is returned.
        ///     If this method fails otherwise it returns an error code.
        /// </returns>
        /// <remarks>
        ///     The duration is specified in 100ns interval units.
        ///     If the duration is zero, then the sample duration is unknown.  In such cases, it is possible that it can be derived from the media type e.g. video frame rate.
        ///     Application should avoid calculating duration of presentation as sum of sample durations because of possible cumulative error.
        ///     For example cumulative error for 60fps video on 24 hours interval is 0.1728 seconds.
        ///         For 60fps, there are 5184000 frames * 166667 100ns/frame = 24 hours + 0.1728 seconds
        /// </remarks>
        [PreserveSig]
        public HRESULT GetSampleDuration(System.Int64* phnsSampleDuration);

        /// <summary>
        ///     The SetSampleDuration method allows the caller to set the duration of the sample.
        /// </summary>
        /// <param name="hnsSampleDuration">
        ///     64 bit value specifying the sample duration.
        /// </param>
        /// <remarks>
        ///     The duration is specified in 100ns interval units.  This value should be set wherever possible to aid in the processing of samples in a multimedia pipeline.
        /// </remarks>
        [PreserveSig]
        public HRESULT SetSampleDuration(System.Int64 hnsSampleDuration);

        /// <summary>
        ///     The GetBufferCount method returns the count of the buffers in that are currently associated with this sample.
        /// </summary>
        /// <param name="pdwBufferCount">
        ///     Pointer to a 32 bit variable where the buffer count will be stored.
        /// </param>
        /// <remarks>
        ///     It is valid to have a sample with a buffer count of zero.  This is considered to be an empty sample.
        /// </remarks>
        [PreserveSig]
        public HRESULT GetBufferCount(System.UInt32* pdwBufferCount);

        /// <summary>
        ///     The GetBufferByIndex method returns a pointer to the IMFMediaBuffer object at a specified index in the sample.
        /// </summary>
        /// <param name="dwIndex">
        ///     32 bit value specifying the index of the buffer object requested.
        /// </param>
        /// <param name="ppBuffer">
        ///     Pointer to a pointer where the buffer object will be stored.
        /// </param>
        [PreserveSig] 
        public HRESULT GetBufferByIndex(System.UInt32 dwIndex, [IsPointerToCOMInterfaceType(typeof(IMFMediaBuffer))] void** ppBuffer);

        /// <summary>
        ///     The GetContiguousBuffer method converts sample with multiple buffers into sample with single buffer and returns a pointer to this buffer.
        /// </summary>
        /// <param name="ppBuffer">
        ///     Pointer to a pointer where the buffer object will be stored.
        /// </param>
        /// <remarks>
        ///     This method copies the content of the sample into a contiguous media buffer, and should be used with care.
        /// </remarks>
        [PreserveSig] 
        public HRESULT ConvertToContiguousBuffer([IsPointerToCOMInterfaceType(typeof(IMFMediaBuffer))] void** ppBuffer);

        /// <summary>
        ///     The AddBuffer method allows the caller to add a media buffer to the end of current list of buffers in the sample.
        /// </summary>
        /// <param name="pBuffer">
        ///     Pointer to a buffer object.
        /// </param>
        /// <returns>
        ///     If the method succeeds, it returns S_OK.
        ///     If sample does not support adding buffers, it returns MF_E_SAMPLE_UNSUPPORTED_OP.
        ///     If it fails, it returns an error code.
        /// </returns>
        /// <remarks>
        ///     The newly added buffer represents sample data at an offset equal to the previous total length of the sample.
        /// </remarks>
        [PreserveSig]
        public HRESULT AddBuffer([IsPointerToCOMInterfaceType(typeof(IMFMediaBuffer))] void* pBuffer);

        /// <summary>
        ///     The RemoveBufferByIndex method allows the caller to remove a buffer object at a specified index within the list of available buffer objects in the sample.
        /// </summary>
        /// <param name="dwIndex">
        ///     32 bit value specifying the index.
        /// </param>
        /// <returns>
        ///     If the method succeeds, it returns S_OK.
        ///     If sample does not support removing buffers, it returns MF_E_SAMPLE_UNSUPPORTED_OP.
        ///     If it fails, it returns an error code.
        /// </returns>
        /// <remarks>
        ///     The total length of the sample is reduced by the length of the removed buffer, and the offsets of all buffer objects that
        ///     were at indices greater than dwIndex are shifted up by the length of the removed buffer.
        /// </remarks>
        [PreserveSig]
        public HRESULT RemoveBufferByIndex(System.UInt32 dwIndex);

        /// <summary>
        ///     The RemoveAllBuffers method allows the caller to remove all buffer objects associated with the sample.
        /// </summary>
        /// <returns>
        ///     If the method succeeds, it returns S_OK.
        ///     If sample does not support removing buffers, it returns MF_E_SAMPLE_UNSUPPORTED_OP.
        ///     If it fails, it returns an error code.
        /// </returns>
        /// <remarks>
        ///     After this call, the buffer count of the sample is zero i.e. it is an empty sample.
        /// </remarks>
        [PreserveSig]
        public HRESULT RemoveAllBuffers();

        /// <summary>
        ///     The GetTotalLength method returns the total length of all buffers objects in the sample.
        /// </summary>
        /// <param name="pcbTotalLength">
        ///     Pointer to a 32 bit variable where the total length in bytes will be stored.
        /// </param>
        /// <remarks>
        ///     The total length of the sample is tracked when buffers are added (via AddBuffer), removed (via RemoveBufferByIndex and RemoveAllBuffers).
        /// </remarks>
        [PreserveSig]
        public HRESULT GetTotalLength(System.UInt32* pcbTotalLength);

        /// <summary>
        ///     The CopyToBuffer method allows the caller to copy the sample data to the buffer object provided.
        /// </summary>
        /// <param name="pBuffer">
        ///     Pointer to a buffer object.
        /// </param>
        /// <returns>
        ///     If the method succeeds, it returns S_OK. If destination buffer is too small, it returns MF_E_BUFFERTOOSMALL. If it fails for other reason, it returns an error code.
        /// </returns>
        [PreserveSig] 
        public HRESULT CopyToBuffer([IsPointerToCOMInterfaceType(typeof(IMFMediaBuffer))] void* pBuffer);
    }
}