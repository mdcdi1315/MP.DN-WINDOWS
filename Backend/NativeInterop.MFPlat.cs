
using MP;
using System;
using MP.ComInterop;
using MP.Annotations;
using MP.AudioLibrary;
using MP.WindowsInterop;
using System.Runtime.Versioning;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;
using MP.AudioLibrary.MediaFoundation;

partial class Interop
{
    public static unsafe class MfPlat
    {
        // This value is unused in the Win7 release and left at its Vista release value
        public const System.UInt32 MF_API_VERSION = 0x0070;

        [SupportedOSPlatform(WindowsVersions._WIN32_WINNT_WIN7)]
        public const System.UInt32 MF_SDK_VERSION = 0x0002;

        public const System.UInt32 MF_VERSION = (MF_SDK_VERSION << 16 | MF_API_VERSION);

        public enum MFWaveFormatExConvertFlags 
        {
            MFWaveFormatExConvertFlag_Normal = 0,
            MFWaveFormatExConvertFlag_ForceExtensible = 1
        }

        public enum MFStartupFlags : System.UInt32
        {
             MFSTARTUP_NOSOCKET = 0x1,
             MFSTARTUP_LITE = MFSTARTUP_NOSOCKET,
             MFSTARTUP_FULL = 0,
        }

        [Flags]
        public enum MFT_ENUM_FLAG : System.UInt32
        {
            MFT_ENUM_FLAG_SYNCMFT = 0x00000001, // Enumerates V1 MFTs. This is default.
            MFT_ENUM_FLAG_ASYNCMFT = 0x00000002, // Enumerates only software async MFTs also known as V2 MFTs
            MFT_ENUM_FLAG_HARDWARE = 0x00000004, // Enumerates V2 hardware async MFTs
            MFT_ENUM_FLAG_FIELDOFUSE = 0x00000008, // Enumerates MFTs that require unlocking
            MFT_ENUM_FLAG_LOCALMFT = 0x00000010, // Enumerates Locally (in-process) registered MFTs
            MFT_ENUM_FLAG_TRANSCODE_ONLY = 0x00000020, // Enumerates decoder MFTs used by transcode only    
            MFT_ENUM_FLAG_SORTANDFILTER = 0x00000040, // Apply system local, do not use and preferred sorting and filtering
            MFT_ENUM_FLAG_SORTANDFILTER_APPROVED_ONLY = 0x000000C0, // Similar to MFT_ENUM_FLAG_SORTANDFILTER, but apply a local policy of: MF_PLUGIN_CONTROL_POLICY_USE_APPROVED_PLUGINS
            MFT_ENUM_FLAG_SORTANDFILTER_WEB_ONLY = 0x00000140, // Similar to MFT_ENUM_FLAG_SORTANDFILTER, but apply a local policy of: MF_PLUGIN_CONTROL_POLICY_USE_WEB_PLUGINS
            MFT_ENUM_FLAG_SORTANDFILTER_WEB_ONLY_EDGEMODE = 0x00000240, // Similar to MFT_ENUM_FLAG_SORTANDFILTER, but apply a local policy of: MF_PLUGIN_CONTROL_POLICY_USE_WEB_PLUGINS_EDGEMODE
            MFT_ENUM_FLAG_UNTRUSTED_STOREMFT = 0x00000400, // Enumerates all untrusted store MFTs downloaded from the store
            MFT_ENUM_FLAG_ALL = 0x0000003F, // Enumerates all MFTs including SW and HW MFTs and applies filtering
        }

        public enum MFASYNC_CALLBACK_QUEUE : System.UInt32
        {
            /// <summary>
            /// Bit mask to distinguish platform work queues from those created by calling <see cref="MFAllocateWorkQueueEx"/>.
            /// For a work queue created by <see cref="MFAllocateWorkQueueEx"/>, the following value is nonzero:
            /// <c>(identifier &amp; MFASYNC_CALLBACK_QUEUE_PRIVATE_MASK)</c>
            /// </summary>
            MFASYNC_CALLBACK_QUEUE_PRIVATE_MASK = 0xFFFF0000,
            MFASYNC_CALLBACK_QUEUE_MULTITHREADED = 0x00000005,
            /// <summary>Undefined work queue.</summary>
            MFASYNC_CALLBACK_QUEUE_UNDEFINED = 0x00000000
        }

