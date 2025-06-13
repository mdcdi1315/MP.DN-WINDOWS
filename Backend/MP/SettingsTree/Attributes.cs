
using System;

namespace MP.SettingsTree
{
    /// <summary>
    /// Classes marked with this attribute makes them valid for 
    /// the Settings Tree reader to build a settings tree for them.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class SettingsTreeLayoutClassAttribute : Attribute { }

    /// <summary>
    /// Fields or properties marked with this attribute are ignored by the settings tree reader. <br />
    /// The tree reader, by default considers the following as a valid setting: <br />
    /// <list type="bullet">
    ///     <item>When the class field is public.</item>
    ///     <item>When the class property is public and both gettable and settable.</item>
    /// </list> <br />
    /// The above are just the enough requirements to build a setting node in the tree. <br />
    /// You may have settings that are just about the app's internals so marking it with this attribute <br />
    /// the reader will not build a setting node for the particular field or property, <br />
    /// before even attempting type resolving of the field or property backing type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property , AllowMultiple = false)]
    public sealed class SettingsTreeIgnoreAttribute : Attribute { }

    /// <summary>
    /// Fields or properties marked with this attribute denote the valid range of values 
    /// that the denoted field or property can accept. <br />
    /// The <typeparamref name="T"/> type indicate the setting type that is defined on the field. <br />
    /// You should use this attribute for numeric values.
    /// </summary>
    /// <typeparam name="T">The type of the setting to mark it's valid range of values.</typeparam>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property , AllowMultiple = false)]
    public sealed class SettingsTreeValidValuesAttribute<T> : Attribute
    {
        private T minval;
        private T maxval;

        public SettingsTreeValidValuesAttribute(T minimum , T maximum)
        {
            this.minval = minimum;
            this.maxval = maximum;
        }

        public T MinimumValue 
        {
            get => minval; 
        }

        public T MaximumValue
        {
            get => maxval;
        }
    }

    /// <summary>
    /// Indicates the parent node where this setting field will be referenced to. <br />
    /// Be noted, a setting can only have one parent node, if any!
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public sealed class SettingsTreeParentAttribute : Attribute 
    {
        private System.String parentnodename;

        public SettingsTreeParentAttribute(System.String nodeid) 
        {
            parentnodename = nodeid;
        }

        public System.String ParentNodeID => parentnodename;
    }

    /// <summary>
    /// Creates a new settings node that will accomondate many setting nodes. <br />
    /// The setting fields and properties use the <see cref="SettingsTreeParentAttribute"/> to indicate in which node they will belong to. <br />
    /// These nodes can be also parented to other nodes.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class , AllowMultiple = true)]
    public sealed class SettingsTreeNodeAttribute : Attribute
    {
        private System.String nodeid , parentnodeid , desc , imgid;

        private SettingsTreeNodeAttribute() 
        {
            nodeid = null;
            parentnodeid = null;
            desc = null;
            imgid = null;
        }

        public SettingsTreeNodeAttribute(System.String nodeid) : this()
        {
            this.nodeid = nodeid;
        }

        public SettingsTreeNodeAttribute(System.String parentid , System.String nodeid) : this()
        {
            this.nodeid = nodeid;
            parentnodeid = parentid;
        }

        public SettingsTreeNodeAttribute(System.String parentid, System.String nodeid , System.String desc) : this()
        {
            this.nodeid = nodeid;
            this.parentnodeid = parentid;
            this.desc = desc;
        }

        public SettingsTreeNodeAttribute(System.String parentid, System.String nodeid, System.String desc , System.String imgid)
        {
            this.nodeid = nodeid;
            this.parentnodeid = parentid;
            this.desc = desc;
            this.imgid = imgid;
        }

        public System.String NodeID
        {
            get => nodeid;
        }

        public System.String ParentNodeID
        {
            get => parentnodeid;
        }

        /// <summary>
        /// Setting this property does denote which image to use for the current node. <br />
        /// This is useful for GUI presenters.
        /// </summary>
        public System.String ImageID
        {
            get => imgid;
        }

        /// <summary>
        /// You can use this property to define a human-readable description of the settings that the node does contain.
        /// </summary>
        public System.String Description
        {
            get => desc;
        }
    }

    /// <summary>
    /// Defines a human-readable description for the current setting. <br />
    /// Adding multiple of these should be grouped into paragrphs in a GUI.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
    public sealed class SettingDescriptionAttribute : Attribute
    {
        private System.String desc;

        public SettingDescriptionAttribute(System.String description) 
        {
            desc = description;
        }

        public System.String Description => desc;
    }

    /// <summary>
    /// Does the same as <see cref="SettingDescriptionAttribute"/> , but loads the string from an external resource.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
    public sealed class SettingDescriptionResourceAttribute : Attribute
    {
        private System.String desc;

        public SettingDescriptionResourceAttribute(System.String description)
        {
            desc = description;
        }

        public System.String Description => desc;
    }

    /// <summary>
    /// Defines a friendly name for the current setting. This is useful for GUI applications.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field , AllowMultiple = false)]
    public sealed class SettingFriendlyNameAttribute : Attribute
    {
        private System.String fn;

        public SettingFriendlyNameAttribute(System.String friendlyname)
        {
            fn = friendlyname;
        }

        public System.String FriendlyName => fn;
    }

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

    /// <summary>
    /// Defines the behavior of a setting , how it should be retrieved and how it should be set.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public sealed class SettingTypeAttribute : Attribute
    {
        private SettingType type;

        public SettingTypeAttribute(SettingType type) => this.type = type;

        public SettingType Type => type;
    }

    /// <summary>
    /// Allows for the current setting to be also accompanied with an image that describes the setting.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public sealed class SettingImageAttribute : Attribute
    {
        private System.String imgid;

        public SettingImageAttribute(System.String imageid)
        {
            imgid = imageid;
        }

        public System.String ImageId => imgid;
    }


}