
using System;
using MP.ComInterop;

namespace MP.AudioLibrary.MediaFoundation
{
    public static class IMFTransformPropertyKeys
    {
        private static Guid AllBaseMFTs => new(0xc57a84c0, 0x1a80, 0x40a3, 0x97, 0xb5, 0x92, 0x72, 0xa4, 0x3, 0xc8, 0xae);

        // All MFT's
        public static PROPERTYKEY MFPKEY_CLSID => new(AllBaseMFTs, 1);

        public static PROPERTYKEY MFPKEY_CATEGORY => new(AllBaseMFTs, 2);

        // ExAttribute-supported MFT's
        public static PROPERTYKEY MFPKEY_EXATTRIBUTE_SUPPORTED => new(new Guid(0x456fe843, 0x3c87, 0x40c0, 0x94, 0x9d, 0x14, 0x9, 0xc9, 0x7d, 0xab, 0x2c), 1);

        // Audio Multichannel
        public static PROPERTYKEY MFPKEY_MULTICHANNEL_CHANNEL_MASK => new(new Guid(0x58bdaf8c, 0x3224, 0x4692, 0x86, 0xd0, 0x44, 0xd6, 0x5c, 0x5b, 0xf8, 0x2b), 0x01);
    }
}