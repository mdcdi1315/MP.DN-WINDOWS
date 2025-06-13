
using System;
using System.Runtime.InteropServices;

partial class Interop
{
    unsafe partial class Kernel32
    {
        private const System.UInt32 HP_COMMON_HEAP_GENERATE_EXCEPTIONS = 0x00000004;
        private const System.UInt32 HP_COMMON_HEAP_NO_SERIALIZE = 0x00000001;
        private const System.UInt32 HP_COMMON_HEAP_ZERO_MEMORY = 0x00000008;

        [Flags]
        public enum HeapAllocFlags : System.UInt32
        {
            None = 0,
            HEAP_GENERATE_EXCEPTIONS = HP_COMMON_HEAP_GENERATE_EXCEPTIONS,
            HEAP_NO_SERIALIZE = HP_COMMON_HEAP_NO_SERIALIZE,
            HEAP_ZERO_MEMORY = HP_COMMON_HEAP_ZERO_MEMORY
        }

        [Flags]
        public enum HeapReAllocFlags : System.UInt32
        {
            None = 0,
            HEAP_GENERATE_EXCEPTIONS = HP_COMMON_HEAP_GENERATE_EXCEPTIONS,
            HEAP_NO_SERIALIZE = HP_COMMON_HEAP_NO_SERIALIZE,
            HEAP_ZERO_MEMORY = HP_COMMON_HEAP_ZERO_MEMORY,
            HEAP_REALLOC_IN_PLACE_ONLY = 0x00000010
        }

        [Flags]
        public enum HeapFreeFlags : System.UInt32
        {
            None = 0,
            HEAP_NO_SERIALIZE = HP_COMMON_HEAP_NO_SERIALIZE,
        }

        [Flags]
        public enum HeapCreateFlags : System.UInt32
        {
            None = 0,
            HEAP_GENERATE_EXCEPTIONS = HP_COMMON_HEAP_GENERATE_EXCEPTIONS,
            HEAP_NO_SERIALIZE = HP_COMMON_HEAP_NO_SERIALIZE,
            HEAP_CREATE_ENABLE_EXECUTE = 0x00040000
        }

        [Flags]
        public enum HeapValidateFlags : System.UInt32
        {
            None = 0,
            HEAP_NO_SERIALIZE = HP_COMMON_HEAP_NO_SERIALIZE,
        }

        [Flags]
        public enum LocalAllocFlags : System.UInt32
        {
            LMEM_FIXED = 0x0000,
            LHND = 0x0042,
            LMEM_ZEROINIT = 0x0040,
            LMEM_MOVEABLE = 0x0002,
        }

        [DllImport(Libraries.Kernel32 , ExactSpelling = true , SetLastError = true)]
        public static extern System.IntPtr GetProcessHeap();

        [DllImport(Libraries.Kernel32 , ExactSpelling = true)]
        public static extern void* HeapAlloc(System.IntPtr heap, HeapAllocFlags flags , System.UInt64 size);

        [DllImport(Libraries.Kernel32, ExactSpelling = true , SetLastError = true)]
        public static extern BOOL HeapFree(System.IntPtr heap, HeapFreeFlags flags , void* mem);

        [DllImport(Libraries.Kernel32, ExactSpelling = true)]
        public static extern void* HeapReAlloc(System.IntPtr heap, HeapReAllocFlags flags , void* mem , System.UInt64 size);

        [DllImport(Libraries.Kernel32, ExactSpelling = true , SetLastError = true)]
        public static extern BOOL HeapDestroy(System.IntPtr heap);

        [DllImport(Libraries.Kernel32, ExactSpelling = true , SetLastError = true)]
        public static extern System.IntPtr HeapCreate(HeapCreateFlags flags , System.UInt64 initialsize , System.UInt64 maximumsize);

        [DllImport(Libraries.Kernel32 , ExactSpelling = true)]
        public static extern BOOL HeapValidate(System.IntPtr heap , HeapValidateFlags flags , void* memonly = null);

        [DllImport(Libraries.Kernel32, ExactSpelling = true , SetLastError = true)]
        public static extern void* LocalAlloc(LocalAllocFlags flags, System.UInt64 memsize);

        [DllImport(Libraries.Kernel32, ExactSpelling = true, SetLastError = true)]
        public static extern void* LocalFree(void* pfree);
    }
}