        public enum MFASYNC_WORKQUEUE_TYPE : System.UInt32
        {
            /// <summary>
            /// Create a work queue without a message loop
            /// </summary>
            MF_STANDARD_WORKQUEUE = 0,
            /// <summary>
            /// Create a work queue with a message loop.
            /// </summary>
            MF_WINDOW_WORKQUEUE = 1,
            /// <summary>
            /// Create a multithreaded work queue. <br />
            /// This type of work queue uses a thread pool to dispatch work items.  <br />
            /// The caller is responsible for serializing the work items.
            /// </summary>
            MF_MULTITHREADED_WORKQUEUE = 2
        }

        [StructLayout(LayoutKind.Explicit , Size = 32)]
        public struct MFT_REGISTER_TYPE_INFO
        {
            [FieldOffset(0)]
            public GUID MajorType;
            [FieldOffset(16)]
            public GUID Subtype;
        }

        /// <summary>
        /// Initializes Microsoft Media Foundation.
        /// </summary>
        [DllImport(Libraries.MfPlat, ExactSpelling = true)]
        public static extern HRESULT MFStartup(System.UInt32 version, MFStartupFlags dwFlags = MFStartupFlags.MFSTARTUP_FULL);

        /// <summary>
        /// Shuts down the Microsoft Media Foundation platform
        /// </summary>
        [DllImport(Libraries.MfPlat, ExactSpelling = true)]
        public static extern HRESULT MFShutdown();

        [DllImport(Libraries.MfPlat, ExactSpelling = true, EntryPoint = "MFCreateMediaType")]
        private static extern HRESULT MFCreateMediaType_Native(void** ppMFmedType);

        /// <summary>
        /// Creates an empty media type.
        /// </summary>
        public static HRESULT MFCreateMediaType(out IMFMediaType ppMFType)
        {
            void* ptr;
            HRESULT hrt = MFCreateMediaType_Native(&ptr);
            if (hrt.FAILED) {
                ppMFType = null;
            } else {
                ppMFType = ComMarshalling.CreateInteropObject(ptr) as IMFMediaType;
            }
            return hrt;
        }

        /// <summary>
        /// Initializes a media type from a <see cref="WAVEFORMATEX"/> structure. 
        /// </summary>
        [DllImport(Libraries.MfPlat, ExactSpelling = true , EntryPoint = "MFInitMediaTypeFromWaveFormatEx")]
        private static extern HRESULT MFInitMediaTypeFromWaveFormatEx_Native(
            [IsPointerToCOMInterfaceType(typeof(IMFMediaType))]
            void* pMFType,  
            WAVEFORMATEX* pWaveFormat,  
            System.Int32 cbBufSize);

        public static HRESULT MFInitMediaTypeFromWaveFormatEx(IMFMediaType medtype , WAVEFORMATEX waveformat)
            => MFInitMediaTypeFromWaveFormatEx_Native(Marshal.GetIUnknownForObject(medtype).ToPointer(), &waveformat, sizeof(WAVEFORMATEX));

        public static HRESULT MFInitMediaTypeFromWaveFormatEx(IMFMediaType medtype, WAVEFORMATEXTENSIBLE extensible)
            => MFInitMediaTypeFromWaveFormatEx_Native(Marshal.GetIUnknownForObject(medtype).ToPointer(), (WAVEFORMATEX*)&extensible, sizeof(WAVEFORMATEXTENSIBLE));

        /// <summary>
        /// Converts a Media Foundation audio media type to a <see cref="WAVEFORMATEX"/> structure.
        /// </summary>
        [DllImport(Libraries.MfPlat, ExactSpelling = true , EntryPoint = "MFCreateWaveFormatExFromMFMediaType")]
        private static extern HRESULT MFCreateWaveFormatExFromMFMediaType_Native(
            [IsPointerToCOMInterfaceType(typeof(IMFMediaType))]
            void* pMFType, 
            WAVEFORMATEX** ppWF, 
            System.UInt32* pcbSize, 
            MFWaveFormatExConvertFlags flags = 0);

