
using System;
using MP.Collections;

namespace MP.AudioLibrary.Windows
{
    public class WindowsAudioFormat : AudioFormat
    {
        private System.Int32 avgbytespersec;

        public WindowsAudioFormat(WAVEFORMATEX ex)
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

        public WindowsAudioFormat(WAVEFORMATEXTENSIBLE extensible)
        {
            if (extensible.BaseFormat.Tag != WAVEFORMATTAG.Extensible)
            {
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
            ArrayBuilder<ChannelType> channels = new();
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
            ChannelLayout = channels.Build();
        }

        /// <inheritdoc />
        public override int AverageBytesPerSecond => avgbytespersec;
    }
}