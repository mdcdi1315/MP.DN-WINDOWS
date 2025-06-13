
using System.Collections.Generic;
using Microsoft.Win32.SafeHandles;

namespace MP.GenericInterop
{
    public static unsafe class NativeMemoryManager
    {
        private static List<IMemoryHandle> handles;

        static NativeMemoryManager() {
            handles = new(20);
        }

        public static IMemoryHandle GetHandle(System.Int32 sizeinbytes)
        {
            SafeLibcMemoryHandle memory = new(sizeinbytes);
            handles.Add(memory);
            return memory;
        }

        public static IMemoryHandle Identify(void* memory)
        {
            if (memory is null) { return null; }
            for (System.Int32 I = 0; I < handles.Count; I++) 
            {
                if (handles[I].MemoryPointer == memory) 
                {
                    return handles[I];
                }
            }
            return null;
        }

        public static void ReleaseHandle(IMemoryHandle memory)
        {
            if (memory is null) { return; }
            ReleaseHandle(memory.MemoryPointer);
        }

        public static void ReleaseHandle(System.IntPtr memory) => ReleaseHandle(memory.ToPointer());

        public static void ReleaseHandle(void* memory)
        {
            if (memory is null) { return; }
            for (System.Int32 I = 0; I < handles.Count; I++)
            {
                if (handles[I].MemoryPointer == memory)
                {
                    handles[I].Dispose();
                    handles[I] = null;
                    handles.RemoveAt(I);
                    break;
                }
            }
        }
    }
}