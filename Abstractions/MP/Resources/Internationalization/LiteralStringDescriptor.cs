
using System;
using System.Diagnostics.CodeAnalysis;

namespace MP.Resources.Internationalization
{
    /// <summary>
    /// Provides a simple translation descriptor that is a string with no formatting arguments.
    /// </summary>
    public sealed class LiteralStringDescriptor : ITranslationDescriptor
    {
        private readonly string text;

        /// <summary>
        /// Initializes a new instance of the <see cref="LiteralStringDescriptor"/> class.
        /// </summary>
        /// <param name="text">The translated literal text to be returned through the <see cref="GetString(object[])"/> method.</param>
        /// <exception cref="ArgumentNullException"><paramref name="text"/> is <see langword="null"/>.</exception>
        public LiteralStringDescriptor(string text)
        {
            ArgumentNullException.ThrowIfNull(text);
            this.text = text;
        }

        /// <inheritdoc />
        [return: NotNull]
        public string GetString([AllowNull] object[] format_arguments) => text;
    }
}