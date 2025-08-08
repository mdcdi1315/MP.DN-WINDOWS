
using System;

namespace MP.SettingsTree
{
    /// <summary>
    /// Classes marked with this attribute makes them valid for 
    /// the Settings Tree reader to build a settings tree for them.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class SettingsTreeLayoutClassAttribute : Attribute { }
}