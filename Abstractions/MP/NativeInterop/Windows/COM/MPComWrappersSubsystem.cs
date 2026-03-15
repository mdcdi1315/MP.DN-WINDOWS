
using System;
using MP.Utilities;
using MP.Collections;
using MP.Annotations;
using System.Threading;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Diagnostics.CodeAnalysis;

namespace MP.NativeInterop.Windows.COM
{
    internal unsafe sealed class MPComWrappersSubsystem
    {
        public const System.String COM_ALLOCATOR = "WINDOWS_COM";

        [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Internal storage class")]
        private sealed class __ComInterfaceInfo
        {
            private readonly Guid IID;
            private readonly VirtualTableBuilder VTable;
            private readonly NativeObjectCreater object_creater;

            private delegate INativeCOMObject NativeObjectCreater(void* p_interface);

            [SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "Supported everywhere, just COM is useful on Windows only.")]
            public __ComInterfaceInfo(Type type)
            {
                COMInterfaceAttribute cia = 
                    type.GetCustomAttribute<COMInterfaceAttribute>() ?? 
                    throw new InvalidOperationException("No COMInterfaceAttribute found on the specified COM interface!!!");
                
                IID = cia.GUID;
                if (!cia.DispatchProvider.ImplementsInterface(typeof(ICOMDispatchServicesProvider))) {
                    throw new InvalidOperationException("Dispatch services provider must implement the ICOMDispatchServicesProvider interface.");
                }
                VirtualTableBuilder builder = new();
                cia.DispatchProvider.InvokeMember("GenerateCCWVirtualTable" , BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static, null , null , new object[] { builder });
                (VTable = builder).Lock();
                object_creater = (
                    cia.DispatchProvider.GetMethod("CreateNativeObject" , BindingFlags.Public | BindingFlags.Static) ??
                    throw new MissingMethodException(cia.DispatchProvider.FullName, "CreateNativeObject")
                ).CreateDelegate<NativeObjectCreater>();
            }

            public Guid InterfaceID => IID;

            public VirtualTableBuilder VirtualTable => VTable;

            public INativeCOMObject CreateObjectInstance(void* p_native) => object_creater.Invoke(p_native);
        }

        private static INativeMemoryManager COM_MEM_MANAGER;
        private static readonly ArrayBasedList<IntPtr> ccw_exposed_objects;
        private static readonly Dictionary<Type, __ComInterfaceInfo> com_interfaces;

        static MPComWrappersSubsystem() {
            COM_MEM_MANAGER = null;
            com_interfaces = new(20);
            ccw_exposed_objects = new(5);
        }

        public static INativeMemoryManager GetComAllocatorOrDefault()
        {
            if (COM_MEM_MANAGER is null)
            {
                var layer = SystemInfo.LayerUsed;
                try {
                    COM_MEM_MANAGER = layer.GetMemoryManager(COM_ALLOCATOR);
                } catch (KeyNotFoundException) {
                    COM_MEM_MANAGER = layer.GetDefaultMemoryManager(); 
                }
            }
            return COM_MEM_MANAGER;
        }

        [StructLayout(LayoutKind.Sequential)]
        private readonly struct UNKNOWN_WRAPPER { public readonly IUnknownVirtualTable* Table; }

        public static HRESULT QueryInterface(void* p_object, Guid guid, out void* p_interface)
        {
            void* ret_p;
            GUID translated_id = GUID.FromGUID(guid);
            HRESULT hr = ((UNKNOWN_WRAPPER*)p_object)->Table->QueryInterface(p_object, &translated_id, &ret_p);
            p_interface = ret_p;
            return hr;
        }

        public static System.UInt32 AddRef(void* p_object) => ((UNKNOWN_WRAPPER*)p_object)->Table->AddRef(p_object);

        public static System.UInt32 Release(void* p_object) => ((UNKNOWN_WRAPPER*)p_object)->Table->Release(p_object);

        public static void* LoadVirtualTableFunction(void* p_object, int ordinal) => *(((void**)((UNKNOWN_WRAPPER*)p_object)->Table) + ordinal);
    
        private static __ComInterfaceInfo GetComInterfaceInfo(Type interface_com)
        {
            __ComInterfaceInfo info;
            Monitor.Enter(com_interfaces);
            try {
                if (!com_interfaces.TryGetValue(interface_com, out info)) {
                    com_interfaces.Add(interface_com, info = new(interface_com));
                }
            } finally {
                Monitor.Exit(com_interfaces);
            }
            return info;
        }

        public static VirtualTableBuilder GetVirtualTableBuilderFor(Type interface_com) => GetComInterfaceInfo(interface_com).VirtualTable;

        public static INativeCOMObject CreateNativeObjectFor(void* com_object, Type interface_to_wrap) => GetComInterfaceInfo(interface_to_wrap).CreateObjectInstance(com_object);

        public static void* CreateCCW(System.Object obj, Type selected_interface)
        {
            CCWComObjectDetails* p_details = (CCWComObjectDetails*)GetComAllocatorOrDefault().Allocate(sizeof(CCWComObjectDetails));
            *p_details = new(obj, selected_interface);
            Monitor.Enter(ccw_exposed_objects);
            try {
                ccw_exposed_objects.Add(new(p_details));
            } finally {
                Monitor.Exit(ccw_exposed_objects);
            }
            return p_details;
        }

        public static void DestroyCCW(void* p_object)
        {
            if (p_object is null) { return; }
            Monitor.Enter(ccw_exposed_objects);
            try {
                void* mem;
                for (int I = 0; I < ccw_exposed_objects.Count; I++)
                {
                    if ((mem = ccw_exposed_objects[I].ToPointer()) == p_object)
                    {
                        ccw_exposed_objects.RemoveAt(I);
                        GetComAllocatorOrDefault().Free(mem);
                        break;
                    }
                }
            } finally {
                Monitor.Exit(ccw_exposed_objects);
            }
        }
    }
}