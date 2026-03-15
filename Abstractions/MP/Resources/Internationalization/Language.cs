
using System;

namespace MP.Resources.Internationalization
{
    /// <summary>
    /// Provides information about a spokable language.
    /// </summary>
    public sealed class Language
    {
        private readonly string code_name;
        private readonly string friendly_name;

        /// <summary>
        /// Initializes a new instance of the <see cref="Language"/> class.
        /// </summary>
        /// <param name="code_name">The code name for this language.</param>
        /// <param name="friendly_name">A friendly name for this language, described as in the language that the newly created object will represent.</param>
        public Language(string code_name, string friendly_name)
        {
            ArgumentNullException.ThrowIfNull(code_name);
            ArgumentNullException.ThrowIfNull(friendly_name);
            this.code_name = code_name;
            this.friendly_name = friendly_name;
        }

        /// <summary>
        /// Gets the code name for this language.
        /// </summary>
        public String CodeName => code_name;

        /// <summary>
        /// Gets a friendly name for this language.
        /// </summary>
        public String FriendlyName => friendly_name;
    }
}