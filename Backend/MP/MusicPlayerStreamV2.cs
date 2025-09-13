using System;
using MP.ComInterop;
using MP.Annotations;
using System.Threading;
using MP.WindowsInterop;
using MP.AudioLibrary.MediaFoundation;
using System.Runtime.CompilerServices;

namespace MP
{
    /// <summary>
    /// A newer version of the <see cref="MusicPlayerStream"/> class but instead it supports <see cref="IMFByteStream"/> as it's base interface. <br />
    /// Additionally, this class can be also inherited.
    /// </summary>
    [Preliminary]
    public unsafe class MusicPlayerStreamV2 : AbstractPropertyStream , IMFByteStream , IMFAttributes
    {
        private Func<IMFAttributes> attributescreator;
        private volatile AsyncWorkData asyncdata;
        private IMFAttributes attributes;

        // Asyncronous work data for an asyncronous request.
        // These data are both used for R/W operations.
        private sealed class AsyncWorkData
        {
            public System.Byte[] AllocatedDotNetBuffer;
            public System.Byte* NativePointer;
            public System.Int32 DataSize;

            public AsyncWorkData(System.Byte[] buffer , System.Byte* native , System.Int32 cb)
            {
                AllocatedDotNetBuffer = buffer;
                NativePointer = native;
                DataSize = cb;
            }
        }

        /// <summary>
        /// Creates a new instance of the <see cref="MusicPlayerStreamV2"/> class by specifying the stream to wrap.
        /// </summary>
        /// <param name="wrappingstream">The stream to wrap.</param>
        public MusicPlayerStreamV2(System.IO.Stream wrappingstream) : this(wrappingstream, false) { }

        /// <summary>
        /// Creates a new instance of the <see cref="MusicPlayerStreamV2"/> class by specifying the stream to wrap,
        /// and a value whether the access to the stream must be syncronized.
        /// </summary>
        /// <param name="wrappingstream">The stream to wrap.</param>
        /// <param name="synchronize">If true, it does then create a syncroized wrapper around <paramref name="wrappingstream"/>.</param>
        public MusicPlayerStreamV2(System.IO.Stream wrappingstream, System.Boolean synchronize) : this(wrappingstream , synchronize , new(static () => new FullyManagedMFAttributes())) { }

        /// <summary>
        /// Creates a new instance of the <see cref="MusicPlayerStreamV2"/> class by specifying 
        /// the stream to wrap, a value whether the access to the stream must be syncronized,
        /// and a function returning an instance of the <see cref="IMFAttributes"/> interface that provides the implementation of this class <see cref="IMFAttributes"/> interface.
        /// </summary>
        /// <param name="wrappingstream">The stream to wrap.</param>
        /// <param name="synchronize">If true, it does then create a syncroized wrapper around <paramref name="wrappingstream"/>.</param>
        /// <param name="supplier">The function, that, when called, it will return an implementation of the <see cref="IMFAttributes"/> interface. The IMFAttributes instance will be created lazily on first access of one of the interface's methods.</param>
        public MusicPlayerStreamV2(System.IO.Stream wrappingstream , System.Boolean synchronize , Func<IMFAttributes> supplier) 
            : base(wrappingstream , synchronize)
        {
            ArgumentNullException.ThrowIfNull(supplier);
            attributes = null;
            attributescreator = supplier; // Created lazily
            asyncdata = null; // No asyncronous data are expected at the beginning
        }

        /// <summary>
        /// Creates a new <see cref="MusicPlayerStream"/> object by using a memory stream.
        /// </summary>
        /// <param name="initcap">The initial memory stream capacity.</param>
        /// <returns>A music player stream created from a memory stream.</returns>
        public static MusicPlayerStreamV2 CreateMemoryStream(System.Int64 initcap)
        {
            MusicPlayerStreamV2 created = new(new Microsoft.IO.MemoryStream(initcap), false);
            created.SetBooleanAttribute(IsStreamOwnerForced, true);
            return created;
        }

