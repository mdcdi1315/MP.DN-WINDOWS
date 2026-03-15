namespace MP.TagReading.ID3
{
    /// <summary>
    /// Defines ID3V2 frame types.
    /// </summary>
    public enum ID3V2FrameType : System.Byte
    {
        /// <summary>No special frame type is found, the data are binary.</summary>
        None = 0, // Raw binary data must be presumed
        /// <summary>The frame contains text data</summary>
        Text, // Multiple values
        /// <summary>The frame contains text that specifies a valid URL.</summary>
        URL, // Multiple values
        /// <summary>The frame contains user-defined text data</summary>
        UserDefinedTextFrame, // aka "TXXX"
        /// <summary>The frame contains a valid user-defined URL.</summary>
        UserDefinedURLLinkFrame, // aka "WXXX"
        /// <summary>The frame contains unsyncronized lyrics</summary>
        UnsyncronizedLyrics, // aka "USLT"
        /// <summary>The frame contains syncronized lyrics</summary>
        SyncronizedLyrics, // aka "SYLT"
        /// <summary>The frame contains an attached picture.</summary>
        Picture, // aka "APIC", or 'Attached Picture'
        /// <summary>The frame contains miscealleanous text data commenting on the file or tag</summary>
        Comments, // aka "COMM"
        /// <summary>The frame contains the publisher's terms of use</summary>
        TermsOfUse, // aka "USER"
        /// <summary>The frame contains ownership information</summary>
        Ownership, // aka "OWNE"
    }
}