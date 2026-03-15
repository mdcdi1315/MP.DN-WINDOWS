
using MP.Annotations;

namespace MP.NativeInterop.Windows.COM
{
    [COMInterfaceGenerator(CommonInteropClsIds.IID_IPersistStream, typeof(IPersist))]
    public unsafe partial interface IPersistStream
    {
        public HRESULT IsDirty();

        public HRESULT Load([IsPointerToCOMInterfaceType(typeof(IStream))] void* ppstm); // ppstm must be an IStream* pointer

        public HRESULT Save([IsPointerToCOMInterfaceType(typeof(IStream))] void* ppstm , BOOL fcleardirty); // ppstm must be an IStream* pointer

        public HRESULT GetSizeMax(ulong* size);
    }
}
