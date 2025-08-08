using System;
using System.Text;
using MP.TagReading.Ogg;
using MP.TagReading.Flac;
using MP.Annotations.CodeAnalysis;

namespace MP.TagReading
{
    /// <summary>
    /// Reads audio tags of the FLAC audio format.
    /// </summary>
    public sealed class FlacAudioTagReader : IOggTagReaderBase
    {
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

        /// <inheritdoc/>
        public string Publisher => comment.GetValue("ORGANIZATION");

        /// <inheritdoc/>
        public string Copyright => comment.GetValue("COPYRIGHT");

        /// <inheritdoc/>
        public string CreationDate => comment.GetValue("DATE");

        /// <inheritdoc/>
        public string WebSiteEncoderUrl => comment.GetValue("CONTACT");

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public string AlbumName => comment.GetValue("ALBUM");

        /// <summary>
        /// This property is not supported and will always throw <see cref="NotSupportedException"/>.
        /// </summary>
        public string Title1 => throw new NotSupportedException("Ogg Comments do not honor or support the Title1 property.");

        /// <inheritdoc/>
        public string Title2 => comment.GetValue("TITLE");

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public string TrackNumber => comment.GetValue("TRACKNUMBER");

        /// <inheritdoc/>
        public string ContributingArtists => comment.ConcatenateMultipleValues("ARTIST");

        /// <inheritdoc/>
        public string AlbumArtist => comment.GetValue("ALBUMARTIST");

        /// <inheritdoc/>
        public string Comments => comment.GetValue("Comments");

        /// <summary>
        /// This property is not supported and will always throw <see cref="NotSupportedException"/>.
        /// </summary>
        public string SubTitle => throw new NotSupportedException("Ogg Comments do not honor or support the SubTitle property.");

        /// <inheritdoc/>
        public string PublisherURL => comment.GetValue("URL");

        /// <inheritdoc/>
        public string Genre => comment.GetValue("GENRE");

        /// <inheritdoc/>
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
        [Throws(
            typeof(ArgumentNullException) ,
            typeof(System.IO.EndOfStreamException) ,
            typeof(System.IO.IOException) ,
            typeof(FormatException)
        )]
        public FlacAudioTagReader(System.IO.Stream stream)
        {
            // The library says that the default image type is the front cover , so setting it in the beginning.
            imgprops = new() { Type = FLACPictureType.FLAC_PICTURE_TYPE_FRONT_COVER, DataLength = 0 };
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
                //System.Console.WriteLine($"Read FLAC header of type {temp.Type} {temp.Length} {temp.IsLast}");
                switch (temp.Type)
                {
                    case MetadataType.FLAC__METADATA_TYPE_PICTURE:
                    case MetadataType.FLAC__METADATA_TYPE_VORBIS_COMMENT:
                        blocks.Add(temp);
                        break;
                    case >= MetadataType.FLAC__MAX_METADATA_TYPE:
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
            if (ArrayEqual(str, new System.Byte[] { 102, 76, 97, 67 }) == false) {
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
                if (blocks[I].Type == MetadataType.FLAC__METADATA_TYPE_VORBIS_COMMENT) {
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
                if (blocks[I].Type == MetadataType.FLAC__METADATA_TYPE_PICTURE) {
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

        /// <summary>
        /// Unreferences data and internal state that this instance holds.
        /// </summary>
        public void Dispose()
        {
            comment = null;
            image = null;
            blocks = null;
            imgprops = null;
        }
    }
}
