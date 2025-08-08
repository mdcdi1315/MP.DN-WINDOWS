
using System;
using System.Diagnostics.CodeAnalysis;

namespace MP.SettingsTree
{
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

        /// <summary>
        /// Constructs a new settings tree node with the specified ID.
        /// </summary>
        /// <param name="nodeid">The ID of the newly created tree node.</param>
        public SettingsTreeNodeAttribute(System.String nodeid) : this()
        {
            this.nodeid = nodeid;
        }

        /// <summary>
        /// Constructs a new settings tree node with the specified ID and textual description for presentation.
        /// </summary>
        /// <param name="nodeid">The ID of the newly created tree node.</param>
        /// <param name="desc">The presentation description of this tree node.</param>
        public SettingsTreeNodeAttribute(System.String nodeid, [MaybeNull] System.String desc) : this()
        {
            this.nodeid = nodeid;
            this.desc = desc;
        }

        /// <summary>
        /// Constructs a new settings tree node with the specified ID, the ID of an already existing node 
        /// that will act as it's parent and a textual description for presentation.
        /// </summary>
        /// <param name="nodeid">The ID of the newly created tree node.</param>
        /// <param name="desc">The presentation description of this tree node.</param>
        /// <param name="parentid">The ID of an existing tree node to act as the parent for the under-construction tree node.</param>
        public SettingsTreeNodeAttribute(System.String nodeid, [MaybeNull] System.String parentid, [MaybeNull] System.String desc) : this()
        {
            this.nodeid = nodeid;
            parentnodeid = parentid;
            this.desc = desc;
        }

        /// <summary>
        /// Constructs a new settings tree node with the specified ID, the ID of an already existing node 
        /// that will act as it's parent, an image resource to display for this node
        /// and a textual description for presentation.
        /// </summary>
        /// <param name="nodeid">The ID of the newly created tree node.</param>
        /// <param name="desc">The presentation description of this tree node.</param>
        /// <param name="parentid">The ID of an existing tree node to act as the parent for the under-construction tree node.</param>
        /// <param name="imgid">A resource ID that contains the image to display for a GUI.</param>
        public SettingsTreeNodeAttribute(System.String nodeid, [MaybeNull] System.String parentid, [MaybeNull] System.String desc, [MaybeNull] System.String imgid)
        {
            this.nodeid = nodeid;
            parentnodeid = parentid;
            this.desc = desc;
            this.imgid = imgid;
        }

        /// <summary>
        /// The ID of the current node.
        /// </summary>
        public System.String NodeID => nodeid;

        /// <summary>
        /// The ID of another node that will act as the parent for the current node.
        /// </summary>
        public System.String ParentNodeID => parentnodeid;

        /// <summary>
        /// Setting this property does denote which image to use for the current node. <br />
        /// This is useful for GUI presenters.
        /// </summary>
        public System.String ImageID => imgid;

        /// <summary>
        /// You can use this property to define a human-readable description of the settings that the node does contain.
        /// </summary>
        public System.String Description => desc;
    }
}