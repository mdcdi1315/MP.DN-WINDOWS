using MP;
using MP.Annotations;
using MP.NativeInterop.Windows;
using MP.NativeInterop.Windows.COM;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

partial class Interop
{
    public static unsafe class Ole32
    {
        [DllImport(Libraries.Ole32 , ExactSpelling = true)]
        public static extern HRESULT CoCreateInstance(GUID* CLSID, void* punkouternotused, CLSCTX clscxt, GUID* refiid, void** obj);

        [DllImport(Libraries.Ole32, EntryPoint = "FreePropVariantArray", ExactSpelling = true)]
        private static extern HRESULT FreePropVariantArray_Native(System.UInt32 aelems, PROPVARIANT* pelems);

        [DllImport(Libraries.Ole32, EntryPoint = "PropVariantCopy" , ExactSpelling = true)]
        private static extern HRESULT PropVariantCopy_Native(PROPVARIANT* outpvar, PROPVARIANT* invar);

        [DllImport(Libraries.Ole32, EntryPoint = "CoGetMalloc", ExactSpelling = true)]
        private static extern HRESULT CoGetMalloc_Native(System.UInt32 dwMemContext, [IsPointerToCOMInterfaceType(typeof(IMalloc))] void** ppMalloc);

        [DllImport(Libraries.Ole32, ExactSpelling = true)]
        public static extern HRESULT PropVariantClear(PROPVARIANT* pvar);

        // Essentially the same as above but this allows to play around with the magic of managed pointers
        // Unsafe.AsPointer works here since the PROPVARIANT is a trully unmanaged structure.
        public static HRESULT PropVariantClear(ref PROPVARIANT pv) => PropVariantClear((PROPVARIANT*)Unsafe.AsPointer(ref pv));

        public static HRESULT FreePropVariantArray(PROPVARIANT[] pvs)
        {
            fixed (PROPVARIANT* pvar = pvs) {
                return FreePropVariantArray_Native(pvs.LongLength.ToUInt32(), pvar);
            }
        }

        public static HRESULT PropVariantCopy(PROPVARIANT vin , out PROPVARIANT pvt)
        {
            PROPVARIANT pvout;
            HRESULT hr = PropVariantCopy_Native(&pvout , &vin);
            pvt = pvout;
            return hr;
        }

        public static HRESULT CoGetMalloc(out IMalloc m)
        {
            m = null;
            void* p;
            HRESULT hr = CoGetMalloc_Native(1U, &p);
            if (hr.SUCCEEDED) { 
                m = ComInterop.CreateRCW<IMalloc>(p);
            }
            return hr;
        }
    }
}