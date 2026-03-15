

using System;
using MP.Utilities;

namespace MP.CGISettings
{
    /// <summary>
    /// Defines a loader for writing settings found into a CGI Settings Loader class instance to the prespecified writer.
    /// </summary>
    /// <typeparam name="T">The type of the setting entries to write.</typeparam>
    public sealed class CGISettingsWriterLoader<T> : IDisposable
        where T : SettingEntry, new()
    {
        private ICGISettingsWriter<T> writer;

        /// <summary>
        /// Creates a new instance of the <see cref="CGISettingsWriterLoader{T}"/> class from the specified 
        /// writer which will finally recieve all the settings to be written to the underlying data source.
        /// </summary>
        /// <param name="writer">The writer to write the found settings into.</param>
        /// <exception cref="ArgumentNullException"><paramref name="writer"/> was <see langword="null"/>.</exception>
        public CGISettingsWriterLoader(ICGISettingsWriter<T> writer)
        {
            if (writer is null) {
                throw new ArgumentNullException(nameof(writer));
            }
            this.writer = writer;
        }

        private void SetProperties(System.Reflection.PropertyInfo[] p , System.Object co)
        {
            foreach (var prop in p) 
            {
                DebugProvider.WriteLine($"CGISettingsWriterLoader: Attempting to write the contents of the property named as {prop.Name}...");
                if (prop.HasAttribute(typeof(CGISettingsLoaderIgnoreAttribute)))
                {
                    DebugProvider.WriteLine($"CGISettingsWriterLoader: The user does not want to save the property named as {prop.Name}. Skipping.");
                    continue;
                }
                if (prop.CanWrite && prop.CanRead) {
                    System.Object val = prop.GetValue(co);
                    if (val is null)
                    {
                        throw new InvalidOperationException("Attempted to inject a null value into a setting. This is not allowed.");
                    }
                    writer.AddSetting(prop.Name, val);
                } else {
                    DebugProvider.WriteLine($"CGISettingsWriterLoader: Property named as {prop.Name} is not both gettable and settable. Skipping generation for this property.");
                }
            }
        }

        private void SetFields(System.Reflection.FieldInfo[] f, System.Object co)
        {
            foreach (var fld in f)
            {
                DebugProvider.WriteLine($"CGISettingsWriterLoader: Attempting to write the contents of the field named as {fld.Name}...");
                if (fld.HasAttribute(typeof(CGISettingsLoaderIgnoreAttribute)))
                {
                    DebugProvider.WriteLine($"CGISettingsWriterLoader: The user does not want to save the field named as {fld.Name}. Skipping.");
                    continue;
                }
                System.Object val = fld.GetValue(co);
                if (val is null)
                {
                    throw new InvalidOperationException("Attempted to inject a null value into a setting. This is not allowed.");
                }
                writer.AddSetting(fld.Name, val);
            }
        }

        /// <summary>
        /// Saves the CGI settings to the provided writer from the provided CGI settings loading class instance.
        /// </summary>
        /// <param name="settingimplclass">The CGI settings loading class to write it's values to the provided writer.</param>
        /// <exception cref="ArgumentNullException"><paramref name="settingimplclass"/> was <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="settingimplclass"/> instance was not decorated with <see cref="CGISettingsLoaderClassAttribute"/>.</exception>
        /// <exception cref="InvalidOperationException">Attempted to create a setting which would have as it's value null.</exception>
        public void WriteFromClassInstance(System.Object settingimplclass)
        {
            ObjectDisposedException.ThrowIf(writer is null , this);
            if (settingimplclass is null) {
                throw new ArgumentNullException(nameof(settingimplclass));
            }
            System.Type t = settingimplclass.GetType();
            if (t.HasAttribute(typeof(CGISettingsLoaderClassAttribute)) == false) {
                throw new ArgumentException("The current object instance is not a CGI Settings Loader class.");
            }
            SetProperties(t.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance), settingimplclass);
            SetFields(t.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance), settingimplclass);
        }

        /// <summary>
        /// Retrieves the CGI Settings Writer object that is associated with this instance.
        /// </summary>
        public ICGISettingsWriter<T> Writer => writer;

        /// <summary>
        /// Disposes this <see cref="CGISettingsWriterLoader{T}"/> instance.
        /// </summary>
        public void Dispose()
        {
            writer?.Dispose();
            writer = null;
        }
    }
}