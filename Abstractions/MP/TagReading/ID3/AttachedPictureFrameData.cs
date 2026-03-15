
using System;

namespace MP.TagReading.ID3
{
    /// <summary>
    /// Provides the data for an ID3 V2 attached picture frame.
    /// </summary>
    public readonly struct AttachedPictureFrameData : IDisposable
    {
        /// <summary>
        /// Defines constants on the type of picture this frame contains.
        /// </summary>
        public enum PictureType : System.Byte
        {
            /// <summary>Other</summary>
            Other = 0,
            /// <summary>32x32 pixels 'file icon' (PNG only)</summary>
            FileIcon = 1,
            /// <summary>Other file icon</summary>
            OtherFileIcon = 2,
            /// <summary>Cover (front)</summary>
            FrontCover = 3,
            /// <summary>Cover (back)</summary>
            BackCover = 4,
            /// <summary>Leaflet page</summary>
            LeafletPage = 5,
            /// <summary>Media (e.g. label side of CD)</summary>
            Media = 6,
            /// <summary>Lead artist/lead performer/soloist</summary>
            LeadArtist = 7,
            /// <summary>Artist/performer</summary>
            Artist = 8,
            /// <summary>Conductor</summary>
            Conductor = 9,
            /// <summary>Band/Orchestra</summary>
            Band = 10,
            /// <summary>Composer</summary>
            Composer = 11,
            /// <summary>Lyricist/text writer</summary>
            Lyricist = 12,
            /// <summary>Recording Location</summary>
            RecordingLocation = 13,
            /// <summary>During recording</summary>
            DuringRecord = 14,
            /// <summary>During performance</summary>
            DuringPerformance = 15,
            /// <summary>Movie/video screen capture</summary>
            MovieCapture = 16,
            /// <summary>A bright coloured fish</summary>
            Fish = 17,
            /// <summary>Illustration</summary>
            Illustration = 18,
            /// <summary>Band/artist logotype</summary>
            BandLogoType = 19,
            /// <summary>Publisher/Studio logotype</summary>
            StudioLogoType = 20,
        }

        /// <summary>
        /// Gets the type of picture this frame contains.
        /// </summary>
        public readonly PictureType Type;

        /// <summary>
        /// Gets the MIME type for this attached picture frame. <br />
        /// If the MIME type is '-->', then the image data do contain a URL to the image.
        /// </summary>
        public readonly string MimeType;

        /// <summary>
        /// Gets a description for this attached picture frame.
        /// </summary>
        public readonly string Description;

        /// <summary>
        /// Gets the picture data as a data stream.
        /// </summary>
        public readonly IO.DataStream PictureData;

        /// <summary>
        /// Disposes this <see cref="AttachedPictureFrameData"/> instance.
        /// </summary>
        public void Dispose() => PictureData.Dispose();

        /// <summary>
        /// Intializes a new instance of the <see cref="AttachedPictureFrameData"/> structure.
        /// </summary>
        /// <param name="type">The type of picture the attached picture frame contains.</param>
        /// <param name="mime_type">The MIME type for the attached picture frame.</param>
        /// <param name="description">A description for the attached picture frame.</param>
        /// <param name="pict_data">The picture data as a data stream. The data stream must be at least readable.</param>
        public AttachedPictureFrameData(
            PictureType type,
            string mime_type,
            string description,
            IO.DataStream pict_data
        )
        {
            ArgumentNullException.ThrowIfNull(type);
            ArgumentNullException.ThrowIfNull(pict_data);
            ArgumentNullException.ThrowIfNull(mime_type);
            ArgumentNullException.ThrowIfNull(description);
            
            Type = type;
            MimeType = mime_type;
            PictureData = pict_data;
            Description = description;
        }
    }
}