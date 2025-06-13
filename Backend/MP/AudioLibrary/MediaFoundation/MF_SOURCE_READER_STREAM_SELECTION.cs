

namespace MP.AudioLibrary.MediaFoundation
{
    public enum MF_SOURCE_READER_STREAM_SELECTION : System.UInt32
    {
        INVALID_STREAM_INDEX = 0xFFFFFFFF,
        ALL_STREAMS = 0xFFFFFFFE,
        ANY_STREAM = 0xFFFFFFFE,
        FIRST_AUDIO_STREAM = 0xFFFFFFFD,
        FIRST_VIDEO_STREAM = 0xFFFFFFFC,
        MEDIASOURCE = 0xFFFFFFFF,
    }
}