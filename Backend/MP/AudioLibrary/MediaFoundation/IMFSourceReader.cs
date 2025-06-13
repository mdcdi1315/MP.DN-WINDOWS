

using MP.ComInterop;
using MP.Annotations;
using MP.WindowsInterop;
using System.Runtime.InteropServices;

namespace MP.AudioLibrary.MediaFoundation
{
    [ComImport]
    [Guid(MediaFoundationInterfaceIds.IID_IMFSourceReader)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe interface IMFSourceReader
    {
        /// <summary>
        ///     Returns whether or not the specified stream is selected.
        ///     If the specified stream does not exist, the error
        ///     MF_E_INVALIDSTREAMNUMBER is returned.
        /// </summary>
        /// <param name="dwStreamIndex">
        ///     Specifies the stream index to query for selection state.
        /// </param>
        /// <param name="pfSelected">
        ///     Specifies a pointer to a variable where the selection state
        ///     will be stored.
        /// </param>
        [PreserveSig]
        public HRESULT GetStreamSelection(MF_SOURCE_READER_STREAM_SELECTION dwStreamIndex, BOOL* pfSelected);

        /// <summary>
        ///     Sets the selection state for the specified stream.
        ///     MF_SOURCE_READER_ALL_STREAMS can be specified in order
        ///     to set the stream selection for all available streams.
        /// </summary>
        /// <param name="dwStreamIndex">
        ///     Specifies the stream index.
        /// </param>
        /// <param name="fSelected">
        ///     Specifies whether or not the stream should be selected.
        /// </param>
        [PreserveSig]
        public HRESULT SetStreamSelection(MF_SOURCE_READER_STREAM_SELECTION dwStreamIndex, BOOL fSelected);

        /// <summary>
        ///     Returns the native media type for the specified stream.
        /// </summary>
        /// <param name="dwStreamIndex">
        ///     Specifies the stream index.
        /// </param>
        /// <param name="dwMediaTypeIndex">
        ///     Specifies the media type index.  As some sources support
        ///     multiple native media types, the index is used to indicate
        ///     the position in the list of supported media types.
        /// </param>
        /// <param name="ppMediaType">
        ///     Receives a copy of the specified native media type.
        /// </param>
        [PreserveSig]
        public HRESULT GetNativeMediaType(
           MF_SOURCE_READER_STREAM_SELECTION dwStreamIndex,
           System.UInt32 dwMediaTypeIndex,
           [IsPointerToCOMInterfaceType(typeof(IMFMediaType))]
           void** ppMediaType
        );

        /// <summary>
        ///     Returns the media type of the samples currently being
        ///     output for the specified stream.
        /// </summary>
        /// <param name="dwStreamIndex">
        ///     Specifies the stream index.
        /// </param>
        /// <param name="ppMediaType">
        ///     Receives a copy of the current media type.
        /// </param>
        [PreserveSig]
        public HRESULT GetCurrentMediaType(
              MF_SOURCE_READER_STREAM_SELECTION dwStreamIndex,
              [IsPointerToCOMInterfaceType(typeof(IMFMediaType))]
              void** ppMediaType
        );

        /// <summary>
        ///     Sets the output media type for samples returned for the specified
        ///     stream.
        /// </summary>
        /// <param name="dwStreamIndex">
        ///     Specifies the stream index.
        /// </param>
        /// <param name="pdwReserved">
        ///     Reserved for future use.
        /// </param>
        /// <param name="pMediaType">
        ///     Specifies the desired media type for the stream.
        /// </param>
        /// <remarks>
        ///     If an appropriate MFT cannot be found to transform the native
        ///     media type into the desired media type, then an error is
        ///     returned, and the current media type remains unchanged.
        /// </remarks>
        [PreserveSig]
        public HRESULT SetCurrentMediaType(
               MF_SOURCE_READER_STREAM_SELECTION dwStreamIndex,
               System.UInt32* pdwReserved,
               [IsPointerToCOMInterfaceType(typeof(IMFMediaType))]
               void* pMediaType
        );

        /// <summary>
        ///     Sets the position for where the next sample will be returned.
        /// </summary>
        /// <param name="guidTimeFormat">
        ///     Reference to a GUID that specifies the time format.
        ///     This can be GUID_NULL, in which case the time format
        ///     is in 100-nanosecond units.
        /// </param>
        /// <param name="varStartPosition">
        ///     Specifies the start position.  The units for this parameter
        ///     are indicated by the time format given in guidTimeFormat.
        /// </param>
        /// <remarks>
        ///     When reading samples asynchronously, the application may still
        ///     receive samples from the previous position if they were already
        ///     queued for delivery before this call was made.
        ///
        ///     If the underlying media source is not seekable, then
        ///     an appropriate error will be returned.
        /// </remarks>
        [PreserveSig]
        public HRESULT SetCurrentPosition(GUID* guidTimeFormat, PROPVARIANT* varPosition);

        /// <summary>
        ///     Requests the next available sample.  The caller can either
        ///     request a sample from a specific stream, or from any of
        ///     the selected streams.
        /// </summary>
        /// <param name="dwStreamIndex">
        ///     Specifies the stream for which the sample request is being made.
        ///     MF_SOURCE_READER_ANY_STREAM can be specified in order to request
        ///     the next sample from any available stream.
        /// </param>
        /// <param name="dwControlFlags">
        ///     Specifies flags that control the behavior of ReadSample.
        ///     See: section above on MF Source Reader Control Flags.
        /// </param>
        /// <param name="pdwActualStreamIndex">
        ///     Receives the actual stream index of the media sample.
        /// </param>
        /// <param name="pdwStreamFlags">
        ///     Receives the accumulated flags for the stream.
        ///     See: section above on MF Source Reader Flags
        /// </param>
        /// <param name="pllTimestamp">
        ///     Receives the presentation time of the sample.
        ///     If MF_SOURCE_READERF_STREAM_TICK is set in the stream flags,
        ///     then this receives the timestamp for the stream tick.
        /// </param>
        /// <param name="ppSample">
        ///     Receives the next sample for the stream.
        /// </param>
        /// <remarks>
        ///     When operating in synchronous mode, the out parameters
        ///     are all required parameters.
        ///
        ///     When operating in asynchronous mode, the out parameters
        ///     must all be set to NULL.
        ///
        ///     Streams must be selected in order to request samples
        ///     from them.
        ///
        ///     It is possible for ReadSample to return S_OK in synchronous
        ///     mode while not returning a sample.  The caller should always
        ///     check for NULL before dereferencing the sample.  This can
        ///     happen if EOS is reached, in which case
        ///     MF_SOURCE_READERF_ENDOFSTREAM will be set for the stream.
        ///     Another reason is if there is a gap in the stream, in which
        ///     case MF_SOURCE_READERF_STREAMTICK will be set.  For stream
        ///     ticks, the sample will be NULL, but the timestamp parameter
        ///     will be set to indicate the position in the stream where the
        ///     gap occurred.
        ///
        /// </remarks>
        [PreserveSig]
        public HRESULT ReadSample(
              MF_SOURCE_READER_STREAM_SELECTION dwStreamIndex,
              MF_SOURCE_READER_CONTROL_FLAG dwControlFlags,
              MF_SOURCE_READER_STREAM_SELECTION* pdwActualStreamIndex,
              MF_SOURCE_READER_FLAG* pdwStreamFlags,
              System.Int64* pllTimestamp,
              [IsPointerToCOMInterfaceType(typeof(IMFSample))]
              void** ppSample
        );

        /// <summary>
        ///     Releases any queued up samples, and cancels any outstanding sample
        ///     requests.
        /// </summary>
        /// <param name="dwStreamIndex">
        ///     Allows the application to specify which stream to flush.
        ///     MF_SOURCE_READER_ALL_STREAMS can be specified in order to 
        ///     flush all available streams.
        /// </param>
        /// <remarks>
        ///     In async mode, the OnFlush callback is called when the
        ///     flush operation completes.  Before receiving the OnFlush
        ///     callback, the application should not request any more
        ///     samples from the source reader.  Doing so will result in
        ///     the MF_E_NOTACCEPTING error being returned.
        /// </remarks>
        [PreserveSig]
        public HRESULT Flush(MF_SOURCE_READER_STREAM_SELECTION dwStreamIndex);

        /// <summary>
        ///     Allows the application to query for services and interfaces
        ///     that are implemented by decoder MFTs for the specified stream.
        /// </summary>
        /// <param name="dwStreamIndex">
        ///     Specifies the stream index.
        ///     MF_SOURCE_READER_MEDIASOURCE can be specified in order to get
        ///     the service off of the media source instead of a particular stream.
        /// </param>
        /// <param name="guidService">
        ///     Specifies the service GUID.
        ///     GUID_NULL can be passed in, in which case the Source Reader
        ///     will attempt to directly query for the interface from the MFT.
        /// </param>
        /// <param name="riid">
        ///     Specifies the interface ID.
        /// </param>
        /// <param name="ppvObject">
        ///     Receives the interface pointer.
        /// </param>
        [PreserveSig]
        public HRESULT GetServiceForStream(
               MF_SOURCE_READER_STREAM_SELECTION dwStreamIndex,
               GUID* guidService,
               GUID* riid,
               void** ppvObject
        );

        /// <summary>
        ///     Allows the application to get attributes from either the
        ///     media source or from a specific stream.
        /// </summary>
        /// <param name="dwStreamIndex">
        ///     Specifies the stream index.
        ///     MF_SOURCE_READER_MEDIASOURCE can be specified in order to get the
        ///     attribute from the media source instead of a particular stream.
        /// </param>
        /// <param name="guidAttribute">
        ///     Specifies the attribute ID.
        /// </param>
        /// <param name="pvarAttribute">
        ///     Receives the attribute value as a PROPVARIANT.
        /// </param>
        [PreserveSig]
        public HRESULT GetPresentationAttribute(
              MF_SOURCE_READER_STREAM_SELECTION dwStreamIndex,
              GUID* guidAttribute,
              PROPVARIANT* pvarAttribute
        );
    }
}