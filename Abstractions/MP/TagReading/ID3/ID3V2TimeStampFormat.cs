namespace MP.TagReading.ID3
{
    /// <summary>
    /// Describes time stamp formats for those ID3V2 frames that use this.
    /// </summary>
    public enum ID3V2TimeStampFormat : System.Byte
    {
        /// <summary>
        /// From <see href="https://id3.org/id3v2.4.0-frames"/>: <br />
        /// MPEG frames as unit
        /// </summary>
        MPEGFrames = 0b1000000,
        /// <summary>
        /// From <see href="https://id3.org/id3v2.4.0-frames"/>: <br />
        /// milliseconds as unit
        /// </summary>
        Milliseconds = 0b0100000
    }
}