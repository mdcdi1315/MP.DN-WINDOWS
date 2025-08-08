
using System;
using System.Diagnostics.CodeAnalysis;

namespace MP.SettingsTree
{
    /// <summary>
    /// Defines a human-readable description for the current setting. <br />
    /// Adding multiple of these should be grouped into paragrphs in a GUI.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
    public sealed class SettingDescriptionAttribute : Attribute
    {
        private System.String desc;

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingDescriptionAttribute"/> class instance,
        /// specifying the desired description that the applied setting should have.
        /// </summary>
        /// <param name="description">The description string that should be bound to the current setting instance.</param>
        public SettingDescriptionAttribute([MaybeNull] System.String description) => desc = description;

        /// <summary>
        /// The description string bound to the current setting field or property.
        /// </summary>
        public System.String Description => desc;
    }
}