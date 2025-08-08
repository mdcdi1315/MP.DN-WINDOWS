
using System;
using System.Diagnostics.CodeAnalysis;

namespace MP.SettingsTree
{
    /// <summary>
    /// Does the same as <see cref="SettingDescriptionAttribute"/> , but loads the string from an external resource. <br /> <br />
    /// Note, however, that both <see cref="SettingDescriptionResourceAttribute"/> and 
    /// <see cref="SettingDescriptionAttribute"/> attributes are mutually exclusive. <br /> <br />
    /// If both attributes are specified, the settings tree builder always selects the <see cref="SettingDescriptionResourceAttribute"/> instance. <br />
    /// There is no way to override this.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public sealed class SettingDescriptionResourceAttribute : Attribute
    {
        private System.String desc;

        /// <summary>
        /// Creates a new instance of the <see cref="SettingDescriptionResourceAttribute"/> class and specifying a resource ID for the description of the current settings tree setting.
        /// </summary>
        /// <param name="description">The description of the setting, specified by a resource ID.</param>
        public SettingDescriptionResourceAttribute([MaybeNull] System.String description) => desc = description;

        /// <summary>
        /// The resource ID of the description of the current setting.
        /// </summary>
        public System.String Description => desc;
    }
}