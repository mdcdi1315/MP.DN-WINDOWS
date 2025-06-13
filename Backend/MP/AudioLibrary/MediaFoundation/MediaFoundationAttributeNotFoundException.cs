


using System;

namespace MP.AudioLibrary.MediaFoundation
{
    public sealed class MediaFoundationAttributeNotFoundException : MediaFoundationException
    {
        private Guid guidkey;

        /// <summary>
        /// Creates a new <see cref="MediaFoundationAttributeNotFoundException"/> class instance
        /// with the specified GUID key that was not found. Primarily thrown by the extensions of the <see cref="IMFAttributes"/> interface.
        /// </summary>
        /// <param name="attributekey">The GUID key that was not found in an instance of the <see cref="IMFAttributes"/> interface.</param>
        public MediaFoundationAttributeNotFoundException(Guid attributekey) : base($"The requested attribute was not found.\nAttribute Key GUID: {attributekey}") 
        {
            guidkey = attributekey;
        }

        /// <summary>
        /// Gets the key that was not found.
        /// </summary>
        public Guid Key => guidkey;
    }
}