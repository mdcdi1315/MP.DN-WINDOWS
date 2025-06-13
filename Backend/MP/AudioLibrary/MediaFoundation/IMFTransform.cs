
using MP.ComInterop;
using MP.Annotations;
using System.Runtime.InteropServices;

namespace MP.AudioLibrary.MediaFoundation
{
    /// <summary>Interface supported by media objects</summary>
    [ComImport]
    [Guid(MediaFoundationInterfaceIds.IID_IMFTransform)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe interface IMFTransform
    {
        //
        // Stream enumeration
        //
        [PreserveSig]
        public HRESULT GetStreamLimits(
            System.UInt32* pdwInputMinimum,
            System.UInt32* pdwInputMaximum,
            System.UInt32* pdwOutputMinimum,
            System.UInt32* pdwOutputMaximum
        );

        [PreserveSig]
        public HRESULT GetStreamCount(
            System.UInt32* pcInputStreams,
            System.UInt32* pcOutputStreams
        );

        [PreserveSig]
        public HRESULT GetStreamIDs(
            System.UInt32 dwInputIDArraySize,
            System.UInt32* pdwInputIDs,
            System.UInt32 dwOutputIDArraySize,
            System.UInt32* pdwOutputIDs
        );

        [PreserveSig]
        public HRESULT GetInputStreamInfo(
            System.UInt32 dwInputStreamID,
            MFT_INPUT_STREAM_INFO* pStreamInfo
        );

        [PreserveSig]
        public HRESULT GetOutputStreamInfo(System.UInt32 dwOutputStreamID, MFT_OUTPUT_STREAM_INFO* pStreamInfo);

        [PreserveSig]
        public HRESULT GetAttributes(
            [IsPointerToCOMInterfaceType(typeof(IMFAttributes))]
            void** pAttributes
        );

        [PreserveSig]
        public HRESULT GetInputStreamAttributes(
            System.UInt32 dwInputStreamID,
            [IsPointerToCOMInterfaceType(typeof(IMFAttributes))]
            void** pAttributes
        );

        [PreserveSig]
        public HRESULT GetOutputStreamAttributes(
            System.UInt32 dwOutputStreamID,
            [IsPointerToCOMInterfaceType(typeof(IMFAttributes))]
            void** pAttributes
        );

        [PreserveSig]
        public HRESULT DeleteInputStream(System.UInt32 dwStreamID);

        [PreserveSig]
        public HRESULT AddInputStreams(System.UInt32 cStreams, System.UInt32* adwStreamIDs);

        //
        // Mediatypes
        //
        //
        // GetxxxAvailableType - iterate through media types supported by a stream.
        //
        [PreserveSig]
        public HRESULT GetInputAvailableType(
            System.UInt32 dwInputStreamID,
            System.UInt32 dwTypeIndex, // 0-based
            [IsPointerToCOMInterfaceType(typeof(IMFMediaType))]
            void** ppType
        );

        [PreserveSig]
        public HRESULT GetOutputAvailableType(
            System.UInt32 dwOutputStreamID,
            System.UInt32 dwTypeIndex, // 0-based
            [IsPointerToCOMInterfaceType(typeof(IMFMediaType))]
            void** ppType
        );

        //
        // SetxxxType - tell the object the type of data it will work with.
        //
        [PreserveSig]
        public HRESULT SetInputType(
            System.UInt32 dwInputStreamID,
            [IsPointerToCOMInterfaceType(typeof(IMFMediaType))]
            void* pType,
            MFT_SET_TYPE_FLAGS dwFlags
        );

        [PreserveSig]
        public HRESULT SetOutputType(
            System.UInt32 dwOutputStreamID,
            [IsPointerToCOMInterfaceType(typeof(IMFMediaType))]
            void* pType,
            MFT_SET_TYPE_FLAGS dwFlags
        );

        //
        // GetxxxCurrentType - get the current type set for the given stream index.
        //
        [PreserveSig]
        public HRESULT GetInputCurrentType(
            System.UInt32 dwInputStreamID,
            [IsPointerToCOMInterfaceType(typeof(IMFMediaType))]
            void** ppType
        );

        [PreserveSig]
        public HRESULT GetOutputCurrentType(
            System.UInt32 dwOutputStreamID,
            [IsPointerToCOMInterfaceType(typeof(IMFMediaType))]
            void** ppType
        );


        //
        // Streaming / state methods
        //
        // GetInputStatus - the only flag defined right now is MFT_INPUT_STATUS_ACCEPT_DATA.
        [PreserveSig]
        public HRESULT GetInputStatus(
            System.UInt32 dwInputStreamID,
            MFT_INPUT_STATUS_FLAGS* pdwFlags // MFT_INPUT_STATUS_ACCEPT_DATA
        );

        // GetOutputStatus - the only flag defined right now is MFT_OUTPUT_STATUS_SAMPLE_READY.
        [PreserveSig]
        public HRESULT GetOutputStatus(MFT_OUTPUT_STATUS_FLAGS* pdwFlags);

        //
        // SetOutputBounds - optional interface to tell transform the desired
        // range of output times desired. Implementation is optional.
        //
        [PreserveSig]
        public HRESULT SetOutputBounds(
            System.Int64 hnsLowerBound,
            System.Int64 hnsUpperBound
        );

        [PreserveSig]
        public HRESULT ProcessEvent(
            System.UInt32 dwInputStreamID,
            [IsPointerToCOMInterfaceType(typeof(IMFMediaEvent))]
            void* pEvent
        );

        //
        // ProcessMessage - used to send a notification or command to a transform
        //
        [PreserveSig]
        public HRESULT ProcessMessage(
            MFT_MESSAGE_TYPE eMessage,
            System.UInt64 ulParam
        );

        //
        // Pass one new buffer to an input stream
        //
        [PreserveSig]
        public HRESULT ProcessInput(
            System.UInt32 dwInputStreamID,
            [IsPointerToCOMInterfaceType(typeof(IMFSample))]
            void* pSample,
            System.UInt32 dwFlagsRSVD = 0
        );

        //
        // ProcessOutput() - generate output for current input buffers
        //
        // Output stream specific status information is returned in the
        // dwStatus member of each buffer wrapper structure.
        //
        [PreserveSig]
        public HRESULT ProcessOutput(
            MFT_PROCESS_OUTPUT_FLAGS dwFlags, // MFT_PROCESS_OUTPUT_FLAGS
            System.UInt32 cOutputBufferCount, // # returned by GetStreamCount()
            MFT_OUTPUT_DATA_BUFFER *pOutputSamples, // one per stream
            MFT_PROCESS_OUTPUT_STATUS* pdwStatus  // MFT_PROCESS_OUTPUT_XXX
        );
    }
}