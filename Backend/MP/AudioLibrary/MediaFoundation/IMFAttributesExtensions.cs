

using MP.ComInterop;
using MP.WindowsInterop;
using System;

namespace MP.AudioLibrary.MediaFoundation
{
    /// <summary>
    /// Defines extension methods for manipulating <see cref="IMFAttributes"/> COM objects more easily.
    /// </summary>
    public static unsafe class IMFAttributesExtensions
    {
        [System.Diagnostics.StackTraceHidden]
        private static void VerifyItemTypeIsSupported(Type t , System.String pname = null)
        {
            ArgumentNullException.ThrowIfNull(t , pname);
            if (t == typeof(System.String) || 
                t == typeof(System.UInt32) || 
                t == typeof(System.UInt64) || 
                t == typeof(Guid) || 
                t == typeof(System.Double) || 
                t == typeof(System.Byte[]))
            {
                // OK, in this case.
            } else if (t == typeof(System.Object) && t.IsCOMObject == false)
            {
                throw new ArgumentException("Other .NET objects except from COM objects are not supported for use with IMFAttributes.");
            } else {
                // In all other cases, just fail.
                throw new ArgumentException($"The specified type cannot be represented as an IMFAttributes value: {t.FullName} .");
            }
        }

        /// <summary>
        /// Attempts to get the specified attribute from the current <see cref="IMFAttributes"/> instance.
        /// </summary>
        /// <typeparam name="TItem">The .NET type of the attribute value you wish to be returned.</typeparam>
        /// <param name="mfattrs"></param>
        /// <param name="key">The GUID of the attribute's key that you wish to be returned.</param>
        /// <param name="item">The attribute value, as a marshalled .NET object.</param>
        /// <returns><see langword="true"/> if the requested attribute was found and returned; otherwise , <see langword="false"/>.</returns>
        /// <exception cref="MP.ExceptionSystem.NativeWindowsCOMException">Any other error that may occur, except for the attribute not found case.</exception>
        public static System.Boolean TryGetAttribute<TItem>(this IMFAttributes mfattrs , Guid key , out TItem item)
        {
            Type t = typeof(TItem);
            VerifyItemTypeIsSupported(t, nameof(TItem));
            PROPVARIANT pv;
            GUID k = GUID.FromGUID(key);
            HRESULT hr = mfattrs.GetItem(&k, &pv);
            item = default;
            switch (hr)
            {
                case MediaFoundationErrorCodes.MF_E_ATTRIBUTENOTFOUND:
                    return false;
                case CommonHResults.S_OK:
                    // Special case: The PROPVARIANT returns WindowsInterop.GUID in Value, make it System.Guid instead
                    if (typeof(TItem) == typeof(Guid)) {
                        item = (TItem)((System.Object)((GUID)pv.Value).GetGuid());
                    } else {
                        item = (TItem)pv.Value;
                    }
                    pv.Dispose(); // Calls PropVariantClear, as we would otherwise expect
                    return true;
                default:
                    hr.ThrowOnFailure();
                    return false;
            }
        }

        /// <summary>
        /// Finds whether a specified attribute does exist in the current <see cref="IMFAttributes"/> instance.
        /// </summary>
        /// <param name="mfattrs"></param>
        /// <param name="key">The GUID of the attribute's key that you want to verify it's existense.</param>
        /// <returns><see langword="true"/> if the requested attribute was found; otherwise , <see langword="false"/>.</returns>
        /// <exception cref="MP.ExceptionSystem.NativeWindowsCOMException">Any other error that may occur, except for the attribute not found case.</exception>
        public static System.Boolean AttributeExists(this IMFAttributes mfattrs, Guid key)
        {
            GUID k = GUID.FromGUID(key);
            HRESULT hr = mfattrs.GetItem(&k, null);
            switch (hr)
            {
                case MediaFoundationErrorCodes.MF_E_ATTRIBUTENOTFOUND:
                    return false;
                case CommonHResults.S_OK:
                    return true;
                default:
                    hr.ThrowOnFailure();
                    return false; // This is possibly unreachable, but put it to mock C# compiler that we have covered all code paths
            }
        }

        /// <summary>
        /// Gets the specified attribute from the current <see cref="IMFAttributes"/> instance. <br />
        /// If the requested attribute was not found , it throws back <see cref="MediaFoundationAttributeNotFoundException"/>.
        /// </summary>
        /// <typeparam name="TItem">The .NET type of the attribute value you wish to be returned.</typeparam>
        /// <param name="attributes"></param>
        /// <param name="key">The GUID of the attribute's key that you wish to be returned.</param>
        /// <returns>The attribute's value, as a marshalled .NET object.</returns>
        /// <exception cref="MediaFoundationAttributeNotFoundException">Thrown when the requested attribute does not exist on the object.</exception>
        /// <exception cref="ExceptionSystem.NativeWindowsCOMException">Any other error that may occur, except for the attribute not found case.</exception>
        public static TItem GetAttribute<TItem>(this IMFAttributes attributes, Guid key) 
        {
            if (TryGetAttribute(attributes , key , out TItem item) == false) {
                throw new MediaFoundationAttributeNotFoundException(key);
            }
            return item;
        }

