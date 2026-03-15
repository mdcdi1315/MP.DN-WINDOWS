
using MP.Collections;
using System.Collections.Generic;

namespace MP.CGISettings
{
    /// <summary>
    /// Defines a single CGI setting entry. <br />
    /// The class can be extended, if the user does require it.
    /// </summary>
    public class SettingEntry
    {
        /// <summary>
        /// Contains the name of the current CGI setting entry.
        /// </summary>
        public System.String Name;
        /// <summary>
        /// Contains all the values that this setting does hold , or a single one if this entry is an older one or has a single value.
        /// </summary>
        public IList<System.Object> ValueList;
        /// <summary>
        /// Defines the effective setting type of this CGI setting.
        /// </summary>
        public CGISettingType Type;
        /// <summary>
        /// Defines whether the setting entry acts as a simple entry with a value , or that it has multiple values.
        /// </summary>
        public System.Boolean IsArrayOfType;

        /// <summary>
        /// Creates a new instance of the <see cref="SettingEntry"/> class.
        /// </summary>
        public SettingEntry()
        {
            Name = null;
            Type = 0;
            ValueList = new ArrayBasedList<System.Object>();
            IsArrayOfType = false;
        }

        /// <summary>
        /// Now anymore a shorthand for ValueList[0]. <br />
        /// Returns the effective value of this entry when the entry is a simple entry
        /// (that is, an entry whose <see cref="IsArrayOfType"/> field is <see langword="false"/>.)<br />
        /// If it is an array entry, then it returns the first element from that array.
        /// </summary>
        public System.Object Value
        {
            get {
                if (ValueList.Count == 0) {
                    ValueList.Add(null);
                    return null;
                } else {
                    return ValueList[0];
                }
            }
            set {
                if (ValueList.Count == 0) { 
                    ValueList.Add(value); 
                } else { 
                    ValueList[0] = value; 
                }
            }
        }

        /// <summary>
        /// Gets a textual description of this CGI Setting Entry.
        /// </summary>
        /// <returns>A textual description of the current CGI Setting Entry.</returns>
        public override string ToString() => $"CGI Setting Entry {{ Name: {Name}, Type: {Type}, Values: {ValueList} }}";
    }

}