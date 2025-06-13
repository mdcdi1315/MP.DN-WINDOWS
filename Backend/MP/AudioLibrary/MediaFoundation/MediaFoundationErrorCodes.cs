

namespace MP.AudioLibrary.MediaFoundation
{
    /// <summary>
    /// Defines several Media Foundation <see cref="ComInterop.HRESULT"/>'s and error codes.
    /// </summary>
    public static class MediaFoundationErrorCodes
    {
        /// <summary>
        /// The requested attribute was not found.
        /// </summary>
        public const System.Int32 MF_E_ATTRIBUTENOTFOUND = unchecked((System.Int32)0xC00D36E6);

        /// <summary>
        /// The buffer was too small to carry out the requested action.
        /// </summary>
        public const System.Int32 MF_E_BUFFERTOOSMALL = unchecked((System.Int32)0xC00D36B1);

        /// <summary>
        /// The caller does not appear to support this transform's asynchronous capabilities.
        /// </summary>
        public const System.Int32 MF_E_TRANSFORM_ASYNC_LOCKED = unchecked((System.Int32)0xC00D6D77);

        /// <summary>
        /// You are calling MFStartup with the wrong MF_VERSION. Mismatched bits?
        /// </summary>
        public const System.Int32 MF_E_BAD_STARTUP_VERSION = unchecked((System.Int32)0xC00D36E3);

        /// <summary>
        /// The transform cannot produce output until it gets more input samples.
        /// </summary>
        public const System.Int32 MF_E_TRANSFORM_NEED_MORE_INPUT = unchecked((System.Int32)0xC00D6D72);

        /// <summary>
        /// A stream change has occurred. Output cannot be produced until the streams have been renegotiated.
        /// </summary>
        public const System.Int32 MF_E_TRANSFORM_STREAM_CHANGE = unchecked((System.Int32)0xC00D6D61);

        /// <summary>
        /// The data specified for the media type is invalid, inconsistent, or not supported by this object.
        /// </summary>
        public const System.Int32 MF_E_INVALIDMEDIATYPE = unchecked((System.Int32)0xC00D36B4);
    }
}