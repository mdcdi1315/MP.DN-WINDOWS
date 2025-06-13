
using MP.SettingsTree;
using System.Windows.Forms;

namespace MP
{
    internal static class SettingsEditorHelpers
    {
        public static void RunRecursively(TreeNode ntarget , SettingsTreeNode nsource)
        {
            if (nsource.ChildrenNodesCount > 0)
            {
                TreeNode t;
                foreach (var el in nsource.ChildNodes)
                {
                    t = new TreeNode() {
                        Tag = $"__NODE-{el.NodeID}",
                        Text = el.Description ?? el.NodeID
                    };
                    if (el.SettingsCount > 0)
                    {
                        foreach (var s in el.Settings)
                        {
                            t.Nodes.Add(new TreeNode() {
                                Name = null,
                                Text = s.FriendlyName ?? s.ID,
                                Tag = s.ID
                            });
                        }
                    }
                    RunRecursively(t, el);
                    ntarget.Nodes.Add(t);
                }
            }
        }

        public struct ExtensionData
        {
            public System.String Name;
            public System.Version Version;
            public System.String Description;
        }

        public struct AudioDeviceData
        {
            public System.String FriendlyName;
            public System.String DeviceID;

            public readonly override System.String ToString() => FriendlyName;
        }
    }
}
