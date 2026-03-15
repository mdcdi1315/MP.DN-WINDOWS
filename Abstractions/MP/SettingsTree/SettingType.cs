

namespace MP.SettingsTree
{
    /// <summary>
    /// Defines valid setting behavior types.
    /// </summary>
    public enum SettingType : System.Byte
    {
        /// <summary>
        /// The setting is just retrieved and set. <br />
        /// This is and the default so you do not have to include a <see cref="SettingTypeAttribute"/>.
        /// </summary>
        Default = 0,
        /// <summary>
        /// The setting only gets specific values given from a list of provided values.
        /// </summary>
        ValueList,
        /// <summary>
        /// The setting is numeric and can only take the values specified in a <see cref="SettingsTreeValidValuesAttribute{T}"/>.
        /// </summary>
        HasSpecificRange,
        /// <summary>
        /// The setting cannot be either got or can be set , it is just used, for example, to do an action.
        /// </summary>
        Dummy,
        /// <summary>
        /// The setting is a color setting , and thus a handler must handle this setting with a special manner.
        /// </summary>
        Color,
        /// <summary>
        /// The setting is a string that acts as a filepath. Thus an GUI implementer should also present a button to browse a file.
        /// </summary>
        IsFilePath,
        /// <summary>
        /// The setting acts the same as the <see cref="IsFilePath"/>, just the GUI implementer must present an option to browse a folder instead.
        /// </summary>
        IsFolderPath
    }
}