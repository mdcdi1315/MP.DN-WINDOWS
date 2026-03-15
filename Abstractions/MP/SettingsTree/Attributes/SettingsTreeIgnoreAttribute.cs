
using System;

namespace MP.SettingsTree
{
    /// <summary>
    /// Fields or properties marked with this attribute are ignored by the settings tree reader. <br />
    /// The settings tree builder by default considers the following as a valid setting: <br />
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
}