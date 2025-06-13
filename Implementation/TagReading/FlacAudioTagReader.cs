using System;
using System.Text;
using MP.TagReading.Ogg;

namespace MP.TagReading
{
    /*
     #r "MusicPlayer.dll"
     using Microsoft.IO;
     using MP.TagReading;
     FileStream FS = new("E:\\brom.flac", FileMode.Open);
     FlacAudioTagReader FR = new(FS);
    */

    /// <summary>
    /// Reads audio tags of the FLAC audio format.
    /// </summary>
    public sealed class FlacAudioTagReader : IOggTagReaderBase
    {
        // Represents a single FLAC metadata block.
        private struct MetadataBlock
        {
            public static MetadataBlock ReadFromBytes(byte[] data)
            {
                MetadataBlock block = new();
                if (data is null) { return block; }
                if (data.Length < MetadataHeaderBytes) { return block; }
                // The data in the first byte do contain the Type and IsLast members...
                byte temp = 0;
                int I = 0;
                bool bit;
                for (; I < TypeMemberBits; I++)
                {
                    bit = data[I >> 3].GetBit(I % 8);
                    if (bit) { temp.SetBit(I % 8, true); }
                }
                block.Type = (MetadataType)temp;
                block.IsLast = data[I >> 3].GetBit(I);
                I += IsLastMemberBits;
                int start = I;
                byte[] lentemp = new byte[sizeof(uint)];
                for (; I < start + LengthMemberBits; I++)
                {
                    bit = data[I >> 3].GetBit(I % 8);
                    if (bit) { lentemp[I >> 3].SetBit(I % 8, true); }
                }
                if (BitConverter.IsLittleEndian) { lentemp.Reverse(); }
                block.Length = lentemp.ToUInt32(0);
                return block;
            }

            public static MetadataBlock ReadFromStream(System.IO.Stream stream)
            {
                byte[] data = new byte[MetadataHeaderBytes];
                if (stream.Read(data, 0, MetadataHeaderBytes) < MetadataHeaderBytes) 
                { throw new System.IO.IOException("Could not read 4 bytes from the stream."); }
                return ReadFromBytes(data);
            }

            public enum MetadataType : byte
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

            public MetadataType Type;
            public bool IsLast;
            public uint Length;
            // This member does not exist in the native structure.
            // It is just used to track the stream offset where the data begin.
            public long StreamOffset;
        }

        // The unfortunate case here is that all the header is encoded to 4 bytes , 
        // and all members are encoded to a bit level.
        private const int MetadataHeaderBytes = 4;
        private const int TypeMemberBits = 7;
        private const int IsLastMemberBits = 1;
        private const int LengthMemberBits = 24;

        // Keeps the raw image bytes.
        private byte[] image;
        // The metadata blocks that exist in the current stream. We only need 2 so the maximum number 
        // of entries will be always 2.
        private MetadataBlock[] blocks;
        // FLAC files write all the string data as Vorbis comments , so just use the preexisting class for that
        private OggVorbisComment comment;
        // Additional properties that the FLAC file format saves for the cover image , such as it's type , mimetype ,description and goes on.
        private FLACImageProperties imgprops;

        /// <summary>
        /// Forwards the <see cref="FLACImageProperties.MimeType"/> property. <br />
        /// If an image was not found , returns the empty string.
        /// </summary>
        public string ImageFormat
        {
            get {
                if (imgprops is null) { return string.Empty; }
                return imgprops.MimeType;
            }
        }

        public string Publisher => comment.GetValue("ORGANIZATION");

        public string Copyright => comment.GetValue("COPYRIGHT");

        public string CreationDate => comment.GetValue("DATE");

        public string WebSiteEncoderUrl => comment.GetValue("CONTACT");

        public string EncodedBy
        {
            get
            {
                string final = comment.GetValue("ENCODER");
                if (string.IsNullOrEmpty(final))
                {
                    final = comment.GetValue("EncodedBy");
                }
                return final;
            }
        }

        public string AlbumName => comment.GetValue("ALBUM");

