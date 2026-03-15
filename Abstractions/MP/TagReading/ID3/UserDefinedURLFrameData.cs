

using System;

namespace MP.TagReading.ID3
{
    /// <summary>
    /// Defines data for a user-defined text frame.
    /// </summary>
    public readonly struct UserDefinedTextFrameData
    {
        /// <summary>
        /// Gets the description of the frame data.
        /// </summary>
        public readonly string Description;

        /// <summary>
        /// Gets the value of the frame data.
        /// </summary>
        public readonly string Value;

        /// <summary>
        /// Intializes a new instance of the <see cref="UserDefinedTextFrameData"/> structure.
        /// </summary>
        /// <param name="value">The text value of the frame data.</param>
        /// <param name="description">The description of the frame data.</param>
        public UserDefinedTextFrameData(string description, string value)
        {
            ArgumentNullException.ThrowIfNull(value);
            ArgumentNullException.ThrowIfNull(description);

            Value = value;
            Description = description; 
        }
    }
}