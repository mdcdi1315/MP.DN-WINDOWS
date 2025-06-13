using System;

namespace MP
{
    public static class PlaylistPrefsExtensions
    {
        /// <summary>
        /// Initializes a new instance of <see cref="PlaylistPreferences"/> class from the specified JSON element array that contains all 
        /// the preferences to load.
        /// </summary>
        /// <param name="array">The JSON array to take the elements from.</param>
        /// <exception cref="ArgumentException"><paramref name="array"/> was not a JSON array that contains strings.</exception>
        public static PlaylistPreferences CreateFrom(System.Text.Json.JsonElement array)
        {
            PlaylistPreferences pf = new();
            if (array.ValueKind != System.Text.Json.JsonValueKind.Array)
            {
                goto _ARG_EX_ARRAY;
            }
            System.Int32 c = array.GetArrayLength();
            for (System.Int32 I = 0; I < c; I++)
            {
                if (array[I].ValueKind != System.Text.Json.JsonValueKind.String) { goto _ARG_EX_ARRAY; }
                pf.Add(new(array[I].GetString()));
            }
            return pf;
        _ARG_EX_ARRAY:
            throw new ArgumentException("array must be a JSON array element that contains strings.", nameof(array));
        }

        /// <summary>
        /// Writes to the specified <see cref="System.Text.Json.Utf8JsonWriter"/> instance the current preferences.
        /// </summary>
        /// <param name="writer">The writer target to write the preferences to.</param>
        /// <param name="prefsavename">The new element name that all the preferences will be saved to.</param>
        public static void WriteToJSON(this PlaylistPreferences prfs, System.Text.Json.Utf8JsonWriter writer, System.String prefsavename)
        {
            writer.WriteStartArray(prefsavename);
            for (System.Int32 I = 0; I < prfs.Count; I++) 
            {
                writer.WriteStringValue(prfs[I].ToString());
            }
            writer.WriteEndArray();
        }
    }
}
