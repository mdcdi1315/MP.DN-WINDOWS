
using System;
using System.Diagnostics.CodeAnalysis;

namespace MP.SettingsTree
{
    /// <summary>
    /// Allows for the current setting to be also accompanied with an image that describes the setting.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public sealed class SettingImageAttribute : Attribute
    {
        private System.String imgid;

        /// <summary>
        /// Creates a new instance of the <see cref="SettingImageAttribute"/> class, specifying the image to be displayed on the specified setting.
        /// </summary>
        /// <param name="imageid">The Resource ID of the image to be displayed in presentation views.</param>
        public SettingImageAttribute([MaybeNull] System.String imageid) => imgid = imageid;

        /// <summary>
        /// Gets a resource ID of the image that is to be displayed in presentation views.
        /// </summary>
        public System.String ImageId => imgid;
    }
}