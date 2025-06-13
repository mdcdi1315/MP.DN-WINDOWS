
using MP.WindowsInterop;
using System.Runtime.InteropServices;

namespace MP.AudioLibrary
{
    /// <summary>
    /// Wave format extensible structure. It is used in WASAPI and similar interfaces. <br />
    /// For more information see <see href="https://learn.microsoft.com/en-us/windows/win32/api/mmreg/ns-mmreg-waveformatextensible"/>
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct WAVEFORMATEXTENSIBLE
    {
        [StructLayout(LayoutKind.Explicit , Size = 2)]
        public struct SAMPLES
        {
            [FieldOffset(0)]
            public System.UInt16 ValidBitsPerSample;

            [FieldOffset(0)]
            public System.UInt16 SamplesPerBlock;

            [FieldOffset(0)]
            public System.UInt16 RSVD;
        }

        [FieldOffset(0)]
        public WAVEFORMATEX BaseFormat;

        [FieldOffset(18)]
        public SAMPLES Samples;

        [FieldOffset(20)]
        public SPEAKERASSIGNMENT ChannelMask;

        [FieldOffset(24)]
        public GUID SubFormatGUID;

        public static WAVEFORMATEXTENSIBLE CreateFrom(System.Int32 samplerate , System.Int32 bitrate , System.Int32 nchannels)
        {
            WAVEFORMATEXTENSIBLE ext = new();
            ext.BaseFormat.Channels = nchannels.ToUInt16();
            ext.BaseFormat.BitsPerSample = bitrate.ToUInt16();
            ext.BaseFormat.SampleRate = samplerate.ToUInt32();
            ext.BaseFormat.BlockAlign = (nchannels * (bitrate / 8)).ToUInt16();
            ext.BaseFormat.AverageBytesPerSecond = ext.BaseFormat.SampleRate * ext.BaseFormat.BlockAlign;
            for (System.Int32 I = 0; I < nchannels; I++)
            {
                ext.ChannelMask |= (SPEAKERASSIGNMENT)(1 << I);
            }
            switch (bitrate)
            {
                case 32:
                    ext.SubFormatGUID = GUID.FromString("00000003-0000-0010-8000-00aa00389b71"); // KSDATAFORMAT_SUBTYPE_IEEE_FLOAT
                    break;
                default:
                    ext.SubFormatGUID = GUID.FromString("00000001-0000-0010-8000-00aa00389b71"); // KSDATAFORMAT_SUBTYPE_PCM
                    break;
            }
            return ext;
        }

        public WAVEFORMATEXTENSIBLE()
        {
            BaseFormat = new();
            Samples = new();
            SubFormatGUID = new();
            BaseFormat.Tag = WAVEFORMATTAG.Extensible;
            BaseFormat.ExtraSize = 22;
        }
    }
}