
using System;
using MP.Utilities;
using System.Collections.Generic;

namespace MP.CGISettings
{
    /// <summary>
    /// Defines a loader for loading CGI Settings into classes that implement the CGI Settings Loader Class logic.
    /// </summary>
    /// <typeparam name="T">The type of the setting entry that the provided reader produces.</typeparam>
    public sealed class CGISettingsReaderLoader<T> : IDisposable
        where T : SettingEntry
    {
        private ICGISettingsReader<T> reader;

        private static void NotLoadedSettingDummyFunction(T entry) { }

        private static void NotPresentInSourceDummyFunction(System.String entryname) { }

        private static System.Boolean TryGetSettingEntryWithNameAndRemove(IList<T> ents, System.String name, out T entry)
        {
            entry = null;
            for (System.Int32 I = 0; I < ents.Count; I++)
            {
                if (ents[I].Name == name) {
                    entry = ents[I];
                    ents.RemoveAt(I);
                    return true;
                }
            }
            return false;
        }

        private static Array ToArray(IList<System.Object> objlist , Type t)
        {
            Array arr = Array.CreateInstance(t, objlist.Count);
            for (System.Int32 I = 0; I < objlist.Count; I++)
            {
                // InvalidCastException will be thrown if one of the objects in the list is not of type 't'.
                arr.SetValue(objlist[I], I);
            }
            // The returned object can be then casted as TYPE[].
            return arr;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="CGISettingsReaderLoader{T}"/> class with the specified settings reader to use.
        /// </summary>
        /// <param name="reader">The settings reader to use.</param>
        /// <exception cref="ArgumentNullException"><paramref name="reader"/> was <see langword="null"/>.</exception>
        public CGISettingsReaderLoader(ICGISettingsReader<T> reader)
        {
            if (reader is null) {
                throw new ArgumentNullException(nameof(reader));
            }
            this.reader = reader;
            CGISettingNotLoaded = new(NotLoadedSettingDummyFunction);
            CGISettingNotPresentInSource = new(NotPresentInSourceDummyFunction);
        }

        private void LoadProperties(System.Reflection.PropertyInfo[] p , IList<T> loadedents , System.Object co)
        {
            foreach (var prop in p) 
            {
                DebugProvider.WriteLine($"CGISettingsReaderLoader: Filling in the property {prop.Name} ...");
                if (prop.HasAttribute(typeof(CGISettingsLoaderIgnoreAttribute)))
                {
                    DebugProvider.WriteLine($"CGISettingsReaderLoader: User does not want to fill in the property named as {prop.Name}. Skipping.");
                    continue;
                }
                if (prop.CanRead && prop.CanWrite) {
                    T se;
                    if (TryGetSettingEntryWithNameAndRemove(loadedents, prop.Name, out se)) {
                        try {
                            if (se.IsArrayOfType) {
                                prop.SetValue(co, ToArray(se.ValueList , se.Value.GetType()));
                            } else {
                                prop.SetValue(co, se.Value);
                            }
                        } catch (System.Exception e) {
                            DebugProvider.WriteLine($"CGISettingsReaderLoader: Capturing exception: {e}");
                            throw new AggregateException(e);
                        }
                    } else {
                        DebugProvider.WriteLine($"CGISettingsReaderLoader: Cannot find the associated setting in the setting list, invoking the event.");
                        CGISettingNotPresentInSource.Invoke(prop.Name);
                    }
                } else {
                    DebugProvider.WriteLine($"CGISettingsReaderLoader: The property with name {prop.Name} was not both gettable and settable, rejecting this property.");
                }
            }
        }

        private void LoadFields(System.Reflection.FieldInfo[] f , IList<T> loadedents , System.Object co) 
        {
            foreach (var fld in f)
            {
                DebugProvider.WriteLine($"CGISettingsReaderLoader: Filling in the field {fld.Name} ...");
                if (fld.HasAttribute(typeof(CGISettingsLoaderIgnoreAttribute)))
                {
                    DebugProvider.WriteLine($"CGISettingsReaderLoader: User does not want to fill in the field named as {fld.Name}. Skipping.");
                    continue;
                }
                T se;
                if (TryGetSettingEntryWithNameAndRemove(loadedents, fld.Name, out se))
                {
                    if (se.IsArrayOfType) {
                        fld.SetValue(co, ToArray(se.ValueList, se.Value.GetType()));
                    } else {
                        fld.SetValue(co, se.Value);
                    }
                } else {
                    DebugProvider.WriteLine($"CGISettingsReaderLoader: Cannot find the associated setting in the setting list, invoking the event.");
                    CGISettingNotPresentInSource.Invoke(fld.Name);
                }
            }
        }

        /// <summary>
        /// Loads the retrieved CGI settings from the provided reader into the provided CGI settings loading class instance.
        /// </summary>
        /// <param name="settingimplclass">The CGI settings loading class instance to fill it's fields with values from the settings reader.</param>
        /// <exception cref="ArgumentNullException"><paramref name="settingimplclass"/> was <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="settingimplclass"/> instance was not decorated with <see cref="CGISettingsLoaderClassAttribute"/>.</exception>
        /// <exception cref="AggregateException">While setting a setting in the properties, a property setter threw an exception.</exception>
        public void LoadClassInstance(System.Object settingimplclass)
        {
            ObjectDisposedException.ThrowIf(reader is null, this);
            if (settingimplclass is null) { 
                throw new ArgumentNullException(nameof(settingimplclass));
            }
            System.Type t = settingimplclass.GetType();
            if (t.HasAttribute(typeof(CGISettingsLoaderClassAttribute)) == false) {
                throw new ArgumentException("The current object instance is not a CGI Settings Loader class.");
            }
            List<T> loadedsets = new List<T>(30);
            foreach (var ent in reader) { loadedsets.Add(ent); }
            LoadProperties(t.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance) , loadedsets , settingimplclass);
            LoadFields(t.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance), loadedsets, settingimplclass);
            // Invoke CGISettingNotLoaded for those settings that did not had an associated property or field.
            foreach (T set in loadedsets) { CGISettingNotLoaded.Invoke(set); }
            loadedsets.Clear();
            loadedsets = null;
        }

        /// <summary>
        /// Retrieves the CGI Settings Reader object that is associated with this instance.
        /// </summary>
        public ICGISettingsReader<T> Reader => reader;

        /// <summary>
        /// Fired up when the specified CGI setting could not be loaded by the lastly provided class instance.
        /// </summary>
        public event CGISettingNotLoadedDelegate<T> CGISettingNotLoaded;

        /// <summary>
        /// Fired up when there is a valid CGI setting in the settings loader class but the loader cannot find an associated setting from the source.
        /// </summary>
        public event CGISettingNotFilledInDelegate CGISettingNotPresentInSource;

        /// <summary>
        /// Disposes this <see cref="CGISettingsReaderLoader{T}"/> instance.
        /// </summary>
        public void Dispose()
        {
            reader?.Dispose();
            reader = null;
        }
    }
}