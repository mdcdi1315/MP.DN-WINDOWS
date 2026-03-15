

using System.Runtime.InteropServices;

namespace MP.AudioLibrary.Windows
{
    /// <summary>
    /// Basic format structure for Windows OS.
    /// </summary>
    [StructLayout(LayoutKind.Explicit , Size = 18)]
    public struct WAVEFORMATEX
    {
        /// <summary>
        /// Wave format tag of the current structure.
        /// </summary>
        [FieldOffset(0)]
        public WAVEFORMATTAG Tag;

        /// <summary>Number of channels.</summary>
        [FieldOffset(2)]
        public System.UInt16 Channels;

        /// <summary>Sample rate of the audio format</summary>
        [FieldOffset(4)]
        public System.UInt32 SampleRate;

        /// <summary>Average bytes per second of the audio format.</summary>
        [FieldOffset(8)]
        public System.UInt32 AverageBytesPerSecond;

        /// <summary>Block alignment of the audio format.</summary>
        [FieldOffset(12)]
        public System.UInt16 BlockAlign;

        /// <summary>Bitrate of the audio</summary>
        [FieldOffset(14)]
        public System.UInt16 BitsPerSample;

        /// <summary>Extra data right after this field.</summary>
        [FieldOffset(16)]
        public System.UInt16 ExtraSize;
    }
}