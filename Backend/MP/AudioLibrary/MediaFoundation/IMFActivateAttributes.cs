
using System;
using MP.WindowsInterop;
using System.Runtime.Versioning;

namespace MP.AudioLibrary.MediaFoundation
{
    public static class IMFActivateAttributes
    {
        /// <summary>
        /// Data type: <see cref="System.UInt32"/> <br />
        /// This attribute is set on the attribute store of <see cref="IMFActivate"/> object that MFTEnumEx creates for every enumerated MFT.
        /// </summary>
        [SupportedOSPlatform(WindowsVersions._WIN32_WINNT_WIN7)]
        public static Guid MF_TRANSFORM_FLAGS_Attribute => new(0x9359bb7e, 0x6275, 0x46c4, 0xa0, 0x25, 0x1c, 0x1, 0xe4, 0x5f, 0x1a, 0x86);

        /// <summary>
        /// Data type: <see cref="Guid"/> <br />
        /// MFTEnumEx stores this on the attribute store of <see cref="IMFActivate"/> object that MFTEnumEx creates for every enumerated MFT.
        /// </summary>
        [SupportedOSPlatform(WindowsVersions._WIN32_WINNT_WIN7)]
        public static Guid MF_TRANSFORM_CATEGORY_Attribute => new(0xceabba49, 0x506d, 0x4757, 0xa6, 0xff, 0x66, 0xc1, 0x84, 0x98, 0x7e, 0x4e);

        /// <summary>
        /// Data type: <see cref="Guid"/> <br />
        /// MFTEnumEx stores this in the attribute store of <see cref="IMFActivate"/> object that MFTEnumEx creates for every enumerated MFT. <br />
        /// CMFTransformActivate's <see cref="IMFActivate.ActivateObject"/> function creates an instance of the CoClass with this CLSID that implements the MFT.
        /// </summary>
        [SupportedOSPlatform(WindowsVersions._WIN32_WINNT_WIN7)]
        public static Guid MFT_TRANSFORM_CLSID_Attribute => new(0x6821c42b, 0x65a4, 0x4e82, 0x99, 0xbc, 0x9a, 0x88, 0x20, 0x5e, 0xcd, 0xc);

        /// <summary>
        /// Data type: <see cref="System.Byte"/> array <br />
        /// MFTEnumEx stores this in the attribute store of <see cref="IMFActivate"/> object that MFTEnumEx creates for every enumerated MFT.  <br />
        /// This is a blob that contains Major and Subtype GUID pairs for each type supported by the input pins of the MFT.
        /// </summary>
        [SupportedOSPlatform(WindowsVersions._WIN32_WINNT_WIN7)]
        public static Guid MFT_INPUT_TYPES_Attributes => new(0x4276c9b1, 0x759d, 0x4bf3, 0x9c, 0xd0, 0xd, 0x72, 0x3d, 0x13, 0x8f, 0x96);

        /// <summary>
        /// Data type: <see cref="System.Byte"/> array <br />
        /// MFTEnumEx stores this in the attribute store of <see cref="IMFActivate"/> object that MFTEnumEx creates for every enumerated MFT. <br />
        /// This is a blob that contains Major and Subtype GUID pairs for each type supported by the output pins of the MFT.
        /// </summary>
        [SupportedOSPlatform(WindowsVersions._WIN32_WINNT_WIN7)]
        public static Guid MFT_OUTPUT_TYPES_Attributes => new(0x8eae8cf3, 0xa44f, 0x4306, 0xba, 0x5c, 0xbf, 0x5d, 0xda, 0x24, 0x28, 0x18);

        /// <summary>
        /// Data type: <see cref="System.String"/> <br />
        /// MFTEnumEx stores this on the attribute store of <see cref="IMFActivate"/> object that MFTEnumEx creates for every enumerated MFT.  <br />
        /// Applications pass this attribute to source resolver to create media source that wraps a hardware. 
        /// </summary>
        [SupportedOSPlatform(WindowsVersions._WIN32_WINNT_WIN7)]
        public static Guid MFT_ENUM_HARDWARE_URL_Attribute => new(0x2fb866ac, 0xb078, 0x4942, 0xab, 0x6c, 0x0, 0x3d, 0x5, 0xcd, 0xa6, 0x74);

        /// <summary>
        /// Data type: <see cref="System.String"/> <br />
        /// MFTEnumEx stores this on the attribute store of <see cref="IMFActivate"/> object that MFTEnumEx creates for every enumerated MFT.  <br />
        /// Applications use this attribute to display a readable name for the device represented by the HW MFT.
        /// </summary>
        [SupportedOSPlatform(WindowsVersions._WIN32_WINNT_WIN7)]
        public static Guid MFT_FRIENDLY_NAME_Attribute => new(0x314ffbae, 0x5b41, 0x4c95, 0x9c, 0x19, 0x4e, 0x7d, 0x58, 0x6f, 0xac, 0xe3);