        public static HRESULT MFCreateWaveFormatExFromMFMediaType(IMFMediaType medtype , out WAVEFORMATEXTENSIBLE extensible)
        {
            System.UInt32 size;
            WAVEFORMATEX* pwf;
            HRESULT hr = MFCreateWaveFormatExFromMFMediaType_Native(Marshal.GetIUnknownForObject(medtype).ToPointer() , &pwf , &size , MFWaveFormatExConvertFlags.MFWaveFormatExConvertFlag_ForceExtensible);
            extensible = *(WAVEFORMATEXTENSIBLE*)pwf;
            Ole32.CoTaskMemFree(pwf);
            if (size > sizeof(WAVEFORMATEXTENSIBLE)) {
                DebugProvider.WriteLine($"MFMEDTYPECNV: Detected audio format loss ({size} bytes out of {sizeof(WAVEFORMATEXTENSIBLE)}). This may result in format decode errors.");
            }
            return hr;
        }

        // The original code should be this but this for a very weird ass reason does cause the runtime to fail with code 0x80131506??
        // I will possibly make it an issue in .NET .
        /*
        [DllImport(Libraries.MfPlat , ExactSpelling = true , EntryPoint = "MFCreateMFByteStreamOnStream")]
        private static extern HRESULT MFCreateMFByteStreamOnStream_Native(
            [IsPointerToCOMInterfaceType(typeof(IStream))]
            void* punkstream,
            [IsPointerToCOMInterfaceType(typeof(IMFByteStream))]
            void** ppbytestr
        );

        /// <summary>
        /// Creates a Microsoft Media Foundation byte stream that wraps an IRandomAccessStream object.
        /// </summary>
        public static HRESULT MFCreateMFByteStreamOnStream(IStream punkStream, out IMFByteStream ppByteStream)
        {
            void* ppbsptr;
            HRESULT hrt = MFCreateMFByteStreamOnStream_Native(Marshal.GetIUnknownForObject(punkStream).ToPointer(), &ppbsptr);
            if (hrt.FAILED) {
                ppByteStream = null;
            } else {
                ppByteStream = ComMarshalling.CreateInteropObject(ppbsptr) as IMFByteStream;
            }
            return hrt;
        }*/

        [DllImport(Libraries.MfPlat , ExactSpelling = true)]
        public static extern HRESULT MFCreateMFByteStreamOnStream(IStream punkStream, out IMFByteStream ppByteStream);

        [DllImport(Libraries.MfPlat , ExactSpelling = true , EntryPoint = "MFCreateAsyncResult")]
        private static extern HRESULT MFCreateAsyncResult_Native(
              void* punkObject,
              [IsPointerToCOMInterfaceType(typeof(IMFAsyncCallback))] void* pCallback,
              void* punkState,
              [IsPointerToCOMInterfaceType(typeof(IMFAsyncResult))] void** ppAsyncResult
        );

        public static HRESULT MFCreateAsyncResult(
            System.Object comobj,
            IMFAsyncCallback callback,
            System.Object comstateobj,
            out IMFAsyncResult result)
        {
            ArgumentNullException.ThrowIfNull(callback);
            void* po = comobj is null ? null : Marshal.GetIUnknownForObject(comobj).ToPointer();
            void* so = comstateobj is null ? null : Marshal.GetIUnknownForObject(comstateobj).ToPointer();
            void* rt;
            HRESULT hr = MFCreateAsyncResult_Native(
                po, 
                Marshal.GetIUnknownForObject(callback).ToPointer(), 
                so, 
                &rt
            );
            if (hr.SUCCEEDED) {
                // This object is under control by the callee most of the times, thus allow the Marshal API to work as .NET expects it to be
                result = ComMarshalling.CreateInteropObject(rt , -1) as IMFAsyncResult;
            } else {
                result = null;
            }
            return hr;
        }

        public static HRESULT MFCreateAsyncResult(
            System.Object comobj,
            IMFAsyncCallback callback,
            void* stateobj,
            out IMFAsyncResult result)
        {
            ArgumentNullException.ThrowIfNull(callback);
            void* po = comobj is null ? null : Marshal.GetIUnknownForObject(comobj).ToPointer();
            void* rt;
            HRESULT hr = MFCreateAsyncResult_Native(
                po, 
                Marshal.GetIUnknownForObject(callback).ToPointer(), 
                stateobj, 
                &rt
             );
            if (hr.SUCCEEDED) {
                // This object is under control by the callee most of the times, thus allow the Marshal API to work as .NET expects it to be
                result = ComMarshalling.CreateInteropObject(rt, -1) as IMFAsyncResult;
            } else {
                result = null;
            }
            return hr;
        }

        [DllImport(Libraries.MfPlat, ExactSpelling = true)]
        public static extern HRESULT MFInvokeCallback([IsPointerToCOMInterfaceType(typeof(IMFAsyncResult))] void* presult);

