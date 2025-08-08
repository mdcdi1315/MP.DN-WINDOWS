
using System;
using System.Collections.Generic;

namespace MP.SettingsTree
{
    internal static class BuilderHelpers
    {
        public static System.Boolean HasDescriptionResourceAttribute(System.Object[] attributes, out System.String value)
        {
            value = null;
            foreach (var a in attributes)
            {
                if (a is SettingDescriptionResourceAttribute sdra)
                {
                    value = sdra.Description;
                    break;
                }
            }
            return value is not null;
        }

        public static System.String JoinDescriptionAttributes(System.Object[] attributes)
        {
            System.Text.StringBuilder sb = new(attributes.Length);
            foreach (var attr in attributes)
            {
                if (attr is SettingDescriptionAttribute sda)
                {
                    sb.Append(sda.Description);
                    sb.Append('\n');
                }
            }
            return sb.ToString();
        }

        public static System.Boolean FindAttribute<T>(System.Object[] attributes, out T instance)
            where T : System.Attribute
        {
            instance = null;
            foreach (var attr in attributes)
            {
                if (attr is T t)
                {
                    instance = t;
                    break;
                }
            }
            return instance is not null;
        }

        public static System.Boolean FindAttribute(System.Object[] attributes, Type attrtype, out System.Object instance)
        {
            instance = null;
            foreach (var attr in attributes)
            {
                if (attr.GetType() == attrtype)
                {
                    instance = attr;
                    break;
                }
            }
            return instance is not null;
        }

        public static void CreateTree(SettingsTreeNode source, IList<SettingsTreeNode> nodestoembed, IList<System.String> parentids)
        {
            SettingsTreeNode current;
            Queue<SettingsTreeNode> nodestack = new();
            nodestack.Enqueue(source);
            while (nodestack.TryDequeue(out current))
            {
                if (current.ChildNodes is null)
                {
                    // This node does not seem to have any child nodes , so pull a new element from the queue
                    continue;
                }
                // Scan all child nodes.
                foreach (var child in current.ChildNodes)
                {
                    // Now , a second loop will scan all the references to the 'child' element.
                    for (System.Int32 J = 0; J < nodestoembed.Count; J++)
                    {
                        if (parentids[J] == child.NodeID)
                        {
                            // J-th element contains the node id of child.
                            // So add it there and remove it.
                            // Additionally , re-run this loop from the beginning.
                            child.AddChildNode(nodestoembed[J]);
                            parentids.RemoveAt(J);
                            nodestoembed.RemoveAt(J);
                            J = -1;
                        }
                    }
                    // OK continue with the next one.
                    // Enqueue the current child to be examined at a later time.
                    nodestack.Enqueue(child);
                }
            }
        }

        public static SettingsTreeNode GetNode(SettingsTreeNode root, System.String id)
        {
            // Originally , check if the current node satisfies it.
            if (root.NodeID == id) { return root; }
            // Well , we must do the heavy lifting now.
            SettingsTreeNode running;
            // Create a node stack to temporarily save nodes in.
            Stack<SettingsTreeNode> nodestack = new(10);
            // Add to the node stack with the current parent node,
            // so that the first time the loop can run.
            nodestack.Push(root);
            while (nodestack.TryPop(out running))
            {
                // Check if the current node satisifies our need,
                // and if so, return it.
                if (running.NodeID == id) { return running; }
                if (running.ChildNodes is null)
                {
                    // No other child nodes for this , 
                    // pop another one to see it's properties.
                    continue;
                }
                foreach (var child in running.ChildNodes)
                {
                    if (child.NodeID == id)
                    {
                        // We can clean the node stack we found out our element.
                        nodestack.Clear();
                        // And, return the node...
                        return child;
                    }
                    else
                    {
                        // Add it to the node stack to be processed later
                        nodestack.Push(child);
                    }
                }
            }
            // If we have processed everything we are ought to return null.
            return null;
        }

        public static IEnumerable<SettingsTreeSetting> EnumerateSettings(SettingsTreeNode root)
        {
            SettingsTreeNode running;
            // Create a node stack to temporarily save nodes in.
            Stack<SettingsTreeNode> nodestack = new(10);
            // Add to the node stack with the current parent node,
            // so that the first time the loop can run.
            nodestack.Push(root);
            while (nodestack.TryPop(out running))
            {
                // Enumerate settings, do not push yet the nodes to the stack to have better mem footprint
                if (running.SettingsCount > 0)
                {
                    foreach (var s in running.Settings) { yield return s; }
                }
                if (running.ChildNodes is not null)
                {
                    nodestack.EnsureCapacity(nodestack.Count + running.ChildrenNodesCount);
                    // Node has children nodes, push them to the node stack
                    foreach (var child in running.ChildNodes) { nodestack.Push(child); }
                }
            }
            // When we reach here, we processed all the nodes and the settings.
            // Thus, we can exit now.
        }
    }
}