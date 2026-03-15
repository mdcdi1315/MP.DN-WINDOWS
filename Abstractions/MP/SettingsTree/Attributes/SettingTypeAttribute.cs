
using System;

namespace MP.SettingsTree
{
    /// <summary>
    /// Defines the behavior of a setting , how it should be retrieved and how it should be set.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public sealed class SettingTypeAttribute : Attribute
    {
        private SettingType type;

        /// <summary>
        /// Creates a new instance of the <see cref="SettingTypeAttribute"/> class, specifying the type of the referenced setting element.
        /// </summary>
        /// <param name="type">The type of the current setting element.</param>
        public SettingTypeAttribute(SettingType type) => this.type = type;

        /// <summary>
        /// Retrieves the of the current setting element.
        /// </summary>
        public SettingType Type => type;
    }
}