

using System;

namespace MP.CGISettings
{
    /// <summary>
    /// Defines common extensions for the <see cref="ICGISettingsWriter{T}"/> interface.
    /// </summary>
    public static class CGISettingsReaderWriterExtensions
    {
        /// <summary>
        /// Adds a new CGI setting to the underlying CGI settings writer.
        /// </summary>
        /// <typeparam name="T">The type of the settings entry to be used to register the new setting entry.</typeparam>
        /// <param name="writer">The writer where the newly created setting will be added to.</param>
        /// <param name="name">The setting name for the setting to be saved under the writer and after retrieved back.</param>
        /// <param name="setvalue">The setting's value to save.</param>
        public static void AddSetting<T>(this ICGISettingsWriter<T> writer, System.String name, System.Object setvalue)
            where T : SettingEntry, new()
        {
            if (System.String.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }
            if (setvalue is null)
            {
                throw new ArgumentNullException(nameof(setvalue));
            }
            T tdata = new() { IsArrayOfType = false, Name = name };
            // Find whether the provided object is an array except from the byte array
            // which must be specially handled.
            if (setvalue.GetType().IsArray && setvalue is not System.Byte[])
            {
                tdata.IsArrayOfType = true;
                foreach (var v in setvalue as System.Array)
                {
                    tdata.ValueList.Add(v);
                }
            } else {
                tdata.IsArrayOfType = false;
                tdata.Value = setvalue;
            }
            writer.Add(tdata);
        }

        /// <summary>
        /// Adds a new CGI setting to the underlying CGI settings writer.
        /// </summary>
        /// <typeparam name="T">The type of the settings entry to be used to register the new setting entry.</typeparam>
        /// <param name="writer">The writer where the newly created setting will be added to.</param>
        /// <param name="name">The setting name for the setting to be saved under the writer and after retrieved back.</param>
        /// <param name="value">The setting's value as a string to save.</param>
        public static void AddSetting<T>(this ICGISettingsWriter<T> writer, System.String name, System.String value)
            where T : SettingEntry, new()
            => AddSetting(writer,name, setvalue: value);

        /// <summary>
        /// Adds a new CGI setting to the underlying CGI settings writer.
        /// </summary>
        /// <typeparam name="T">The type of the settings entry to be used to register the new setting entry.</typeparam>
        /// <param name="writer">The writer where the newly created setting will be added to.</param>
        /// <param name="name">The setting name for the setting to be saved under the writer and after retrieved back.</param>
        /// <param name="data">The setting's value as a byte array to save.</param>
        public static void AddSetting<T>(this ICGISettingsWriter<T> writer, System.String name, System.Byte[] data)
            where T : SettingEntry, new()
            => AddSetting(writer, name, setvalue: data);
    }
}