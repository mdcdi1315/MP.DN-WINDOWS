namespace MP.TagReading.ID3
{
    /// <summary>
    /// Defines ID3V2 frame types.
    /// </summary>
    public enum ID3V2FrameType : System.Byte
    {
        None = 0, // Raw binary data must be presumed
        Text, // Multiple values
        URL, // Multiple values
        UserDefinedTextFrame, // aka "TXXX"
        UserDefinedURLLinkFrame, // aka "WXXX"
        UnsyncronizedLyrics, // aka "USLT"
        SyncronizedLyrics, // aka "SYLT"
        Picture, // aka "APIC", or 'Attached Picture'
        Comments // aka "COMM"
    }
}