        #region IMFByteStream implementation

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "It is simply a flag not defined back then, it can be ignored by Media Foundation.")]
        HRESULT IMFByteStream.GetCapabilities(MFBYTESTREAM_CAPABILITIES* pdwCapabilities)
        {
            MFBYTESTREAM_CAPABILITIES cps = MFBYTESTREAM_CAPABILITIES.DOES_NOT_USE_NETWORK;
            if (Wrapped.CanSeek) {
                cps |= MFBYTESTREAM_CAPABILITIES.IS_SEEKABLE;
            }
            if (Wrapped.CanWrite) {
                cps |= MFBYTESTREAM_CAPABILITIES.IS_WRITABLE;
            }
            if (Wrapped.CanRead) {
                cps |= MFBYTESTREAM_CAPABILITIES.IS_READABLE;
            }
            *pdwCapabilities = cps;
            return CommonHResults.S_OK;
        }

        HRESULT IMFByteStream.GetLength(ulong* pqwLength)
        {
            long length = 0;
            try {
                length = Wrapped.Length;
                *pqwLength = length.ToUInt64();
            } catch (NotSupportedException) {
                return MediaFoundationErrorCodes.MF_E_BYTESTREAM_UNKNOWN_LENGTH;
            }
            return CommonHResults.S_OK;
        }

        HRESULT IMFByteStream.SetLength(ulong qwLength)
        {
            if (qwLength > System.Int64.MaxValue) {
                return CommonHResults.E_INVALIDARG;
            }
            try {
                Wrapped.SetLength(qwLength.ToInt64());    
            } catch (NotSupportedException) {
                return CommonHResults.E_FAIL;
            } catch (System.IO.IOException) {
                return CommonHResults.STG_E_WRITEFAULT;
            }
            return CommonHResults.S_OK;
        }

        HRESULT IMFByteStream.GetCurrentPosition(ulong* pqwPosition)
        {
            long pos = 0;
            try {
                pos = Wrapped.Position;
                *pqwPosition = pos.ToUInt64();
            } catch (NotSupportedException) {
                return CommonHResults.E_FAIL;
            }
            return CommonHResults.S_OK;
        }

        HRESULT IMFByteStream.SetCurrentPosition(ulong qwPosition)
        {
            if (qwPosition > System.Int64.MaxValue) {
                return CommonHResults.E_INVALIDARG;
            }
            try {
                Wrapped.Seek(qwPosition.ToInt64(), System.IO.SeekOrigin.Begin);
            } catch (System.IO.IOException) { 
                return CommonHResults.STG_E_SEEKERROR;
            } catch (NotSupportedException) {
                return CommonHResults.E_FAIL;
            }
            return CommonHResults.S_OK;
        }

        HRESULT IMFByteStream.IsEndOfStream(BOOL* pfEndOfStream)
        {
            try {
                *pfEndOfStream = Wrapped.Position < Wrapped.Length ? BOOL.FALSE : BOOL.TRUE;
            } catch (NotSupportedException) {
                return CommonHResults.E_FAIL;
            } catch (System.IO.IOException) {
                return CommonHResults.STG_E_SEEKERROR;
            }
            return CommonHResults.S_OK;
        }

        HRESULT IMFByteStream.Read(byte* pd, uint cb, uint* pcbRead)
        {
            if (pd is null || pcbRead is null) { return CommonHResults.E_INVALIDARG; }
            if (cb > System.Int32.MaxValue) { return CommonHResults.E_INVALIDARG; }
            var sp = System.Buffers.ArrayPool<System.Byte>.Shared;
            System.Byte[] temp = null;
            try
            {
                temp = sp.Rent(cb.ToInt32());
                int read = Wrapped.Read(temp, 0, temp.Length);
                if (read == -1) { *pcbRead = 0; return CommonHResults.S_OK; }
                fixed (System.Byte* ps = temp)
                {
                    Unsafe.CopyBlockUnaligned(pd, ps, *pcbRead = read.ToUInt32());
                }
            } catch (System.IO.IOException) {
                return CommonHResults.E_FAIL;
            } catch (OutOfMemoryException) {
                return CommonHResults.E_OUTOFMEMORY;
            } catch (NotSupportedException) { 
                return CommonHResults.STG_E_READFAULT;
            } catch {
                return CommonHResults.E_FAIL;
            } finally {
                if (temp is not null)
                {
                    sp.Return(temp);
                }
            }
            return CommonHResults.S_OK;
        }

        // BeginRead in this implementation prepares the environment for an asyncronous read run.
        // It does also test whether the intermediary buffer can be created, and performs basic parameter validation.
        HRESULT IMFByteStream.BeginRead(byte* pb, uint cb, void* pCallback, void* punkState)
        {
            if (cb > System.Int32.MaxValue) {
                return CommonHResults.E_INVALIDARG;
            }
            System.Byte[] temp;
            var sp = System.Buffers.ArrayPool<System.Byte>.Shared;
            try {
                temp = sp.Rent(cb.ToInt32());
            } catch (OutOfMemoryException) {
                // Fail fast
                return CommonHResults.E_OUTOFMEMORY;
            }
            if (Interlocked.CompareExchange(ref asyncdata, new AsyncWorkData(temp, pb, cb.ToInt32()), null) != null) {
                sp.Return(temp);
                return CommonHResults.E_ILLEGAL_METHOD_CALL;
            }
            return Interop.MfPlat.MFPutWorkItem(AudioLibrary.WindowsAudioLibrary.ApplicationMediaFoundationWorkQueue, pCallback, punkState);
        }

        HRESULT IMFByteStream.EndRead(void* pResult, uint* pcbRead)
        {
            AsyncWorkData d = Interlocked.Exchange(ref asyncdata, null); // This will immediately null the field so that the next async operation can be queued
            if (d is null) { return CommonHResults.E_ILLEGAL_METHOD_CALL; } // Invalid to call without context
            try {
                // Calling the heavy method inside the async execution.
                // Note that I do not call the Stream's dedicated ReadAsync method since we are already into an asyncronous context.
                var async_br = Wrapped.Read(d.AllocatedDotNetBuffer, 0, d.DataSize).ToUInt32();
                fixed (System.Byte* p = d.AllocatedDotNetBuffer)
                {
                    Unsafe.CopyBlockUnaligned(d.NativePointer, p, async_br);
                }
                // In my implementation, using the pcbRead value is optional, however doc enforces that everyone should pass this parameter a valid value.
                if (pcbRead is not null) { *pcbRead = async_br; }
            } catch (System.IO.IOException) {
                return CommonHResults.STG_E_READFAULT;
            } catch {
                return CommonHResults.E_FAIL;
            } finally {
                System.Buffers.ArrayPool<System.Byte>.Shared.Return(d.AllocatedDotNetBuffer);
            }
            return CommonHResults.S_OK;
        }

        HRESULT IMFByteStream.Write(byte* pb, uint cb, uint* pcbWritten)
        {
            if (pb is null || cb > System.Int32.MaxValue || pcbWritten is null) { return CommonHResults.E_INVALIDARG; }
            var p = System.Buffers.ArrayPool<System.Byte>.Shared;
            System.Byte[] temp = null;
            try
            {
                temp = p.Rent(cb.ToInt32());
                fixed (System.Byte* pc = temp)
                {
                    Unsafe.CopyBlockUnaligned(pb, pc, cb);
                }
                Wrapped.Write(temp, 0, cb.ToInt32());
                if (pcbWritten is not null) { *pcbWritten = cb; }
            } catch (System.IO.IOException) {
                return CommonHResults.E_FAIL;
            } catch (NotSupportedException) {
                return CommonHResults.STG_E_WRITEFAULT;
            } catch (OutOfMemoryException) {
                return CommonHResults.E_OUTOFMEMORY;
            } finally {
                if (temp is not null) { p.Return(temp); }
            }
            return CommonHResults.S_OK;
        }

        // BeginWrite in this implementation prepares the environment for an asyncronous write run.
        // It does also test whether the intermediary buffer can be created, and performs basic parameter validation.
        HRESULT IMFByteStream.BeginWrite(byte* pb, uint cb, void* pCallback, void* punkState)
        {
            if (cb > System.Int32.MaxValue) {
                return CommonHResults.E_INVALIDARG;
            }
            System.Byte[] temp;
            var sp = System.Buffers.ArrayPool<System.Byte>.Shared;
            try {
                temp = sp.Rent(cb.ToInt32());
            } catch (OutOfMemoryException) {
                // Fail fast
                return CommonHResults.E_OUTOFMEMORY;
            }
            if (Interlocked.CompareExchange(ref asyncdata, new AsyncWorkData(temp, pb, cb.ToInt32()), null) != null) {
                sp.Return(temp);
                return CommonHResults.E_ILLEGAL_METHOD_CALL;
            }
            return Interop.MfPlat.MFPutWorkItem(AudioLibrary.WindowsAudioLibrary.ApplicationMediaFoundationWorkQueue, pCallback, punkState);
        }

        HRESULT IMFByteStream.EndWrite(void* pResult, uint* pcbWritten)
        {
            AsyncWorkData d = Interlocked.Exchange(ref asyncdata, null); // This will immediately null the field so that the next async operation can be queued
            if (d is null) { return CommonHResults.E_ILLEGAL_METHOD_CALL; } // Invalid to call without context
            try {
                var ds = d.DataSize.ToUInt32();
                fixed (System.Byte* pc = d.AllocatedDotNetBuffer)
                {
                    Unsafe.CopyBlockUnaligned(d.NativePointer, pc, ds);
                }
                // Calling the heavy method inside the async execution.
                // Note that I do not call the Stream's dedicated WriteAsync method since we are already into an asyncronous context.
                Wrapped.Write(d.AllocatedDotNetBuffer, 0, d.DataSize);
                // In my implementation, using the pcbWritten value is optional, however doc enforces that everyone should pass this parameter a valid value.
                if (pcbWritten is not null) { *pcbWritten = ds; }
            } catch (System.IO.IOException) {
                return CommonHResults.E_FAIL;
            } catch (NotSupportedException) {
                return CommonHResults.STG_E_WRITEFAULT;
            } finally {
                // Either case or failure this buffer will be returned to the array pool
                System.Buffers.ArrayPool<System.Byte>.Shared.Return(d.AllocatedDotNetBuffer);
            }
            return CommonHResults.S_OK;
        }

        HRESULT IMFByteStream.Seek(MFBYTESTREAM_SEEK_ORIGIN SeekOrigin, long llSeekOffset, MFBYTESTREAM_SEEK_FLAGS dwSeekFlags, ulong* pqwCurrentPosition)
        {
            System.IO.SeekOrigin so = SeekOrigin switch { MFBYTESTREAM_SEEK_ORIGIN.Begin => System.IO.SeekOrigin.Begin, MFBYTESTREAM_SEEK_ORIGIN.Current => System.IO.SeekOrigin.Current , _ => (System.IO.SeekOrigin)800 };
            if (so == (System.IO.SeekOrigin)800) { return CommonHResults.E_INVALIDARG; }
            try {
                ulong ulofs = Wrapped.Seek(llSeekOffset, so).ToUInt64();
                if (pqwCurrentPosition is not null) { *pqwCurrentPosition = ulofs; }
            } catch (System.IO.IOException) {
                return CommonHResults.STG_E_SEEKERROR;
            } catch (NotSupportedException) {
                return CommonHResults.E_FAIL;
            }
            return CommonHResults.S_OK;
        }

        HRESULT IMFByteStream.Flush()
        {
            try {
                Wrapped.Flush();
            } catch (System.IO.IOException) {
                return CommonHResults.E_FAIL;
            }
            return CommonHResults.S_OK;
        }

        HRESULT IMFByteStream.Close() => CommonHResults.S_OK;

        #endregion

        #region IMFAttributes implementation

        HRESULT IMFAttributes.GetItem(GUID* guidKey, PROPVARIANT* pValue) => IMFAttributesStore.GetItem(guidKey , pValue);

        HRESULT IMFAttributes.GetItemType(GUID* guidKey, MF_ATTRIBUTE_TYPE* pType) => IMFAttributesStore.GetItemType(guidKey , pType);

        HRESULT IMFAttributes.CompareItem(GUID* guidKey, PROPVARIANT* Value, BOOL* pbResult) => IMFAttributesStore.CompareItem(guidKey , Value, pbResult);

        HRESULT IMFAttributes.Compare(void* pTheirs, MF_ATTRIBUTES_MATCH_TYPE MatchType, BOOL* pbResult) => IMFAttributesStore.Compare(pTheirs, MatchType, pbResult);

        HRESULT IMFAttributes.GetUINT32(GUID* guidKey, uint* punValue) => IMFAttributesStore.GetUINT32(guidKey , punValue);

        HRESULT IMFAttributes.GetUINT64(GUID* guidKey, ulong* punValue) => IMFAttributesStore.GetUINT64(guidKey , punValue);

        HRESULT IMFAttributes.GetDouble(GUID* guidKey, double* pfValue) => IMFAttributesStore.GetDouble(guidKey , pfValue);

        HRESULT IMFAttributes.GetGUID(GUID* guidKey, GUID* pguidValue) => IMFAttributesStore.GetGUID(guidKey , pguidValue);

        HRESULT IMFAttributes.GetStringLength(GUID* guidKey, uint* pcchLength) => IMFAttributesStore.GetStringLength(guidKey , pcchLength);

        HRESULT IMFAttributes.GetString(GUID* guidKey, char* pwszValue, uint cchBufSize, uint* pcchLength) => IMFAttributesStore.GetString(guidKey , pwszValue , cchBufSize , pcchLength);

        HRESULT IMFAttributes.GetAllocatedString(GUID* guidKey, char** ppwszValue, uint* pcchLength) => IMFAttributesStore.GetAllocatedString(guidKey, ppwszValue, pcchLength);

        HRESULT IMFAttributes.GetBlobSize(GUID* guidKey, uint* pcbBlobSize) => IMFAttributesStore.GetBlobSize(guidKey, pcbBlobSize);

        HRESULT IMFAttributes.GetBlob(GUID* guidKey, byte* pBuf, uint cbBufSize, uint* pcbBlobSize) => IMFAttributesStore.GetBlob(guidKey, pBuf, cbBufSize, pcbBlobSize);

        HRESULT IMFAttributes.GetAllocatedBlob(GUID* guidKey, byte** ppBuf, uint* pcbSize) => IMFAttributesStore.GetAllocatedBlob(guidKey, ppBuf, pcbSize);

        HRESULT IMFAttributes.GetUnknown(GUID* guidKey, GUID* riid, void** ppv) => IMFAttributesStore.GetUnknown(guidKey, riid, ppv);

        HRESULT IMFAttributes.SetItem(GUID* guidKey, PROPVARIANT* Value) => IMFAttributesStore.SetItem(guidKey, Value);

        HRESULT IMFAttributes.DeleteItem(GUID* guidKey) => IMFAttributesStore.DeleteItem(guidKey);

        HRESULT IMFAttributes.DeleteAllItems() => IMFAttributesStore.DeleteAllItems();

        HRESULT IMFAttributes.SetUINT32(GUID* guidKey, uint unValue) => IMFAttributesStore.SetUINT32(guidKey, unValue);

        HRESULT IMFAttributes.SetUINT64(GUID* guidKey, ulong unValue) => IMFAttributesStore.SetUINT64(guidKey, unValue);

        HRESULT IMFAttributes.SetDouble(GUID* guidKey, double fValue) => IMFAttributesStore.SetDouble(guidKey, fValue);

        HRESULT IMFAttributes.SetGUID(GUID* guidKey, GUID* guidValue) => IMFAttributesStore.SetGUID(guidKey, guidValue);

        HRESULT IMFAttributes.SetString(GUID* guidKey, char* wszValue) => IMFAttributesStore.SetString(guidKey, wszValue);

        HRESULT IMFAttributes.SetBlob(GUID* guidKey, byte* pBuf, uint cbBufSize) => IMFAttributesStore.SetBlob(guidKey, pBuf, cbBufSize);

        HRESULT IMFAttributes.SetUnknown(GUID* guidKey, void* pUnknown) => IMFAttributesStore.SetUnknown(guidKey, pUnknown);

        HRESULT IMFAttributes.LockStore() => IMFAttributesStore.LockStore();

        HRESULT IMFAttributes.UnlockStore() => IMFAttributesStore.UnlockStore();

        HRESULT IMFAttributes.GetCount(uint* pcItems) => IMFAttributesStore.GetCount(pcItems);

        HRESULT IMFAttributes.GetItemByIndex(uint unIndex, GUID* pguidKey, PROPVARIANT* pValue) => IMFAttributesStore.GetItemByIndex(unIndex, pguidKey, pValue);

        HRESULT IMFAttributes.CopyAllItems(void* pDest) => IMFAttributesStore.CopyAllItems(pDest);

        #endregion

        /// <summary>
        /// Returns the associated <see cref="IMFAttributes"/> instance with this <see cref="MusicPlayerStreamV2"/> object. <br />
        /// The instance is lazily created, that means that it is created and saved once at the time that you invoke the getter of this property.
        /// </summary>
        protected IMFAttributes IMFAttributesStore
        {
            get {
                if (attributes is null) {
                    attributes = attributescreator();
                    attributescreator = null;
                }
                return attributes;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (attributes is not null)
            {
                // ArgumentException is thrown if the attributes object is a managed object (NOT COM).
                try { 
                    ComMarshalling.ReleaseInteropObject(attributes); 
                } catch (ArgumentException) {
                    // Since we know that the object is managed, test also for IDisposable if it does implement it, and call it.
                    // Also catch any exceptions if that throws.
                    try {
                        if (attributes is IDisposable d) { d.Dispose(); }
                    } catch (Exception e) { 
                        DebugProvider.WriteLine($"MUSICPLAYERSTREAMV2: Dispose implmentation of attributes reference threw an exception! Exception:\n{e}"); 
                    }
                }
                attributes = null;
            }
            attributescreator = null;
            base.Dispose(disposing);
        }
    }
}