        [DllImport(Libraries.MfPlat, ExactSpelling = true)]
        public static extern HRESULT MFPutWorkItem(MFASYNC_CALLBACK_QUEUE dwQueue, [IsPointerToCOMInterfaceType(typeof(IMFAsyncCallback))] void* pCallback, void* pState);

        [DllImport(Libraries.MfPlat, ExactSpelling = true , EntryPoint = "MFAllocateWorkQueueEx")]
        private static extern HRESULT MFAllocateWorkQueueEx_Native(MFASYNC_WORKQUEUE_TYPE type , MFASYNC_CALLBACK_QUEUE* pworkid);

        public static HRESULT MFAllocateWorkQueueEx(MFASYNC_WORKQUEUE_TYPE type , out MFASYNC_CALLBACK_QUEUE q)
        {
            MFASYNC_CALLBACK_QUEUE qp;
            HRESULT hr = MFAllocateWorkQueueEx_Native(type, &qp);
            q = qp;
            return hr;
        }

        [DllImport(Libraries.MfPlat, ExactSpelling = true)]
        public static extern HRESULT MFUnlockWorkQueue(MFASYNC_CALLBACK_QUEUE queue);

        [DllImport(Libraries.MfPlat , ExactSpelling = true , EntryPoint = "MFCreateSample")]
        private static extern HRESULT MFCreateSample_Native([IsPointerToCOMInterfaceType(typeof(IMFSample))] void** ppimfsample);

        /// <summary>
        /// Creates an empty media sample.
        /// </summary>
        public static HRESULT MFCreateSample(out IMFSample ppIMFSample)
        {
            void* ppimfsample;
            HRESULT hrt = MFCreateSample_Native(&ppimfsample);
            if (hrt.FAILED) {
                ppIMFSample = null;
            } else {
                ppIMFSample = ComMarshalling.CreateInteropObject(ppimfsample) as IMFSample;
            }
            return hrt;
        }

        [DllImport(Libraries.MfPlat , ExactSpelling = true , EntryPoint = "MFCreateMemoryBuffer")]
        private static extern HRESULT MFCreateMemoryBuffer_Native(
            System.Int32 cbmaxlen ,
            [IsPointerToCOMInterfaceType(typeof(IMFMediaBuffer))]
            void** ppimfmedbuf);

        public static HRESULT MFCreateMemoryBuffer_IntPtr(System.Int32 cbmaxlen, out System.IntPtr pbuf)
        {
            void* ppb;
            HRESULT hr = MFCreateMemoryBuffer_Native(cbmaxlen, &ppb);
            if (hr.FAILED) { 
                pbuf = IntPtr.Zero;
            } else {
                pbuf = new(ppb);
            }
            return hr;
        }

        /// <summary>
        /// Allocates system memory and creates a media buffer to manage it.
        /// </summary>
        public static HRESULT MFCreateMemoryBuffer(System.Int32 cbMaxLength, out IMFMediaBuffer ppBuffer)
        {
            void* ppimfmedbuf;
            HRESULT hrt = MFCreateMemoryBuffer_Native(cbMaxLength, &ppimfmedbuf);
            if (hrt.FAILED) {
                ppBuffer = null;
            } else {
                ppBuffer = ComMarshalling.CreateInteropObject(ppimfmedbuf) as IMFMediaBuffer;
            }
            return hrt;
        }

        [DllImport(Libraries.MfPlat , ExactSpelling = true , EntryPoint = "MFCreateAttributes")]
        private static extern HRESULT MFCreateAttributes_Native(
            [IsPointerToCOMInterfaceType(typeof(IMFAttributes))]
            void** ppmfattrs, 
            System.Int32 cinitsize
        );

        /// <summary>
        /// Creates an empty attribute store. 
        /// </summary>
        public static HRESULT MFCreateAttributes(System.Int32 cInitialSize , out IMFAttributes mfattributes)
        {
            void* ppmfattrs;
            HRESULT hrt = MFCreateAttributes_Native(&ppmfattrs , cInitialSize);
            if (hrt.FAILED) {
                mfattributes = null;
            } else {
                mfattributes = ComMarshalling.CreateInteropObject(ppmfattrs) as IMFAttributes;
            }
            return hrt;
        }

