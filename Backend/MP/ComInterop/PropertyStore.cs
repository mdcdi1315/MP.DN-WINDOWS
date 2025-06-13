
using MP.WindowsInterop;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MP.ComInterop
{
    /// <summary>
    /// Defines an interop class for the <see cref="IPropertyStore"/> interface.
    /// </summary>
    public unsafe sealed class PropertyStore : IDisposable
    {
        private IPropertyStore store;

        /// <summary>
        /// Loads a previously saved <see cref="PropertyStore"/> instance that was saved via the <see cref="Save(System.IO.Stream)"/> method.
        /// </summary>
        /// <param name="stream">The stream to load back.</param>
        /// <returns>The previously saved and reloaded <see cref="NamedPropertyStore"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> was <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="stream"/> was not readable.</exception>
        /// <exception cref="System.IO.IOException">Could not read the stream contents.</exception>
        public static PropertyStore LoadFrom(System.IO.Stream stream)
        {
            if (stream is null) { throw new ArgumentNullException(nameof(stream)); }
            if (stream.CanRead == false) { throw new ArgumentException("The stream was not readable.", nameof(stream)); }
            if (stream.Length > Array.MaxLength) { throw new ArgumentException("Cannot load such a large stream.", nameof(stream)); }
            System.Byte[] buftemp = new System.Byte[stream.Length];
            // Read the entire stream atomically
            if (stream.Read(buftemp, 0, buftemp.Length) < buftemp.Length) {
                throw new System.IO.IOException($"Could not read {buftemp.Length} bytes from the stream.");
            }
            // Now load the COM object
            Guid interfaceid = ComMarshalling.GetComInterfaceID<IPersistSerializedPropStorage>();
            HRESULT hr = Interop.PropSys.PSCreateMemoryPropertyStore(GUID.FromGUID(interfaceid), out void* pi);
            if (hr.FAILED) { throw hr.MappingException; }
            IPersistSerializedPropStorage storage = ComMarshalling.CreateInteropObject(pi) as IPersistSerializedPropStorage;
            // Provide the data to the deserializer
            fixed (System.Byte* pdt = buftemp)
            {
                hr = storage.SetPropertyStorage(pdt, buftemp.Length.ToUInt32());
            }
            if (hr.FAILED) { throw hr.MappingException; }
            // return the property store by using QueryInterface on it
            return new(storage as IPropertyStore);
        }

        /// <summary>
        /// Creates a temporary <see cref="PropertyStore"/> whose backing store is memory.
        /// </summary>
        /// <returns>A new in-memory <see cref="PropertyStore"/> instance (uses the <strong>PSCreateMemoryPropertyStore</strong> behind).</returns>
        public static PropertyStore CreateMemoryStore()
        {
            Guid interfaceid = ComMarshalling.GetComInterfaceID<IPropertyStore>();
            HRESULT hr = Interop.PropSys.PSCreateMemoryPropertyStore(GUID.FromGUID(interfaceid) , out void* pi);
            if (hr.FAILED) { throw hr.MappingException; }
            return new(ComMarshalling.CreateInteropObject(pi) as IPropertyStore);
        }

        /// <summary>
        /// Creates a <see cref="PropertyStore"/> instance from a preexisting COM object of type <see cref="IPropertyStore"/>. <br />
        /// Note that after passing in the store here, the passed property store is managed by the created instance.
        /// </summary>
        /// <param name="store">The COM object to create a new <see cref="NamedPropertyStore"/> instance.</param>
        /// <exception cref="ArgumentNullException"><paramref name="store"/> was <see langword="null"/>.</exception>
        public PropertyStore(IPropertyStore store)
        {
            if (store is null) { throw new ArgumentNullException(nameof(store)); }
            this.store = store;
        }

        /// <summary>
        /// Gets the number of properties defined in this property store.
        /// </summary>
        /// <exception cref="ObjectDisposedException">The object was disconnected from the native COM object.</exception>
        public System.Int32 Count
        {
            get {
                ObjectDisposedException.ThrowIf(store is null, this);
                System.UInt32 nprops;
                HRESULT hr = store.GetCount(&nprops);
                if (hr.SUCCEEDED) {
                    return nprops.ToInt32();
                } else {
                    throw hr.MappingException;
                }
            }
        }

        /// <summary>
        /// Gets the property key defined in the specified property index.
        /// </summary>
        /// <param name="index">The zero-based property index to get the property key.</param>
        /// <returns>The retrieved property key at <paramref name="index"/>.</returns>
        /// <exception cref="ObjectDisposedException">The object was disconnected from the native COM object.</exception>
        public PROPERTYKEY this[System.Int32 index]
        {
            get {
                ObjectDisposedException.ThrowIf(store is null, this);
                if (index < 0 || index >= Count) {
                    throw new ArgumentOutOfRangeException(nameof(index) ,"The property index was out of the internal array bounds.");
                }
                PROPERTYKEY pret;
                HRESULT hr = store.GetAt(index.ToUInt32(), &pret);
                if (hr.SUCCEEDED) { 
                    return pret;
                } else {
                    throw hr.MappingException;
                }
            }
        }

        /// <summary>
        /// Gets or sets the property value of a property key as a <see cref="PROPVARIANT"/> value.
        /// </summary>
        /// <param name="key">The property key to get it's current value.</param>
        /// <returns>The value of the <paramref name="key"/> parameter.</returns>
        /// <exception cref="ObjectDisposedException">The object was disconnected from the native COM object.</exception>
        public PROPVARIANT this[PROPERTYKEY key]
        {
            get {
                ObjectDisposedException.ThrowIf(store is null, this);
                PROPVARIANT pvret;
                HRESULT hr = store.GetValue(&key , &pvret);
                if (hr.SUCCEEDED) { 
                    return pvret;
                } else {
                    throw hr.MappingException;
                }
            }
            set {
                ObjectDisposedException.ThrowIf(store is null, this);
                HRESULT hr = store.SetValue(&key, &value);
                if (hr.FAILED) {
                    throw hr.MappingException;
                }
            }
        }

        /// <summary>
        /// Gets a property in it's entirety.
        /// </summary>
        /// <param name="key">The property key to retrieve the property.</param>
        /// <returns>The requested property.</returns>
        /// <exception cref="ObjectDisposedException">The object was disconnected from the native COM object.</exception>
        public PropertyStoreProperty GetProperty(PROPERTYKEY key) => new(key, this[key]);

        /// <summary>
        /// Queries for the specified property with the key defined; <br />
        /// If the property exists and the call was successfull , the method returns at the <paramref name="property"/> parameter the property.
        /// </summary>
        /// <param name="key">The property to query by key.</param>
        /// <param name="property">The queried property information.</param>
        /// <returns><see langword="true"/> when the call succeeded; otherwise it returns <see langword="false"/>.</returns>
        /// <exception cref="ObjectDisposedException">The object was disconnected from the native COM object.</exception>
        public System.Boolean TryGetProperty(PROPERTYKEY key , out PropertyStoreProperty property)
        {
            ObjectDisposedException.ThrowIf(store is null, this);
            PROPVARIANT vt;
            HRESULT hr = store.GetValue(&key , &vt);
            if (hr.SUCCEEDED) {
                property = new(key, vt);
                return true;
            }
            property = null;
            return false;
        }

        /// <summary>
        /// Saves the changes done to the property store (by using the <see cref="PROPERTYKEY"/> setter on the object)
        /// </summary>
        /// <exception cref="ObjectDisposedException">The object was disconnected from the native COM object.</exception>
        public void Commit()
        {
            ObjectDisposedException.ThrowIf(store is null, this);
            HRESULT hr = store.Commit();
            // If the call is not implemented, just return and consider that the property store has commited the changes
            if (hr == CommonHResults.E_NOTIMPL) { return; }
            if (hr.FAILED) {
                throw hr.MappingException;
            }
        }

        /// <summary>
        /// Saves the entire property store into a stream, if supported.
        /// </summary>
        /// <param name="stream">The stream to save the property store.</param>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> was <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="stream"/> was not writeable.</exception>
        /// <exception cref="NotSupportedException">Property save services are not implemented for this COM object.</exception>
        /// <exception cref="ObjectDisposedException">The object was disconnected from the native COM object.</exception>
        public void Save(System.IO.Stream stream)
        {
            const System.Int32 BUFSIZE = 1024;
            ObjectDisposedException.ThrowIf(store is null, this);
            if (stream is null) { throw new ArgumentNullException(nameof(stream)); }
            // First, QueryInterface to the object to learn out whether IPersistSerializedPropStorage is supported
            // If not, we must throw the below exception.
            if (store is not IPersistSerializedPropStorage storage)
            {
                throw new NotSupportedException("Save services are not provided for this implementation of the IPropertyStore interface.");
            }
            storage.SetFlags(PERSIST_SPROPSTORE_FLAGS.FPSPS_DEFAULT); // Default usage, if possible.
            System.Byte* pbuf;
            System.UInt32 datalen;
            HRESULT hr = storage.GetPropertyStorage(&pbuf, &datalen);
            if (hr.FAILED) { throw hr.MappingException; }
            try {
                System.UInt32 blks = datalen / BUFSIZE, rem = datalen % BUFSIZE;
                System.UInt32 idx = 0;
                System.Byte[] buffertemp = new System.Byte[BUFSIZE];
                // Write the data from the native buffer as 1024 byte blocks
                while (idx < blks)
                {
                    // Native to managed translation
                    Unsafe.CopyBlockUnaligned(ref buffertemp[0], ref pbuf[idx * BUFSIZE], BUFSIZE);
                    stream.Write(buffertemp, 0, buffertemp.Length);
                    idx++;
                }
                // Write last data found in the buffer
                if (rem > 0)
                {
                    buffertemp = new System.Byte[rem];
                    Unsafe.CopyBlockUnaligned(ref buffertemp[0], ref pbuf[datalen - rem], rem);
                    stream.Write(buffertemp, 0, buffertemp.Length);
                }
            } finally {
                // If failed or succeeded we must free the buffer with CoTaskMemFree
                Interop.Ole32.CoTaskMemFree(pbuf);
            }
        }

        /// <summary>
        /// Disposes this property store instance.
        /// </summary>
        public void Dispose() 
        {
            if (store is not null)
            {
                ComMarshalling.ReleaseInteropObject(store);
                store = null;
            }
            GC.SuppressFinalize(this);
        }
    }

    public sealed class PropertyStoreProperty
    {
        private PROPVARIANT propertynative;
        private PROPERTYKEY propertykey;

        public PropertyStoreProperty(PROPERTYKEY key , PROPVARIANT propertyvalue)
        {
            propertynative = propertyvalue;
            propertykey = key;
        }

        public PROPERTYKEY Key => propertykey;

        public PROPVARIANT PropertyValueNative => propertynative;

        public System.Object Value => propertynative.Value;

        unsafe ~PropertyStoreProperty()
        {
            // We do not care when this will be run , we just need it to run in fact.
            fixed (PROPVARIANT* pv = &propertynative)
            {
#if DEBUG
                HRESULT hrt = Interop.Ole32.PropVariantClear(pv);
                DebugProvider.WriteLine($"ObjectGraphTracker: Freed PROPVARIANT {new System.IntPtr(pv)} and error code was {hrt}.");
#else
                Interop.Ole32.PropVariantClear(pv);
#endif
            }
        }
        
    }
}