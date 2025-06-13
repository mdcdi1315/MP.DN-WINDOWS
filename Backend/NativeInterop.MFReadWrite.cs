
using MP.ComInterop;
using MP.Annotations;
using System.Runtime.InteropServices;
using MP.AudioLibrary.MediaFoundation;

partial class Interop
{
    public static unsafe class MfReadWrite
    {
        [DllImport(Libraries.MfReadWrite , EntryPoint = "MFCreateSourceReaderFromURL" , ExactSpelling = true)]
        private static extern HRESULT MFCreateSourceReaderFromURL_Native(
            System.Char* pwszURL,
            [IsPointerToCOMInterfaceType(typeof(IMFAttributes))]
            void* pAttributes,
            [IsPointerToCOMInterfaceType(typeof(IMFSourceReader))]
            void** ppSourceReader
        );

        [DllImport(Libraries.MfReadWrite, EntryPoint = "MFCreateSourceReaderFromByteStream", ExactSpelling = true)]
        private static extern HRESULT MFCreateSourceReaderFromByteStream_Native(
            [IsPointerToCOMInterfaceType(typeof(IMFByteStream))]
            void* pByteStream,
            [IsPointerToCOMInterfaceType(typeof(IMFAttributes))]
            void* pAttributes,
            [IsPointerToCOMInterfaceType(typeof(IMFSourceReader))]
            void** ppSourceReader
        );

        /// <summary>
        ///      This function is used to instantiate an MF Source Reader object for
        ///      the specified URL.
        /// </summary>
        /// <param name="url">
        ///      URL that specifies the location of the media content to open.
        /// </param>
        /// <param name="attributes">
        ///      Optional parameter specifying additional Source Reader configuration. <br />
        ///      This can be <see langword="null"/>.
        /// </param>
        /// <param name="srcreader">
        ///     Specifies a variable where the source reader object will be stored.
        /// </param>
        /// <remarks>
        ///     This function is synchronous and performs I/O that can
        ///     block the calling thread.
        /// </remarks>
        public static HRESULT MFCreateSourceReaderFromURL(System.String url , IMFAttributes attributes , out IMFSourceReader srcreader)
        {
            HRESULT hr;
            void* psrcreader;
            void* pattrs = attributes is null ? null : Marshal.GetIUnknownForObject(attributes).ToPointer();
            fixed (System.Char* purl = url)
            {
                hr = MFCreateSourceReaderFromURL_Native(purl, pattrs, &psrcreader);
            }
            if (hr.FAILED) {
                srcreader = null;
            } else {
                srcreader = ComMarshalling.CreateInteropObject(psrcreader) as IMFSourceReader;
            }
            return hr;
        }

        /// <summary>
        ///      This function is used to instantiate an MF Source Reader object for
        ///      the specified bytestream.
        /// </summary>
        /// <param name="bsm">
        ///      Instance of an <see cref="IMFByteStream"/> that contains the media content.
        /// </param>
        /// <param name="attributes">
        ///      Optional parameter specifying additional Source Reader configuration. <br />
        ///      This can be <see langword="null"/>.
        /// </param>
        /// <param name="srcreader">
        ///     Specifies a pointer to a variable where the source reader object will be stored.
        /// </param>
        /// <remarks>
        ///     This function is synchronous and performs I/O that can
        ///     block the calling thread.
        /// </remarks>
        public static HRESULT MFCreateSourceReaderFromByteStream(IMFByteStream bsm , IMFAttributes attributes , out IMFSourceReader srcreader)
        {
            void* psrcreader;
            void* pattrs = attributes is null ? null : Marshal.GetIUnknownForObject(attributes).ToPointer();
            void* pbytestream = Marshal.GetIUnknownForObject(bsm).ToPointer();
            HRESULT hr = MFCreateSourceReaderFromByteStream_Native(pbytestream , pattrs , &psrcreader);
            if (hr.FAILED) {
                srcreader = null;
            } else {
                srcreader = ComMarshalling.CreateInteropObject(psrcreader) as IMFSourceReader;
            }
            return hr;
        }
    }
}

