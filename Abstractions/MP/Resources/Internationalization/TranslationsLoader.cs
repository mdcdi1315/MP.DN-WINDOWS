
using System;
using MP.Utilities;
using MP.Annotations.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;

namespace MP.Resources.Internationalization
{
    /// <summary>
    /// Defines an abstraction for how to read and get translation strings.
    /// </summary>
    public abstract class TranslationsLoader
    {
        private sealed class UnknownTranslationDescriptor : ITranslationDescriptor
        {
            private readonly string translation;
            private readonly string translation_not_found_format;

            public UnknownTranslationDescriptor(string translation, string translation_not_found_format)
            {
                this.translation = translation;
                this.translation_not_found_format = translation_not_found_format;
            }

            public string GetString([AllowNull] object[] format_arguments) => String.Format(translation_not_found_format, translation);
        }

        /// <summary>
        /// Loads translation data from the specified data stream.
        /// </summary>
        /// <param name="stream">The data stream containing encoded translation data.</param>
        [Throws(typeof(ArgumentNullException))]
        public abstract void LoadTranslations(System.IO.Stream stream);

        /// <summary>
        /// Gets the number of translations loaded.
        /// </summary>
        public abstract int TranslationsCount { get; }

        /// <summary>
        /// Gets a string that is used if a translation was not found. <br />
        /// It must be a composite format string containing a single format argument, that is filled by the loader.
        /// </summary>
        [NotNull]
        public abstract string UnknownTranslationStringFormat { get; }

        /// <summary>
        /// Gets a translation with the specified name.
        /// </summary>
        /// <param name="name">The ID of the translation to retrieve.</param>
        /// <returns>An instance of the <see cref="ITranslationDescriptor"/> interface, describing a loaded translation.</returns>
        /// <exception cref="ArgumentException"><paramref name="name"/> represents the empty string (&quot;&quot;).</exception>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
        [return: NotNull]
        [Throws(typeof(ArgumentNullException), typeof(ArgumentException))]
        public ITranslationDescriptor GetTranslation(string name)
        {
            ArgumentException.ThrowIfNullOrEmpty(name);

            Optional<ITranslationDescriptor> optional = GetTranslationImpl(name);

            if (optional.HasValue) {
                return optional.Get();
            } else {
                return new UnknownTranslationDescriptor(name, UnknownTranslationStringFormat);
            }
        }

        /// <summary>
        /// Defines the actual implementation for the <see cref="GetTranslation(string)"/> method.
        /// </summary>
        /// <param name="name">The name of the translation to return a descriptor for.</param>
        /// <returns>
        /// The translation implementation for the specified translation ID. <br />
        /// If the translation was not found for any particular reason, this method should return the value of the <see cref="Optional{ITranslationDescriptor}.Empty"/> method.
        /// </returns>
        protected abstract Optional<ITranslationDescriptor> GetTranslationImpl([DisallowNull] string name);
    }
}