        public string Title1 => throw new NotSupportedException("Ogg Comments do not honor or support the Title1 property.");

        public string Title2 => comment.GetValue("TITLE");

        public string DiscOrdinal
        {
            get
            {
                // The disc ordinal can be found either simply as DISC or even DISCNUMBER.
                string final = comment.GetValue("DISC");
                if (string.IsNullOrEmpty(final))
                {
                    final = comment.GetValue("DISCNUMBER");
                }
                return final;
            }
        }

        public string TrackNumber => comment.GetValue("TRACKNUMBER");

        public string ContributingArtists => comment.ConcatenateMultipleValues("ARTIST");

        public string AlbumArtist => comment.GetValue("ALBUMARTIST");

        public string Comments => comment.GetValue("Comments");

        public string SubTitle => throw new NotSupportedException("Ogg Comments do not honor or support the SubTitle property.");

        public string PublisherURL => comment.GetValue("URL");

        public string Genre => comment.GetValue("GENRE");

        public byte[] Image => image;

        /// <summary>
        /// [FLAC Reader Specific] Retrieves the image properties of the current image if any.
        /// </summary>
        public FLACImageProperties ImageProperties => imgprops;

        /// <summary>
        /// [FLAC Reader Specific] Gets the FLAC library vendor. <br />
        /// An example is : reference libFLAC 1.2.1 20070917 <br />
        /// On some rare cases , this string might also be empty.
        /// </summary>
        public string FLACLibraryVendor => comment.Vendor;

        /// <summary>
        /// Forwards the <see cref="FLACLibraryVendor"/> property.
        /// </summary>
        public string Vendor => comment.Vendor;

        /// <summary>
        /// Creates a new instance of the <see cref="FlacAudioTagReader"/> class from the specified stream that contains the data to read.
        /// </summary>
        /// <param name="stream">The FLAC stream data to read.</param>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> was <see langword="null"/>.</exception>
        /// <exception cref="System.IO.EndOfStreamException">This exception is thrown at many cases. See the thrown exception message for more info.</exception>
        /// <exception cref="System.IO.IOException">This exception is thrown at many cases. See the thrown exception message for more info.</exception>
        /// <exception cref="FormatException">Fails when the stream given fails to conform with the FLAC format.</exception>
        public FlacAudioTagReader(System.IO.Stream stream)
        {
            // The library says that the default image type is the front cover , so setting it in the beginning.
            imgprops = new() { Type = FLACPictureType.FLAC_PICTURE_TYPE_FRONT_COVER , DataLength = 0 };
            comment = null;
            if (stream is null) { throw new ArgumentNullException("stream"); }
            VerifyHeader(stream);
            ReadTagData(stream);
            ReadImageData(stream);
            // Metadata blocks are not required anymore - delete them.
            blocks = null;
        }

        // Gets the required metadata blocks.
        private static MetadataBlock[] GetTagBlocks(System.IO.Stream stream)
        {
            // We have read the FLAC bytes header , read all metadata blocks.
            MetadataBlock temp;
            System.Collections.Generic.List<MetadataBlock> blocks = new();
            while (stream.Position < stream.Length) 
            {
                temp = MetadataBlock.ReadFromStream(stream);
                temp.StreamOffset = stream.Position;
                switch (temp.Type)
                {
                    case MetadataBlock.MetadataType.FLAC__METADATA_TYPE_PICTURE:
                    case MetadataBlock.MetadataType.FLAC__METADATA_TYPE_VORBIS_COMMENT:
                        blocks.Add(temp);
                        break;
                    case >= MetadataBlock.MetadataType.FLAC__MAX_METADATA_TYPE:
                        throw new FormatException("The stream does contain invalid data.");
                }
                stream.Position += temp.Length;
                // Last metadata block was read , exit and throw exception if applicable.
                if (temp.IsLast) { break; }
            }
            if (blocks.Count > 0) { return blocks.ToArray(); }
            throw new FormatException("No tag data exist in this reader.");
        }

