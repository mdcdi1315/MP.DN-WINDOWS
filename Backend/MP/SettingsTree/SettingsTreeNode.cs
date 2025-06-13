
using System.Collections.Generic;

namespace MP.SettingsTree
{
    /// <summary>
    /// A setting tree node is the parent that hosts and groups all the settings and other nodes too. <br />
    /// It is the fundamental element of the Settings Tree infrastracture.
    /// </summary>
    public sealed class SettingsTreeNode
    {
        private List<SettingsTreeSetting> settings;
        private List<SettingsTreeNode> children;
        private System.String id, imgid, desc;

        /// <summary>
        /// Initializes an empty node instance.
        /// </summary>
        public SettingsTreeNode() 
        {
            settings = null;
            children = null;
            id = null;
            imgid = null;
            desc = null;
        }

        /// <summary>
        /// The settings elements that this node does define. <br />
        /// It returns <see langword="null"/> or an empty list if no settings are defined for this node.
        /// </summary>
        public IEnumerable<SettingsTreeSetting> Settings => settings;

        /// <summary>
        /// The child setting tree nodes that this node does define. <br />
        /// It returns <see langword="null"/> or an empty list if no children nodes are defined for this node.
        /// </summary>
        public IEnumerable<SettingsTreeNode> ChildNodes => children;

        public void AddNewSetting(SettingsTreeSetting setting) => (settings ??= new(2)).Add(setting);

        public void AddNewSetting(SettingType type, System.String id, System.Type typeofsetting , System.String desc = null,
            System.Boolean descisres = false, System.String friendlyname = null, System.String imageid = null,
            SettingTreeSettingValidRange range = null) 
         => (settings ??= new(2)).Add(new(type, id, typeofsetting, desc, descisres, friendlyname, imageid, range));

