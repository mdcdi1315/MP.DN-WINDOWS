

using System.Runtime.InteropServices;

namespace MP.AudioLibrary
{
    [StructLayout(LayoutKind.Explicit , Size = 18)]
    public struct WAVEFORMATEX
    {
        [FieldOffset(0)]
        public WAVEFORMATTAG Tag;

        [FieldOffset(2)]
        public System.UInt16 Channels;

        [FieldOffset(4)]
        public System.UInt32 SampleRate;

        [FieldOffset(8)]
        public System.UInt32 AverageBytesPerSecond;

        [FieldOffset(12)]
        public System.UInt16 BlockAlign;

        [FieldOffset(14)]
        public System.UInt16 BitsPerSample;

        [FieldOffset(16)]
        public System.UInt16 ExtraSize;
    }
}