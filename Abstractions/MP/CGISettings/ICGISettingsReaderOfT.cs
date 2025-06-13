
using System;
using System.Collections.Generic;

namespace MP.CGISettings
{
    /// <summary>
    /// Defines the low-level abstraction of a CGI Settings Reader.
    /// </summary>
    /// <typeparam name="T">A more derived type of the <see cref="SettingEntry"/> class, if you deem it necessary by your implementing class.</typeparam>
    public interface ICGISettingsReader<out T> : IEnumerable<T>, IDisposable 
        where T : SettingEntry
    {
        /// <summary>
        /// Gets the version read from the current reader. <br />
        /// This is a reader-specific property. 
        /// </summary>
        public System.Int32 Version { get; }

        /// <summary>
        /// Gets the name of the application that created the underlying CGI settings data. May also be null or empty.
        /// </summary>
        public System.String ApplicationName { get; }

        /// <summary>
        /// Gets the encoding that is currently used for saving string data inside the underlying CGI settings data.
        /// </summary>
        public System.Text.Encoding Encoding { get; }

        /// <summary>
        /// Registers a CGI settings extension to be used with this instance.
        /// </summary>
        /// <param name="extension">The CGI settings extension to register.</param>
        /// <exception cref="ArgumentNullException"><paramref name="extension"/> was <see langword="null"/>.</exception>
        public void RegisterExtension(ICGISettingExtension extension);
    }
}