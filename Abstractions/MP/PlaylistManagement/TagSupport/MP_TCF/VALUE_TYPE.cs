


namespace MP.PlaylistManagement.TagSupport.MP_TCF
{
    internal enum VALUE_TYPE : ushort
    {
        /// <summary>
        /// Value is a <see cref="System.String"/>. <br />
        /// The next byte is a <see cref="STRING_ENCODING"/> value indicating the encoding used to encode the string value itself. <br />
        /// The above encoding value IS NOT stored to the <see cref="ENTRY.ValueLength"/> field.
        /// </summary>
        String = 0,
        /// <summary>
        /// Value is a <see cref="System.Byte"/>[].
        /// </summary>
        FixedByteArray = 1,
        /// <summary>
        /// Value is a <see cref="TagReading.ID3.CommentFrameData"/>. <br />
        /// The value additionally contains a <see cref="ID3V2CMTBLOCK"/> structure containing some extra information needed to decode the comment.
        /// </summary>
        ID3V2_COMMENT_BLOCK = 2
    }
}