
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.Marshalling;

namespace MP.GenericInterop
{
    [CustomMarshaller(typeof(System.String) , MarshalMode.Default , typeof(Utf16LeStringMarshaller))]
    public static unsafe class Utf16LeStringMarshaller
    {
        public static System.Char* ConvertToUnmanaged(string managed)
        {
            if (managed is null) { return null; }
            IMemoryHandle memory = NativeMemoryManager.GetHandle(managed.Length * sizeof(System.Char));
            if (managed.Length > 0) 
            {
                fixed (System.Char* pptr = managed)
                {
                    Unsafe.CopyBlockUnaligned(memory.MemoryPointer, pptr, memory.MemoryLength.ToUInt32());
                }
            }
            return (System.Char*)memory.MemoryPointer;
        }

        public static void Free(System.Char* unmanaged) => NativeMemoryManager.ReleaseHandle(unmanaged);

        public static string ConvertToManaged(System.Char* unmanaged)
        {
            if (unmanaged is null) { return null; }
            return new(unmanaged);
        }
    }
}