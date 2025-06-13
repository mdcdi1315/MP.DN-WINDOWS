
using System.Runtime.InteropServices;

namespace MP.ComInterop
{
    [ComImport]
    [Guid(CommonInteropClsIds.IID_IAgileObject)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IAgileObject { }
}