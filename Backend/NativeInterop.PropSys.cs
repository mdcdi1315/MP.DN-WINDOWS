
using MP;
using MP.ComInterop;
using System.Runtime.InteropServices;

partial class Interop
{
    // Note: Wherever REFPROPERTYKEY use PROPERTYEY*

    public unsafe static class PropSys
    {
        private const System.Int32 PKEY_PIDSTR_MAX = 10;
        private const System.Int32 GUIDSTRING_MAX = 1 + 8 + 1 + 4 + 1 + 4 + 1 + 4 + 1 + 12 + 1 + 1;
        public const System.Int32 PKEYSTR_MAX = GUIDSTRING_MAX + 1 + PKEY_PIDSTR_MAX;


        [DllImport(Libraries.PropSys , EntryPoint = "PSStringFromPropertyKey" , ExactSpelling = true)]
        private static extern HRESULT PSStringFromPropertyKey_Native(PROPERTYKEY* pk , System.Char* outstr , System.UInt32 nch);

        [DllImport(Libraries.PropSys, EntryPoint = "PSPropertyKeyFromString", ExactSpelling = true)]
        private static extern HRESULT PSPropertyKeyFromString_Native(System.Char* pkeystr, PROPERTYKEY* pk);

        [DllImport(Libraries.PropSys, EntryPoint = "PSGetNameFromPropertyKey", ExactSpelling = true)]
        private static extern HRESULT PSGetNameFromPropertyKey_Native(PROPERTYKEY* pkey, System.Char** outname);

        [DllImport(Libraries.PropSys, EntryPoint = "PSCreateMemoryPropertyStore", ExactSpelling = true)]
        private static extern HRESULT PSCreateMemoryPropertyStore_Native(MP.WindowsInterop.GUID* iid, void** cobj);

        [DllImport(Libraries.PropSys, EntryPoint = "InitPropVariantFromCLSID", ExactSpelling = true)]
        private static extern HRESULT InitPropVariantFromCLSID_Native(MP.WindowsInterop.GUID* clsid, PROPVARIANT* propvariant);

        [DllImport(Libraries.PropSys, ExactSpelling = true)]
        public static extern HRESULT InitPropVariantFromBuffer(void* pbuf, System.UInt32 pbuflen, PROPVARIANT* ppropvar);

        public static HRESULT PSStringFromPropertyKey(PROPERTYKEY key , out System.String keystr)
        {
            keystr = new('\0', PKEYSTR_MAX + 2);
            HRESULT hrt;
            fixed (System.Char* pkeystr = keystr)
            {
                hrt = PSStringFromPropertyKey_Native(&key, pkeystr, keystr.Length.ToUInt32());
            }
            System.Int32 s0idx;
            if (hrt.SUCCEEDED && (s0idx = keystr.IndexOf('\0')) > -1)
            {
                keystr = keystr.Remove(s0idx);
            }
            return hrt;
        }
        
        public static HRESULT PSPropertyKeyFromString(System.String key , out PROPERTYKEY pckey)
        {
            HRESULT hrt;
            PROPERTYKEY pkey;
            fixed (System.Char* psrckey = key)
            {
                hrt = PSPropertyKeyFromString_Native(psrckey, &pkey);
            }
            pckey = pkey;
            return hrt;
        }

        public static HRESULT PSGetNameFromPropertyKey(PROPERTYKEY key , out System.String name)
        {
            name = null;
            System.Char* poutname;
            HRESULT hrt = PSGetNameFromPropertyKey_Native(&key , &poutname);
            if (hrt == CommonHResults.S_OK)
            {
                name = new(poutname);
                Ole32.CoTaskMemFree(poutname);
            }
            return hrt;
        }
       
        public static HRESULT PSCreateMemoryPropertyStore(MP.WindowsInterop.GUID iid , out void* pinterface)
        {
            void* pcobj;
            HRESULT hr = PSCreateMemoryPropertyStore_Native(&iid, &pcobj);
            pinterface = pcobj;
            return hr;
        }

        public static HRESULT InitPropVariantFromCLSID(MP.WindowsInterop.GUID guid, out PROPVARIANT pv)
        {
            PROPVARIANT pvt;
            HRESULT hr = InitPropVariantFromCLSID_Native(&guid, &pvt);
            pv = pvt;
            return hr;
        }
    }
}