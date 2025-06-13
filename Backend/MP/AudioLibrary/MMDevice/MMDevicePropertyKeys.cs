
using MP.ComInterop;
using MP.WindowsInterop;

namespace MP.AudioLibrary.MMDevice
{
    /// <summary>
    /// Defines <see cref="PROPERTYKEY"/> properties which you can use to query the property store of an <see cref="IMMDevice"/>.
    /// </summary>
    public static class MMDevicePropertyKeys
    {
        private static GUID DeviceAccessGUID => WindowsInterop.GUID.FromString("A45C254E-DF1C-4EFD-8020-67D146A850E0");

        private static GUID AudioEndpointGUID => WindowsInterop.GUID.FromString("1da5d803-d492-4edd-8c23-e0c0ffee7f0e");

        private static GUID DeviceInterfaceGUID => WindowsInterop.GUID.FromString("026E516E-B814-414B-83CD-856D6FEF4822");

        private static GUID DeviceSetupClassGUID => WindowsInterop.GUID.FromString("259ABFFC-50A7-47CE-AF08-68C9A7D73366");

        /// <summary>
        /// PKEY_AudioEndpoint_FormFactor:  AudioEndpointFormFactor (cast to <see cref="System.UInt32"/>) of an Audio Endpoint <see cref="IMMDevice"/>. <br />
        /// The <see cref="PROPVARIANT.Type"/> field reports the value of this property as <see cref="VARTYPE.VT_UI4"/>.
        /// </summary>
        public static PROPERTYKEY FormFactor => new(AudioEndpointGUID, 0);

        /// <summary>
        /// PKEY_AudioEndpoint_ControlPanelPageProvider: mmsys.cpl device properties page extensions.  Used in device interface, devnode and endpoint propertystores <br />
        /// The <see cref="PROPVARIANT.Type"/> field reports the value of this property as <see cref="VARTYPE.VT_LPWSTR"/> -- a UUID.
        /// </summary>
        public static PROPERTYKEY ControlPanelPageProvider => new(AudioEndpointGUID, 1);

        /// <summary>
        /// PKEY_AudioEndpoint_Association: ks pin category to associate with an "endpoint target" property store in an inf file <br />
        /// The <see cref="PROPVARIANT.Type"/> field reports the value of this property as <see cref="VARTYPE.VT_LPWSTR"/> (stringized GUID)
        /// </summary>
        public static PROPERTYKEY Association => new(AudioEndpointGUID, 2);

        /// <summary>
        /// PKEY_AudioEndpoint_PhysicalSpeakers: the channel configuration of speakers that are physically present in a users system <br />
        /// The <see cref="PROPVARIANT.Type"/> field reports the value of this property as <see cref="VARTYPE.VT_UI4"/>.
        /// </summary>
        public static PROPERTYKEY PhysicalSpeakers => new(AudioEndpointGUID, 3);

        /// <summary>
        /// PKEY_AudioEndpoint_GUID: A GUID associated with this audio endpoint, unique across all audio endpoints. This GUID can be used as the device identifier in the DirectSound APIs. <br />
        /// The <see cref="PROPVARIANT.Type"/> field reports the value of this property as <see cref="VARTYPE.VT_LPWSTR"/> (stringized GUID)
        /// </summary>
        public static PROPERTYKEY GUID => new(AudioEndpointGUID, 4);

        /// <summary>
        /// PKEY_Endpoint_Disable_SysFx: Boolean that when TRUE enables SysFx for the endpoint <br />
        /// The <see cref="PROPVARIANT.Type"/> field reports the value of this property as <see cref="VARTYPE.VT_UI4"/>, however you should cast this to the <see cref="ENDPOINT_SYSFX"/> enumeration.
        /// </summary>
        public static PROPERTYKEY Disable_SysFx => new(AudioEndpointGUID, 5);

        /// <summary>
        /// PKEY_AudioEndpoint_FullRangeSpeakers: the channel configuration of speakers that are FullRangely present in a users system <br />
        /// The <see cref="PROPVARIANT.Type"/> field reports the value of this property as <see cref="VARTYPE.VT_UI4"/>.
        /// </summary>
        public static PROPERTYKEY FullRangeSpeakers => new(AudioEndpointGUID, 6);

        /// <summary>
        /// PKEY_AudioEndpoint_Supports_EventDriven_Mode: INF supplied property that indicates endpoint supports event-driven mode <br />
        /// The <see cref="PROPVARIANT.Type"/> field reports the value of this property as <see cref="VARTYPE.VT_UI4"/>.
        /// </summary>
        public static PROPERTYKEY Supports_EventDriven_Mode => new(AudioEndpointGUID, 7);

        /// <summary>
        /// PKEY_AudioEndpoint_JackSubType:  KS Category ID (GUID) of an Audio Endpoint <br />
        /// The <see cref="PROPVARIANT.Type"/> field reports the value of this property as <see cref="VARTYPE.VT_LPWSTR"/> (stringized GUID)
        /// </summary>
        public static PROPERTYKEY JackSubType => new(AudioEndpointGUID, 8);

