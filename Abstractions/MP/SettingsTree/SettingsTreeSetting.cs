

namespace MP.SettingsTree
{
    /// <summary>
    /// Defines a setting node in the settings tree. <br />
    /// Instances of the class can only be created from <see cref="SettingsTreeNode"/>s; and that is the recommended way to do it.
    /// </summary>
    public sealed class SettingsTreeSetting
    {
        private SettingType type;
        private System.String imageid;
        private System.String settingid;
        private System.Type settingtype;
        private System.String descorres;
        private System.String friendlyname;
        private System.Boolean isresource;
        private System.Object additionaldata;

        internal SettingsTreeSetting()
        {
            type = SettingType.Default;
            imageid = null;
            settingid = null;
            settingtype = null;
            descorres = null;
            friendlyname = null;
            isresource = false;
            additionaldata = null;
        }

        internal SettingsTreeSetting(SettingType type, System.String id, System.Type settingtype, System.String desc = null, 
            System.Boolean descisres = false, System.String friendlyname = null , System.String imageid = null ,
            System.Object additionaldata = null)
        {
            this.settingtype = settingtype;
            this.type = type;
            this.imageid = imageid;
            settingid = id;
            descorres = desc;
            isresource = descisres;
            this.friendlyname = friendlyname;
            this.additionaldata = additionaldata;
        }

        /// <summary>
        /// Gets the kind of this setting.
        /// </summary>
        public SettingType Type
        {
            get => type;
            internal set => type = value;
        }

        /// <summary>
        /// Gets the ID of the current setting in the entire tree.
        /// </summary>
        public System.String ID
        {
            get => settingid;
            internal set => settingid = value;
        }

        /// <summary>
        /// Gets a Resource ID of the image to display in GUI's.
        /// </summary>
        public System.String ImageID
        {
            get => imageid;
            internal set => imageid = value;
        }

        /// <summary>
        /// Gets the description of the current setting.
        /// </summary>
        public System.String Description
        {
            get => descorres;
            internal set => descorres = value;
        }

        /// <summary>
        /// Gets a name that should be visible for presentation, such as GUI's.
        /// </summary>
        public System.String FriendlyName
        {
            get => friendlyname;
            internal set => friendlyname = value;
        }

        /// <summary>
        /// The backing type of the current setting element.
        /// </summary>
        public System.Type TypeOfValue
        {
            get => settingtype;
            internal set => settingtype = value;
        }

        /// <summary>
        /// Gets a value whether the contents of the <see cref="Description"/> property is instead a resource ID.
        /// </summary>
        public System.Boolean DescriptionIsResource
        {
            get => isresource;
            internal set => isresource = value;
        }

        /// <summary>
        /// When <see cref="SettingType.HasSpecificRange"/> is specified, this property should return a valid numeric range that the final value should have.
        /// </summary>
        public SettingTreeSettingValidRange ValidNumericRange
        {
            get => additionaldata as SettingTreeSettingValidRange;
            internal set => additionaldata = value;
        }
    }
}