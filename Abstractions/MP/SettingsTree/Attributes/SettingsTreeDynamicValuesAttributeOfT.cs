

using System;

namespace MP.SettingsTree
{
    /// <summary>
    /// Defines the <see cref="Type"/> to invoke for providing a <see cref="IDynamicValueListProvider{T}"/> implementation.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field , AllowMultiple = false)]
    public sealed class SettingsTreeDynamicValuesAttribute : Attribute
    {
        private Type dynamicobjectstype;

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsTreeDynamicValuesAttribute"/> class,
        /// providing the <see cref="Type"/> that holds a <see cref="IDynamicValueListProvider{T}"/> implementation.
        /// </summary>
        /// <param name="provider">The <see cref="Type"/> holding the dynamic values provider.</param>
        public SettingsTreeDynamicValuesAttribute(Type provider) => dynamicobjectstype = provider;

        /// <summary>
        /// Returns the <see cref="Type"/> that holds the dynamic values provider.
        /// </summary>
        public Type Provider => dynamicobjectstype;
    }
}