        /// <summary>
        /// Data type: IUnknown <br />
        /// For hardware MFTs, this attribute on the output stream holds the <see cref="IMFAttributes"/> from the input stream.  <br />
        /// The MFT can query this object for information it needs to perform medium negotiation or other functions dependant on information from the downstream MFT
        /// </summary>
        [SupportedOSPlatform(WindowsVersions._WIN32_WINNT_WIN7)]
        public static Guid MFT_CONNECTED_STREAM_ATTRIBUTE => new(0x71eeb820, 0xa59f, 0x4de2, 0xbc, 0xec, 0x38, 0xdb, 0x1d, 0xd6, 0x11, 0xa4);

        /// <summary>
        /// Data type: <see cref="System.UInt32"/> <br />
        /// When two hardware MFT streams get connected, this attribute would be set to <see cref="BOOL.TRUE"/>.
        /// </summary>
        [SupportedOSPlatform(WindowsVersions._WIN32_WINNT_WIN7)]
        public static Guid MFT_CONNECTED_TO_HW_STREAM => new(0x34e6e728, 0x6d6, 0x4491, 0xa5, 0x53, 0x47, 0x95, 0x65, 0xd, 0xb9, 0x12);

        /// <summary>
        /// Data type: IUnknown, which should be <see cref="IMFMediaType"/> <br />
        /// Application uses this attribute to store the HW encoder MFT preferred output media type on the <see cref="IMFActivate"/> object that MFTEnumEx creates. <br />
        /// When HW encoder is created, the media type will be  set as its current output media type.  
        /// </summary>
        [SupportedOSPlatform(WindowsVersions._WIN32_WINNT_WIN7)]
        public static Guid MFT_PREFERRED_OUTPUTTYPE_Attribute => new("7E700499-396A-49ee-B1B4-F628021E8C9D");

        /// <summary>
        /// Data type: <see cref="System.UInt32"/>, which should be a <see cref="BOOL"/> <br />
        /// This attribute is stored with a value of <see cref="BOOL.TRUE"/> in the <see cref="IMFActivate"/> object for in-process registered MFTs
        /// </summary>
        [SupportedOSPlatform(WindowsVersions._WIN32_WINNT_WIN7)]
        public static Guid MFT_PROCESS_LOCAL_Attribute => new("543186E4-4649-4e65-B588-4AA352AFF379");

        /// <summary>
        /// Data type: IUnknown <br />
        /// The IUnknown is a pointer to <see cref="IMFAttributes"/> interface. <br />
        /// This attribute store stores preferred encoder profile settings.
        /// </summary>
        [SupportedOSPlatform(WindowsVersions._WIN32_WINNT_WIN7)] 
        public static Guid MFT_PREFERRED_ENCODER_PROFILE => new("53004909-1EF5-46d7-A18E-5A75F8B5905F");

        /// <summary>
        /// Data type: <see cref="BOOL"/> <br />
        /// When this attribute is <see cref="BOOL.TRUE"/>, it indicates to a hardware MFT to timestamp outgoing samples with system time (QueryPerformanceCounter value)
        /// </summary>
        [SupportedOSPlatform(WindowsVersions._WIN32_WINNT_WIN7)]
        public static Guid MFT_HW_TIMESTAMP_WITH_QPC_Attribute => new(0x8d030fb8, 0xcc43, 0x4258, 0xa2, 0x2e, 0x92, 0x10, 0xbe, 0xf8, 0x9b, 0xe4);

        /// <summary>
        /// Data type: IUnknown <br />
        /// This attribute stores the interface to unlock the field-of-use MFTs.
        /// </summary>
        [SupportedOSPlatform(WindowsVersions._WIN32_WINNT_WIN7)] 
        public static Guid MFT_FIELDOFUSE_UNLOCK_Attribute => new("8EC2E9FD-9148-410d-831E-702439461A8E");

        /// <summary>
        /// Data type: <see cref="System.UInt32"/> <br />
        /// This attribute is attached to the MFT activator object and stores the merit if the MFT if it has one.
        /// </summary>
        [SupportedOSPlatform(WindowsVersions._WIN32_WINNT_WIN7)] 
        public static Guid MFT_CODEC_MERIT_Attribute => new("88a7cb15-7b07-4a34-9128-e64c6703c4d3");

        /// <summary>
        /// Data type: <see cref="System.UInt32"/> <br />
        /// This attribute is attached to the MFT activator object and specifies if the MFT could be used in a transcode topology. <br />
        /// If set to 1 then it should be used only in a transcode topology. 
        /// If set to 0 then it could be used in transcode or playback topology.  <br />
        /// Currently this is used only by devproxy based HW MFTs. 
        /// </summary>
        [SupportedOSPlatform(WindowsVersions._WIN32_WINNT_WIN7)]
        public static Guid MFT_ENUM_TRANSCODE_ONLY_ATTRIBUTE => new("111EA8CD-B62A-4bdb-89F6-67FFCDC2458B");
    }
}