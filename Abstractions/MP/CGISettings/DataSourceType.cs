

namespace MP.CGISettings
{
    /// <summary>
    /// Defines the implementation class returned indirectly as a <see cref="ICGISettingExtensionDataSource"/> instance.
    /// </summary>
    public enum DataSourceType : System.Byte
    {
        /// <summary>
        /// The data source implements both the Read and Write methods.
        /// </summary>
        Both,
        /// <summary>
        /// The data source implements the reader part only.
        /// </summary>
        Reader,
        /// <summary>
        /// The data source implements the writer part only.
        /// </summary>
        Writer
    }
}