


namespace MP.NativeInterop.Windows.COM
{
    /// <summary>
    /// Defines values for when seeking in a <see cref="IStream"/> object.
    /// </summary>
    public enum STREAM_SEEK : System.UInt32
    {
        /// <summary>Seeking is done based on the stream's beginning.</summary>
        STREAM_SEEK_SET = 0,
        /// <summary>Seeking is done based on the stream's current position.</summary>
        STREAM_SEEK_CUR = 1,
        /// <summary>Seeking is done based on the stream's end.</summary>
        STREAM_SEEK_END = 2
    }
}