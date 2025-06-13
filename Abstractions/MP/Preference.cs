

using System;

namespace MP
{
    /// <summary>
    /// Stores and holds a playlist preference.
    /// </summary>
    public sealed class Preference
    {
        private readonly System.String name;
        private readonly System.Object value;
        private readonly System.String desc;
        private readonly PreferenceValueType type;
        private readonly PreferenceBehaviorFlags flags;

        private static void ValidateName(System.String name)
        {
            System.Boolean valid = true;
            foreach (System.Char c in name)
            {
                if (c == '_' || ((System.UInt32)(c - '0') <= ('9' - '0'))) {
                    continue;
                } else if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z')) {
                    continue;
                } else {
                    valid = false;
                    break;
                }
            }
            if (valid == false) {
                throw new System.FormatException("The name must consist only of Engilsh letters , digits or underscores.");
            }
        }

        /// <summary>
        /// Creates a new instance from a preference serialized string. This string is returned by the <see cref="ToString"/> method.
        /// </summary>
        /// <param name="serializedstring">The serialized string to parse.</param>
        [Annotations.DeprecatedMayBeRemoved]
        public Preference(System.String serializedstring)
        {
            System.Int32 idx1 = serializedstring.IndexOf('['),
                idx2 = serializedstring.IndexOf(']');
            type = System.Enum.Parse<PreferenceValueType>(serializedstring.Substring(idx1 + 1, (idx2 - idx1) - 1));
            idx1 = idx2;
            idx2 = serializedstring.IndexOf('{');
            name = serializedstring.Substring(idx1 + 1, (idx2 - idx1) - 1);
            ValidateName(name);
            idx1 = idx2;
            idx2 = serializedstring.IndexOf('}');
            desc = serializedstring.Substring(idx1 + 1, (idx2 - idx1) - 1);
            idx1 = serializedstring.Substring(idx2).IndexOf(':') + idx2;
            value = serializedstring.Substring(idx1 + 1, (serializedstring.Length - idx1) - 1);
        }

        /// <summary>
        /// Creates a new preference from the specified name and value.
        /// </summary>
        /// <param name="Name">The name of the new preference.</param>
        /// <param name="Value">The value of the new preference.</param>
        public Preference(System.String Name, System.String Value)
        {
            name = Name;
            value = Value;
            type = PreferenceValueType.String;
            flags = PreferenceBehaviorFlags.None;
        }

        /// <summary>
        /// Creates a new preference from the specified name and value,
        /// and a description string that describes the preference.
        /// </summary>
        /// <param name="Name">The name of the new preference.</param>
        /// <param name="Value">The value of the new preference.</param>
        /// <param name="Description">A textual description of the preference.</param>
        public Preference(System.String Name, System.String Value, System.String Description) : this(Name, Value)
        {
            desc = Description;
        }

        /// <summary>
        /// Creates a new preference from the specified name and value.
        /// </summary>
        /// <param name="Name">The name of the new preference.</param>
        /// <param name="Value">The value of the new preference.</param>
        public Preference(System.String Name, System.Int64 Value)
        {
            name = Name;
            ValidateName(name);
            value = Value;
            type = PreferenceValueType.Number;
            flags = PreferenceBehaviorFlags.None;
        }

        /// <summary>
        /// Creates a new preference from the specified name and value,
        /// and a description string that describes the preference.
        /// </summary>
        /// <param name="Name">The name of the new preference.</param>
        /// <param name="Value">The value of the new preference.</param>
        /// <param name="Description">A textual description of the preference.</param>
        public Preference(System.String Name, System.Int64 Value, System.String Description) : this(Name, Value)
        {
            desc = Description;
        }

        /// <summary>
        /// Creates a new preference from the specified name and value.
        /// </summary>
        /// <param name="Name">The name of the new preference.</param>
        /// <param name="Value">The value of the new preference.</param>
        public Preference(System.String Name, System.Boolean Value)
        {
            name = Name;
            ValidateName(name);
            value = Value;
            type = PreferenceValueType.Boolean;
            flags = PreferenceBehaviorFlags.None;
        }

        /// <summary>
        /// Creates a new preference from the specified name and value,
        /// and a description string that describes the preference.
        /// </summary>
        /// <param name="Name">The name of the new preference.</param>
        /// <param name="Value">The value of the new preference.</param>
        /// <param name="Description">A textual description of the preference.</param>
        public Preference(System.String Name, System.Boolean Value, System.String Description) : this(Name, Value)
        {
            desc = Description;
        }

        /// <summary>
        /// Creates a new preference from the specified name and value,
        /// and a description string that describes the preference.
        /// </summary>
        /// <param name="Name">The name of the new preference.</param>
        /// <param name="Value">The value of the new preference.</param>
        /// <param name="Description">A textual description of the preference.</param>
        /// <param name="type">The type of the new preference that the <see cref="Value"/> will hold.</param>
        /// <param name="flags">Additional flags that modify the behavior of the current Preference.</param>
        public Preference(System.String Name, System.Object Value, System.String Description, PreferenceValueType type , PreferenceBehaviorFlags flags = PreferenceBehaviorFlags.None)
        {
            name = Name;
            ValidateName(name);
            value = Value;
            this.type = type;
            desc = Description;
            this.flags = flags;
        }

        /// <summary>Gets the preference's name.</summary>
        public System.String Name => name;

        /// <summary>Gets the preference's value.</summary>
        public System.Object Value => value;

        /// <summary>Gets the preference's textual description.</summary>
        public System.String Description => desc;

        /// <summary>Gets the preference's value as a boolean.</summary>
        public System.Boolean BooleanValue => (System.Boolean)value;

        /// <summary>Gets the preference's as a numeric value.</summary>
        public System.Int64 NumericValue => (System.Int64)value;

        /// <summary>Gets the type of the value contained in the <see cref="Value"/> property.</summary>
        public PreferenceValueType TypeOfValue => type;

        /// <summary>Gets flags that may affect the behavior of the current <see cref="Preference"/>.</summary>
        public PreferenceBehaviorFlags Flags => flags;

        /// <summary>
        /// Gets a serialized string that is equal to the class contents. <br />
        /// Pass it to <see cref="Preference(string)"/> constructor to get back this structure.
        /// </summary>
        public override string ToString() => $"[{type}]{name}{{{desc}}}:{value}";
    }

    /// <summary>
    /// Represents common constants upon the value saved in a specified preference.
    /// </summary>
    public enum PreferenceValueType : System.Byte
    {
        /// <summary>The <see cref="Preference.Value"/> property represents a string of UTF16LE characters.</summary>
        String,
        /// <summary>The <see cref="Preference.Value"/> property represents a numeric value.</summary>
        Number,
        /// <summary>The <see cref="Preference.Value"/> property represents a boolean (<see langword="true"/> or <see langword="false"/>).</summary>
        Boolean
    }

    /// <summary>
    /// Defines flags that modify the behavior of a <see cref="Preference"/>.
    /// </summary>
    [Flags]
    public enum PreferenceBehaviorFlags : System.Byte
    {
        /// <summary>No additional flags defined.</summary>
        None,
        /// <summary>The specified string value is a file-path.</summary>
        FilePath = 1,
        /// <summary>The specified numeric value should be only positive.</summary>
        NumberShouldBePositiveOnly = 2,

    }

}