

using System;

namespace MP.CGISettings
{
    /// <summary>
    /// Provides a model for loading and saving data abstractly when working with the <see cref="ICGISettingExtension"/> class. <br />
    /// You DO NOT need to implement this interface; It is part of the infrastracture defined for the CGI setting extension subsystem.
    /// </summary>
    public interface ICGISettingExtensionDataSource : IDisposable
    {
        /// <summary>
        /// Gets the length of this data source, when data are read from this source.
        /// </summary>
        public System.Int64 Length { get; }

        /// <summary>
        /// Gets the encoding defined by the CGI settings reader or writer. <br />
        /// Under this encoding all the strings involved must be retrieved and saved as.
        /// </summary>
        public System.Text.Encoding StringEncoding { get; }

        /// <summary>Gets the type of this data source.</summary>
        public DataSourceType Type { get; }

        /// <summary>
        /// Reads data from this data source.
        /// </summary>
        /// <param name="buffer">The buffer to place the read data into.</param>
        /// <param name="offset">The offset inside <paramref name="buffer"/> to start placing data from.</param>
        /// <param name="count">The number of bytes to copy to <paramref name="buffer"/>.</param>
        /// <returns>
        /// The number of bytes actually read from the source. <br />
        /// This might be less than the number of bytes requested in <paramref name="count"/>.
        /// </returns>
        public System.Int32 Read(System.Byte[] buffer , System.Int32 offset, System.Int32 count);

        /// <summary>
        /// Writes data through this data source.
        /// </summary>
        /// <param name="buffer">The buffer to write through this source</param>
        /// <param name="offset">The offset inside <paramref name="buffer"/> to start writing data from.</param>
        /// <param name="count">The number of bytes to write.</param>
        public void Write(System.Byte[] buffer, System.Int32 offset, System.Int32 count);
    }
}