        private void VerifyHeader(System.IO.Stream str)
        {
            // The FLAC audio header starts exactly with 'fLaC' (without quotes plz) so verify first if we have such a stream.
            if (ArrayEqual(str, [102, 76, 97, 67]) == false) {
                throw new FormatException("This is not a FLAC stream. The magic value retrieved is incorrect.");
            }
            blocks = GetTagBlocks(str);
            // We have done by reading any required blocks.
        }

        private void ReadTagData(System.IO.Stream stream)
        {
            // OK. The contained array must contain the tag metadata block.
            // Then , apply the stream offset.
            bool found = false;
            MetadataBlock blk = default;
            for (int I = 0; I < blocks.Length; I++)
            {
                blk = blocks[I];
                if (blocks[I].Type == MetadataBlock.MetadataType.FLAC__METADATA_TYPE_VORBIS_COMMENT) {
                    found = true;
                    break;
                }
            }
            if (found == false) { return; }
            // Read Vorbis comment
            comment = new(stream , blk.StreamOffset , blk.Length);
            // OK. The tag data were read.
        }

        private void ReadImageData(System.IO.Stream stream)
        {
            // OK. The contained array must contain the image metadata block.
            // Then , apply the stream offset.
            bool found = false;
            for (int I = 0; I < blocks.Length; I++)
            {
                if (blocks[I].Type == MetadataBlock.MetadataType.FLAC__METADATA_TYPE_PICTURE) {
                    stream.Position = blocks[I].StreamOffset;
                    found = true;
                    break;
                }
            }
            // If no image data exist , exit.
            if (found == false) {
                // Set image properties to null.
                imgprops = null;
                return; 
            }
            int slotsize;
            byte[] temp;
            // Normally ,someone would have expected to marshal the actual image structure , but the library itself
            // uses it's own rules to save image data.
            // The first four bytes define the picture type.
            // The four next is the picture mime type size to read.
            // Here these bytes are architecture-dependent so we will let the ReadInt32 to decide what is best.
            imgprops.Type = (FLACPictureType)ReadInt32(stream); // Read picture type
            slotsize = ReadInt32(stream); // Give image mime type size.
            temp = new byte[slotsize]; // Initialise array and read data
            if (stream.Read(temp, 0, slotsize) < slotsize)
            { // Classicly fail with exception at this point , but only for mimetype we can say that this is true.
              // For all other fields , 0 means for them that are just unused.
                throw new System.IO.EndOfStreamException($"Requested {slotsize} bytes but those were not available. End of stream was considered.");
            }
            imgprops.MimeType = Encoding.UTF8.GetString(temp); // Get the mimetype.
            // Next step is to read all optional fields.
            // All are int32's with the notable exception of two , which are the description string and the data length.
            slotsize = ReadInt32(stream); // Read optional string size
            temp = new byte[slotsize]; // Initialise array and read data
            stream.Read(temp, 0, slotsize); // We cannot perform checks since this is optional
            imgprops.Description = Encoding.UTF8.GetString(temp); // Get the description string.
            // Read repeatedly all the int32's and data length and save them appropriately.
            imgprops.Width = ReadInt32(stream);
            imgprops.Height = ReadInt32(stream);
            imgprops.Depth = ReadInt32(stream);
            imgprops.Colors = ReadInt32(stream);
            // Check whether the data length has been correctly retrieved
            if ((imgprops.DataLength = ReadUInt32(stream)) == 0) {
                throw new System.IO.EndOfStreamException("Expected to have a value , but no value could be read out.");
            }
            // Now read our image...
            image = new byte[imgprops.DataLength];
            if ((slotsize = stream.Read(image , 0 , image.Length)) < imgprops.DataLength) {
                throw new System.IO.IOException($"Expected to read {imgprops.DataLength} bytes while read {slotsize} bytes.");
            }
            // and we are good to go...
        }

        // Reads a 32-bit signed integer from the stream.
        private static int ReadInt32(System.IO.Stream stream , bool reverseiflendian = true) 
        {
            byte[] bytes = new byte[sizeof(int)];
            if (stream.Read(bytes, 0, sizeof(int)) < sizeof(int)) { throw new System.IO.EndOfStreamException("Cannot read the next required integer as the implementation requires. Method failed unexpectedly."); }
            if (BitConverter.IsLittleEndian && reverseiflendian) { Array.Reverse(bytes); }
            return bytes.ToInt32(0);
        }

