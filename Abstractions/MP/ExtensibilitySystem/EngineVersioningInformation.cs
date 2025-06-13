

using System.Collections.Generic;

namespace MP.ExtensibilitySystem
{
    /// <summary>
    /// Returns various versioning information for the extensions so that to identify themselves if they should finally load or not. <br />
    /// This is useful on cases where breaking changes are happening on the base app libraries that the extensions were dependent of and
    /// thus would otherwise throw link exceptions.
    /// </summary>
    public abstract class EngineVersioningInformation
    {
        /// <summary>
        /// The extensibility engine version. 
        /// Breaking changes may happen and into the engine implementation itself.
        /// </summary>
        public abstract System.Version EngineVersion { get; }

        /// <summary>
        /// Returns app versioning information as a list of versions - each app using the extensibility system engine
        /// should say which ordinals correspond to each resource version that the app defines itself.
        /// </summary>
        public abstract IList<System.Version> AppVersions { get; }
    }
}