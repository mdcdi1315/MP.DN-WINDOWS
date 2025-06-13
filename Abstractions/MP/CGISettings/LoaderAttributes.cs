
using System;

namespace MP.CGISettings
{
    /// <summary>
    /// When defined on any class definition, it marks it valid for the CGI Settings Loader to fill in this class with values obtained from a <see cref="ICGISettingsReader{T}"/>.
    /// or to write it's data to a <see cref="ICGISettingsWriter{T}"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class , AllowMultiple = false)]
    public sealed class CGISettingsLoaderClassAttribute : Attribute { }

    /// <summary>
    /// Mandates to the loader not to write or load a value for this field or property. <br />
    /// This is valid , when for example , the field or property in question is filled in by the application at run-time.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public sealed class CGISettingsLoaderIgnoreAttribute : Attribute { }
}