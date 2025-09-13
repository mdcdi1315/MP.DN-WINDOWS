
using System;
using MP.Annotations;
using MP.ComInterop;
using MP.WindowsInterop;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace MP.AudioLibrary.MediaFoundation
{
    /// <summary>
    /// Defines a .NET implementation of the <see cref="IMFAttributes"/> interface. <br />
    /// The class can be inherited. <br />
    /// Notes about the class: <br />
    /// The class does not expose any .NET-inherent method or property for querying the contents of the object to retain the encapsulation and logic of the <see cref="IMFAttributes"/> interface. <br />
    /// Instead, the user must use the methods that <see cref="IMFAttributes"/> elsewise exposes. <br />
    /// The user can also use the extension methods provided in <see cref="IMFAttributesExtensions"/> class as well if needs .NET communication.
    /// </summary>
    public class FullyManagedMFAttributes : IMFAttributes
    {
        private unsafe sealed class IUnknownHolder : IDisposable
        {
            public IUnknownHolder(void* p)
            {
                ArgumentNullException.ThrowIfNull(p);
                ComMarshalling.AddRef(Pointer = p);
            }

            public void* Pointer;

            public void Dispose()
            {
                if (Pointer is not null)
                {
                    ComMarshalling.Release(Pointer);
                    Pointer = null;
                }
            }
        }

        private Dictionary<GUID, System.Object> attributesstore;

        /// <summary>
        /// Initializes an empty instance of the <see cref="FullyManagedMFAttributes"/> class.
        /// </summary>
        public FullyManagedMFAttributes() => attributesstore = new(4);

        private static unsafe PROPVARIANT GetValueDirect(System.Object obj)
        {
            switch (obj)
            {
                case System.Double d:
                    return PROPVARIANT.FromDouble(d);
                case System.UInt32 ui:
                    return PROPVARIANT.FromUInt(ui);
                case System.UInt64 uli:
                    return PROPVARIANT.FromULong(uli);
                case System.String s:
                    return PROPVARIANT.FromLPWSTR(s);
                case GUID g:
                    return PROPVARIANT.FromGUID(g);
                case System.Byte[] b:
                    return PROPVARIANT.FromByteArrayUseBlob(b);
                case IUnknownHolder h:
                    PROPVARIANT pvc = new();
                    pvc.Type = VARTYPE.VT_UNKNOWN;
                    ComMarshalling.AddRef(h.Pointer);
                    pvc.PPVTValue.pointerValue = new(h.Pointer);
                    return pvc;
                default:
                    throw new ArgumentException();
            }
        }

        private PROPVARIANT GetValue(GUID key)
        {
            if (attributesstore.TryGetValue(key, out var value)) 
            {
                return GetValueDirect(value);
            }
            throw new ArgumentException();
        }

        /// <inheritdoc />
        public unsafe HRESULT GetItem(GUID* guidKey, PROPVARIANT* pValue)
        {
            if (guidKey is null) { return CommonHResults.E_INVALIDARG; }
            try {
                if (pValue is null) {
                    return attributesstore.ContainsKey(*guidKey) ? CommonHResults.S_OK : MediaFoundationErrorCodes.MF_E_ATTRIBUTENOTFOUND;
                }
                *pValue = GetValue(*guidKey);
                return CommonHResults.S_OK;
            } catch (ArgumentException) {
                return MediaFoundationErrorCodes.MF_E_ATTRIBUTENOTFOUND;
            } catch (OutOfMemoryException) {
                return CommonHResults.E_OUTOFMEMORY;
            }
        }

        /// <inheritdoc />
        public unsafe HRESULT GetItemType(GUID* guidKey, MF_ATTRIBUTE_TYPE* pType)
        {
            if (guidKey is null) { return CommonHResults.E_INVALIDARG; }
            if (pType is null) { return CommonHResults.E_INVALIDARG; }
            if (attributesstore.TryGetValue(*guidKey , out var value)) {
                switch (value)
                {
                    case System.UInt32:
                        *pType = MF_ATTRIBUTE_TYPE.MF_ATTRIBUTE_UINT32;
                        break;
                    case System.UInt64:
                        *pType = MF_ATTRIBUTE_TYPE.MF_ATTRIBUTE_UINT64;
                        break;
                    case IUnknownHolder:
                        *pType = MF_ATTRIBUTE_TYPE.MF_ATTRIBUTE_IUNKNOWN;
                        break;
                    case System.String:
                        *pType = MF_ATTRIBUTE_TYPE.MF_ATTRIBUTE_STRING;
                        break;
                    case GUID:
                        *pType = MF_ATTRIBUTE_TYPE.MF_ATTRIBUTE_GUID;
                        break;
                    case System.Byte[]:
                        *pType = MF_ATTRIBUTE_TYPE.MF_ATTRIBUTE_BLOB;
                        break;
                    default:
                        return CommonHResults.E_FAIL;
                }
                return CommonHResults.S_OK;
            }
            return MediaFoundationErrorCodes.MF_E_ATTRIBUTENOTFOUND;
        }

        /// <inheritdoc />
        public unsafe HRESULT CompareItem(GUID* guidKey, PROPVARIANT* Value, BOOL* pbResult)
        {
            if (guidKey is null) { return CommonHResults.E_INVALIDARG; }
            if (Value is null) { return CommonHResults.E_INVALIDARG; }
            try {
                PROPVARIANT p = GetValue(*guidKey);
                if (pbResult is not null) {
                    *pbResult = p.Value.Equals(*Value) ? BOOL.TRUE : BOOL.FALSE;
                }
                return CommonHResults.S_OK;
            } catch (ArgumentException) {
                if (pbResult is not null) { *pbResult = BOOL.FALSE; }
                return CommonHResults.S_OK;
            }
        }

        // TODO: Implement this method
        public unsafe HRESULT Compare([IsPointerToCOMInterfaceType(typeof(IMFAttributes))] void* pTheirs, MF_ATTRIBUTES_MATCH_TYPE MatchType, BOOL* pbResult)
        {
            if (pTheirs is null) { return CommonHResults.E_INVALIDARG; }
            return CommonHResults.E_NOTIMPL;
        }

        public unsafe HRESULT GetUINT32(GUID* guidKey, uint* punValue)
        {
            if (guidKey is null) { return CommonHResults.E_INVALIDARG; }
            if (punValue is null) { return CommonHResults.E_INVALIDARG; }
            if (attributesstore.TryGetValue(*guidKey, out var value)) 
            { 
                if (value is not System.UInt32 u) {
                    return MediaFoundationErrorCodes.MF_E_INVALIDTYPE;
                }
                *punValue = u;
                return CommonHResults.S_OK;
            }
            return MediaFoundationErrorCodes.MF_E_ATTRIBUTENOTFOUND;
        }

        public unsafe HRESULT GetUINT64(GUID* guidKey, ulong* punValue)
        {
            if (guidKey is null) { return CommonHResults.E_INVALIDARG; }
            if (punValue is null) { return CommonHResults.E_INVALIDARG; }
            if (attributesstore.TryGetValue(*guidKey, out var value))
            {
                if (value is not System.UInt64 u) {
                    return MediaFoundationErrorCodes.MF_E_INVALIDTYPE;
                }
                *punValue = u;
                return CommonHResults.S_OK;
            }
            return MediaFoundationErrorCodes.MF_E_ATTRIBUTENOTFOUND;
        }

        public unsafe HRESULT GetDouble(GUID* guidKey, double* pfValue)
        {
            if (guidKey is null) { return CommonHResults.E_INVALIDARG; }
            if (pfValue is null) { return CommonHResults.E_INVALIDARG; }
            if (attributesstore.TryGetValue(*guidKey, out var value))
            {
                if (value is not System.Double d) {
                    return MediaFoundationErrorCodes.MF_E_INVALIDTYPE;
                }
                *pfValue = d;
                return CommonHResults.S_OK;
            }
            return MediaFoundationErrorCodes.MF_E_ATTRIBUTENOTFOUND;
        }

        public unsafe HRESULT GetGUID(GUID* guidKey, GUID* pguidValue)
        {
            if (guidKey is null) { return CommonHResults.E_INVALIDARG; }
            if (pguidValue is null) { return CommonHResults.E_INVALIDARG; }
            if (attributesstore.TryGetValue(*guidKey, out var value))
            {
                if (value is not GUID guid) {
                    return MediaFoundationErrorCodes.MF_E_INVALIDTYPE;
                }
                *pguidValue = guid;
                return CommonHResults.S_OK;
            }
            return MediaFoundationErrorCodes.MF_E_ATTRIBUTENOTFOUND;
        }

        public unsafe HRESULT GetStringLength(GUID* guidKey, uint* pcchLength)
        {
            if (guidKey is null) { return CommonHResults.E_INVALIDARG; }
            if (pcchLength is null) { return CommonHResults.E_INVALIDARG; }
            if (attributesstore.TryGetValue(*guidKey, out var value)) 
            {
                if (value is not System.String s) {
                    return MediaFoundationErrorCodes.MF_E_INVALIDTYPE;
                }
                *pcchLength = s.Length.ToUInt32();
                return CommonHResults.S_OK;
            }
            return MediaFoundationErrorCodes.MF_E_ATTRIBUTENOTFOUND;
        }

        public unsafe HRESULT GetString(GUID* guidKey, char* pwszValue, uint cchBufSize, uint* pcchLength)
        {
            if (guidKey is null) { return CommonHResults.E_INVALIDARG; }
            if (pwszValue is null) { return CommonHResults.E_INVALIDARG; }
            if (attributesstore.TryGetValue(*guidKey, out var value))
            {
                if (value is not System.String s) {
                    return MediaFoundationErrorCodes.MF_E_INVALIDTYPE;
                }
                System.UInt32 u32len = s.Length.ToUInt32();
                if (pcchLength is not null) { *pcchLength = u32len; }
                if (cchBufSize + 1 < s.Length) { return HRESULT.FromWin32(Interop.Errors.ERROR_INSUFFICIENT_BUFFER); }
                fixed (System.Char* pc = s)
                {
                    Unsafe.CopyBlockUnaligned(pwszValue , pc, u32len + 1); // +1 character is the null character added implicitly by .NET
                }
                return CommonHResults.S_OK;
            }
            return MediaFoundationErrorCodes.MF_E_ATTRIBUTENOTFOUND;
        }

        public unsafe HRESULT GetAllocatedString(GUID* guidKey, char** ppwszValue, uint* pcchLength)
        {
            if (guidKey is null) { return CommonHResults.E_INVALIDARG; }
            if (ppwszValue is null) { return CommonHResults.E_INVALIDARG; }
            if (attributesstore.TryGetValue(*guidKey, out var value))
            {
                if (value is not System.String s) {
                    return MediaFoundationErrorCodes.MF_E_INVALIDTYPE;
                }
                System.UInt32 u32len = s.Length.ToUInt32();
                if (pcchLength is not null) { *pcchLength = u32len; }
                *ppwszValue = (System.Char*)Interop.Ole32.CoTaskMemAlloc(u32len + 1);
                if (*ppwszValue is null) { return CommonHResults.E_OUTOFMEMORY; }
                fixed (System.Char* pc = s)
                {
                    Unsafe.CopyBlockUnaligned(*ppwszValue, pc, u32len + 1);
                }
                return CommonHResults.S_OK;
            }
            return MediaFoundationErrorCodes.MF_E_ATTRIBUTENOTFOUND;
        }

        public unsafe HRESULT GetBlobSize(GUID* guidKey, uint* pcbBlobSize)
        {
            if (guidKey is null) { return CommonHResults.E_INVALIDARG; }
            if (pcbBlobSize is null) { return CommonHResults.E_INVALIDARG; }
            if (attributesstore.TryGetValue(*guidKey, out var value)) 
            {
                if (value is not System.Byte[] b) {
                    return MediaFoundationErrorCodes.MF_E_INVALIDTYPE;
                }
                *pcbBlobSize = b.LongLength.ToUInt32();
                return CommonHResults.S_OK;
            }
            return MediaFoundationErrorCodes.MF_E_ATTRIBUTENOTFOUND;
        }

        public unsafe HRESULT GetBlob(GUID* guidKey, byte* pBuf, uint cbBufSize, uint* pcbBlobSize)
        {
            if (guidKey is null) { return CommonHResults.E_INVALIDARG; }
            if (pBuf is null) { return CommonHResults.E_INVALIDARG; }
            if (attributesstore.TryGetValue(*guidKey, out var value))
            {
                if (value is not System.Byte[] b)
                {
                    return MediaFoundationErrorCodes.MF_E_INVALIDTYPE;
                }
                System.UInt32 dlen = b.LongLength.ToUInt32();
                if (pcbBlobSize is not null) { *pcbBlobSize = dlen; }
                if (dlen > cbBufSize) { return HRESULT.FromWin32(Interop.Errors.ERROR_INSUFFICIENT_BUFFER); }
                fixed (System.Byte* pb = b)
                {
                    Unsafe.CopyBlockUnaligned(pBuf , pb , dlen);
                }
                return CommonHResults.S_OK;
            }
            return MediaFoundationErrorCodes.MF_E_ATTRIBUTENOTFOUND;
        }

        public unsafe HRESULT GetAllocatedBlob(GUID* guidKey, byte** ppBuf, uint* pcbSize)
        {
            if (guidKey is null) { return CommonHResults.E_INVALIDARG; }
            if (ppBuf is null) { return CommonHResults.E_INVALIDARG; }
            if (attributesstore.TryGetValue(*guidKey, out var value))
            {
                if (value is not System.Byte[] b) {
                    return MediaFoundationErrorCodes.MF_E_INVALIDTYPE;
                }
                System.UInt32 u32len = b.LongLength.ToUInt32();
                if (pcbSize is not null) { *pcbSize = u32len; }
                *ppBuf = (System.Byte*)Interop.Ole32.CoTaskMemAlloc(u32len);
                if (*ppBuf is null) { return CommonHResults.E_OUTOFMEMORY; }
                fixed (System.Byte* pc = b)
                {
                    Unsafe.CopyBlockUnaligned(*ppBuf , pc , u32len);
                }
                return CommonHResults.S_OK;
            }
            return MediaFoundationErrorCodes.MF_E_ATTRIBUTENOTFOUND;
        }

        public unsafe HRESULT GetUnknown(GUID* guidKey, GUID* riid, void** ppv)
        {
            if (guidKey is null) { return CommonHResults.E_INVALIDARG; }
            if (riid is null) { return CommonHResults.E_INVALIDARG; }
            if (ppv is null) { return CommonHResults.E_INVALIDARG; }
            if (attributesstore.TryGetValue(*guidKey , out var value)) {
                if (value is not IUnknownHolder h) {
                    return MediaFoundationErrorCodes.MF_E_INVALIDTYPE;
                }
                void* p; 
                HRESULT hr = ComMarshalling.QueryInterface(h.Pointer, *riid, out p);
                if (hr.FAILED) {
                    *ppv = null;
                    return hr; 
                }
                if (p == h.Pointer) {
                    ComMarshalling.AddRef(p);
                }
                *ppv = p;
                return CommonHResults.S_OK;
            }
            return MediaFoundationErrorCodes.MF_E_ATTRIBUTENOTFOUND;
        }

        public unsafe HRESULT SetItem(GUID* guidKey, PROPVARIANT* Value)
        {
            if (guidKey is null) { return CommonHResults.E_INVALIDARG; }
            if (Value is null) { return CommonHResults.E_INVALIDARG; }
            switch (Value->Type)
            {
                case VARTYPE.VT_UI4:
                    attributesstore[*guidKey] = Value->PPVTValue.uintVal;
                    return CommonHResults.S_OK;
                case VARTYPE.VT_UI8:
                    attributesstore[*guidKey] = Value->PPVTValue.uhVal;
                    return CommonHResults.S_OK;
                case VARTYPE.VT_R8:
                    attributesstore[*guidKey] = Value->PPVTValue.dblVal;
                    return CommonHResults.S_OK;
                case VARTYPE.VT_BLOB:
                    attributesstore[*guidKey] = Value->GetBlob();
                    return CommonHResults.S_OK;
                case VARTYPE.VT_LPWSTR:
                    attributesstore[*guidKey] = new System.String((System.Char*)Value->PPVTValue.pointerValue.ToPointer());
                    return CommonHResults.S_OK;
                case VARTYPE.VT_UNKNOWN:
                    attributesstore[*guidKey] = new IUnknownHolder(Value->PPVTValue.pointerValue.ToPointer());
                    return CommonHResults.S_OK;
                case VARTYPE.VT_CLSID:
                    attributesstore[*guidKey] = Unsafe.ReadUnaligned<GUID>(Value->PPVTValue.pointerValue.ToPointer());
                    return CommonHResults.S_OK;
                default:
                    return CommonHResults.E_INVALIDARG;
            }
        }

        public unsafe HRESULT DeleteItem(GUID* guidKey)
        {
            if (guidKey is null) { return CommonHResults.E_INVALIDARG; }
            return attributesstore.Remove(*guidKey) ? CommonHResults.S_OK : MediaFoundationErrorCodes.MF_E_ATTRIBUTENOTFOUND;
        }

        public HRESULT DeleteAllItems()
        {
            foreach (var i in attributesstore) 
            {
                if (i.Value is IUnknownHolder h) { h.Dispose(); }
            }
            attributesstore.Clear();
            return CommonHResults.S_OK;
        }

        public unsafe HRESULT SetUINT32(GUID* guidKey, uint unValue)
        {
            if (guidKey is null) { return CommonHResults.E_INVALIDARG; }
            attributesstore[*guidKey] = unValue;
            return CommonHResults.S_OK;
        }

        public unsafe HRESULT SetUINT64(GUID* guidKey, ulong unValue)
        {
            if (guidKey is null) { return CommonHResults.E_INVALIDARG; }
            attributesstore[*guidKey] = unValue;
            return CommonHResults.S_OK;
        }

        public unsafe HRESULT SetDouble(GUID* guidKey, double fValue)
        {
            if (guidKey is null) { return CommonHResults.E_INVALIDARG; }
            attributesstore[*guidKey] = fValue;
            return CommonHResults.S_OK;
        }

        public unsafe HRESULT SetGUID(GUID* guidKey, GUID* guidValue)
        {
            if (guidKey is null) { return CommonHResults.E_INVALIDARG; }
            attributesstore[*guidKey] = *guidValue;
            return CommonHResults.S_OK;
        }

        public unsafe HRESULT SetString(GUID* guidKey, char* wszValue)
        {
            if (guidKey is null) { return CommonHResults.E_INVALIDARG; }
            attributesstore[*guidKey] = new System.String(wszValue);
            return CommonHResults.S_OK;
        }

        public unsafe HRESULT SetBlob(GUID* guidKey, byte* pBuf, uint cbBufSize)
        {
            if (guidKey is null) { return CommonHResults.E_INVALIDARG; }
            System.Byte[] bm = new System.Byte[cbBufSize];
            fixed (System.Byte* pdest = bm) 
            {
                Unsafe.CopyBlockUnaligned(pdest, pBuf, cbBufSize);
            }
            attributesstore[*guidKey] = bm;
            return CommonHResults.S_OK;
        }

        public unsafe HRESULT SetUnknown(GUID* guidKey, void* pUnknown)
        {
            if (guidKey is null) { return CommonHResults.E_INVALIDARG; }
            attributesstore[*guidKey] = new IUnknownHolder(pUnknown);
            return CommonHResults.S_OK;
        }

        public HRESULT LockStore() => CommonHResults.S_OK;

        public HRESULT UnlockStore() => CommonHResults.S_OK;

        public unsafe HRESULT GetCount(uint* pcItems)
        {
            if (pcItems is null) { return CommonHResults.E_INVALIDARG; }
            *pcItems = attributesstore.Count.ToUInt32();
            return CommonHResults.S_OK;
        }

        public unsafe HRESULT GetItemByIndex(uint unIndex, GUID* pguidKey, PROPVARIANT* pValue)
        {
            if (pguidKey is null) { return CommonHResults.E_INVALIDARG; }
            if (pValue is null) { return CommonHResults.E_INVALIDARG; }
            System.UInt32 index = 0;
            foreach (var e in attributesstore)
            {
                if (++index < unIndex) { continue; }
                *pguidKey = e.Key;
                try { *pValue = GetValueDirect(e.Value); } catch { return CommonHResults.E_UNEXPECTED; }
                return CommonHResults.S_OK;
            }
            return CommonHResults.E_BOUNDS;
        }

        public unsafe HRESULT CopyAllItems([IsPointerToCOMInterfaceType(typeof(IMFAttributes))] void* pDest)
        {
            if (pDest is null) { return CommonHResults.E_INVALIDARG; }
            IMFAttributes attrs = ComMarshalling.CreateInteropObject(pDest , -1) as IMFAttributes;
            try {
                GUID tempg;
                HRESULT hr;
                PROPVARIANT tempp;
                foreach (var e in attributesstore)
                {
                    tempg = e.Key;
                    tempp = GetValueDirect(e.Value);
                    hr = attrs.SetItem(&tempg, &tempp);
                    if (hr.FAILED) { return hr; }
                }
                return CommonHResults.S_OK;
            } catch (ArgumentException) {
                return CommonHResults.E_FAIL;
            } finally {
                ComMarshalling.ReleaseInteropObject(attrs);
            }
        }
    
        ~FullyManagedMFAttributes()
        {
            foreach (var e in attributesstore)
            {
                if (e.Value is IUnknownHolder h) { h.Dispose(); } 
            }
        }
    }
}
