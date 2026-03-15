

# The CGI Settings loader classes.

The CGI Settings Loader classes assist the programmer to load and store custom setting classes that you already use in your app by just selecting the fields and the properties you want to use and boom, you have a CGI Settings Loader Class.

The Settings Loader classes are using behind-the-scenes .NET reflection API's to communicate with the actual class object.

Advantages:
- Minimal code refactoring if used on an existing class by just appending some attributes
- Supports all the readers and writers implementing the low-level CGI settings interfaces
- Can store fields of custom types if a CGI settings extension for the type has been registered 
- Does not require specific constructor layouts. You provide the object instance to the loader classes, whatever that instance is.

## What is a CGI Settings Loader Class? What kind of class is considered a CGI Settings Loader Class?

All classes marked with the [CGISettingsLoaderClassAttribute](../../MP/CGISettings/LoaderAttributes.cs#L11) attribute are considered valid CGI Settings Loader Classes. 
However, when the CGI Settings Loader is invoked for a particular class, it considers the following members as valid setting candidates:
- All `public` fields
- All `public` properties that have both `get` and `set` accessors.
- All the above members without having the [CGISettingsLoaderIgnoreAttribute](../../MP/CGISettings/LoaderAttributes.cs#L18) marked on them.

The [CGISettingsLoaderIgnoreAttribute](../../MP/CGISettings/LoaderAttributes.cs#L18) attribute allows to the user to instruct the CGI Settings Loader to not interpret the marked setting as a valid candidate, if it is one. You typically use this on fields where they have a custom type and they are valid for the particular settings object instance only.

Note that, if no fields and properties satisfy the above statements, no settings data will be written to or read from.

However, when reading data from a class but a referenced setting has been deleted from the CGI Settings Loader class, the [CGISettingNotLoaded](../../MP/CGISettings/CGISettingsReaderLoader.cs#L155) event will be fired for these settings that are now orphaned.

## An example CGI Settings Loader Class

~~~C#

using MP.CGISettings;

namespace ANamespace
{
    [CGISettingsLoaderClass]
    public class SettingsClass 
    {
        public int NameColumnWidth; // Valid candidate

        public int NameColumnHeight; // This is also valid too.

        // The below setting WILL NOT BE INCLUDED in the setting list to be written to and read from.
        private double fractional;

        // The same with this one too...
        private int valuemodified;

        [CGISettingsLoaderIgnore]
        public long ALongSetting; // Although valid, the user has explicitly said that this shall not be included in the settings list.

        // Although a public property, this property does also need a set accessor.
        public string DefaultFormat
        {
            get => "";
        }

        // This is however, valid.
        public int ModifidableValue
        {
            get => valuemodified;
            set => valuemodified = value;
        }
    }
}

~~~