        /// <summary>
        /// Creates or updates an attribute that has a value of type <see cref="System.UInt32"/>.
        /// </summary>
        /// <param name="attributes"></param>
        /// <param name="key">The GUID of the attribute's key to set.</param>
        /// <param name="value">The new attribute value.</param>
        /// <exception cref="OutOfMemoryException">Not enough memory to complete the operation.</exception>
        public static void SetAttribute(this IMFAttributes attributes, Guid key, System.UInt32 value)
        {
            GUID k = GUID.FromGUID(key);
            // Doc says that only S_OK and E_OUTOFMEMORY can occur , so we can just use ThrowOnFailure.
            attributes.SetUINT32(&k, value).ThrowOnFailure();
        }

        /// <summary>
        /// Creates or updates an attribute that has a value of type <see cref="System.UInt64"/>.
        /// </summary>
        /// <param name="attributes"></param>
        /// <param name="key">The GUID of the attribute's key to set.</param>
        /// <param name="value">The new attribute value.</param>
        /// <exception cref="OutOfMemoryException">Not enough memory to complete the operation.</exception>
        public static void SetAttribute(this IMFAttributes attributes , Guid key , System.UInt64 value)
        {
            GUID k = GUID.FromGUID(key);
            // Doc says that only S_OK and E_OUTOFMEMORY can occur , so we can just use ThrowOnFailure.
            attributes.SetUINT64(&k, value).ThrowOnFailure();
        }

        /// <summary>
        /// Creates or updates an attribute that has a value of type <see cref="System.String"/>.
        /// </summary>
        /// <param name="attributes"></param>
        /// <param name="key">The GUID of the attribute's key to set.</param>
        /// <param name="value">The new attribute value.</param>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> was <see langword="null"/>.</exception>
        /// <exception cref="OutOfMemoryException">Not enough memory to complete the operation.</exception>
        public static void SetAttribute(this IMFAttributes attributes , Guid key , System.String value)
        {
            ArgumentNullException.ThrowIfNull(value);
            GUID k = GUID.FromGUID(key);
            fixed (System.Char* pvalue = value)
            {
                attributes.SetString(&k, pvalue).ThrowOnFailure();
            }
        }

        /// <summary>
        /// Creates or updates an attribute that has a value of a byte array.
        /// </summary>
        /// <param name="attributes"></param>
        /// <param name="key">The GUID of the attribute's key to set.</param>
        /// <param name="value">The new attribute value.</param>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> was <see langword="null"/>.</exception>
        /// <exception cref="OutOfMemoryException">Not enough memory to complete the operation.</exception>
        public static void SetAttribute(this IMFAttributes attributes , Guid key , System.Byte[] value)
        {
            ArgumentNullException.ThrowIfNull(value);
            GUID k = GUID.FromGUID(key);
            fixed (System.Byte* pvalue = value)
            {
                attributes.SetBlob(&k , pvalue , value.LongLength.ToUInt32()).ThrowOnFailure();
            }
        }

        /// <summary>
        /// Creates or updates an attribute that has a value of a byte array.
        /// </summary>
        /// <param name="attributes"></param>
        /// <param name="key">The GUID of the attribute's key to set.</param>
        /// <param name="value">The new attribute value.</param>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> was <see langword="null"/>.</exception>
        /// <exception cref="OutOfMemoryException">Not enough memory to complete the operation.</exception>
        public static void SetAttribute(this IMFAttributes attributes , Guid key , Span<System.Byte> value)
        {
            GUID k = GUID.FromGUID(key);
            fixed (System.Byte* pvalue = value)
            {
                attributes.SetBlob(&k, pvalue, value.Length.ToUInt32()).ThrowOnFailure();
            }
        }

        /// <summary>
        /// Creates or updates an attribute that has a value of type <see cref="System.Double"/>.
        /// </summary>
        /// <param name="attributes"></param>
        /// <param name="key">The GUID of the attribute's key to set.</param>
        /// <param name="value">The new attribute value.</param>
        /// <exception cref="OutOfMemoryException">Not enough memory to complete the operation.</exception>
        public static void SetAttribute(this IMFAttributes attributes, Guid key, System.Double value)
        {
            GUID k = GUID.FromGUID(key);
            // Doc says that only S_OK and E_OUTOFMEMORY can occur , so we can just use ThrowOnFailure.
            attributes.SetDouble(&k, value).ThrowOnFailure();
        }

        /// <summary>
        /// Creates or updates an attribute that has a value of type <see cref="Guid"/>.
        /// </summary>
        /// <param name="attributes"></param>
        /// <param name="key">The GUID of the attribute's key to set.</param>
        /// <param name="value">The new attribute value.</param>
        /// <exception cref="OutOfMemoryException">Not enough memory to complete the operation.</exception>
        public static void SetAttribute(this IMFAttributes attributes, Guid key, Guid value)
        {
            GUID k = GUID.FromGUID(key);
            GUID v = GUID.FromGUID(value);
            // Doc says that only S_OK and E_OUTOFMEMORY can occur , so we can just use ThrowOnFailure.
            attributes.SetGUID(&k, &v).ThrowOnFailure();
        }

        /// <summary>
        /// Gets the number of attributes contained in the current set.
        /// </summary>
        /// <param name="attributes"></param>
        /// <returns>The number of attributes contained.</returns>
        /// <exception cref="ExceptionSystem.NativeWindowsCOMException">Thrown when any unexpected error occurs</exception>
        public static System.UInt32 GetCount(this IMFAttributes attributes) 
        {
            System.UInt32 pcitems;
            attributes.GetCount(&pcitems).ThrowOnFailure();
            return pcitems;
        }
    }
}