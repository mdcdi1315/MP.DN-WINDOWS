

namespace MP.CGISettings
{
    /// <summary>
    /// Event delegate that is used when the <see cref="CGISettingsReaderLoader{T}.CGISettingNotLoaded"/> event
    /// is fired up to provide the setting that was not loaded.
    /// </summary>
    /// <param name="entry">The setting entry that was not loaded into the implementation class.</param>
    public delegate void CGISettingNotLoadedDelegate<in T>(T entry) where T : SettingEntry;

    /// <summary>
    /// Event delegate that is used when the <see cref="CGISettingsReaderLoader{T}.CGISettingNotPresentInSource"/> event
    /// is fired up to provide the setting that was not found in the <see cref="ICGISettingsReader{T}"/>.
    /// </summary>
    /// <param name="entryname">The name of the setting entry of the class that was not filled in.</param>
    public delegate void CGISettingNotFilledInDelegate(System.String entryname);
}