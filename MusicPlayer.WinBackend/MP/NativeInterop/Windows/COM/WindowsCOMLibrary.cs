
using System;
using MP.Annotations;
using System.Reflection;
using System.Diagnostics.CodeAnalysis;

namespace MP.NativeInterop.Windows.COM
{
    public static unsafe class WindowsCOMLibrary
    {
        private static readonly COMMemoryManager manager;

        [RequiresNativeLayer]
        static WindowsCOMLibrary() {
            Interop.Ole32.CoGetMalloc(out IMalloc m).ThrowOnFailure();
            manager = new(m);
        }

        public static COMMemoryManager COMMemoryManager => manager;

        public static HRESULT CreateInstance(GUID clsid, GUID iid, [AllowNull] void* p_aggregate_object, CLSCTX context, [MaybeNull] out void* p_instance)
        {
            void* p_out;
            HRESULT hr = Interop.Ole32.CoCreateInstance(&clsid, p_aggregate_object, context, &iid, &p_out);
            p_instance = p_out;
            return hr;
        }

        public static TI CreateInstance<TI>(CLSCTX context)
            where TI : class
        {
            Type t = typeof(TI);
            COMInterfaceAttribute cia = t.GetCustomAttribute<COMInterfaceAttribute>();
            if (cia is null) {
                throw new ArgumentException($"The interface of type {t.FullName} does not specify an instance of the COMInterfaceAttribute.");
            } else {
                DefaultCOMInterfaceObjectGuidAttribute g = t.GetCustomAttribute<DefaultCOMInterfaceObjectGuidAttribute>();
                if (g is null) {
                    throw new ArgumentException($"The interface of type {t.FullName} does not specify an instance of the DefaultCOMInterfaceObjectGuidAttribute.");
                } else {
                    CreateInstance(
                        GUID.FromString(g.ObjectIdentifier), 
                        GUID.FromGUID(cia.GUID), 
                        null, 
                        context, 
                        out void* p_instance
                    ).ThrowOnFailure();
                    return ComInterop.CreateRCW<TI>(p_instance);
                }
            }
        }

        public static TI CreateInstance<TI>(CLSCTX context, Guid clsid) 
            where TI : class
        {
            Type t = typeof(TI);
            COMInterfaceAttribute cia = t.GetCustomAttribute<COMInterfaceAttribute>();
            if (cia is null) {
                throw new ArgumentException($"The interface of type {t.FullName} does not specify an instance of the COMInterfaceAttribute.");
            } else {
                CreateInstance(GUID.FromGUID(clsid), GUID.FromGUID(cia.GUID), null, context, out void* p_instance).ThrowOnFailure();
                return ComInterop.CreateRCW<TI>(p_instance);
            }
        }

        
    }
}