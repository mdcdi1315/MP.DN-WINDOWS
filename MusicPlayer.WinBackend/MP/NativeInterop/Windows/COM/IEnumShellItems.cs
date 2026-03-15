
using MP.Annotations;

namespace MP.NativeInterop.Windows.COM
{
    [COMInterfaceGenerator(CommonInteropClsIds.IID_IEnumShellItems)]
    public unsafe partial interface IEnumShellItems
    {
        public HRESULT Next(System.UInt32 celt , void** rgelt , System.UInt32 pceltfetched);

        public HRESULT Skip(System.UInt32 celt);

        public HRESULT Reset();

        public HRESULT Clone(void** ppenum);
    }
}