        // Reads a 32-bit unsigned integer from the stream.
        private static uint ReadUInt32(System.IO.Stream stream, bool reverseiflendian = true)
        {
            byte[] bytes = new byte[sizeof(uint)];
            if (stream.Read(bytes, 0, sizeof(uint)) < sizeof(uint)) { throw new System.IO.EndOfStreamException("Cannot read the next required integer as the implementation requires. Method failed unexpectedly."); }
            if (BitConverter.IsLittleEndian && reverseiflendian) { Array.Reverse(bytes); }
            return bytes.ToUInt32(0);
        }

        private static bool ArrayEqual(System.IO.Stream str , byte[] bytes)
        {
            int len = bytes.Length;
            // we need to retrieve len bytes at first place.
            byte[] temp = new byte[len];
            if (str.Read(temp, 0, len) != len) // we did not read len bytes , do not fail with exception , but return false though.
            {
                return false;
            }
            // Now compare the two arrays and get results.
            bool match = true;
            for (int I = 0; I < len; I++) 
            {
                if (temp[I] != bytes[I]) { match = false; break; }
            }
            temp = null; // Dispose temp
            return match;
        }

        public void Dispose()
        {
            comment = null;
            image = null;
            blocks = null;
            imgprops = null;
        }
    }

    /// <summary>
    /// Defines the type of the picture defined inside the FLAC saved data. <br />
    /// The values of this enumeration are extracted from the official FLAC__StreamMetadata_Picture_Type library enumeration.
    /// </summary>
    public enum FLACPictureType : byte {
        FLAC_PICTURE_TYPE_OTHER = 0, /* Other */
        FLAC_PICTURE_TYPE_FILE_ICON_STANDARD = 1, /* 32x32 pixels 'file icon' (PNG only) */
        FLAC_PICTURE_TYPE_FILE_ICON = 2, /* Other file icon */
        FLAC_PICTURE_TYPE_FRONT_COVER = 3, /* Cover (front) */
        FLAC_PICTURE_TYPE_BACK_COVER = 4, /* Cover (back) */
        FLAC_PICTURE_TYPE_LEAFLET_PAGE = 5, /* Leaflet page */
        FLAC_PICTURE_TYPE_MEDIA = 6, /* Media (e.g. label side of CD) */
        FLAC_PICTURE_TYPE_LEAD_ARTIST = 7, /* Lead artist/lead performer/soloist */
        FLAC_PICTURE_TYPE_ARTIST = 8, /* Artist/performer */
        FLAC_PICTURE_TYPE_CONDUCTOR = 9, /* Conductor */
        FLAC_PICTURE_TYPE_BAND = 10, /* Band/Orchestra */
        FLAC_PICTURE_TYPE_COMPOSER = 11, /* Composer */
        FLAC_PICTURE_TYPE_LYRICIST = 12, /* Lyricist/text writer */
        FLAC_PICTURE_TYPE_RECORDING_LOCATION = 13, /* Recording Location */
        FLAC_PICTURE_TYPE_DURING_RECORDING = 14, /* During recording */
        FLAC_PICTURE_TYPE_DURING_PERFORMANCE = 15, /* During performance */
        FLAC_PICTURE_TYPE_VIDEO_SCREEN_CAPTURE = 16, /* Movie/video screen capture */
        FLAC_PICTURE_TYPE_FISH = 17, /* A bright coloured fish */
        FLAC_PICTURE_TYPE_ILLUSTRATION = 18, /* Illustration */
        FLAC_PICTURE_TYPE_BAND_LOGOTYPE = 19, /* Band/artist logotype */
        FLAC_PICTURE_TYPE_PUBLISHER_LOGOTYPE = 20, /* Publisher/Studio logotype */
        FLAC_PICTURE_TYPE_UNDEFINED
    }

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
