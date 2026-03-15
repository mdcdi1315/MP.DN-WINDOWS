

using System;

namespace MP.CGISettings
{
    /// <summary>
    /// Defines the low-level abstraction of a CGI Settings Writer.
    /// </summary>
    /// <typeparam name="T">A more derived type of the <see cref="SettingEntry"/> class, if you deem it necessary by your implementing class.</typeparam>
    public interface ICGISettingsWriter<T> : IDisposable
        where T : SettingEntry
    {
        /// <summary>
        /// Gets or sets the name of the application that saves the resulting CGI settings data. <br />
        /// You may not set anything to this property.
        /// </summary>
        public System.String ApplicationName { get; set; }

        /// <summary>
        /// Gets or sets the encoding under which all strings involved will be saved as. <br />
        /// Should be able to be changed after the first successfull <see cref="Add"/> call happens.
        /// </summary>
        public System.Text.Encoding StringsEncoding { get; set; }

        /// <summary>
        /// Registers a CGI settings extension to be used with this instance.
        /// </summary>
        /// <param name="extension">The CGI settings extension to register.</param>
        /// <exception cref="ArgumentNullException"><paramref name="extension"/> was <see langword="null"/>.</exception>
        public void RegisterExtension(ICGISettingExtension extension);

        /// <summary>
        /// Adds the specified setting to the list of settings to be written.
        /// </summary>
        /// <param name="value">The setting to add.</param>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> was <see langword="null"/>.</exception>
        public void Add(T value);

        /// <summary>
        /// Causes all the CGI settings to be written to the underlying data source.
        /// </summary>
        public void Generate();
    }
}