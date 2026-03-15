
using System;

namespace MP.SettingsTree
{
    /// <summary>
    /// Specifies the node where this setting field will be referenced to. <br />
    /// Be noted, a setting can only have one parent node, if any!
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public sealed class SettingsTreeParentAttribute : Attribute 
    {
        private System.String parentnodename;

        /// <summary>
        /// Creates a new instance of the <see cref="SettingsTreeParentAttribute"/> specifying 
        /// a node ID that will act as the parent settings tree node for the current setting.
        /// </summary>
        /// <param name="nodeid">The ID of the node.</param>
        public SettingsTreeParentAttribute(System.String nodeid) => parentnodename = nodeid;

        /// <summary>
        /// The node ID that this setting will be parented to.
        /// </summary>
        public System.String ParentNodeID => parentnodename;
    }
}