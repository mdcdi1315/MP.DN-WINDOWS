

using System;
using MP.WindowsInterop;
using System.Runtime.Versioning;

namespace MP.AudioLibrary.MediaFoundation
{
    public static class IMFTransformAttributes
    {
        /// <summary>
        /// Data type: <see cref="System.UInt32"/> <br />
        /// If present, indicates the number of samples that an MFT requires to be allocated.
        /// This value is used if the next node downstream has an IMFVideoSampleAllocator.
        /// </summary>
        public static Guid MF_SA_REQUIRED_SAMPLE_COUNT => new(0x18802c61, 0x324b, 0x4952, 0xab, 0xd0, 0x17, 0x6f, 0xf5, 0xc6, 0x96, 0xff);

        /// <summary>
        /// Data type: <see cref="BOOL"/> <br />
        /// If MFT specifies this attribute, then async wrapper will forward <see cref="MediaEventType.MFT_MESSAGE_NOTIFY_END_STREAMING"/> to MFT 
        /// </summary>
        public static Guid MFT_END_STREAMING_AWARE => new("70FBC845-B07E-4089-B064-399DC6110F29");

        /// <summary>
        /// Data type: <see cref="BOOL"/> <br />
        /// If present and set to a nonzero value, indicates that this decoder 
        /// transform is audio endpoint device-aware and can accept the audio 
        /// endpoint ID via the <see cref="MFT_AUDIO_DECODER_AUDIO_ENDPOINT_ID"/> attribute.
        /// </summary>
        [SupportedOSPlatform(WindowsVersions.NTDDI_WIN10_RS2)]
        public static Guid MF_SA_AUDIO_ENDPOINT_AWARE => new("C0381701-805C-42B2-AC8D-E2B4BF21F4F8");

        /// <summary>
        /// Data type: <see cref="System.String"/> <br />
        /// If set on the transform to a nonzero value, indicates that the data from 
        /// this audio decoder transform will be rendered on the audio endpoint
        /// device specified by the null-terminated wide-character string.
        /// </summary>
        [SupportedOSPlatform(WindowsVersions.NTDDI_WIN10_RS2)]
        public static Guid MFT_AUDIO_DECODER_AUDIO_ENDPOINT_ID => new("C7CCDD6E-5398-4695-8BE7-51B3E95111BD");

        /// <summary>
        /// Data type: IUnknown pointer <br />
        /// If present and set to a nonzero value, indicates that this audio decoder 
        /// will expect metadata items collections to be activated using the specified
        /// audio metadata client, specified by the ISpatialAudioMetadataClient interface.
        /// </summary>
        [SupportedOSPlatform(WindowsVersions.NTDDI_WIN10_RS2)]
        public static Guid MFT_AUDIO_DECODER_SPATIAL_METADATA_CLIENT => new("05987DF4-1270-4999-925F-8E939A7C0AF7");

        /// <summary>
        /// Data type: <see cref="System.UInt32"/> <br />
        /// If present, indicates the minimum number of samples that the MFT should allow to be oustanding (i.e. provided to the pipeline via <see cref="IMFTransform.ProcessOutput"/>) at any given time. <br />
        /// This value is only applicable to MFTs that allocate output samples themselves and use a circular allocator. Other MFTs can ignore this attribute.
        /// </summary>
        [SupportedOSPlatform(WindowsVersions._WIN32_WINNT_WIN8)]
        public static Guid MF_SA_MINIMUM_OUTPUT_SAMPLE_COUNT => new(0x851745d5, 0xc3d6, 0x476d, 0x95, 0x27, 0x49, 0x8e, 0xf2, 0xd1, 0xd, 0x18);

