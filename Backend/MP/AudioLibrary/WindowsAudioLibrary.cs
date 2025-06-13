
using System;
using MP.WindowsInterop;
using System.Collections.Generic;
using MP.ComInterop;

namespace MP.AudioLibrary
{
    /// <summary>
    /// Defines the one and unique static class that is responsible for initializing the Windows Audio Library.
    /// </summary>
    public static class WindowsAudioLibrary
    {
        private sealed class WindowsTranslatedAF : AudioFormat
        {
            private System.Int32 avgbytespersec;

            public WindowsTranslatedAF(WAVEFORMATEX ex)
            {
                switch (ex.Tag)
                {
                    case WAVEFORMATTAG.Pcm:
                        CommonFormat = CommonAudioFormat.PCM;
                        break;
                    case WAVEFORMATTAG.IeeeFloat:
                        CommonFormat = CommonAudioFormat.IEEEFloat;
                        break;
                    default:
                        // Possibly not supported, but return the tag value through common format so that it can be further inspected
                        CommonFormat = (CommonAudioFormat)ex.Tag;
                        break;
                }
                SampleRate = ex.SampleRate.ToInt32();
                BitRate = ex.BitsPerSample;
                avgbytespersec = ex.AverageBytesPerSecond.ToInt32();
                ChannelLayout = CommonChannelTypes.CreateLayoutFromNumberOfChannels(ex.Channels);
                BlockAlignment = ex.BlockAlign;
            }

            public WindowsTranslatedAF(WAVEFORMATEXTENSIBLE extensible)
            {
                if (extensible.BaseFormat.Tag != WAVEFORMATTAG.Extensible) {
                    throw new InvalidOperationException("Attempted to convert an invalidly created WAVEFORMATEXTENSIBLE structure.");
                }
                switch (extensible.SubFormatGUID.ToString().ToLowerInvariant())
                {
                    case "00000001-0000-0010-8000-00aa00389b71": // KSDATAFORMAT_SUBTYPE_PCM
                        CommonFormat = CommonAudioFormat.PCM;
                        break;
                    case "00000003-0000-0010-8000-00aa00389b71": // KSDATAFORMAT_SUBTYPE_IEEE_FLOAT
                        CommonFormat = CommonAudioFormat.IEEEFloat;
                        break;
                    default:
                        CommonFormat = CommonAudioFormat.Unknown;
                        DebugProvider.WriteLine($"WindowsAudioLibrary: FMTCNV: Sub Type GUID may not be supported: {extensible.SubFormatGUID}");
                        break;
                }
                SampleRate = extensible.BaseFormat.SampleRate.ToInt32();
                BitRate = extensible.BaseFormat.BitsPerSample;
                BlockAlignment = extensible.BaseFormat.BlockAlign;
                avgbytespersec = extensible.BaseFormat.AverageBytesPerSecond.ToInt32();
                List<ChannelType> channels = new(10);
                if (extensible.ChannelMask.HasFlag(SPEAKERASSIGNMENT.FRONT_LEFT))
                {
                    channels.Add(ChannelType.Left);
                }
                if (extensible.ChannelMask.HasFlag(SPEAKERASSIGNMENT.FRONT_RIGHT))
                {
                    channels.Add(ChannelType.Right);
                }
                if (extensible.ChannelMask.HasFlag(SPEAKERASSIGNMENT.FRONT_CENTER))
                {
                    channels.Add(ChannelType.Center);
                }
                if (extensible.ChannelMask.HasFlag(SPEAKERASSIGNMENT.LOW_FREQUENCY))
                {
                    channels.Add(ChannelType.LowFrequency);
                }
                if (extensible.ChannelMask.HasFlag(SPEAKERASSIGNMENT.BACK_LEFT))
                {
                    channels.Add(ChannelType.BackLeft);
                }
                if (extensible.ChannelMask.HasFlag(SPEAKERASSIGNMENT.BACK_RIGHT))
                {
                    channels.Add(ChannelType.BackRight);
                }
                if (extensible.ChannelMask.HasFlag(SPEAKERASSIGNMENT.FRONT_LEFT_OF_CENTER))
                {
                    channels.Add(ChannelType.FrontLeftOfCenter);
                }
                if (extensible.ChannelMask.HasFlag(SPEAKERASSIGNMENT.FRONT_RIGHT_OF_CENTER))
                {
                    channels.Add(ChannelType.FrontRightOfCenter);
                }
                if (extensible.ChannelMask.HasFlag(SPEAKERASSIGNMENT.BACK_CENTER))
                {
                    channels.Add(ChannelType.BackCenter);
                }
                if (extensible.ChannelMask.HasFlag(SPEAKERASSIGNMENT.SIDE_LEFT))
                {
                    channels.Add(ChannelType.SideLeft);
                }
                if (extensible.ChannelMask.HasFlag(SPEAKERASSIGNMENT.SIDE_RIGHT))
                {
                    channels.Add(ChannelType.SideRight);
                }
                if (extensible.ChannelMask.HasFlag(SPEAKERASSIGNMENT.TOP_CENTER))
                {
                    channels.Add(ChannelType.TopCenter);
                }
                if (extensible.ChannelMask.HasFlag(SPEAKERASSIGNMENT.TOP_FRONT_LEFT))
                {
                    channels.Add(ChannelType.TopFrontLeft);
                }
                if (extensible.ChannelMask.HasFlag(SPEAKERASSIGNMENT.TOP_FRONT_CENTER))
                {
                    channels.Add(ChannelType.TopFrontCenter);
                }
                if (extensible.ChannelMask.HasFlag(SPEAKERASSIGNMENT.TOP_FRONT_RIGHT))
                {
                    channels.Add(ChannelType.TopFrontRight);
                }
                if (extensible.ChannelMask.HasFlag(SPEAKERASSIGNMENT.TOP_BACK_LEFT))
                {
                    channels.Add(ChannelType.TopBackLeft);
                }
                if (extensible.ChannelMask.HasFlag(SPEAKERASSIGNMENT.TOP_BACK_CENTER))
                {
                    channels.Add(ChannelType.TopBackCenter);
                }
                if (extensible.ChannelMask.HasFlag(SPEAKERASSIGNMENT.TOP_BACK_RIGHT))
                {
                    channels.Add(ChannelType.TopBackRight);
                }
                ChannelLayout = channels.ToArray();
                channels.Clear();
                channels = null;
            }

