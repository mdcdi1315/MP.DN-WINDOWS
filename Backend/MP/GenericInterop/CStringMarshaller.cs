

using System.Runtime.InteropServices.Marshalling;

namespace MP.GenericInterop
{
    [CustomMarshaller(typeof(System.String) , MarshalMode.Default , typeof(CStringMarshaller))]
    public static unsafe class CStringMarshaller
    {
        public static System.Byte* ConvertToUnmanaged(string managed)
        {
            if (managed is null) { return null; }
            IMemoryHandle handle = NativeMemoryManager.GetHandle(managed.Length * sizeof(System.Byte));
            for (System.Int32 I = 0; I < managed.Length; I++) 
            {
                if (managed[I] <= 255) {
                    handle[I] = managed[I].ToByte();
                } else {
                    handle[I] = 63; // Places the ASCII '?' character instead.
                }
            }
            return handle.MemoryPointer;
        }

        public static string ConvertToManaged(System.Byte* unmanaged)
        {
            if (unmanaged is null) { return null; }
            return new((System.SByte*)unmanaged);
        }

        public static void Free(System.Byte* unmanaged) => NativeMemoryManager.ReleaseHandle(unmanaged);
    }
}