        /// <summary>
        /// Data type: <see cref="System.UInt32"/> <br />
        /// If present, indicates the minimum number of progressive samples that the MFT should allow to be oustanding (i.e. provided to the pipeline via <see cref="IMFTransform.ProcessOutput"/>) at any given time. <br />
        ///This value is only applicable to MFTs that allocate output samples themselves and use a circular allocator. Other MFTs can ignore this attribute. 
        /// </summary>
        [SupportedOSPlatform(WindowsVersions._WIN32_WINNT_WIN8)]
        public static Guid MF_SA_MINIMUM_OUTPUT_SAMPLE_COUNT_PROGRESSIVE => new(0xf5523a5, 0x1cb2, 0x47c5, 0xa5, 0x50, 0x2e, 0xeb, 0x84, 0xb4, 0xd1, 0x4a);

        /// <summary>
        /// Data Type: <see cref="System.UInt32"/> (treat as <see cref="BOOL"/>). <br />
        /// Advertised by encoder MFTs that support receiving <see cref="MediaEventType.MEEncodingParameters"/> event while streaming (via <see cref="IMFTransform.ProcessEvent"/>)
        /// </summary>
        [SupportedOSPlatform(WindowsVersions._WIN32_WINNT_WIN8)]
        public static Guid MFT_ENCODER_SUPPORTS_CONFIG_EVENT => new("86A355AE-3A77-4EC4-9F31-01149A4E92DE");

        /// <summary>
        /// Data Type: WSTR
        /// For hardware MFT's, this attribute specifies the vendor ID of the hardware that the HMFT is using for processing.  <br />
        /// This is only for reference and is not used/verified by the topology
        /// </summary>
        [SupportedOSPlatform(WindowsVersions._WIN32_WINNT_WIN8)]
        public static Guid MFT_ENUM_HARDWARE_VENDOR_ID_Attribute => new(0x3aecb0cc, 0x35b, 0x4bcc, 0x81, 0x85, 0x2b, 0x8d, 0x55, 0x1e, 0xf3, 0xaf);

        /// <summary>
        /// Data type: <see cref="System.UInt32"/> <br />
        /// If present and set to a nonzero value, indicates that this MFT functions as an asynchronous MFT. <br />
        /// Only callers that understand how to call an asynchronous MFT can use this MFT; 
        /// those callers need to set the MF_TRANSFORM_ASYNC_UNLOCK before making any <see cref="IMFTransform"/> calls.
        /// </summary>
        [SupportedOSPlatform(WindowsVersions._WIN32_WINNT_WIN7)]
        public static Guid MF_TRANSFORM_ASYNC => new(0xf81a699a, 0x649a, 0x497d, 0x8c, 0x73, 0x29, 0xf8, 0xfe, 0xd6, 0xad, 0x7a);

        // This attribute is set by the caller (not the MFT) on the MFT's attributes
        // store.  This is relevant only if the MFT has set the MF_TRANSFORM_ASYNC
        // attribute to a nonzero value.  Callers that plan to use such an MFT must
        // first set this attribute to 1; otherwise all IMFTransform calls will fail
        // with MF_E_TRANSFORM_ASYNC_LOCKED.
        // The Media Foundation pipeline will set this attribute if appropriate; 
        // applications using this transform in a topology using the MF pipeline should
        // not set it directly.
        /// <summary>
        /// Data type: <see cref="System.UInt32"/> <br />
        /// This attribute is set by the caller (not the MFT) on the MFT's attributes store. <br />
        /// This is relevant only if the MFT has set the <see cref="MF_TRANSFORM_ASYNC"/> attribute to a nonzero value. <br />
        /// Callers that plan to use such an MFT must first set this attribute to 1; otherwise all <see cref="IMFTransform"/> calls will fail with <see cref="MediaFoundationErrorCodes.MF_E_TRANSFORM_ASYNC_LOCKED"/>. <br />
        /// The Media Foundation pipeline will set this attribute if appropriate;  applications using this transform in a topology using the MF pipeline should not set it directly.
        /// </summary>
        [SupportedOSPlatform(WindowsVersions._WIN32_WINNT_WIN7)]
        public static Guid MF_TRANSFORM_ASYNC_UNLOCK => new(0xe5666d6b, 0x3422, 0x4eb6, 0xa4, 0x21, 0xda, 0x7d, 0xb1, 0xf8, 0xe2, 0x7);

    }
}