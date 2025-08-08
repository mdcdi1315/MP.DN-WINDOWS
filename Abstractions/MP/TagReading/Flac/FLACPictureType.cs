

namespace MP.TagReading.Flac
{
    /// <summary>
    /// Defines the type of the picture defined inside the FLAC saved data. <br />
    /// The values of this enumeration are extracted from the official FLAC__StreamMetadata_Picture_Type library enumeration.
    /// </summary>
    public enum FLACPictureType : System.Byte
    {
        /// <summary>Other</summary>
        FLAC_PICTURE_TYPE_OTHER = 0,
        /// <summary>32x32 pixels 'file icon' (PNG only)</summary>
        FLAC_PICTURE_TYPE_FILE_ICON_STANDARD = 1,
        /// <summary>Other file icon</summary>
        FLAC_PICTURE_TYPE_FILE_ICON = 2,
        /// <summary>Cover (front)</summary>
        FLAC_PICTURE_TYPE_FRONT_COVER = 3,
        /// <summary>Cover (back)</summary>
        FLAC_PICTURE_TYPE_BACK_COVER = 4,
        /// <summary>Leaflet page</summary>
        FLAC_PICTURE_TYPE_LEAFLET_PAGE = 5,
        /// <summary>Media (e.g. label side of CD)</summary>
        FLAC_PICTURE_TYPE_MEDIA = 6,
        /// <summary>Lead artist/lead performer/soloist</summary>
        FLAC_PICTURE_TYPE_LEAD_ARTIST = 7,
        /// <summary>Artist/performer</summary>
        FLAC_PICTURE_TYPE_ARTIST = 8,
        /// <summary>Conductor</summary>
        FLAC_PICTURE_TYPE_CONDUCTOR = 9,
        /// <summary>Band/Orchestra</summary>
        FLAC_PICTURE_TYPE_BAND = 10,
        /// <summary>Composer</summary>
        FLAC_PICTURE_TYPE_COMPOSER = 11,
        /// <summary>Lyricist/text writer</summary>
        FLAC_PICTURE_TYPE_LYRICIST = 12, 
        /// <summary>Recording Location</summary>
        FLAC_PICTURE_TYPE_RECORDING_LOCATION = 13,
        /// <summary>During recording</summary>
        FLAC_PICTURE_TYPE_DURING_RECORDING = 14, 
        /// <summary>During performance</summary>
        FLAC_PICTURE_TYPE_DURING_PERFORMANCE = 15,
        /// <summary>Movie/video screen capture</summary>
        FLAC_PICTURE_TYPE_VIDEO_SCREEN_CAPTURE = 16,
        /// <summary>A bright coloured fish</summary>
        FLAC_PICTURE_TYPE_FISH = 17,
        /// <summary>Illustration</summary>
        FLAC_PICTURE_TYPE_ILLUSTRATION = 18,
        /// <summary>Band/artist logotype</summary>
        FLAC_PICTURE_TYPE_BAND_LOGOTYPE = 19,
        /// <summary>Publisher/Studio logotype</summary>
        FLAC_PICTURE_TYPE_PUBLISHER_LOGOTYPE = 20,
        /// <summary>Undefined</summary>
        FLAC_PICTURE_TYPE_UNDEFINED
    }

}