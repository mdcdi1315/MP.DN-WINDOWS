
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using MP.Annotations.CodeAnalysis;

namespace MP.SettingsTree
{
    /// <summary>
    /// The settings tree builder class reads from a class that does contain the Settings Tree attributes and
    /// transforms the attribute metadata into a useful setting tree allowing to be traversed using it's root element. <br />
    /// A wrinkle of this implementation is that it does not require to implement from a special class or an interface ,
    /// but instead the entire work is done through pure attributes that specify their meaning.
    /// </summary>
    public sealed class SettingsTreeBuilder : IDisposable
    {
        private System.Object settingsbase;
        private System.Type reverselookup; // We do a reverse lookup in the settingsbase by doing reflection, so it's name is completely justified! -:)
        private SettingsTreeNode rootnode;
        private List<SettingsTreeSetting> settingsall;
        private List<System.String> parentnodeids;
        private IResourcesProvider provider;

        /// <summary>
        /// Creates a new settings tree builder instance. <br />
        /// Note that the constructor just creates an instance of <see cref="SettingsTreeBuilder"/>; <br /> 
        /// you need to call the <see cref="Build"/> method to start building the settings tree.
        /// </summary>
        /// <param name="obj">The class object that will be reversed-looked up to create a settings tree.</param>
        /// <exception cref="ArgumentNullException"><paramref name="obj"/> is <see langword="null"/>.</exception>
        public SettingsTreeBuilder(System.Object obj)
        {
            ArgumentNullException.ThrowIfNull(obj);
            settingsbase = obj;
            reverselookup = settingsbase.GetType();
            rootnode = null;
        }

        private void GetFieldSettings()
        {
            DebugProvider.WriteLine("SETTINGSTREEBUILDER: Builder finds for public fields...");
            foreach (var field in reverselookup.GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public))
            {
                DebugProvider.WriteLine($"SETTINGSTREEBUILDER: Builder found the field {field.Name} .");
                var data = field.GetCustomAttributes(typeof(SettingsTreeIgnoreAttribute), false);
                // Skip constructing this field if the user does not want to use it
                if (data.Length > 0)
                {
                    DebugProvider.WriteLine($"SETTINGSTREEBUILDER: Builder detected that the user does not want to generate a setting for the field {field.Name}. Skipping.");
                    continue;
                }
                // Nice. Now construct a new object to save it.
                SettingsTreeSetting set = new();
                set.ID = field.Name; // By default it is the name of the field
                set.TypeOfValue = field.FieldType;
                DebugProvider.WriteLine("SETTINGSTREEBUILDER: Acquiring custom attributes for the field.");
                var cda = field.GetCustomAttributes(false);
                DebugProvider.WriteLine("SETTINGSTREEBUILDER: Getting description data...");
                if (BuilderHelpers.HasDescriptionResourceAttribute(cda, out var desc))
                {
                    set.DescriptionIsResource = true;
                    set.Description = desc;
                }
                else
                {
                    set.DescriptionIsResource = false;
                    set.Description = BuilderHelpers.JoinDescriptionAttributes(cda);
                }
                DebugProvider.WriteLine("SETTINGSTREEBUILDER: Getting metadata values...");
                if (BuilderHelpers.FindAttribute(cda, out SettingFriendlyNameAttribute a))
                {
                    set.FriendlyName = a.FriendlyName;
                }
                if (BuilderHelpers.FindAttribute(cda, out SettingImageAttribute a1))
                {
                    set.ImageID = a1.ImageId;
                }
                if (BuilderHelpers.FindAttribute(cda, out SettingTypeAttribute a2))
                {
                    set.Type = a2.Type;
                }
                if (set.Type == SettingType.HasSpecificRange)
                {
                    System.Type temp = typeof(SettingsTreeValidValuesAttribute<>);
                    temp = temp.MakeGenericType(set.TypeOfValue);
                    if (BuilderHelpers.FindAttribute(cda, temp, out var range))
                    {
                        var bflags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.GetProperty;
                        set.ValidNumericRange = new(temp.InvokeMember("MinimumValue", bflags, null, range, Array.Empty<Object>()),
                                                    temp.InvokeMember("MaximumValue", bflags, null, range, Array.Empty<Object>()));
                    }
                } else if (set.Type == SettingType.ValueList &&
                        BuilderHelpers.FindAttribute(cda , out SettingsTreeDynamicValuesAttribute stdv))
                {
                    try {
                        set.DynamicValuesList = Activator.CreateInstance(stdv.Provider);
                    } catch (Exception e) {
                        throw new InvalidSettingClassDefinitionLayoutException("Cannot create the dynamic values list provider.", e);
                    }
                }
                if (BuilderHelpers.FindAttribute(cda, out SettingsTreeParentAttribute pa))
                {
                    parentnodeids.Add(pa.ParentNodeID);
                }
                else
                {
                    parentnodeids.Add(null); // The setting is located at the root node.
                }
                settingsall.Add(set);
                DebugProvider.WriteLine($"SETTINGSTREEBUILDER: Done collecting data for field {field.Name}.");
            }
            DebugProvider.WriteLine("SETTINGSTREEBUILDER: Done collecting field settings!");
        }

