
using System;
using System.Diagnostics.CodeAnalysis;

namespace MP.SettingsTree
{
    /// <summary>
    /// Defines a friendly name for the current setting. This is useful for presentation or specifically, for GUI applications.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field , AllowMultiple = false)]
    public sealed class SettingFriendlyNameAttribute : Attribute
    {
        private System.String fn;

        /// <summary>
        /// Creates a new instance of the <see cref="SettingFriendlyNameAttribute"/> class, specifying the name to display for presentation.
        /// </summary>
        /// <param name="friendlyname">The friendly name to be displayed instead of the setting's ID.</param>
        public SettingFriendlyNameAttribute([MaybeNull] System.String friendlyname) => fn = friendlyname;

        /// <summary>
        /// Gets the presentation name for the referenced setting element.
        /// </summary>
        public System.String FriendlyName => fn;
    }
}