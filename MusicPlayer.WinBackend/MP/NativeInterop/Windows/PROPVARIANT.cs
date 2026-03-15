
using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using MP.NativeInterop.Windows.COM;

namespace MP.NativeInterop.Windows
{
    /// <summary>
    /// Declares the property variant structure in plain .NET. <br />
    /// It is primarily used for the needs of the Windows Property System, 
    /// but it is also used in the MMDevice API and you also see this in Media Foundation.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct PROPVARIANT : IDisposable, ICloneable
    {
        [FieldOffset(0)]
        public VARTYPE Type;

        [FieldOffset(2)]
        public System.UInt16 RSVD1;

        [FieldOffset(4)]
        public System.UInt16 RSVD2;

        [FieldOffset(6)]
        public System.UInt16 RSVD3;

        [FieldOffset(8)]
        public PROPVARIANTVALUE PPVTValue;

        /// <summary>
        /// Property value
        /// </summary>
        public unsafe System.Object Value
        {
            get {
                switch (Type)
                {
                    case VARTYPE.VT_I1:
                        return PPVTValue.cVal;
                    case VARTYPE.VT_UI1:
                        return PPVTValue.bVal;
                    case VARTYPE.VT_UI2:
                        return PPVTValue.uiVal;
                    case VARTYPE.VT_I2:
                        return PPVTValue.iVal;
                    case VARTYPE.VT_I4:
                    case VARTYPE.VT_INT:
                        return PPVTValue.lVal;
                    case VARTYPE.VT_I8:
                        return PPVTValue.hVal;
                    case VARTYPE.VT_UI4:
                    case VARTYPE.VT_UINT:
                        return PPVTValue.ulVal;
                    case VARTYPE.VT_UI8:
                        return PPVTValue.uhVal;
                    case VARTYPE.VT_CY:
                        return PPVTValue.cy;
                    case VARTYPE.VT_BLOB:
                    case VARTYPE.VT_VECTOR | VARTYPE.VT_UI1:
                        return GetBlob();
                    case VARTYPE.VT_CLSID:
                        return Unsafe.ReadUnaligned<GUID>(PPVTValue.pointerValue.ToPointer());
                    case VARTYPE.VT_LPWSTR:
                        return new System.String((System.Char*)PPVTValue.pointerValue.ToPointer());
                    case VARTYPE.VT_BSTR:
                        // BSTR needs a bit different handling...
                        BSTR b = new(PPVTValue.pointerValue);
                        try {
                            return b.ToString();
                        } finally {
                            // We need to do this because the BSTR pointer is owned by the PROPVARIANT
                            b.SetHandleAsInvalid();
                            b = null;
                        }
                    case VARTYPE.VT_FILETIME:
                        return PPVTValue.filetime.ToDateTimeUtc();
                    case VARTYPE.VT_BOOL:
                        switch (PPVTValue.boolVal)
                        {
                            case -1:
                                return true;
                            case 0:
                                return false;
                            default:
                                throw new NotSupportedException("PropVariant VT_BOOL must be either -1 or 0");
                        }
                    case VARTYPE.VT_PTR:
                        return PPVTValue.pointerValue;
                    case VARTYPE.VT_EMPTY:
                        return null;
                    // I think VT_NULL means a database null as per the description, so could return DBNull.Value
                }
                throw new NotImplementedException($"PropVariant type {Type} is not supported.");
            }
            set {
                // Perform a PropVariantClear on this object before continuing
                Dispose();
                switch (value)
                {
                    case null:
                        Type = VARTYPE.VT_EMPTY;
                        break;
                    case System.String s:
                        System.UInt32 flen = (s.Length + 1).ToUInt32() * sizeof(System.Char);
                        void* ptr = WindowsCOMLibrary.COMMemoryManager.Allocate(flen);
                        if (ptr is null) {
                            throw new OutOfMemoryException("Cannot allocate memory for the LPWSTR!!");
                        }
                        // .NET constructs the string pointer so that it contains a null value at the end.
                        fixed (System.Char* psrc = s)
                        {
                            Unsafe.CopyBlockUnaligned(ptr, psrc, flen);
                        }
                        Type = VARTYPE.VT_LPWSTR;
                        PPVTValue.pointerValue = new(ptr);
                        break;
                    case System.SByte sb:
                        Type = VARTYPE.VT_I1;
                        PPVTValue.iVal = sb;
                        break;
                    case System.Byte b:
                        Type = VARTYPE.VT_I1;
                        PPVTValue.bVal = b;
                        break;
                    case System.Int16 ssi:
                        Type = VARTYPE.VT_I2;
                        PPVTValue.iVal = ssi;
                        break;
                    case System.UInt16 sui:
                        Type = VARTYPE.VT_UI2;
                        PPVTValue.uiVal = sui;
                        break;
                    case System.Int32 sni:
                        Type = VARTYPE.VT_I4;
                        PPVTValue.lVal = sni;
                        break;
                    case System.UInt32 ui:
                        Type = VARTYPE.VT_UI4;
                        PPVTValue.ulVal = ui;
                        break;
                    case System.Int64 li:
                        Type = VARTYPE.VT_I8;
                        PPVTValue.hVal = li;
                        break;
                    case System.UInt64 lui:
                        Type = VARTYPE.VT_UI8;
                        PPVTValue.uhVal = lui;
                        break;
                    case System.Boolean bl:
                        Type = VARTYPE.VT_BOOL;
                        PPVTValue.boolVal = bl ? (System.Int16)(-1) : (System.Int16)0;
                        break;
                    case CURRENCY cy:
                        Type = VARTYPE.VT_CY;
                        PPVTValue.cy = cy;
                        break;
                    case GUID guid:
                        GUID* pguid = (GUID*)WindowsCOMLibrary.COMMemoryManager.Allocate(sizeof(GUID));
                        if (pguid is null) {
                            throw new OutOfMemoryException("Cannot allocate memory for the GUID!");
                        }
                        *pguid = guid;
                        Type = VARTYPE.VT_CLSID;
                        PPVTValue.pointerValue = new(pguid);
                        break;
                    case System.Byte[] bt:
                        BLOB blb = new();
                        blb.Data = (System.Byte*)WindowsCOMLibrary.COMMemoryManager.Allocate(blb.Length = bt.LongLength.ToUInt32());
                        if (blb.Data is null) {
                            throw new OutOfMemoryException("Cannot allocate memory for the native array!");
                        }
                        fixed (System.Byte* psrc = bt) {
                            Unsafe.CopyBlockUnaligned(blb.Data, psrc, blb.Length);
                        }
                        Type = VARTYPE.VT_BLOB;
                        PPVTValue.blobVal = blb;
                        break;
                    case System.DateTime dt:
                        Type = VARTYPE.VT_FILETIME;
                        PPVTValue.filetime = FILETIME.FromDateTime(dt);
                        break;
                }
                throw new InvalidCastException($"Cast to the type {value.GetType().FullName} is currently undefined");
            }
        }

        /// <summary>
        /// Disposes this <see cref="PROPVARIANT"/> structure.
        /// </summary>
        public unsafe readonly void Dispose()
        {
            // To successfully free a PROPVARIANT we need to also free 'itself'.
            // So, use Unsafe.AsRef to get the used reference to the current PROPVARIANT,
            // then call in the optimized method.
            Interop.Ole32.PropVariantClear(ref Unsafe.AsRef(in this)).ThrowOnFailure();
        }

        /// <summary>
        /// Essentially this method exists to avoid calling the <see cref="UnsafeMethods.Copy{T}(T)"/> extension method,
        /// that would copy essentially the structure contents but would not create new pointers as would be required in some cases.
        /// </summary>
        /// <returns>The copy of the contents of the current <see cref="PROPVARIANT"/> instance.</returns>
        public readonly PROPVARIANT Copy() => Clone();

        /// <summary>
        /// Creates a new <see cref="PROPVARIANT"/> that is the copy of this <see cref="PROPVARIANT"/>.
        /// </summary>
        /// <returns>The copy of the contents of the current <see cref="PROPVARIANT"/> instance.</returns>
        public readonly PROPVARIANT Clone()
        {
            Interop.Ole32.PropVariantCopy(this, out var pv).ThrowOnFailure();
            return pv;
        }

        readonly System.Object ICloneable.Clone() => Clone();

        /// <summary>
        /// Helper method to get blob or byte array data from a PROPVARIANT
        /// </summary>
        internal readonly unsafe System.Byte[] GetBlob()
        {
            var blob = new System.Byte[PPVTValue.blobVal.Length];
            if (blob.LongLength == 0) { return blob; }
            fixed (System.Byte* dst = blob) 
            {
                // CopyBlockUnaligned for the fastest copy as possible.
                Unsafe.CopyBlockUnaligned(dst, PPVTValue.blobVal.Data, blob.LongLength.ToUInt32());
            }
            return blob;
        }

        /// <summary>
        /// Returns a string representing the contents of this <see cref="PROPVARIANT"/>.
        /// </summary>
        /// <returns>The contents of this <see cref="PROPVARIANT"/>.</returns>
        public override System.String ToString()
        {
            System.Object o = null;
            try { o = Value; } catch { }
            return $"PROPVARIANT: {{ Type: {Type} Value: {o} }}";
        }

        /// <summary>
        /// Creates a new PropVariant containing a long value
        /// </summary>
        public static PROPVARIANT FromLong(System.Int64 value) => new() { Type = VARTYPE.VT_I8, PPVTValue = new() { hVal = value } };

        /// <summary>
        /// Creates a new PropVariant containing an unsigned long value
        /// </summary>
        public static PROPVARIANT FromULong(System.UInt64 value) => new() { Type = VARTYPE.VT_UI8 , PPVTValue = new() { uhVal = value } };

        /// <summary>
        /// Creates a new PropVariant containing an integer value
        /// </summary>
        public static PROPVARIANT FromInt(System.Int32 value) => new() { Type = VARTYPE.VT_I4 , PPVTValue = new() { lVal = value } };

        /// <summary>
        /// Creates a new PropVariant containing an unsigned integer value
        /// </summary>
        public static PROPVARIANT FromUInt(System.UInt32 value) => new() { Type = VARTYPE.VT_UI4, PPVTValue = new() { uintVal = value } };

        /// <summary>
        /// Creates a new PropVariant containing a short integer value
        /// </summary>
        public static PROPVARIANT FromShort(System.Int16 value) => new() { Type = VARTYPE.VT_I2, PPVTValue = new() { iVal = value } };

        /// <summary>
        /// Creates a new PropVariant containing an unsigned short integer value
        /// </summary>
        public static PROPVARIANT FromUShort(System.UInt16 value) => new() { Type = VARTYPE.VT_UI2 , PPVTValue = new() { uiVal = value } };

        /// <summary>
        /// Creates a new PropVariant containing a byte value
        /// </summary>
        public static PROPVARIANT FromByte(System.Byte value) => new() { Type = VARTYPE.VT_UI1 , PPVTValue = new() { bVal = value } };

        /// <summary>
        /// Creates a new PropVariant containing a signed byte value
        /// </summary>
        public static PROPVARIANT FromSByte(System.SByte value) => new() { Type = VARTYPE.VT_I1 , PPVTValue = new() { cVal = value } };

        /// <summary>
        /// Creates a new PropVariant containing a double-precision floating integer value
        /// </summary>
        public static PROPVARIANT FromDouble(System.Double dbl) => new() { Type = VARTYPE.VT_R8 , PPVTValue = new() { dblVal = dbl } };

        /// <summary>
        /// Creates a new PropVariant containing a single-precision floating integer value
        /// </summary>
        public static PROPVARIANT FromSingle(System.Single flt) => new() { Type = VARTYPE.VT_R4 , PPVTValue = new() { fltVal = flt } };

        /// <summary>
        /// Creates a new PropVariant containing a string encoded by the semantics of the <see cref="BSTR"/> type.
        /// </summary>
        /// <param name="bstr">The string to encode by the <see cref="BSTR"/> semantics.</param>
        /// <returns>The created PropVariant instance</returns>
        public static unsafe PROPVARIANT FromBSTR(System.String bstr)
        {
            if (bstr is null) { throw new ArgumentNullException(nameof(bstr)); }
            PROPVARIANT pv = new();
            pv.Type = VARTYPE.VT_BSTR;
            System.Char* p;
            fixed (System.Char* src = bstr)
            {
                p = Interop.OleAut32.SysAllocStringLen(src , bstr.Length.ToUInt32());
            }
            if (p is null) { throw new OutOfMemoryException("Not enough memory to allocate the BSTR itself"); }
            pv.PPVTValue.pointerValue = new(p);
            return pv;
        }

        /*
         The C/C++ implementation of the below method is this:
        // Creates a VT_LPWSTR propvariant.
        inline HRESULT InitPropVariantFromString(_In_ PCWSTR psz, _Out_ PROPVARIANT *ppropvar)
        {
            HRESULT hr = psz != nullptr ? S_OK : E_INVALIDARG; // Previous API behavior counter to the SAL requirement.
            if (SUCCEEDED(hr))
            {
                SIZE_T const byteCount = static_cast<SIZE_T>((wcslen(psz) + 1) * sizeof(*psz));
                V_UNION(ppropvar, pwszVal) = static_cast<PWSTR>(CoTaskMemAlloc(byteCount));
                hr = V_UNION(ppropvar, pwszVal) ? S_OK : E_OUTOFMEMORY;
                if (SUCCEEDED(hr))
                {
                    memcpy_s(V_UNION(ppropvar, pwszVal), byteCount, psz, byteCount);
                    V_VT(ppropvar) = VT_LPWSTR;
                }
            }
            if (FAILED(hr))
            {
                PropVariantInit(ppropvar);
            }
            return hr;
        }
         */

        /// <summary>
        /// Creates a new PropVariant containing a null-terminated Unicode string. <br />
        /// The string is created by using the <strong>CoTaskMemAlloc</strong> function.
        /// </summary>
        /// <param name="pwstr">The string to encode.</param>
        /// <returns>The created PropVariant instance</returns>
        /// <exception cref="ArgumentNullException"><paramref name="pwstr"/> was <see langword="null"/>.</exception>
        /// <exception cref="OutOfMemoryException">Not enough memory to allocate for the given string.</exception>
        public static unsafe PROPVARIANT FromLPWSTR(System.String pwstr)
        {
            if (pwstr is null) { throw new ArgumentNullException(nameof(pwstr)); }
            System.UInt32 flen = ((pwstr.Length + 1U) * sizeof(System.Char)).ToUInt32();
            void* ptr = WindowsCOMLibrary.COMMemoryManager.Allocate(flen);
            if (ptr is null) {
                throw new OutOfMemoryException("Cannot allocate memory for the LPWSTR!!");
            }
            // .NET constructs the string pointer so that it contains a null value at the end.
            fixed (System.Char* psrc = pwstr)
            {
                Unsafe.CopyBlockUnaligned(ptr, psrc, flen);
            }
            PROPVARIANT pv = new();
            pv.Type = VARTYPE.VT_LPWSTR;
            pv.PPVTValue.pointerValue = new(ptr);
            return pv;
        }

        /// <summary>
        /// Creates a new PropVariant containing data from the specified byte array. <br />
        /// The data are saved by using the <see cref="VARTYPE.VT_VECTOR"/> variant type.
        /// </summary>
        /// <param name="array">The byte array to store.</param>
        /// <returns>The created PropVariant instance</returns>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> was <see langword="null"/>.</exception>
        /// <exception cref="OutOfMemoryException">There was not enough memory to allocate this array in native code.</exception>
        public static unsafe PROPVARIANT FromByteArray(System.Byte[] array)
        {
            ArgumentNullException.ThrowIfNull(array);
            HRESULT hr;
            PROPVARIANT pout;
            fixed (System.Byte* psrc = array)
            {
                hr = Interop.PropSys.InitPropVariantFromBuffer(psrc, array.Length.ToUInt32() , &pout);
            }
            hr.ThrowOnFailure();
            return pout;
        }

        /// <summary>
        /// Creates a new PropVariant containing data from the specified byte array. <br />
        /// The data are saved by using the <see cref="VARTYPE.VT_BLOB"/> variant type.
        /// </summary>
        /// <param name="array">The byte array to store.</param>
        /// <returns>The created PropVariant instance</returns>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> was <see langword="null"/>.</exception>
        /// <exception cref="OutOfMemoryException">There was not enough memory to allocate this array in native code.</exception>
        public static unsafe PROPVARIANT FromByteArrayUseBlob(System.Byte[] array)
        {
            ArgumentNullException.ThrowIfNull(array);
            BLOB blb = new();
            blb.Length = array.LongLength.ToUInt32();
            blb.Data = (System.Byte*)WindowsCOMLibrary.COMMemoryManager.Allocate(blb.Length);
            if (blb.Data is null) {
                throw new OutOfMemoryException("Cannot allocate memory for the native array!");
            } else {
                fixed (System.Byte* psrc = array) { Unsafe.CopyBlockUnaligned(blb.Data, psrc, blb.Length); }
                PROPVARIANT pv = new();
                pv.Type = VARTYPE.VT_BLOB;
                pv.PPVTValue.blobVal = blb;
                return pv;
            }
        }

        /// <summary>
        /// Creates a new PropVariant containing the specified <see cref="GUID"/>. <br />
        /// The GUID is stored by using the <see cref="VARTYPE.VT_CLSID"/> variant type.
        /// </summary>
        /// <param name="guid">The <see cref="GUID"/> to store.</param>
        /// <returns>The created PropVariant instance</returns>
        /// <exception cref="OutOfMemoryException">There was not enough memory to allocate this <see cref="GUID"/> in native code.</exception>
        public static unsafe PROPVARIANT FromGUID(GUID guid)
        {
            var hr = Interop.PropSys.InitPropVariantFromCLSID(guid, out var pv);
            if (hr.FAILED) {
                if (hr == CommonHResults.E_OUTOFMEMORY) {
                    throw new OutOfMemoryException("Cannot allocate memory for the GUID!");
                }
                throw hr.CreateException();
            }
            return pv;
        }
    }
}