        /// <summary>
        /// PKEY_AudioEndpoint_Default_VolumeInDb: INF supplied property that indicates default volume in DB for an endpoint. <br />
        /// The <see cref="PROPVARIANT.Type"/> field reports the value of this property as <see cref="VARTYPE.VT_UI4"/>. (Float value expressed in fixed point 16.16 format)
        /// </summary>
        public static PROPERTYKEY Default_VolumeInDb => new(AudioEndpointGUID, 9);

        /// <summary>
        /// PKEY_AudioEngine_DeviceFormat: The format device format (can be PCM integer)
        /// The <see cref="PROPVARIANT.Type"/> field reports the value of this property as <see cref="VARTYPE.VT_BLOB"/>.
        /// </summary>
        public static PROPERTYKEY DeviceFormat => new(new GUID() { 
            Data1 = 0xf19f064d,
            Data2 = 0x82c,
            Data3 = 0x4e27,
            Data4_0 = 0xbc,
            Data4_1 = 0x73,
            Data4_2 = 0x68,
            Data4_3 = 0x82,
            Data4_4 = 0xa1,
            Data4_5 = 0xbb,
            Data4_6 = 0x8e,
            Data4_7 = 0x4c
        } , 0);

        public static PROPERTYKEY Device_DeviceDesc => new(DeviceAccessGUID, 2); // DEVPROP_TYPE_STRING

        public static PROPERTYKEY Device_HardwareIds => new(DeviceAccessGUID, 3); // DEVPROP_TYPE_STRING_LIST

        public static PROPERTYKEY Device_CompatibleIds => new(DeviceAccessGUID, 4); // DEVPROP_TYPE_STRING_LIST

        public static PROPERTYKEY Device_Service => new(DeviceAccessGUID, 6); // DEVPROP_TYPE_STRING

        public static PROPERTYKEY Device_Class => new(DeviceAccessGUID, 9); // DEVPROP_TYPE_STRING

        public static PROPERTYKEY Device_ClassGuid => new(DeviceAccessGUID, 10);  // DEVPROP_TYPE_GUID

        public static PROPERTYKEY Device_Driver => new(DeviceAccessGUID, 11); // DEVPROP_TYPE_STRING

        public static PROPERTYKEY Device_ConfigFlags => new(DeviceAccessGUID, 12); // DEVPROP_TYPE_UINT32

        public static PROPERTYKEY Device_Manufacturer => new(DeviceAccessGUID, 13); // DEVPROP_TYPE_UINT32

        public static PROPERTYKEY Device_FriendlyName => new(DeviceAccessGUID, 14); // DEVPROP_TYPE_STRING

        public static PROPERTYKEY Device_InstanceId => new(new GUID() { 
            Data1 = 0x78c34fc8,
            Data2 = 0x104a,
            Data3 = 0x4aca,
            Data4_0 = 0x9e,
            Data4_1 = 0xa4,
            Data4_2 = 0x52,
            Data4_3 = 0x4d,
            Data4_4 = 0x52,
            Data4_5 = 0x99,
            Data4_6 = 0x6e,
            Data4_7 = 0x57
        }, 256); // DEVPROP_TYPE_STRING

        public static PROPERTYKEY DeviceInterface_FriendlyName => new(DeviceInterfaceGUID, 2); // DEVPROP_TYPE_STRING

        public static PROPERTYKEY DeviceInterface_Enabled => new(DeviceInterfaceGUID, 3); // DEVPROP_TYPE_BOOLEAN

        public static PROPERTYKEY DeviceInterface_ClassGuid => new(DeviceInterfaceGUID, 4); // DEVPROP_TYPE_GUID

        public static PROPERTYKEY DeviceClass_Name => new(DeviceSetupClassGUID, 2); // DEVPROP_TYPE_STRING

        public static PROPERTYKEY DeviceClass_ClassName => new(DeviceSetupClassGUID, 3); // DEVPROP_TYPE_STRING

        public static PROPERTYKEY DeviceClass_Icon => new(DeviceSetupClassGUID, 4); // DEVPROP_TYPE_STRING

        public static PROPERTYKEY DeviceClass_ClassInstaller => new(DeviceSetupClassGUID, 5); // DEVPROP_TYPE_STRING

        public static PROPERTYKEY DeviceClass_PropPageProvider => new(DeviceSetupClassGUID, 6); // DEVPROP_TYPE_STRING

        public static PROPERTYKEY DeviceClass_NoInstallClass => new(DeviceSetupClassGUID, 7); // DEVPROP_TYPE_BOOLEAN

        public static PROPERTYKEY DeviceClass_NoDisplayClass => new(DeviceSetupClassGUID, 8); // DEVPROP_TYPE_BOOLEAN

        public static PROPERTYKEY DeviceClass_SilentInstall => new(DeviceSetupClassGUID, 9); // DEVPROP_TYPE_BOOLEAN

        public static PROPERTYKEY DeviceClass_NoUseClass => new(DeviceSetupClassGUID, 10); // DEVPROP_TYPE_BOOLEAN

        public static PROPERTYKEY DeviceClass_DefaultService => new(DeviceSetupClassGUID, 11); // DEVPROP_TYPE_STRING

        public static PROPERTYKEY DeviceClass_IconPath => new(DeviceSetupClassGUID, 12); // DEVPROP_TYPE_STRING_LIST

    }
}