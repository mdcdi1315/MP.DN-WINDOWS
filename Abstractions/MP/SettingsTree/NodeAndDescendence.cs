

namespace MP.SettingsTree
{
    /// <summary>
    /// Defines a settings tree node and it's depth in the setting tree.
    /// </summary>
    public sealed class NodeAndDescendence
    {
        /// <summary>The ID of the parent node of the current node.</summary>
        public System.String ParentNodeId;
        /// <summary>The current node.</summary>
        public SettingsTreeNode Node;
        /// <summary>The depth of the node in the settings tree.</summary>
        public System.Int32 Depth;
    }
}