        public System.Boolean RemoveSettingWithId(System.String id)
        {
            if (System.String.IsNullOrEmpty(id)) { throw new System.ArgumentNullException(nameof(id)); }
            if (settings is null) { return false; }
            for (System.Int32 I = 0; I < settings.Count; I++) 
            {
                if (settings[I].ID == id) 
                {
                    settings.RemoveAt(I);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Adds a constructed tree node to the list of currently defined nodes.
        /// </summary>
        /// <param name="newnode">The settings tree node to add.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="newnode"/> was <see langword="null"/>.</exception>
        public void AddChildNode(SettingsTreeNode newnode)
        {
            if (newnode is null) { throw new System.ArgumentNullException(nameof(newnode)); }
            (children ??= new(2)).Add(newnode);
        }

        /// <summary>
        /// Removes all the settings that are currently defined for this node.
        /// </summary>
        public void ClearSettings() => settings?.Clear();

        /// <summary>
        /// Removes all the child nodes that are currently defined for this node. <br />
        /// Note that if you have referenced the child nodes to another variable they do remain alive; <br />
        /// just they are becoming detached from this tree node.
        /// </summary>
        public void ClearChildren() => children?.Clear();

        /// <summary>
        /// Finds and retrieves a setting from the specified identifier that uniquely identifies this setting. <br />
        /// If the setting is not found in the current node , it traverses all the child nodes in order to find it.
        /// </summary>
        /// <param name="id">The setting to identify.</param>
        /// <returns>A new <see cref="SettingsTreeSetting"/> instance representing the requested setting , or <see langword="null"/> indicating that the setting was not found.</returns>
        public SettingsTreeSetting GetSetting(System.String id)
        {
            if (System.String.IsNullOrEmpty(id)) { throw new System.ArgumentNullException(nameof(id)); }
            // Search first in the current node.
            if (settings is not null) {
                foreach (var set in settings)
                {
                    if (set.ID == id) { return set; }
                }
            }
            // If not found , search and into the child nodes too.
            Queue<SettingsTreeNode> nodes = new(children ?? new(0));
            while (nodes.TryDequeue(out var sh))
            {
                if (sh.settings is not null) {
                    foreach (var set in sh.settings)
                    {
                        if (set.ID == id) { return set; }
                    }
                }
                if (sh.children is not null)
                {
                    nodes.EnsureCapacity(sh.children.Count);
                    foreach (var child in sh.children) 
                    { 
                        nodes.Enqueue(child); 
                    }
                }
            }
            nodes = null;
            return null;
        }

        /// <summary>
        /// Finds and retrieves a sub-node from the specified identifier that uniquely identifies this node. <br />
        /// If the node is not found in the children list of the current node , it traverses all the child nodes in order to find it.
        /// </summary>
        /// <param name="id">The node's id to get.</param>
        /// <returns>A new <see cref="SettingsTreeNode"/> instance representing the requested node , or <see langword="null"/> indicating that the node was not found.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="id"/> was <see langword="null"/> or the empty string.</exception>
        public SettingsTreeNode GetNode(System.String id)
        {
            if (System.String.IsNullOrEmpty(id)) { throw new System.ArgumentNullException(nameof(id)); }
            return BuilderHelpers.GetNode(this, id);
        }

        /// <summary>
        /// Gets all the settings contained in this and the childs nodes of this node.
        /// </summary>
        /// <returns>A collection of all the currently-defined settings for this node.</returns>
        public IEnumerable<SettingsTreeSetting> EnumerateSettings()
        {
            // Get all the settings defined in the current node.
            if (settings is not null)
            {
                foreach (var set in settings)
                {
                    if (set.ID == id) { yield return set; }
                }
            }
            Queue<SettingsTreeNode> nodes = new(children ?? new(0));
            while (nodes.TryDequeue(out var sh))
            {
                if (sh.settings is not null)
                {
                    foreach (var set in sh.settings)
                    {
                        if (set.ID == id) { yield return set; }
                    }
                }
                if (sh.children is not null)
                {
                    foreach (var child in sh.children)
                    {
                        nodes.Enqueue(child);
                    }
                }
            }
            nodes = null;
        }

        /// <summary>
        /// Gets all the setting tree nodes defined , including this node. <br />
        /// Additionally , this method returns and a depth index to indicate where the node is located , compared to other nodes. <br />
        /// Note that depth 0 means always the current node.
        /// </summary>
        /// <returns>The enumerated nodes.</returns>
        public IEnumerable<NodeAndDescendence> EnumerateNodes()
        {
            // Return the current node.
            yield return new() { Node = this , Depth = 0 };
            Queue<NodeAndDescendence> nodes = new();
            NodeAndDescendence ct;
            if (children is not null)
            {
                nodes.EnsureCapacity(children.Count);
                foreach (var child in children)
                {
                    ct = new NodeAndDescendence() { Depth = 1, Node = child , ParentNodeId = null };
                    nodes.Enqueue(ct);
                    yield return ct;
                }
            }
            while (nodes.TryDequeue(out var sh))
            {
                if (sh.Node.children is not null)
                {
                    foreach (var child in sh.Node.children)
                    {
                        ct = new NodeAndDescendence() { Depth = sh.Depth + 1, Node = child , ParentNodeId = sh.Node.NodeID };
                        nodes.Enqueue(ct);
                        yield return ct;
                    }
                }
            }
            nodes = null;
        }

        /// <summary>
        /// Gets the number of settings defined in this node.
        /// </summary>
        public System.Int32 SettingsCount => settings is null ? 0 : settings.Count;

        /// <summary>
        /// Gets the number of the currently defined child nodes defined for this node.
        /// </summary>
        public System.Int32 ChildrenNodesCount => children is null ? 0 : children.Count;

        /// <summary>
        /// Gets the ID of the current node.
        /// </summary>
        public System.String NodeID
        {
            get => id;
            set => id = value;
        }

        /// <summary>
        /// Gets the description of the current node.
        /// </summary>
        public System.String Description
        {
            get => desc;
            set => desc = value;
        }

        /// <summary>
        /// Gets the image resource id to display for the current node.
        /// </summary>
        public System.String ImageID
        {
            get => imgid;
            set => imgid = value;
        }
    }

    public sealed class NodeAndDescendence
    {
        public System.String ParentNodeId;
        public SettingsTreeNode Node;
        public System.Int32 Depth;
    }
}