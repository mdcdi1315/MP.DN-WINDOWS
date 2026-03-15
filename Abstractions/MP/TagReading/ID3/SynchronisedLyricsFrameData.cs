
using System;

namespace MP.TagReading.ID3
{
    /// <summary>
    /// Defines data for a syncronised lyrics data frame.
    /// </summary>
    public readonly struct SynchronisedLyricsFrameData
    {
        /// <summary>
        /// Syncronised lyrics content type value. <br />
        /// Values are taken as described in the <see href="https://id3.org/id3v2.4.0-frames"/> document.
        /// </summary>
        public enum ContentType : System.Byte
        {
            /// <summary>Other</summary>
            Other = 0,
            /// <summary>Lyrics</summary>
            Lyrics = 128,
            /// <summary>Text transcription</summary>
            TextTranscription = 64,
            /// <summary>Movement/part name (e.g. "Adagio")</summary>
            Movement_PartName = 192,
            /// <summary>Events (e.g. "Don Quijote enters the stage")</summary>
            Events = 32,
            /// <summary>Chord (e.g. "Bb F Fsus")</summary>
            Chord = 160,
            /// <summary>Trivia/'pop up' information</summary>
            Trivia_PopUpInfo = 96,
            /// <summary>URLs to webpages</summary>
            WebpagesURL = 224,
            /// <summary>URLs to images</summary>
            ImagesURL = 16
        }

        /// <summary>
        /// Provides the definition of a single syllable of a syncronised lyrics frame.
        /// </summary>
        public readonly struct Syllable
        {
            /// <summary>
            /// The text contained in the current syllable.
            /// </summary>
            public readonly string Text;

            /// <summary>
            /// The time stamp contained in the current syllable.
            /// </summary>
            public readonly int TimeStamp;

            /// <summary>
            /// Constructs a new instance of the <see cref="Syllable"/> structure.
            /// </summary>
            /// <param name="text">The text associated with this syllable</param>
            /// <param name="timeStamp">The timestamp of this syllable, expressed as <see cref="TimeStampFormat"/>.</param>
            public Syllable(string text, int timeStamp)
            {
                Text = text ?? string.Empty;
                TimeStamp = timeStamp;
            }
        }

        /// <summary>
        /// Gets the language that the lyrics are written into.
        /// </summary>
        public readonly string Language;

        /// <summary>
        /// Gets the type of the content stored in the current frame.
        /// </summary>
        public readonly ContentType Type;

        /// <summary>
        /// Gets the content descriptor for this lyrics frame.
        /// </summary>
        public readonly string ContentDescriptor;

        /// <summary>
        /// For each syllable, this contains the time stamp format for the current frame.
        /// </summary>
        public readonly ID3V2TimeStampFormat TimeStampFormat;

        /// <summary>
        /// Gets all the syllables that are part of this frame.
        /// </summary>
        public readonly Syllable[] Syllables;

        /// <summary>
        /// Intializes a new instance of the <see cref="SynchronisedLyricsFrameData"/> structure.
        /// </summary>
        /// <param name="type">The content type of the frame data.</param>
        /// <param name="syllables">The syllables comprising the frame.</param>
        /// <param name="cd">The content descriptor of the frame data.</param>
        /// <param name="language">The lyrics language of the frame data.</param>
        /// <param name="tsf">The time stamp format used for the syllables.</param>
        public SynchronisedLyricsFrameData(string language, string cd, ContentType type, ID3V2TimeStampFormat tsf, Syllable[] syllables)
        {
            ArgumentNullException.ThrowIfNull(cd);
            ArgumentNullException.ThrowIfNull(syllables);
            ArgumentNullException.ThrowIfNull(language);

            Type = type;
            Syllables = syllables;
            Language = language;
            TimeStampFormat = tsf;
            ContentDescriptor = cd;
        }
    }
}