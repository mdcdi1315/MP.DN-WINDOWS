using MP;
using System;
using MP.ComInterop;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

partial class Interop
{
    public static unsafe class Ole32
    {
        [Flags]
        public enum COINIT
        {
            COINIT_APARTMENTTHREADED = 0x2,
            COINIT_MULTITHREADED = 0x0,
            COINIT_DISABLE_OLE1DDE = 0x4,
            COINIT_SPEED_OVER_MEMORY = 0x8
        }

        [DllImport(Libraries.Ole32 , ExactSpelling = true)]
        public static extern void* CoTaskMemAlloc(System.UInt64 size);

        [DllImport(Libraries.Ole32 , ExactSpelling = true)]
        public static extern void CoTaskMemFree(void* mem);

        [DllImport(Libraries.Ole32, ExactSpelling = true)]
        public static extern void* CoTaskMemRealloc(void* oldmem , System.UInt64 size);

        [DllImport(Libraries.Ole32 , ExactSpelling = true)]
        public static extern HRESULT CoCreateInstance(GUID* CLSID, void* punkouternotused, CLSCTX clscxt, GUID* refiid, void** obj);

        [DllImport(Libraries.Ole32, ExactSpelling = true)]
        public static extern HRESULT CoInitializeEx(void* nothing, COINIT initflags);

        [DllImport(Libraries.Ole32, EntryPoint = "FreePropVariantArray", ExactSpelling = true)]
        private static extern HRESULT FreePropVariantArray_Native(System.UInt32 aelems, PROPVARIANT* pelems);

        [DllImport(Libraries.Ole32, EntryPoint = "PropVariantCopy" , ExactSpelling = true)]
        private static extern HRESULT PropVariantCopy_Native(PROPVARIANT* outpvar, PROPVARIANT* invar);

        [DllImport(Libraries.Ole32, ExactSpelling = true)]
        public static extern HRESULT PropVariantClear(PROPVARIANT* pvar);

        // Essentially the same as above but this allows to play around with the magic of managed pointers
        // Unsafe.AsPointer works here since the PROPVARIANT is a trully unmanaged structure.
        public static HRESULT PropVariantClear(ref PROPVARIANT pv) => PropVariantClear((PROPVARIANT*)Unsafe.AsPointer(ref pv));

        public static HRESULT FreePropVariantArray(PROPVARIANT[] pvs)
        {
            fixed (PROPVARIANT* pvar = pvs)
            {
                return FreePropVariantArray_Native(pvs.Length.ToUInt32(), pvar);
            }
        }

        public static HRESULT PropVariantCopy(PROPVARIANT vin , out PROPVARIANT pvt)
        {
            PROPVARIANT pvout;
            HRESULT hr = PropVariantCopy_Native(&pvout , &vin);
            pvt = pvout;
            return hr;
        }
    }
}