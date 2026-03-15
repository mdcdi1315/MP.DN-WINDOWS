
using System.Diagnostics;
using Microsoft.Win32.SafeHandles;
using System.Diagnostics.CodeAnalysis;

namespace MP.NativeInterop
{
    unsafe partial class MemoryUtils
    {
        private abstract class SafeDefaultBaseMemHandle : SafeBaseMemoryHandle
        {
            private FreeFunction ff;
            private readonly ulong size;

            protected delegate bool FreeFunction([AllowNull] void* ptr);

            [StackTraceHidden]
            protected SafeDefaultBaseMemHandle(FreeFunction ff, ulong size) : base()
            {
                this.ff = ff;
                this.size = size;
            }

            public sealed override ulong MemoryLength => size;

            protected sealed override bool ReleaseHandle()
            {
                if (ff is null) {
                    return false;
                } else {
                    bool value = ff(handle.ToPointer());
                    ff = null;
                    return value;
                }
            }
        }

        private sealed class SafeNonAlignedMemHandle : SafeDefaultBaseMemHandle
        {
            [StackTraceHidden]
            public SafeNonAlignedMemHandle(INativeMemoryManager manager, ulong size)
                : base(new(manager.Free), size) 
                => handle = new(manager.Allocate(size));
        }

        private sealed class SafeAlignedMemHandle : SafeDefaultBaseMemHandle
        {
            [StackTraceHidden]
            public SafeAlignedMemHandle(INativeMemoryManager manager, ulong size, uint align) 
                : base(new(manager.FreeAligned), size)
                => handle = new(manager.AllocateAligned(size, align));
        }

        [StackTraceHidden]
        public static IMemoryHandle CreateDefaultNonAlignedHandle(INativeMemoryManager manager, ulong size) => new SafeNonAlignedMemHandle(manager, size);

        [StackTraceHidden]
        public static IMemoryHandle CreateDefaultAlignedHandle(INativeMemoryManager manager, ulong size, uint align) => new SafeAlignedMemHandle(manager, size, align);
    }
 }