        private void GetPropertySettings()
        {
            DebugProvider.WriteLine("SETTINGSTREEBUILDER: Builder finds for public properties...");
            foreach (var prop in reverselookup.GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public))
            {
                DebugProvider.WriteLine($"SETTINGSTREEBUILDER: Builder found the property {prop.Name} .");
                // Verify that we can read/write to this property
                if (prop.CanRead == false || prop.CanWrite == false)
                {
                    DebugProvider.WriteLine($"SETTINGSTREEBUILDER: Ignoring property {prop.Name} since it is not both readable and writeable. Skipping.");
                    continue;
                }
                var data = prop.GetCustomAttributes(typeof(SettingsTreeIgnoreAttribute), false);
                // Skip constructing this field if the user does not want to use it
                if (data.Length > 0)
                {
                    DebugProvider.WriteLine($"SETTINGSTREEBUILDER: Builder detected that the user does not want to generate a setting for the property {prop.Name}. Skipping.");
                    continue;
                }
                // Nice. Now construct a new object to save it.
                SettingsTreeSetting set = new();
                set.ID = prop.Name; // By default it is the name of the field
                set.TypeOfValue = prop.PropertyType;
                DebugProvider.WriteLine("SETTINGSTREEBUILDER: Acquiring custom attributes for the field.");
                var cda = prop.GetCustomAttributes(false);
                DebugProvider.WriteLine("SETTINGSTREEBUILDER: Getting description data...");
                if (BuilderHelpers.HasDescriptionResourceAttribute(cda, out var desc))
                {
                    set.DescriptionIsResource = true;
                    set.Description = desc;
                }
                else
                {
                    set.DescriptionIsResource = false;
                    set.Description = BuilderHelpers.JoinDescriptionAttributes(cda);
                }
                DebugProvider.WriteLine("SETTINGSTREEBUILDER: Getting metadata values...");
                if (BuilderHelpers.FindAttribute(cda, out SettingFriendlyNameAttribute a))
                {
                    set.FriendlyName = a.FriendlyName;
                }
                if (BuilderHelpers.FindAttribute(cda, out SettingImageAttribute a1))
                {
                    set.ImageID = a1.ImageId;
                }
                if (BuilderHelpers.FindAttribute(cda, out SettingTypeAttribute a2))
                {
                    set.Type = a2.Type;
                }
                if (set.Type == SettingType.HasSpecificRange)
                {
                    System.Type temp = typeof(SettingsTreeValidValuesAttribute<>);
                    temp = temp.MakeGenericType(set.TypeOfValue);
                    if (BuilderHelpers.FindAttribute(cda, temp, out var range))
                    {
                        var bflags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.GetProperty;
                        set.ValidNumericRange = new(temp.InvokeMember("MinimumValue", bflags, null, range, Array.Empty<Object>()),
                                                    temp.InvokeMember("MaximumValue", bflags, null, range, Array.Empty<Object>()));
                    }
                } else if (set.Type == SettingType.ValueList &&
                        BuilderHelpers.FindAttribute(cda , out SettingsTreeDynamicValuesAttribute stdv))
                {
                    try {
                        set.DynamicValuesList = Activator.CreateInstance(stdv.Provider);
                    } catch (Exception e) {
                        throw new InvalidSettingClassDefinitionLayoutException("Cannot create the dynamic values list provider.", e);
                    }
                }
                if (BuilderHelpers.FindAttribute(cda, out SettingsTreeParentAttribute pa))
                {
                    parentnodeids.Add(pa.ParentNodeID);
                }
                else
                {
                    parentnodeids.Add(null); // The setting is located at the root node.
                }
                settingsall.Add(set);
                DebugProvider.WriteLine($"SETTINGSTREEBUILDER: Done collecting data for property {prop.Name}.");
            }
            DebugProvider.WriteLine("SETTINGSTREEBUILDER: Done collecting property settings!");
        }

        /// <summary>
        /// Builds out the settings tree for the given settings tree object.
        /// </summary>
        public void Build()
        {
            if (rootnode is not null) { return; }
            if (reverselookup.BaseType is not null && reverselookup.BaseType != typeof(System.Object))
            {
                throw new InvalidSettingClassDefinitionLayoutException("A Settings Tree class must only derive from the System.Object class.");
            }
            if (reverselookup.IsInterface)
            {
                throw new InvalidSettingClassDefinitionLayoutException("A Settings Tree class must be it's true class instance and not an interface.");
            }
            var data = reverselookup.GetCustomAttributes(typeof(SettingsTreeLayoutClassAttribute), false);
            if (data.Length == 0)
            {
                throw new NotASettingsTreeException(reverselookup);
            }
            DebugProvider.WriteLine("SETTINGSTREEBUILDER: Starting tree build operation...");
            // Initialize find operation
            settingsall = new();
            parentnodeids = new();
            DebugProvider.WriteLine("SETTINGSTREEBUILDER: Collecting settings data...");
            GetFieldSettings();
            GetPropertySettings();
            DebugProvider.WriteLine("SETTINGSTREEBUILDER: Done collecting settings data!");
            // Now, settingsall and parentnodeids contain all the built data.
            // We leave these as is for the moment.
            // Now, fetch and temporarily create all the tree nodes.
            List<SettingsTreeNode> nds = new();
            List<System.String> parentnds = new();
            DebugProvider.WriteLine("SETTINGSTREEBUILDER: Finding additional tree nodes...");
            foreach (var nd in reverselookup.GetCustomAttributes(false))
            {
                if (nd is SettingsTreeNodeAttribute stna)
                {
                    // OK. We have a tree node , construct it
                    SettingsTreeNode temp = new();
                    temp.NodeID = stna.NodeID;
                    temp.Description = stna.Description;
                    temp.ImageID = stna.ImageID;
                    parentnds.Add(stna.ParentNodeID);
                    nds.Add(temp);
                }
            }
            // Create root node and set to it basic stuff
            DebugProvider.WriteLine($"SETTINGSTREEBUILDER: Found {nds.Count} nodes for the tree.");
            rootnode = new();
            rootnode.NodeID = null;
            rootnode.Description = null;
            rootnode.ImageID = null;
            // Nice. We have now all the nodes so we have to split them out and create children for these.
            // This will be done in two passes:
            // -> The first pass adds all the nodes to the parent node.
            // -> The second pass groups all the nodes together.
            // Create a new list that will contain the indexes to remove after the pass has finished
            DebugProvider.WriteLine("SETTINGSTREEBUILDER: Building first tree layer...");
            // Enumerate through the parent id's
            for (System.Int32 I = 0; I < nds.Count; I++)
            {
                if (parentnds[I] == null)
                {
                    DebugProvider.WriteLine($"SETTINGSTREEBUILDER: Node {nds[I].NodeID} does not have a parent , adding it to the root element.");
                    rootnode.AddChildNode(nds[I]);
                    nds.RemoveAt(I);
                    parentnds.RemoveAt(I);
                    I = -1;
                }
            }
            DebugProvider.WriteLine($"SETTINGSTREEBUILDER: After first shot cleanup the temporary tree has remained with {nds.Count} nodes to add.");
            DebugProvider.WriteLine("SETTINGSTREEBUILDER: Building entire tree...");
            // Second pass now requires the parent node id's on the first object.
            BuilderHelpers.CreateTree(rootnode, nds, parentnds);
            DebugProvider.WriteLine($"SETTINGSTREEBUILDER: Done building the tree!");
            // At this point we must have left with 0 unused tree nodes.
            System.Diagnostics.Debug.Assert(nds.Count <= 0, $"For a very weird reason we have left with {nds.Count} unused nodes.");
            // We are done with the nodes 
            // Now the final step is to place all the settings tree nodes to their parents!!
            DebugProvider.WriteLine("SETTINGSTREEBUILDER: Writing settings nodes...");
            for (System.Int32 I = 0; I < settingsall.Count; I++)
            {
                if (parentnodeids[I] is null)
                {
                    // Directly add to the root node for perf
                    rootnode.AddNewSetting(settingsall[I]);
                }
                else
                {
                    var nd = BuilderHelpers.GetNode(rootnode, parentnodeids[I]);
                    System.Diagnostics.Debug.Assert(nd is not null, $"Cannot find the element with id {parentnodeids[I]}.");
                    nd.AddNewSetting(settingsall[I]);
                    nd = null;
                }
                parentnodeids.RemoveAt(I);
                settingsall.RemoveAt(I);
                // Reset index , search from the beginning
                I = -1;
            }
            DebugProvider.WriteLine("SETTINGSTREEBUILDER: Operation finished successfully.");
            parentnodeids = null;
            settingsall = null;
            // Done!
        }

        /// <summary>
        /// Gets the resources provider for this settings tree.
        /// </summary>
        [MaybeNull]
        public IResourcesProvider Resources
        {
            get => provider;
            [Throws(typeof(InvalidOperationException), typeof(ArgumentNullException))]
            set
            {
                ArgumentNullException.ThrowIfNull(value);
                if (provider is not null)
                {
                    throw new InvalidOperationException("A resource provider is already provided.");
                }
                provider = value;
            }
        }

        /// <summary>
        /// After the <see cref="Build"/> call has succeeded , you should see this property populated with the correct data.
        /// </summary>
        public SettingsTreeNode RootElement => rootnode;

        /// <summary>
        /// From a given setting id , it gets it's current value as reported by the given class object.
        /// </summary>
        /// <typeparam name="T">The type of the setting value to be retrieved.</typeparam>
        /// <param name="id">The name of the setting id to retrieve.</param>
        /// <returns>The setting's value.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="id"/> was null or empty.</exception>
        /// <exception cref="InvalidOperationException"><typeparamref name="T"/> is not the proper type for the setting to be retrieved , or the settings tree has not being built yet.</exception>
        /// <exception cref="SettingNotFoundException">The setting with name <paramref name="id"/> cannot be found.</exception>
        public T GetSettingValue<T>(System.String id)
        {
            if (System.String.IsNullOrEmpty(id)) { throw new ArgumentNullException(nameof(id)); }
            if (rootnode is null) { throw new InvalidOperationException("The tree must be built first."); }
            SettingsTreeSetting set = rootnode.GetSetting(id);
            if (set is null) { throw new SettingNotFoundException(id); }
            if (typeof(T) != set.TypeOfValue)
            {
                throw new InvalidOperationException($"The type to retrieve the value and the setting type do not match. \nExpected type: \"{set.TypeOfValue.AssemblyQualifiedName}\"");
            }
            foreach (var fd in reverselookup.GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public))
            {
                if (fd.Name == set.ID)
                {
                    return (T)fd.GetValue(settingsbase);
                }
            }
            foreach (var prop in reverselookup.GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public))
            {
                if (prop.Name == set.ID)
                {
                    return (T)prop.GetValue(settingsbase);
                }
            }
            throw new NotImplementedException("The code should not reach this point!");
        }

        /// <summary>
        /// Gets the current value of a specified setting.
        /// </summary>
        /// <typeparam name="T">The backing type of the setting value.</typeparam>
        /// <param name="set">The setting to get it's current value.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"><paramref name="set"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">The tree has not been built yet -or- the backing types are mismatching.</exception>
        /// <exception cref="NotImplementedException">Unexpected code path reached.</exception>
        [Throws(typeof(InvalidOperationException), typeof(ArgumentNullException))]
        public T GetSettingValue<T>(SettingsTreeSetting set)
        {
            if (set is null) { throw new ArgumentNullException(nameof(set)); }
            if (rootnode is null) { throw new InvalidOperationException("The tree must be built first."); }
            if (typeof(T) != set.TypeOfValue)
            {
                throw new InvalidOperationException($"The type to retrieve the value and the setting type do not match. \nExpected type: \"{set.TypeOfValue.AssemblyQualifiedName}\"");
            }
            foreach (var fd in reverselookup.GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public))
            {
                if (fd.Name == set.ID)
                {
                    return (T)fd.GetValue(settingsbase);
                }
            }
            foreach (var prop in reverselookup.GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public))
            {
                if (prop.Name == set.ID)
                {
                    return (T)prop.GetValue(settingsbase);
                }
            }
            throw new NotImplementedException("The code should not reach this point!");
        }

        /// <summary>
        /// Gets the current value of a specified setting.
        /// </summary>
        /// <param name="set">The setting to get it's current value.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"><paramref name="set"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">The tree has not been built yet.</exception>
        /// <exception cref="NotImplementedException">Unexpected code path reached.</exception>
        [Throws(typeof(InvalidOperationException), typeof(ArgumentNullException))]
        public System.Object GetSettingValueAsObject(SettingsTreeSetting set)
        {
            if (set is null) { throw new ArgumentNullException(nameof(set)); }
            if (rootnode is null) { throw new InvalidOperationException("The tree must be built first."); }
            foreach (var fd in reverselookup.GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public))
            {
                if (fd.Name == set.ID)
                {
                    return fd.GetValue(settingsbase);
                }
            }
            foreach (var prop in reverselookup.GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public))
            {
                if (prop.Name == set.ID)
                {
                    return prop.GetValue(settingsbase);
                }
            }
            throw new NotImplementedException("The code should not reach this point!");
        }

        /// <summary>
        /// From a given setting id , it updates the value to the given class object. <br />
        /// No additional validation is performed; this has to be done by your own code.
        /// </summary>
        /// <typeparam name="T">The type of the setting value to be set.</typeparam>
        /// <param name="id">The name of the setting id to be set.</param>
        /// <param name="value">The new setting value to set.</param>
        /// <exception cref="ArgumentNullException"><paramref name="id"/> was null or empty.</exception>
        /// <exception cref="InvalidOperationException"><typeparamref name="T"/> is not the proper type for the setting to be retrieved , or the settings tree has not being built yet.</exception>
        /// <exception cref="SettingNotFoundException">The setting with name <paramref name="id"/> cannot be found.</exception>
        [Throws(typeof(InvalidOperationException), typeof(ArgumentNullException), typeof(SettingNotFoundException))]
        public void SetSettingValue<T>(System.String id, T value)
        {
            if (System.String.IsNullOrEmpty(id)) { throw new ArgumentNullException(nameof(id)); }
            if (rootnode is null) { throw new InvalidOperationException("The tree must be built first."); }
            SettingsTreeSetting set = rootnode.GetSetting(id);
            if (set is null) { throw new SettingNotFoundException(id); }
            if (typeof(T) != set.TypeOfValue)
            {
                throw new InvalidOperationException($"The type to retrieve the value and the setting type do not match. \nExpected type: \"{set.TypeOfValue.AssemblyQualifiedName}\"");
            }
            foreach (var fd in reverselookup.GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public))
            {
                if (fd.Name == set.ID)
                {
                    fd.SetValue(settingsbase, value);
                    return;
                }
            }
            foreach (var prop in reverselookup.GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public))
            {
                if (prop.Name == set.ID)
                {
                    prop.SetValue(settingsbase, value);
                    return;
                }
            }
        }

        /// <summary>
        /// From a given setting, it updates the value to the given class object. <br />
        /// No additional validation is performed; this has to be done by your own code.
        /// </summary>
        /// <typeparam name="T">The type of the setting value to be set.</typeparam>
        /// <param name="set">The setting id to be set.</param>
        /// <param name="value">The new setting value to set.</param>
        /// <exception cref="ArgumentNullException"><paramref name="set"/> was null.</exception>
        /// <exception cref="InvalidOperationException"><typeparamref name="T"/> is not the proper type for the setting to be retrieved , or the settings tree has not being built yet.</exception>
        public void SetSettingValue<T>(SettingsTreeSetting set, T value)
        {
            if (set is null) { throw new ArgumentNullException(nameof(set)); }
            if (typeof(T) != set.TypeOfValue)
            {
                throw new InvalidOperationException($"The type to retrieve the value and the setting type do not match. \nExpected type: \"{set.TypeOfValue.AssemblyQualifiedName}\"");
            }
            foreach (var fd in reverselookup.GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public))
            {
                if (fd.Name == set.ID)
                {
                    fd.SetValue(settingsbase, value);
                    return;
                }
            }
            foreach (var prop in reverselookup.GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public))
            {
                if (prop.Name == set.ID)
                {
                    prop.SetValue(settingsbase, value);
                    return;
                }
            }
        }

        /// <summary>
        /// From a given setting, it updates the value to the given class object. <br />
        /// No additional validation is performed; this has to be done by your own code.
        /// </summary>
        /// <param name="set">The setting id to be set.</param>
        /// <param name="value">The new setting value to set.</param>
        /// <exception cref="ArgumentNullException"><paramref name="set"/> was null.</exception>
        public void SetSettingValueAsObject(SettingsTreeSetting set, System.Object value)
        {
            if (set is null) { throw new ArgumentNullException(nameof(set)); }
            foreach (var fd in reverselookup.GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public))
            {
                if (fd.Name == set.ID)
                {
                    fd.SetValue(settingsbase, value);
                    return;
                }
            }
            foreach (var prop in reverselookup.GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public))
            {
                if (prop.Name == set.ID)
                {
                    prop.SetValue(settingsbase, value);
                    return;
                }
            }
        }

        /// <summary>
        /// Releases all the data used by the current <see cref="SettingsTreeBuilder"/> instance.
        /// </summary>
        public void Dispose()
        {
            reverselookup = null;
            foreach (var s in this.GetAllSettings())
            {
                // A dynamic value list is always an IDisposable.
                // I use this pattern instead to avoid performing an additional null check.
                if (s.DynamicValuesList is IDisposable d) { d.Dispose(); }
            }
            rootnode = null;
            settingsall = null;
            settingsbase = null;
            provider?.Dispose();
            provider = null;
        }
    }
}