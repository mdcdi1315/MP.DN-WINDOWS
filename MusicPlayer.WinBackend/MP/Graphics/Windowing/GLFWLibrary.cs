
using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace MP.Graphics.Windowing
{
    internal static unsafe class GLFWLibrary
    {
        private static volatile System.UInt32 library_ref_count;
        private static readonly Interop.GLFW.GLFW_ALLOCATOR allocator;

        static GLFWLibrary() 
        {
            library_ref_count = 0U;
            allocator = new() {
                Free = &Allocator_Free,
                Allocate = &Allocator_Allocate,
                Reallocate = &Allocator_Reallocate
            };
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        private static void* Allocator_Allocate(UIntPtr size, void* user) => SystemInfo.GetDefaultMemoryManager().Allocate(size);

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        private static void Allocator_Free(void* block, void* user) => SystemInfo.GetDefaultMemoryManager().Free(block);

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        private static void* Allocator_Reallocate(void* block, UIntPtr size, void* user) => SystemInfo.GetDefaultMemoryManager().ReAllocate(block, size);

        public static void AddReference()
        {
            if (library_ref_count++ == 0U)
            {
                DebugProvider.WriteLine($"GLFW Library: {Interop.GLFW.glfwGetVersionString()}");
                Interop.GLFW.glfwInitAllocator(allocator);
                Interop.GLFW.glfwInitHint(Interop.GLFW.GLFW_INIT_HINT.GLFW_COCOA_MENUBAR, (int)Interop.GLFW.GLFW_BOOLEAN.GLFW_FALSE);
                Interop.GLFW.glfwInitHint(Interop.GLFW.GLFW_INIT_HINT.GLFW_PLATFORM, (int)Interop.GLFW.GLFW_PLATFORM.GLFW_ANY_PLATFORM);
                Interop.GLFW.glfwInitHint(Interop.GLFW.GLFW_INIT_HINT.GLFW_COCOA_CHDIR_RESOURCES, (int)Interop.GLFW.GLFW_BOOLEAN.GLFW_FALSE);
                Interop.GLFW.glfwInit();
            }
        }

        public static void RemoveReference()
        { 
            if (library_ref_count-- == 0U)
            {
                Interop.GLFW.glfwTerminate();
                library_ref_count = 0U;
            }
        }
    }
}