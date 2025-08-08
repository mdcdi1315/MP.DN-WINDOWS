

namespace MP.TagReading.Flac
{
    /// <summary>
    /// This class is defined as the .NET structure for FLAC__StreamMetadata_Picture native structure. <br />
    /// It does exactly correspond to that one.
    /// </summary>
    // Be noted that the official library defines these numeric properties as System.UInt32 ones , but these can never reach too high
    // because they are image properties; i.e. I have never seen in my life an image whose size is 500000x500000 pixels.
    // Only maybe the data length might be a large number which requires UInt32 but even in such case very hardly you will see such case;
    // Generaly , the library authors as it seems were overrated the situation (and it is a good act I do not blame it) but for this reader Int32's are more than enough.
    public sealed class FLACImageProperties
    {
        private FLACPictureType type;
        private string mimetype, desc;
        private int wd, ht, colors, depth;

        /// <summary>
        /// Creates a new instance of <see cref="FLACImageProperties"/> class.
        /// </summary>
        internal FLACImageProperties() { }

        /// <summary>
        /// The FLAC picture type that this image belongs to.
        /// </summary>
        public FLACPictureType Type { get => type; internal set => type = value; }

        /// <summary>
        /// The picture mimetype. Can be one of image/jpeg or image/png.
        /// </summary>
        public string MimeType { get => mimetype; internal set => mimetype = value; }

        /// <summary>
        /// The picture description. This is an optional field and it can be empty.
        /// </summary>
        public string Description { get => desc; internal set => desc = value; }

        /// <summary>
        /// The picture width. This is an optional field and it can be zero.
        /// </summary>
        public int Width { get => wd; internal set => wd = value; }

        /// <summary>
        /// The picture height. This is an optional field and it can be zero.
        /// </summary>
        public int Height { get => ht; internal set => ht = value; }

        /// <summary>
        /// The picture depth. This is an optional field and it can be zero.
        /// </summary>
        public int Depth { get => depth; internal set => depth = value; }

        /// <summary>
        /// The picture number of colors. This is an optional field and it can be zero.
        /// </summary>
        public int Colors { get => colors; internal set => colors = value; }

        /// <summary>
        /// Internal field to be kept by the reader before registering the slot itself.
        /// </summary>
        internal uint DataLength;
    }
}