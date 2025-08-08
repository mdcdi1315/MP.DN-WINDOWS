

namespace MP.TagReading.Flac
{
    /// <summary>
    /// Defines different <see cref="MetadataBlock"/> types that can exist in a FLAC stream.
    /// </summary>
    public enum MetadataType : System.Byte
    {
        /// <summary>
        /// Stream information.
        /// </summary>
        FLAC__METADATA_TYPE_STREAMINFO = 0,

        /// <summary>
        /// Padding block. Skip to the number of bytes denoted by the length field.
        /// </summary>
        FLAC__METADATA_TYPE_PADDING = 1,

        /// <summary>
        /// Application-defined block.
        /// </summary>
        FLAC__METADATA_TYPE_APPLICATION = 2,

        /// <summary>
        /// Defines the seektable block.
        /// </summary>
        FLAC__METADATA_TYPE_SEEKTABLE = 3,

        /// <summary>
        /// Defines the vorbis comment block. Here inside is the tag of this stream...
        /// </summary>
        FLAC__METADATA_TYPE_VORBIS_COMMENT = 4,

        /// <summary>
        /// Defines the cue sheet information.
        /// </summary>
        FLAC__METADATA_TYPE_CUESHEET = 5,

        /// <summary>
        /// Defines the cover image.
        /// </summary>
        FLAC__METADATA_TYPE_PICTURE = 6,

        /// <summary>
        /// marker to denote beginning of undefined type range; this number will increase as new metadata types are added
        /// </summary>
        FLAC__METADATA_TYPE_UNDEFINED = 7,

        /// <summary>
        /// Any value larger or equal than this must be not accepted!
        /// </summary>
        FLAC__MAX_METADATA_TYPE = 126,
    }
}