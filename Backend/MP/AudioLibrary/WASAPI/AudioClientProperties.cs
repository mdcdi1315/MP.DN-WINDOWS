using MP.WindowsInterop;
using System.Runtime.InteropServices;

namespace MP.AudioLibrary.WASAPI
{
    [StructLayout(LayoutKind.Explicit)]
    public struct AudioClientProperties
    {
        [FieldOffset(0)]
        private System.UInt32 Size;
        [FieldOffset(4)]
        public BOOL IsOffload;
        [FieldOffset(8)]
        public AUDIO_STREAM_CATEGORY Category;
        [FieldOffset(12)]
        public AUDCLNT_STREAMOPTIONS Options;

        public unsafe AudioClientProperties()
        {
            Size = sizeof(AudioClientProperties).ToUInt32();
        }
    }
}