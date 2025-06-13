

namespace MP.SettingsTree
{
    /// <summary>
    /// Defines a setting node in the settings tree.
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
        private SettingTreeSettingValidRange range;

        internal SettingsTreeSetting()
        {
            type = SettingType.Default;
            imageid = null;
            settingid = null;
            settingtype = null;
            descorres = null;
            friendlyname = null;
            isresource = false;
            range = null;
        }

        internal SettingsTreeSetting(SettingType type, System.String id, System.Type settingtype, System.String desc = null, 
            System.Boolean descisres = false, System.String friendlyname = null , System.String imageid = null ,
            SettingTreeSettingValidRange range = null)
        {
            this.settingtype = settingtype;
            this.type = type;
            this.imageid = imageid;
            settingid = id;
            descorres = desc;
            isresource = descisres;
            this.friendlyname = friendlyname;
            this.range = range;
        }

        public SettingType Type
        {
            get => type;
            internal set => type = value;
        }

        public System.String ID
        {
            get => settingid;
            internal set => settingid = value;
        }

        public System.String ImageID
        {
            get => imageid;
            internal set => imageid = value;
        }

        public System.String Description
        {
            get => descorres;
            internal set => descorres = value;
        }

        public System.String FriendlyName
        {
            get => friendlyname;
            internal set => friendlyname = value;
        }

        public System.Type TypeOfValue
        {
            get => settingtype;
            internal set => settingtype = value;
        }

        public System.Boolean DescriptionIsResource
        {
            get => isresource;
            internal set => isresource = value;
        }

        public SettingTreeSettingValidRange ValidNumericRange
        {
            get => range;
            internal set => range = value;
        }
    }

    /// <summary>
    /// Has the values for a setting that provides the <see cref="SettingsTreeValidValuesAttribute{T}"/>.
    /// </summary>
    public sealed class SettingTreeSettingValidRange
    {
        private System.Object minval , maxval;

        public SettingTreeSettingValidRange(System.Object minimum , System.Object maximum)
        {
            minval = minimum;
            maxval = maximum;
        }

        public System.Object MinimumBound => minval;

        public System.Object MaximumBound => maxval;
    }
}