using System.Diagnostics.CodeAnalysis;

namespace MP.Resources.Internationalization
{
    /// <summary>
    /// Defines a translation descriptor. <br />
    /// Translation descriptors may define different ways of how a string is formatted and finally returned through the <see cref="GetString"/> method.
    /// </summary>
    public interface ITranslationDescriptor
    {
        /// <summary>
        /// Translates, formats and returns the resulting string.
        /// </summary>
        /// <param name="format_arguments">Additional formatting arguments that may be required.</param>
        /// <returns>The translated and formatted string.</returns>
        [return: NotNull]
        public abstract string GetString([AllowNull] object[] format_arguments);
    }
}