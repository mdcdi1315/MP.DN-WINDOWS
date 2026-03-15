

namespace MP.CGISettings
{
    /// <summary>
    /// Defines a CGI settings extension. <br />
    /// These extensions 'extend' the base format to include complex .NET types, if those provided by default do not suffice.
    /// </summary>
    public interface ICGISettingExtension
    {
        /// <summary>
        /// Gets the type that this CGI setting extension overrides or implements.
        /// </summary>
        public CGISettingType RegisteredType { get; }

        /// <summary>
        /// Gets the .NET type that this CGI setting extension saves and retrieves as.
        /// </summary>
        public System.Type WrappingType { get; }

        /// <summary>
        /// Loads the object previously stored into a packed CGI setting.
        /// </summary>
        /// <param name="source">The data source implemented by a CGI settings reader to read and load the object from.</param>
        /// <returns>The loaded object.</returns>
        public System.Object LoadObject(ICGISettingExtensionDataSource source);

        /// <summary>
        /// Saves the object into the data source provided from the <paramref name="source"/> parameter.
        /// </summary>
        /// <param name="source">The data source to save the object into.</param>
        /// <param name="obj">The object to be saved.</param>
        public void SaveObject(ICGISettingExtensionDataSource source , System.Object obj);
    }
}