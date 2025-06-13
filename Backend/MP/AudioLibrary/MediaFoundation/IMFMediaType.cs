
using MP.ComInterop;
using MP.Annotations;
using MP.WindowsInterop;
using System.Runtime.InteropServices;

namespace MP.AudioLibrary.MediaFoundation
{


    [ComImport]
    [Guid(MediaFoundationInterfaceIds.IID_IMFMediaType)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe interface IMFMediaType : IMFAttributes
    {
        [PreserveSig]
        public new HRESULT GetItem(GUID* guidKey, PROPVARIANT* pValue);

        [PreserveSig]
        public new HRESULT GetItemType(GUID* guidKey, MF_ATTRIBUTE_TYPE* pType);

        [PreserveSig]
        public new HRESULT CompareItem(GUID* guidKey, PROPVARIANT* Value, BOOL* pbResult);

        [PreserveSig]
        public new HRESULT Compare([IsPointerToCOMInterfaceType(typeof(IMFAttributes))] void* pTheirs, MF_ATTRIBUTES_MATCH_TYPE MatchType, BOOL* pbResult);

        [PreserveSig]
        public new HRESULT GetUINT32(GUID* guidKey, System.UInt32* punValue);

        [PreserveSig]
        public new HRESULT GetUINT64(GUID* guidKey, System.UInt64* punValue);

        [PreserveSig]
        public new HRESULT GetDouble(GUID* guidKey, System.Double* pfValue);

        [PreserveSig]
        public new HRESULT GetGUID(GUID* guidKey, GUID* pguidValue);

        [PreserveSig]
        public new HRESULT GetStringLength(GUID* guidKey, System.UInt32* pcchLength);

        [PreserveSig]
        public new HRESULT GetString(GUID* guidKey, System.Char* pwszValue, System.UInt32 cchBufSize, System.UInt32* pcchLength);

        [PreserveSig]
        public new HRESULT GetAllocatedString(GUID* guidKey, System.Char* ppwszValue, System.UInt32* pcchLength);

        [PreserveSig]
        public new HRESULT GetBlobSize(GUID* guidKey, System.UInt32* pcbBlobSize);

        [PreserveSig]
        public new HRESULT GetBlob(GUID* guidKey, System.Byte* pBuf, System.UInt32 cbBufSize, System.UInt32* pcbBlobSize);

        [PreserveSig]
        public new HRESULT GetAllocatedBlob(GUID* guidKey, System.Byte** ppBuf, System.UInt32* pcbSize);

        [PreserveSig]
        public new HRESULT GetUnknown(GUID* guidKey, GUID* riid, void** ppv);

        [PreserveSig]
        public new HRESULT SetItem(GUID* guidKey, PROPVARIANT* Value);

        [PreserveSig]
        public new HRESULT DeleteItem(GUID* guidKey);

        [PreserveSig]
        public new HRESULT DeleteAllItems();

        [PreserveSig]
        public new HRESULT SetUINT32(GUID* guidKey, System.UInt32 unValue);

        [PreserveSig]
        public new HRESULT SetUINT64(GUID* guidKey, System.UInt64 unValue);

        [PreserveSig]
        public new HRESULT SetDouble(GUID* guidKey, double fValue);

        [PreserveSig]
        public new HRESULT SetGUID(GUID* guidKey, GUID* guidValue);

        [PreserveSig]
        public new HRESULT SetString(GUID* guidKey, System.Char* wszValue);

        [PreserveSig]
        public new HRESULT SetBlob(GUID* guidKey, System.Byte* pBuf, System.UInt32 cbBufSize);

        [PreserveSig]
        public new HRESULT SetUnknown(GUID* guidKey, void* pUnknown);

        [PreserveSig]
        public new HRESULT LockStore();

        [PreserveSig]
        public new HRESULT UnlockStore();

        [PreserveSig]
        public new HRESULT GetCount(System.UInt32* pcItems);

        [PreserveSig]
        public new HRESULT GetItemByIndex(System.UInt32 unIndex, GUID* pguidKey, PROPVARIANT* pValue);

        [PreserveSig]
        public new HRESULT CopyAllItems([IsPointerToCOMInterfaceType(typeof(IMFAttributes))] void* pDest);

        [PreserveSig]
        public HRESULT GetMajorType(GUID* pguidMajorType);

        [PreserveSig]
        public HRESULT IsCompressedFormat(BOOL* pfCompressed);

        [PreserveSig]
        public HRESULT IsEqual([IsPointerToCOMInterfaceType(typeof(IMFMediaType))] void* pIMediaType, MF_MEDIATYPE_EQUALITY_FLAGS* pdwFlags);

        [PreserveSig]
        public HRESULT GetRepresentation(GUID guidRepresentation, void** ppvRepresentation);

        [PreserveSig]
        public HRESULT FreeRepresentation(GUID guidRepresentation, void* pvRepresentation);
    }

}