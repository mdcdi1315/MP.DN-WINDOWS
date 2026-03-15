
using MP.Utilities;
using MP.NativeInterop.Windows;
using System.Runtime.InteropServices;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace MP.AudioLibrary.Windows
{
    /// <summary>
    /// Wave format extensible structure. It is used in WASAPI and similar interfaces. <br />
    /// For more information see <see href="https://learn.microsoft.com/en-us/windows/win32/api/mmreg/ns-mmreg-waveformatextensible"/>
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Size = 40)]
    public struct WAVEFORMATEXTENSIBLE : IDerivedStruct<WAVEFORMATEXTENSIBLE, WAVEFORMATEX>
    {
        /// <summary>
        /// Union structure for <see cref="Samples"/> field.
        /// </summary>
        [StructLayout(LayoutKind.Explicit , Size = 2)]
        public struct SAMPLES
        {
            /// <summary>Valid bits per sample.</summary>
            [FieldOffset(0)]
            public System.UInt16 ValidBitsPerSample;

            /// <summary>Samples per block.</summary>
            [FieldOffset(0)]
            public System.UInt16 SamplesPerBlock;

            /// <summary>Reserved otherwise.</summary>
            [FieldOffset(0)]
            public System.UInt16 RSVD;
        }

        /// <summary>
        /// Base format structure, referenced indirectly.
        /// </summary>
        [FieldOffset(0)]
        public WAVEFORMATEX BaseFormat;

        /// <summary>
        /// Sample information data.
        /// </summary>
        [FieldOffset(18)]
        public SAMPLES Samples;

        /// <summary>Channel arrangements mask.</summary>
        [FieldOffset(20)]
        public SPEAKERASSIGNMENT ChannelMask;

        /// <summary>
        /// Sub format GUID of the current audio format.
        /// </summary>
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

        /// <summary>
        /// Initializes a new instance of the <see cref="WAVEFORMATEXTENSIBLE"/> structure.
        /// </summary>
        public WAVEFORMATEXTENSIBLE()
        {
            BaseFormat = new();
            Samples = new();
            SubFormatGUID = new();
            BaseFormat.Tag = WAVEFORMATTAG.Extensible;
            BaseFormat.ExtraSize = 22;
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator WAVEFORMATEX([DisallowNull] WAVEFORMATEXTENSIBLE t) => t.BaseFormat;
    }
}