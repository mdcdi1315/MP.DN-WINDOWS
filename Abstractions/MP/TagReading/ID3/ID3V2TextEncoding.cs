namespace MP.TagReading.ID3
{
    /// <summary>
    /// Gets the text encoding used to encode the text data of the frame. 
    /// </summary>
    public enum ID3V2TextEncoding : System.Byte
    {
        /// <summary>A subset of <see cref="System.Text.Encoding.ASCII"/> encoding was used.</summary>
        ISO_8859_1 = 0,
        /// <summary>The <see cref="System.Text.Encoding.Unicode"/> encoding with byte-order marks was used.</summary>
        UTF16BOM = 1,
        /// <summary>The <see cref="System.Text.Encoding.BigEndianUnicode"/> encoding was used.</summary>
        UTF16BE = 2,
        /// <summary>The <see cref="System.Text.Encoding.UTF8"/> encoding was used.</summary>
        UTF8 = 3
    }
}