            public override int AverageBytesPerSecond => avgbytespersec;
        }

        private static System.Boolean initialized;

        static WindowsAudioLibrary() => initialized = false;

        private static SPEAKERASSIGNMENT CreateMask(ChannelType[] layout)
        {
            SPEAKERASSIGNMENT assignment = 0;
            for (System.Int32 I = 0; I < layout.Length; I++)
            {
                assignment |= layout[I] switch
                {
                    ChannelType.Left => SPEAKERASSIGNMENT.FRONT_LEFT,
                    ChannelType.Right => SPEAKERASSIGNMENT.FRONT_RIGHT,
                    ChannelType.Center => SPEAKERASSIGNMENT.FRONT_CENTER,
                    ChannelType.LowFrequency => SPEAKERASSIGNMENT.LOW_FREQUENCY,
                    ChannelType.BackLeft => SPEAKERASSIGNMENT.BACK_LEFT,
                    ChannelType.BackRight => SPEAKERASSIGNMENT.BACK_RIGHT,
                    ChannelType.FrontLeftOfCenter => SPEAKERASSIGNMENT.FRONT_LEFT_OF_CENTER,
                    ChannelType.FrontRightOfCenter => SPEAKERASSIGNMENT.FRONT_RIGHT_OF_CENTER,
                    ChannelType.BackCenter => SPEAKERASSIGNMENT.BACK_CENTER,
                    ChannelType.SideLeft => SPEAKERASSIGNMENT.SIDE_LEFT,
                    ChannelType.SideRight => SPEAKERASSIGNMENT.SIDE_RIGHT,
                    ChannelType.TopCenter => SPEAKERASSIGNMENT.TOP_CENTER,
                    ChannelType.TopFrontLeft => SPEAKERASSIGNMENT.TOP_FRONT_LEFT,
                    ChannelType.TopFrontRight => SPEAKERASSIGNMENT.TOP_FRONT_RIGHT,
                    ChannelType.TopFrontCenter => SPEAKERASSIGNMENT.TOP_FRONT_CENTER,
                    ChannelType.TopBackLeft => SPEAKERASSIGNMENT.TOP_BACK_LEFT,
                    ChannelType.TopBackRight => SPEAKERASSIGNMENT.TOP_BACK_RIGHT,
                    ChannelType.TopBackCenter => SPEAKERASSIGNMENT.TOP_BACK_CENTER,
                    _ => throw new System.NotSupportedException($"Unsupported channel type {layout[I]}")
                };
            }
            return assignment;
        }

        private static void CreateAudioFormatConverterTypes()
        {
            // WAVEFORMATEX translations

            AudioFormatConverterToNativeRegistrationInfo aftowavefmtex = new();
            aftowavefmtex.OutputType = typeof(WAVEFORMATEX);
            aftowavefmtex.ConverterDelegate = new(AUDIOFORMAT_TOWAVEFORMATEX);
            AudioFormatConverter.RegisterConverterToNativeObject(aftowavefmtex);
            NativeToAudioFormatConverterRegistrationInfo wavefmtextoaf = new();
            wavefmtextoaf.InputType = typeof(WAVEFORMATEX);
            wavefmtextoaf.ConverterDelegate = new(WAVEFORMATEX_TOAUDIOFORMAT);
            AudioFormatConverter.RegisterNativeObjectToConverter(wavefmtextoaf);

            // WAVEFORMATEXTENSIBLE translations

            AudioFormatConverterToNativeRegistrationInfo aftowavefmtextensible = new();
            aftowavefmtextensible.OutputType = typeof(WAVEFORMATEXTENSIBLE);
            aftowavefmtextensible.ConverterDelegate = new(AUDIOFORMAT_TOWAVEFORMATEXTENSIBLE);
            AudioFormatConverter.RegisterConverterToNativeObject(aftowavefmtextensible);
            NativeToAudioFormatConverterRegistrationInfo wavefmtextensibletoaf = new();
            wavefmtextensibletoaf.InputType = typeof(WAVEFORMATEXTENSIBLE);
            wavefmtextensibletoaf.ConverterDelegate = new(WAVEFORMATEXTENSIBLE_TOAUDIOFORMAT);
            AudioFormatConverter.RegisterNativeObjectToConverter(wavefmtextensibletoaf);
        }

