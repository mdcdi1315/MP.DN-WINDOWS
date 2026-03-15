

using System;
using System.Diagnostics.CodeAnalysis;

namespace MP.Resources.Internationalization
{
    /// <summary>
    /// Provides a translation descriptor that is a string with formatting arguments parsed with the help of <see cref="String.Format(string, object?)"/> method.
    /// </summary>
    public sealed class DotNetFormattedStringDescriptor : ITranslationDescriptor
    {
        private readonly string format;

        /// <summary>
        /// Initializes a new instance of the <see cref="DotNetFormattedStringDescriptor"/> class from the translated formatted string.
        /// </summary>
        /// <param name="format">The format string to use.</param>
        public DotNetFormattedStringDescriptor(string format)
        {
            ArgumentNullException.ThrowIfNull(format);
            this.format = format;
        }

        /// <inheritdoc />
        [return: NotNull]
        public string GetString([AllowNull] object[] format_arguments) => String.Format(format, format_arguments ?? Array.Empty<Object>());
    }
}