

using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using MP.WindowsInterop;

namespace MP.ComInterop
{
    /// <summary>
    /// Defines an alternative way to store properties in COM. <br />
    /// Unlike the <see cref="PropertyStore"/> class which saves all the keys in a specific format, the <see cref="NamedPropertyStore"/> takes that a step further and allows the keys to be any arbitrary string. <br />
    /// Another difference of this class with the <see cref="PropertyStore"/> one is that this one does not define a Commit method and does not have any support for returning <see cref="PropertyStoreProperty"/> instances.
    /// </summary>
    public unsafe sealed class NamedPropertyStore : IDisposable
    {
        private INamedPropertyStore store;

        /// <summary>
        /// Loads a previously saved <see cref="NamedPropertyStore"/> instance that was saved via the <see cref="Save(System.IO.Stream)"/> method.
        /// </summary>
        /// <param name="stream">The stream to load back.</param>
        /// <returns>The previously saved and reloaded <see cref="NamedPropertyStore"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> was <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="stream"/> was not readable.</exception>
        /// <exception cref="System.IO.IOException">Could not read the stream contents.</exception>
        public static NamedPropertyStore LoadFrom(System.IO.Stream stream)
        {
            if (stream is null) { throw new ArgumentNullException(nameof(stream)); }
            if (stream.CanRead == false) { throw new ArgumentException("The stream was not readable.", nameof(stream)); }
            if (stream.Length > Array.MaxLength) { throw new ArgumentException("Cannot load such a large stream." , nameof(stream)); }
            System.Byte[] buftemp = new System.Byte[stream.Length];
            // Read the entire stream atomically
            if (stream.Read(buftemp, 0, buftemp.Length) < buftemp.Length) {
                throw new System.IO.IOException($"Could not read {buftemp.Length} bytes from the stream.");
            }
            // Now load the COM object
            GUID interfaceid = GUID.FromGUID(ComMarshalling.GetComInterfaceID<IPersistSerializedPropStorage>());
            HRESULT hr = Interop.PropSys.PSCreateMemoryPropertyStore(interfaceid, out void* pi);
            if (hr.FAILED) { throw hr.MappingException; }
            IPersistSerializedPropStorage storage = ComMarshalling.CreateInteropObject(pi) as IPersistSerializedPropStorage;
            if (storage is null) {
                Marshal.Release(hr);
                throw new NotSupportedException();
            }
            // Provide the data to the deserializer
            fixed (System.Byte* pdt = buftemp)
            {
                hr = storage.SetPropertyStorage(pdt, buftemp.Length.ToUInt32());
            }
            if (hr.FAILED) { throw hr.MappingException; }
            // return the named property store by using QueryInterface on it
            return new(storage as INamedPropertyStore);
        }

        /// <summary>
        /// Creates a temporary <see cref="NamedPropertyStore"/> whose backing store is memory. <br />
        /// Works exactly the same as you would expect from the <see cref="PropertyStore.CreateMemoryStore"/> method.
        /// </summary>
        /// <returns>A new in-memory <see cref="NamedPropertyStore"/> instance (uses the <strong>PSCreateMemoryPropertyStore</strong> behind).</returns>
        public static NamedPropertyStore CreateMemoryStore()
        {
            Guid interfaceid = ComMarshalling.GetComInterfaceID<INamedPropertyStore>();
            HRESULT hr = Interop.PropSys.PSCreateMemoryPropertyStore(GUID.FromGUID(interfaceid), out void* pi);
            if (hr.FAILED) { throw hr.MappingException; }
            return new(ComMarshalling.CreateInteropObject(pi) as INamedPropertyStore);
        }

        /// <summary>
        /// Creates a <see cref="NamedPropertyStore"/> instance from a preexisting COM object of type <see cref="INamedPropertyStore"/>. <br />
        /// Note that after passing in the store here, the passed property store is managed by the created instance.
        /// </summary>
        /// <param name="store">The COM object to create a new <see cref="NamedPropertyStore"/> instance.</param>
        /// <exception cref="ArgumentNullException"><paramref name="store"/> was <see langword="null"/>.</exception>
        public NamedPropertyStore(INamedPropertyStore store)
        {
            if (store is null) { throw new ArgumentNullException(nameof(store)); }
            this.store = store;
        }

        /// <summary>
        /// Gets the number of properties defined in the current property store.
        /// </summary>
        public System.Int32 Count
        {
            get {
                ObjectDisposedException.ThrowIf(store is null, this);
                System.UInt32 ct;
                HRESULT hr = store.GetNameCount(&ct);
                if (hr.FAILED) { throw hr.MappingException; }
                return ct.ToInt32();
            }
        }

        /// <summary>
        /// Gets the property key defined in the specified property index, as a string.
        /// </summary>
        /// <param name="index">The zero-based property index to get the property key.</param>
        /// <returns>The retrieved property key at <paramref name="index"/>.</returns>
        /// <exception cref="ObjectDisposedException">The object was disconnected from the native COM object.</exception>
        public System.String this[System.Int32 index]
        {
            get {
                ObjectDisposedException.ThrowIf(store is null, this);
                if (index < 0 || index >= Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), "The property index was out of the internal array bounds.");
                }
                System.Char* pname;
                HRESULT hr = store.GetNameAt(index.ToUInt32() , &pname);
                if (hr.SUCCEEDED) {
                    BSTR bstr = null;
                    try {
                        bstr = new(pname);
                        return bstr.ToString();
                    } finally {
                        bstr?.Dispose();
                    }
                } else {
                    throw hr.MappingException;
                }
            }
        }

        /// <summary>
        /// Gets or sets the property value of a property key string as a <see cref="PROPVARIANT"/> value. <br />
        /// If the specified key does not exist in the store it returns a <see cref="PROPVARIANT"/> whose <see cref="PROPVARIANT.Type"/> is <see cref="VARTYPE.VT_EMPTY"/>. <br />
        /// When setting and the specified key does not exist, a new one is created on the fly.
        /// </summary>
        /// <param name="key">The property key string to get it's current value.</param>
        /// <returns>The value of the <paramref name="key"/> parameter.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> was <see langword="null"/>.</exception>
        /// <exception cref="ObjectDisposedException">The object was disconnected from the native COM object.</exception>
        public PROPVARIANT this[System.String key]
        {
            get {
                ObjectDisposedException.ThrowIf(store is null, this);
                if (System.String.IsNullOrEmpty(key)) { throw new ArgumentNullException(nameof(key)); }
                PROPVARIANT pvret;
                HRESULT hr;
                fixed (System.Char* pk = key)
                {
                    hr = store.GetNamedValue(pk, &pvret);
                }
                if (hr.SUCCEEDED) {
                    return pvret;
                } else {
                    throw hr.MappingException;
                }
            }
            set {
                ObjectDisposedException.ThrowIf(store is null, this);
                if (System.String.IsNullOrEmpty(key)) { throw new ArgumentNullException(nameof(key)); }
                HRESULT hr;
                fixed (System.Char* pk = key)
                {
                    hr = store.SetNamedValue(pk, &value);
                }
                if (hr.FAILED)
                {
                    throw hr.MappingException;
                }
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
            if (stream.CanWrite == false) { throw new ArgumentException("The stream was not writeable." , nameof(stream)); }
            // First, QueryInterface to the object to learn out whether IPersistSerializedPropStorage is supported
            // If not, we must throw the below exception.
            if (store is not IPersistSerializedPropStorage storage)
            {
                throw new NotSupportedException("Save services are not provided for this implementation of the INamedPropertyStore interface.");
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
        /// Disposes this <see cref="NamedPropertyStore"/> instance.
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
}