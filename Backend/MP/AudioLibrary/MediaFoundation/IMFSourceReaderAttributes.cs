
using System;
using MP.ComInterop;
using MP.WindowsInterop;

namespace MP.AudioLibrary.MediaFoundation
{
    public static class IMFSourceReaderAttributes
    {
        /// <summary>
        /// Data type: IUnknown <br />
        /// This attribute should be set to the IUnknown interface of an object that implements the IMFSourceReaderCallback interface. <br />
        /// If the MF_SOURCE_READER_ASYNC_CALLBACK attribute is set, then the Source Reader will operate in asynchronous mode. <br />
        /// Otherwise, by default, the Source Reader operates synchronously.
        /// </summary>
        // The prefix MF_SOURCE_READER_ is omitted for brevity.
        public static Guid ASYNC_CALLBACK => new(0x1e3dbeac, 0xbb43, 0x4c35, 0xb5, 0x07, 0xcd, 0x64, 0x44, 0x64, 0xc9, 0x65);

        /// <summary>
        /// Data type: IUnknown <br />
        /// The Source Reader uses the MF Source Resolver API to instantiate the MF Media Source when passed either a URL or a bytestream. <br />
        /// The Source Resolver allows an application to pass runtime configuration parameters to the Media Source via an <see cref="IPropertyStore"/> interface. <br />
        /// The application can use this attribute when creating the Source Reader to pass the same <see cref="IPropertyStore"/> configuration down to the underlying Media Source. <br />
        /// The attribute should be set as the IUnknown interface of an object that implements the <see cref="IPropertyStore"/> interface.
        /// </summary>
        // The prefix MF_SOURCE_READER_ is omitted for brevity.
        public static Guid MEDIASOURCE_CONFIG => new(0x9085abeb, 0x0354, 0x48f9, 0xab, 0xb5, 0x20, 0x0d, 0xf8, 0x38, 0xc6, 0x8e);

        /// <summary>
        /// Data type: UINT32 <br />
        /// The application can query for this attribute using the IMFSourceReader::GetPresentationAttribute API in order to determine the characteristics of the underlying media source. <br />
        /// The value returned is a bitwise OR of zero or more flags from the MFMEDIASOURCE_CHARACTERISTICS enumeration. 
        /// </summary>
        // The prefix MF_SOURCE_READER_ is omitted for brevity.
        public static Guid MEDIASOURCE_CHARACTERISTICS => new(0x6d23f5c8, 0xc5d7, 0x4a9b, 0x99, 0x71, 0x5d, 0x11, 0xf8, 0xbc, 0xa8, 0x80);

        /// <summary>
        /// Data type: UINT32 (User must cast to <see cref="BOOL"/> type). <br />
        /// By default, if the source reader is passed in an existing media source object, then it will also shutdown the media source. <br />
        /// The application can set this attribute to <see cref="BOOL.TRUE"/> in order to have the source reader stop the media source and wait for MESourceStopped and MEStreamStopped events instead of shutting the media source down. <br />
        /// After receiving the stop events, the source reader will disconnect from the media source's MEG allowing the application to reuse the media source.
        /// </summary>
        // The prefix MF_SOURCE_READER_ is omitted for brevity.
        public static Guid DISCONNECT_MEDIASOURCE_ON_SHUTDOWN => new(0x56b67165, 0x219e, 0x456d, 0xa2, 0x2e, 0x2d, 0x30, 0x04, 0xc7, 0xfe, 0x56);

        /// <summary>
        /// Data type: UINT32 <br />
        /// By default, the Source Reader will not use MFTs that are registered for transcode use only. <br />
        /// This attribute can be set to <see cref="BOOL.TRUE"/> to enable use of these transcode only MFTs. 
        /// </summary>
        // The prefix MF_SOURCE_READER_ is omitted for brevity.
        public static Guid ENABLE_TRANSCODE_ONLY_TRANSFORMS => new(0xdfd4f008, 0xb5fd, 0x4e78, 0xae, 0x44, 0x62, 0xa1, 0xe6, 0x7b, 0xbe, 0x27);


    }
}