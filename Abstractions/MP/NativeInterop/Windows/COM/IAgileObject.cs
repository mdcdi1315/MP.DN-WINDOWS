
using MP.Annotations;
using System.Runtime.Versioning;

namespace MP.NativeInterop.Windows.COM
{
    /// <summary>
    /// Marker interface for indicating to COM that the object implementing this interface is apartment-free.
    /// </summary>
    [SupportedOSPlatform(WindowsVersions.NTDDI_WIN8)]
    [COMInterfaceGenerator("94EA2B94-E9CC-49E0-C0FF-EE64CA8F5B90")]
    public partial interface IAgileObject {}
}