        [DllImport(Libraries.MfPlat , EntryPoint = "MFInitAttributesFromBlob" , ExactSpelling = true)]
        private static extern HRESULT MFInitAttributesFromBlob_Native(
            [IsPointerToCOMInterfaceType(typeof(IMFAttributes))]
            void* pattributes,
            System.Byte* pdatablock,
            System.UInt32 datablocksize
        );

        public static HRESULT MFInitAttributesFromBlob(IMFAttributes attributes , System.Byte[] data)
        {
            HRESULT hr;
            void* pattributes = Marshal.GetIUnknownForObject(attributes).ToPointer();
            fixed (System.Byte* pdata = data)
            {
                hr = MFInitAttributesFromBlob_Native(pattributes , pdata , data.LongLength.ToUInt32());
            }
            return hr;
        }

        [DllImport(Libraries.MfPlat , EntryPoint = "MFGetAttributesAsBlob" , ExactSpelling = true)]
        private static extern HRESULT MFGetAttributesAsBlob_Native(
            [IsPointerToCOMInterfaceType(typeof(IMFAttributes))]
            void* pattributes,
            System.Byte* pbuffer,
            System.UInt32 buffersize
        );

        // Note that this can return E_OUTOFMEMORY if it cannot allocate the requested native memory!
        public static HRESULT MFGetAttributesAsBlob(IMFAttributes attributes , out System.Byte[] buffer)
        {
            HRESULT hr;
            buffer = null;
            void* pattributes = Marshal.GetIUnknownForObject(attributes).ToPointer();
            SafeLibcMemoryHandle memtemp;
            try {
                memtemp = new(400); // Allocate 400 bytes at least
            } catch (OutOfMemoryException) {
                // Return COM out of memory , do not throw exceptions here, let the error be managed by higher-level clients.
                return CommonHResults.E_OUTOFMEMORY;
            }
        G_Retry:
            hr = MFGetAttributesAsBlob_Native(pattributes , memtemp.MemoryPointer , memtemp.MemoryLength.ToUInt32());
            if (hr == MediaFoundationErrorCodes.MF_E_BUFFERTOOSMALL)
            {
                // Buffer was too small, increase the buffer by 100 bytes and retry
                // Reallocation has the danger of introducing OutOfMemoryException.
                // If that happens, the older block will not be destroyed.
                try {
                    memtemp.Reallocate(memtemp.MemoryLength + 100);
                } catch (OutOfMemoryException) {
                    memtemp.Dispose();
                    // Return COM out of memory , do not throw exceptions here, let the error be managed by higher-level clients.
                    return CommonHResults.E_OUTOFMEMORY;
                }
                goto G_Retry;
            } else if (hr.SUCCEEDED) {
                // Success, create managed buffer and return that instead.
                buffer = memtemp.ToManaged();
            }
            // On any case, ensure to dispose the buffer.
            memtemp.Dispose();
            return hr;
        }

        [DllImport(Libraries.MfPlat, EntryPoint = "MFTEnumEx", ExactSpelling = true)]
        private static extern HRESULT MFTEnumEx_Native(
            GUID guidcat, 
            MFT_ENUM_FLAG flags, 
            MFT_REGISTER_TYPE_INFO* pintypes, 
            MFT_REGISTER_TYPE_INFO* pouttypes,
            [IsPointerToCOMInterfaceType(typeof(IMFActivate))]
            void*** PPMFActivates, 
            System.UInt32* pnumMFActivates
        );

        public static HRESULT MFTEnumEx(Guid guidcategory, MFT_ENUM_FLAG flags, MFT_REGISTER_TYPE_INFO[] intypes, MFT_REGISTER_TYPE_INFO[] outtypes, out IMFActivate[] activates)
        {
            void** nativearray;
            System.UInt32 elements;
            HRESULT ret;
            fixed (MFT_REGISTER_TYPE_INFO* pin = intypes)
            fixed (MFT_REGISTER_TYPE_INFO* pout = outtypes)
            {
                ret = MFTEnumEx_Native(GUID.FromGUID(guidcategory), flags, pin, pout, &nativearray, &elements);
            }
            if (ret.FAILED) { activates = null; return ret; }
            activates = new IMFActivate[elements];
            for (System.Int32 I = 0; I < activates.Length; I++)
            {
                activates[I] = ComMarshalling.CreateInteropObject(nativearray[I]) as IMFActivate;
            }
            Ole32.CoTaskMemFree(nativearray);
            return ret;
        }
    }

}