        private static System.Object AUDIOFORMAT_TOWAVEFORMATEX(AudioFormat fmt)
            => new WAVEFORMATEX() { 
                Tag = fmt.CommonFormat switch { 
                    CommonAudioFormat.PCM => WAVEFORMATTAG.Pcm,
                    CommonAudioFormat.IEEEFloat => WAVEFORMATTAG.IeeeFloat,
                    _ => throw new NotSupportedException($"Format conversion not supported: {fmt.CommonFormat}")
                },
                AverageBytesPerSecond = fmt.AverageBytesPerSecond.ToUInt32(),
                BlockAlign = fmt.BlockAlignment.ToUInt16(),
                BitsPerSample = fmt.BitRate.ToUInt16(),
                Channels = fmt.ChannelLayout.Length.ToUInt16(),
                SampleRate = fmt.SampleRate.ToUInt32(),
                ExtraSize = 0, // For typical scenarios, this is zero.
            };

        private static System.Object AUDIOFORMAT_TOWAVEFORMATEXTENSIBLE(AudioFormat fmt)
        {
            WAVEFORMATEXTENSIBLE e = new();
            e.BaseFormat.AverageBytesPerSecond = fmt.AverageBytesPerSecond.ToUInt32();
            e.BaseFormat.BlockAlign = fmt.BlockAlignment.ToUInt16();
            e.BaseFormat.BitsPerSample = fmt.BitRate.ToUInt16();
            e.BaseFormat.Channels = fmt.ChannelLayout.Length.ToUInt16();
            e.BaseFormat.SampleRate = fmt.SampleRate.ToUInt32();
            e.ChannelMask = CreateMask(fmt.ChannelLayout);
            e.Samples.ValidBitsPerSample = fmt.BitRate.ToUInt16();
            switch (fmt.CommonFormat)
            {
                case CommonAudioFormat.PCM:
                    e.SubFormatGUID = GUID.FromString("00000001-0000-0010-8000-00aa00389b71");
                    break;
                case CommonAudioFormat.IEEEFloat:
                    e.SubFormatGUID = GUID.FromString("00000003-0000-0010-8000-00aa00389b71");
                    break;
                default:
                    throw new NotSupportedException($"Common format {fmt.CommonFormat} not supported.");
            }
            return e;
        }

        private static WindowsTranslatedAF WAVEFORMATEX_TOAUDIOFORMAT(System.Object obj)
        {
            if (obj is WAVEFORMATEX ex)
            {
                return new WindowsTranslatedAF(ex);
            }
            return null;
        }

        private static WindowsTranslatedAF WAVEFORMATEXTENSIBLE_TOAUDIOFORMAT(System.Object obj)
        {
            if (obj is WAVEFORMATEXTENSIBLE e)
            {
                return new WindowsTranslatedAF(e);
            }
            return null;
        }


        public static void Initialize()
        {
            if (initialized) { return; }
            DebugProvider.WriteLine("WindowsAudioLibrary: Initializing Windows audio format converters.");
            CreateAudioFormatConverterTypes();
            DebugProvider.WriteLine("WindowsAudioLibrary: Successfully registered the Windows audio format converters!!");
            DebugProvider.WriteLine("WindowsAudioLibrary: Initializing Media Foundation.");
            HRESULT hr = Interop.MfPlat.MFStartup(Interop.MfPlat.MF_VERSION);
            switch (hr)
            {
                case MediaFoundation.MediaFoundationErrorCodes.MF_E_BAD_STARTUP_VERSION:
                    throw new MediaFoundation.MediaFoundationIncorrectStartupVersionException(Interop.MfPlat.MF_VERSION);
                case CommonHResults.E_NOTIMPL:
                    throw new NotImplementedException("Media Foundation is not completely implemented. This might indicate an incorrect install of the component.");
                default:
                    hr.ThrowOnFailure();
                    break;
            }
            DebugProvider.WriteLine("WindowsAudioLibrary: Media Foundation was initialized.");
            initialized = true;
        }

        public static void Uninitialize()
        {
            if (initialized == false) { return; }
            DebugProvider.WriteLine("WindowsAudioLibrary: Shutting down Media Foundation...");
            Interop.MfPlat.MFShutdown().ThrowOnFailure();
            DebugProvider.WriteLine("WindowsAudioLibrary: Media Foundation was successfully shut down.");
            initialized = false;
        }

    }
}