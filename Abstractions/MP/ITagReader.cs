
using System;

namespace MP
{
    /// <summary>
    /// Provides the ability to read audio tags. <br />
    /// Not all tags have the functionality stated by the <see cref="ITagReader"/> <see langword="interface"/>. <br />
    /// For the properties that are not functioning , prefer to directly throw <see cref="NotSupportedException"/>s , <br />
    /// while for features that have not yet implemented but supported , throw directly a <see cref="NotImplementedException"/> instead. <br />
    /// Note: returning an empty or <see langword="null"/> value on these properties means that the read tag does not have that information <br />
    /// for the read tag , and not that they are unsupported.
    /// </summary>
    public interface ITagReader : IDisposable
    {
        /// <summary>
        /// Returns the website URL of the encoder for the current tag. <br />
        /// Returns <see cref="System.String.Empty"/> if this field is not present.
        /// </summary>
        public System.String WebSiteEncoderUrl { get; }

        /// <summary>
        /// Returns the encoder name of the current tag. <br />
        /// Returns <see cref="System.String.Empty"/> if this field is not present.
        /// </summary>
        public System.String EncodedBy { get; }

        /// <summary>
        /// Returns the album of the current tag. <br />
        /// Returns <see cref="System.String.Empty"/> if this field is not present.
        /// </summary>
        public System.String AlbumName { get; }

        /// <summary>
        /// Returns the first title of the current tag. <br />
        /// Returns <see cref="System.String.Empty"/> if this field is not present.
        /// </summary>
        public System.String Title1 { get; }

        /// <summary>
        /// Returns the second title of the current tag. <br />
        /// This title is the main title of the current track. <br />
        /// Returns <see cref="System.String.Empty"/> if this field is not present.
        /// </summary>
        public System.String Title2 { get; }

        /// <summary>
        /// Returns the disc number that the tag of the current track belongs to. <br />
        /// Returns <see cref="System.String.Empty"/> if this field is not present.
        /// </summary>
        public System.String DiscOrdinal { get; }

        /// <summary>
        /// Returns the track number of the current tag. <br />
        /// Returns <see cref="System.String.Empty"/> if this field is not present.
        /// </summary>
        public System.String TrackNumber { get; }

        /// <summary>
        /// Returns the contributing artists of the current tag. <br />
        /// Returns <see cref="System.String.Empty"/> if this field is not present.
        /// </summary>
        public System.String ContributingArtists { get; }

        /// <summary>
        /// Returns the album's artist of the current tag. <br />
        /// Returns <see cref="System.String.Empty"/> if this field is not present.
        /// </summary>
        public System.String AlbumArtist { get; }

        /// <summary>
        /// Returns any comments of the current tag. <br />
        /// Returns <see cref="System.String.Empty"/> if this field is not present.
        /// </summary>
        public System.String Comments { get; }

        /// <summary>
        /// Returns the subtitle of the current tag. <br />
        /// Returns <see cref="System.String.Empty"/> if this field is not present.
        /// </summary>
        public System.String SubTitle { get; }

        /// <summary>
        /// Returns the publisher URL of the current tag. <br />
        /// Returns <see cref="System.String.Empty"/> if this field is not present.
        /// </summary>
        public System.String PublisherURL { get; }

        /// <summary>
        /// Returns the genre of the current tag. <br />
        /// Returns <see cref="System.String.Empty"/> if this field is not present.
        /// </summary>
        public System.String Genre { get; }

        /// <summary>
        /// Returns the track cover image , if any. <br />
        /// Returns <see langword="null"/> if it does not contain any cover image.
        /// </summary>
        public System.Byte[] Image { get; }
    }
}
