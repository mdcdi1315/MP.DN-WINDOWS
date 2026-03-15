using System;
using System.Diagnostics.CodeAnalysis;

namespace MP.TagReading
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
        /// Returns the main title of the current tag. <br />
        /// Returns <see cref="System.String.Empty"/> if this field is not present.
        /// </summary>
        public System.String Title { get; }

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
        /// Gets a URI or a format string that specifies the image format that the <see cref="Image"/> property contains. <br />
        /// If the <see cref="Image"/> property is effectively <see langword="null"/> , then this property will also be <see langword="null"/>.
        /// </summary>
        public System.String ImageFormat { get; }

        /// <summary>
        /// Gets the publishing company of this track. <br />
        /// Returns <see cref="System.String.Empty"/> if this field is not present.
        /// </summary>
        public System.String Publisher { get; }

        /// <summary>
        /// Gets the company's legal copyrights of this track. <br />
        /// Returns <see cref="System.String.Empty"/> if this field is not present.
        /// </summary>
        public System.String Copyright { get; }

        /// <summary>
        /// Gets the track creation date. <br />
        /// Returns <see cref="System.String.Empty"/> if this field is not present.
        /// </summary>
        public System.String CreationDate { get; }

        /// <summary>
        /// Returns the track cover image , if any. <br />
        /// Returns <see langword="null"/> if it does not contain any cover image.
        /// </summary>
        public IO.DataStream Image { get; }

        /// <summary>
        /// Gets a non-standard property from the current reader. <br />
        /// The name passed to the <paramref name="name"/> parameter is reader-specific. <br />
        /// If the requested property does not exist, this returns <see langword="null"/>.
        /// </summary>
        /// <param name="name">The reader-specific property to retrieve.</param>
        /// <returns>A reader-specific object describing that property.</returns>
        [return: MaybeNull]
        public System.Object GetProperty(System.String name);
    }
}
