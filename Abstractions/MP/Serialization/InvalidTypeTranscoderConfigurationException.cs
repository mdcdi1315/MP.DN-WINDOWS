using System.Diagnostics.CodeAnalysis;

namespace MP.Serialization
{
    /// <summary>
    /// Thrown on when a registered <see cref="ITypeTranscoder"/> has defined invalid type resolving semantics, 
    /// such as when the transcoder has defined one of the type flags.
    /// </summary>
    public sealed class InvalidTypeTranscoderConfigurationException : SerializationException
    {
        private ITypeTranscoder thetranscoder;

        /// <summary>
        /// Creates a default instance of the <see cref="InvalidTypeTranscoderConfigurationException"/> class.
        /// </summary>
        public InvalidTypeTranscoderConfigurationException() : this("The specified type transcoder has been misconfigured.") { }

        /// <summary>
        /// Creates a new instance of the <see cref="InvalidTypeTranscoderConfigurationException"/>, specifying a message that represents the exact misconfiguration.
        /// </summary>
        /// <param name="message">The exact misconfiguration error.</param>
        public InvalidTypeTranscoderConfigurationException(System.String message) : base(message) => thetranscoder = null;

        /// <summary>
        /// Creates a new instance of the <see cref="InvalidTypeTranscoderConfigurationException"/>, specifying a message that represents the exact misconfiguration,
        /// and the transcoder that triggered this exception.
        /// </summary>
        /// <param name="message">The exact misconfiguration error.</param>
        /// <param name="failingtranscoder">Optional. The <see cref="ITypeTranscoder"/> instance causing this exception to be created.</param>
        public InvalidTypeTranscoderConfigurationException(System.String message, [AllowNull] ITypeTranscoder failingtranscoder) : base(message) => thetranscoder = failingtranscoder;

        /// <summary>
        /// The type transcoder which was misconfigured. May be not always available, so check for null first.
        /// </summary>
        [MaybeNull]
        public ITypeTranscoder Transcoder => thetranscoder;
    }
}
