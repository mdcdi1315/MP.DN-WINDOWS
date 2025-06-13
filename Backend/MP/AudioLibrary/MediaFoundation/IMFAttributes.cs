
using MP.ComInterop;
using MP.Annotations;
using MP.WindowsInterop;
using System.Runtime.InteropServices;

namespace MP.AudioLibrary.MediaFoundation
{
    /// <summary>
    ///     The IMFAttributes interface is a general-purpose interface for storing
    ///     key/value pairs, where the key is a GUID and the value is one of a
    ///     small number of common data types, including UINT32, UINT64, double,
    ///     GUID, Unicode string, and BLOB (counted array of UINT8).  In addition
    ///     to the methods inherited from IUnknown, the IMFAttributes interface
    ///     exposes the following methods.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         The LockStore and UnlockStore calls may not be nested, and may not
    ///         be called from different threads.
    ///     </para>
    /// </remarks>
    [ComImport]
    [Guid(MediaFoundationInterfaceIds.IID_IMFAttributes)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe interface IMFAttributes
    {
        /// <summary>
        ///     GetItem returns a value from the attributes store that corresponds
        ///     to the given key (a GUID).
        /// </summary>
        /// <param name="guidKey">
        ///     Key corresponding to value to search for
        /// </param>
        /// <param name="pValue">
        ///     A PROPVARIANT structure provided by the caller. It is filled in with
        ///     a copy of the stored value, if the value is found. The PROPVARIANT
        ///     must be cleared with PropVariantClear when the caller is done with
        ///     the value. If pValue is NULL, the method will still return S_OK if
        ///     the key is found, but the value will not be returned.
        /// </param>
        /// <returns>
        ///     <para>
        ///         S_OK.
        ///             The key exists (and the value was copied into the
        ///             PROPVARIANT structure, if provided).
        ///     </para>
        ///     <para>
        ///         MF_E_ATTRIBUTENOTFOUND.
        ///             No value corresponding to this key is stored in this object.
        ///     </para>
        /// </returns>
        [PreserveSig]
        public HRESULT GetItem(GUID* guidKey, PROPVARIANT* pValue);

        /// <summary>
        ///     GetItemType returns the type of a value from the attributes store
        ///     that corresponds to the given key (a GUID).
        /// </summary>
        /// <param name="guidKey">
        ///     Key corresponding to value to search for
        /// </param>
        /// <param name="pType">
        ///     Pointer to an MF_ATTRIBUTE_TYPE value.  It is filled in with a copy
        ///     of the type of the stored value, if the value is found.
        /// </param>
        /// <returns>
        ///     <para>
        ///         S_OK.
        ///             The key exists, and the value of pType signifies the type of
        ///             the item value.
        ///     </para>
        ///     <para>
        ///         MF_E_ATTRIBUTENOTFOUND.
        ///             No value corresponding to this key is stored in this object.
        ///     </para>
        /// </returns>
        [PreserveSig]
        public HRESULT GetItemType(GUID* guidKey, MF_ATTRIBUTE_TYPE* pType);

        /// <summary>
        ///     CompareItem checks whether the given PROPVARIANT is equivalent to
        ///     one in store with the same key.
        /// </summary>
        /// <param name="guidKey">
        ///     Key corresponding to value to search for
        /// </param>
        /// <param name="Value">
        ///     Reference to a PROPVARIANT value to be compared.
        /// </param>
        /// <param name="pbResult">
        ///     BOOL value indicating whether an equivalent value was found with the
        ///     same key.
        /// </param>
        /// <returns>
        ///     <para>
        ///         S_OK.
        ///             The key exists, and comparison was made.  Check the value of
        ///             *pbResult to determine whether the values were equivalent.
        ///     </para>
        ///     <para>
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         pbResult, if not NULL, will be set to FALSE in any case where
        ///         CompareItem returns a code other than S_OK.  Additionally, of
        ///         course, it is set to FALSE in the case where the call is
        ///         successful, but the value is not found or not equivalent.
        ///     </para>
        /// </remarks>
        [PreserveSig]
        public HRESULT CompareItem(GUID* guidKey, PROPVARIANT* Value, BOOL* pbResult);

        /// <summary>
        ///     Compare checks all of "our" items against "theirs", or "theirs"
        ///     against "ours," or both, according to the match type passed in by
        ///     the caller.
        /// </summary>
        /// <param name="pTheirs">
        ///     pointer to an <see cref="IMFAttributes"/> interface to compare items in.
        /// </param>
        /// <param name="MatchType">
        ///     Enumeration value indicating how to compare sets.
        /// </param>
        /// <param name="pbResult">
        ///     <see cref="BOOL"/> value indicating whether sets exhibit the desired level of
        ///     equivalency.
        /// </param>
        /// <remarks>
        ///     <para>
        ///         pbResult, if not NULL, will be set to FALSE in any case where
        ///         Compare returns a code other than S_OK.  Additionally, of
        ///         course, it is set to FALSE in the case where the call is
        ///         successful, but the sets do not meet the equivalency condition.
        ///     </para>
        /// </remarks>
        /// <returns>
        ///     <para>
        ///         S_OK.
        ///             Sets were compared successfully.  Check the value of
        ///             *pbResult to determine whether the sets met the desired
        ///             equivalency condition.
        ///     </para>
        /// </returns>
        [PreserveSig]
        public HRESULT Compare([IsPointerToCOMInterfaceType(typeof(IMFAttributes))] void* pTheirs, MF_ATTRIBUTES_MATCH_TYPE MatchType, BOOL* pbResult);

        /// <summary>
        ///     GetUINT32 retrieves a value of type UINT32 corresponding to the
        ///     given key.
        /// </summary>
        /// <param name="guidKey">
        ///     GUID specifying the value to retrieve.
        /// </param>
        /// <param name="punValue">
        ///     Value of the property, in a UINT32.
        /// </param>
        /// <returns>
        ///     <para>
        ///         S_OK.
        ///             Value was found and retrieved successfully.
        ///     </para>
        ///     <para>
        ///         MF_E_INVALIDTYPE
        ///             Value was found, but is not of type UINT32.
        ///     </para>
        ///     <para>
        ///         MF_E_ATTRIBUTENOTFOUND.
        ///             No value corresponding to this key is stored in this object.
        ///     </para>
        /// </returns>
        [PreserveSig]
        public HRESULT GetUINT32(GUID* guidKey, System.UInt32* punValue);

        /// <summary>
        ///     GetUINT64 retrieves a value of type UINT64 corresponding to the
        ///     given key.
        /// </summary>
        /// <param name="guidKey">
        ///     GUID specifying the value to retrieve.
        /// </param>
        /// <param name="punValue">
        ///     Value of the property, in a UINT64.
        /// </param>
        /// <returns>
        ///     <para>
        ///         S_OK.
        ///             Value was found and retrieved successfully.
        ///     </para>
        ///     <para>
        ///         MF_E_INVALIDTYPE
        ///             Value was found, but is not of type UINT64.
        ///     </para>
        ///     <para>
        ///         MF_E_ATTRIBUTENOTFOUND.
        ///             No value corresponding to this key is stored in this object.
        ///     </para>
        /// </returns>
        [PreserveSig]
        public HRESULT GetUINT64(GUID* guidKey, System.UInt64* punValue);

        /// <summary>
        ///     GetDouble retrieves a value of type double corresponding to the
        ///     given key.
        /// </summary>
        /// <param name="guidKey">
        ///     GUID specifying the value to retrieve.
        /// </param>
        /// <param name="pfValue">
        ///     Value of the property, in a double.
        /// </param>
        /// <returns>
        ///     <para>
        ///         S_OK.
        ///             Value was found and retrieved successfully.
        ///     </para>
        ///     <para>
        ///         MF_E_INVALIDTYPE
        ///             Value was found, but is not of type double.
        ///     </para>
        ///     <para>
        ///         MF_E_ATTRIBUTENOTFOUND.
        ///             No value corresponding to this key is stored in this object.
        ///     </para>
        /// </returns>
        [PreserveSig] 
        public HRESULT GetDouble(GUID* guidKey, System.Double* pfValue);

        /// <summary>
        ///     GetGUID retrieve a value of type GUID corresponding to the given
        ///     key.
        /// </summary>
        /// <param name="guidKey">
        ///     GUID specifying the value to retrieve.
        /// </param>
        /// <param name="pguidValue">
        ///     Value of the property, in a GUID.
        /// </param>
        /// <returns>
        ///     <para>
        ///         S_OK.
        ///             Value was found and retrieved successfully.
        ///     </para>
        ///     <para>
        ///         MF_E_INVALIDTYPE
        ///             Value was found, but is not of type GUID.
        ///     </para>
        ///     <para>
        ///         MF_E_ATTRIBUTENOTFOUND.
        ///             No value corresponding to this key is stored in this object.
        ///     </para>
        /// </returns>
        [PreserveSig] 
        public HRESULT GetGUID(GUID* guidKey, GUID* pguidValue);

        /// <summary>
        ///     GetStringLength retrieve the length of a string value corresponding
        ///     to the given key.
        /// </summary>
        /// <param name="guidKey">
        ///     GUID specifying the value to check.
        /// </param>
        /// <param name="pcchLength">
        ///     Upon success, holds the length of the string, in characters, not
        ///     including the NULL terminator.
        /// </param>
        /// <returns>
        ///     <para>
        ///         S_OK.
        ///             Value was found and retrieved successfully.
        ///     </para>
        ///     <para>
        ///         E_OUTOFMEMORY
        ///             Value was found, but string length is too large to fit in a
        ///             UINT32 value.
        ///     </para>
        ///     <para>
        ///         MF_E_INVALIDTYPE
        ///             Value was found, but is not of type LPWSTR.
        ///     </para>
        ///     <para>
        ///         MF_E_ATTRIBUTENOTFOUND.
        ///             No value corresponding to this key is stored in this object.
        ///     </para>
        /// </returns>
        [PreserveSig]
        public HRESULT GetStringLength(GUID* guidKey, System.UInt32* pcchLength);

        /// <summary>
        ///     GetString retrieves a string value corresponding to the given key.
        /// </summary>
        /// <param name="guidKey">
        ///     GUID specifying the value to retrieve.
        /// </param>
        /// <param name="pwszValue">
        ///     Upon success, holds the retrieved LPWSTR value.
        /// </param>
        /// <param name="cchBufSize">
        ///     Specifies the size, in characters, of the buffer passed in the
        ///     pwszValue parameter.  Note that this is the size of the entire
        ///     buffer, not string length, so no further space for NULL
        ///     termination will be assumed.
        /// </param>
        /// <param name="pcchLength">
        ///     Upon success, contains the length, in characters, of the string in the
        ///     pwszValue parameter, excluding NULL termination.  pcchLength may be
        ///     NULL if the length of the return string is not needed.
        /// </param>
        /// <returns>
        ///     <para>
        ///         S_OK.
        ///             Value was found and retrieved successfully.
        ///     </para>
        ///     <para>
        ///         E_OUTOFMEMORY
        ///             Value was found, but string length is too large to fit in a
        ///             UINT32 value.
        ///     </para>
        ///     <para>
        ///         HRESULT_FROM_WIN32( ERROR_INSUFFICIENT_BUFFER )
        ///             cchBufSize was not sufficiently large to hold the string
        ///             value with NULL terminator.  In this case, *pcchLength will be
        ///             set to the length of the string value in chars.  The caller
        ///             will need to allocate a buffer of size at least
        ///             *ppchLength + 1 chars and call the method again.
        ///     </para>
        ///     <para>
        ///         MF_E_INVALIDTYPE
        ///             Value was found, but is not of type LPWSTR.
        ///     </para>
        ///     <para>
        ///         MF_E_ATTRIBUTENOTFOUND.
        ///             No value corresponding to this key is stored in this object.
        ///     </para>
        /// </returns>
        [PreserveSig]
        public HRESULT GetString(GUID* guidKey, System.Char* pwszValue, System.UInt32 cchBufSize, System.UInt32* pcchLength);

        /// <summary>
        ///     GetAllocatedString retrieves a string value corresponding to the
        ///     given key.
        /// </summary>
        /// <param name="guidKey">
        ///     GUID specifying the value to retrieve.
        /// </param>
        /// <param name="pwszValue">
        ///     Upon success, holds the retrieved LPWSTR value.
        /// </param>
        /// <param name="pcchLength">
        ///     Upon success, contains the size, in characters, of the string in the
        ///     pwszValue parameter, excluding NULL termination.
        /// </param>
        /// <returns>
        ///     <para>
        ///         S_OK.
        ///             Value was found and retrieved successfully.
        ///     </para>
        ///     <para>
        ///         E_OUTOFMEMORY
        ///             insufficient memory was available to allocate *pwszValue.
        ///     </para>
        ///     <para>
        ///         MF_E_INVALIDTYPE
        ///             Value was found, but is not of type LPWSTR.
        ///     </para>
        ///     <para>
        ///         MF_E_ATTRIBUTENOTFOUND.
        ///             No value corresponding to this key is stored in this object.
        ///     </para>
        /// </returns>
        /// <remarks>
        ///     Memory for the string returned from GetAllocatedString is allocated
        ///     by the callee using CoTaskMemAlloc.  It is the responsibility of the
        ///     caller to free the memory via CoTaskMemFree.
        /// </remarks>
        [PreserveSig]
        public HRESULT GetAllocatedString(GUID* guidKey, System.Char* ppwszValue, System.UInt32* pcchLength);

        /// <summary>
        ///     GetBlobSize retrieves the size of a blob value corresponding to the
        ///     given key.
        /// </summary>
        /// <param name="guidKey">
        ///     GUID specifying the value to check.
        /// </param>
        /// <param name="pcbBlobSize">
        ///     Upon success, holds the size of the blob, in bytes.
        /// </param>
        /// <returns>
        ///     <para>
        ///         S_OK.
        ///             Value was found and retrieved successfully.
        ///     </para>
        ///     <para>
        ///         MF_E_INVALIDTYPE
        ///             Value was found, but is not of type VT_BLOB.
        ///     </para>
        ///     <para>
        ///         MF_E_ATTRIBUTENOTFOUND.
        ///             No value corresponding to this key is stored in this object.
        ///     </para>
        /// </returns>
        [PreserveSig]
        public HRESULT GetBlobSize(GUID* guidKey, System.UInt32* pcbBlobSize);

        /// <summary>
        ///     GetBlob retrieves the blob of data corresponding to the given key.
        /// </summary>
        /// <param name="guidKey">
        ///     GUID specifying the value to retrieve.
        /// </param>
        /// <param name="pBuf">
        ///     A buffer supplied by the caller which will be filled with a copy of
        ///     the blob data from the attributes store.
        /// </param>
        /// <param name="cbBufSize">
        ///     Specifies the size, in bytes, of the data buffer.
        /// </param>
        /// <param name="pcbBlobSize">
        ///     Upon success, holds size, in bytes, of the data blob.
        /// </param>
        /// <returns>
        ///     <para>
        ///         S_OK.
        ///             Value was found and retrieved successfully.
        ///     </para>
        ///     <para>
        ///         HRESULT_FROM_WIN32( ERROR_INSUFFICIENT_BUFFER )
        ///             cbBufSize was not sufficiently large to hold the data blob.
        ///             *pcbBlobSize contains the size, in bytes, necessary to copy
        ///             the data blob.
        ///     </para>
        ///     <para>
        ///         MF_E_INVALIDTYPE
        ///             Value was found, but is not of type VT_BLOB.
        ///     </para>
        ///     <para>
        ///         MF_E_ATTRIBUTENOTFOUND.
        ///             No value corresponding to this key is stored in this object.
        ///     </para>
        /// </returns>
        [PreserveSig]
        public HRESULT GetBlob(GUID* guidKey, System.Byte* pBuf, System.UInt32 cbBufSize, System.UInt32* pcbBlobSize);

        /// <summary>
        ///     GetAllocatedBlob retrieves a blob of data corresponding to the
        ///     given key.
        /// </summary>
        /// <param name="guidKey">
        ///     GUID specifying the value to retrieve.
        /// </param>
        /// <param name="ppBuf">
        ///     Upon success, holds the retrieved data blob.
        /// </param>
        /// <param name="pcbSize">
        ///     Upon success, contains the size, in bytes, of the buffer in the
        ///     ppBuf parameter.
        /// </param>
        /// <returns>
        ///     <para>
        ///         S_OK.
        ///             Value was found and retrieved successfully.
        ///     </para>
        ///     <para>
        ///         E_OUTOFMEMORY
        ///             insufficient memory was available to allocate *ppBuf.
        ///     </para>
        ///     <para>
        ///         MF_E_INVALIDTYPE
        ///             Value was found, but is not of type VT_BLOB.
        ///     </para>
        ///     <para>
        ///         MF_E_ATTRIBUTENOTFOUND.
        ///             No value corresponding to this key is stored in this object.
        ///     </para>
        /// </returns>
        /// <remarks>
        ///     Memory for the data blob returned from GetAllocatedBlob is allocated
        ///     by the callee using CoTaskMemAlloc.  It is the responsibility of the
        ///     caller to free the memory via CoTaskMemFree.
        /// </remarks>
        [PreserveSig]
        public HRESULT GetAllocatedBlob(GUID* guidKey, System.Byte** ppBuf, System.UInt32* pcbSize);

        /// <summary>
        ///     GetUnknown retrieves an interface pointer to the requested
        ///      interface, in an LPVOID, from the value corresponding to the given
        ///     key.
        /// </summary>
        /// <param name="guidKey">
        ///     GUID specifying the value to retrieve.
        /// </param>
        /// <param name="riid"></param>
        /// <param name="ppv">
        ///     Value of the property, in an void**.  Returned value needs to be cast
        ///     to an interface pointer of the appropriate type.
        /// </param>
        /// <returns>
        ///     <para>
        ///         S_OK.
        ///             Value was found and retrieved successfully.
        ///     </para>
        ///     <para>
        ///         MF_E_INVALIDTYPE
        ///             Value was found, but is not of type IUnknown.
        ///     </para>
        ///     <para>
        ///         E_NOINTERFACE
        ///             Value was found, but does not support the interface
        ///             specified by the riid parameter.
        ///     </para>
        ///     <para>
        ///         MF_E_ATTRIBUTENOTFOUND.
        ///             No value corresponding to this key is stored in this object.
        ///     </para>
        /// </returns>
        /// <remarks>
        ///     It is the responsibility of the caller to call Release on the
        ///     returned interface pointer when done with it.
        /// </remarks>
        [PreserveSig]
        public HRESULT GetUnknown(GUID* guidKey, GUID* riid, void** ppv);

        /// <summary>
        ///     SetItem associates the given value with the specified key.
        /// </summary>
        /// <param name="guidKey">
        ///     GUID specifying the key.
        /// </param>
        /// <param name="Value">
        ///     Value of the property.
        /// </param>
        /// <returns>
        ///     <para>
        ///         S_OK.
        ///             Value was successfully set.
        ///     </para>
        ///     <para>
        ///         E_OUTOFMEMORY
        ///             insufficient memory was available to create a new item.
        ///     </para>
        /// </returns>
        /// <remarks>
        ///     Use with caution.  SetItem does not perform type-checking on the
        ///     PROPVARIANT Value, so it is easy to associate a key in one set
        ///     with a value of one type, and the same key in another set with a
        ///     value of a different type.  This will lead to unexpected results
        ///     from item and set comparisons.  SetItem will only allow values to be
        ///     set which are of a type corresponding to an MF_ATTRIBUTE_TYPE value,
        ///     i.e. VT_UI4 (UINT32), VT_UI8 (UINT64), VT_CLSID (GUID), VT_LPWSTR
        ///     (String), VT_VECTOR | VT_UI1 (Blob), and VT_UNKNOWN (IUnknown).
        /// </remarks>
        [PreserveSig]
        public HRESULT SetItem(GUID* guidKey, PROPVARIANT* Value);

        /// <summary>
        ///     DeleteItem removes the value associated with the specified key from
        ///     the attribute set.
        /// </summary>
        /// <param name="guidKey">
        ///     GUID specifying the key.
        /// </param>
        /// <returns>
        ///     <para>
        ///         S_OK.
        ///             Value was successfully removed.
        ///     </para>
        /// </returns>
        [PreserveSig]
        public HRESULT DeleteItem(GUID* guidKey);

        /// <summary>
        ///     DeleteAllItems removes all values from the attribute set.
        /// </summary>
        /// <returns>
        ///     <para>
        ///         S_OK.
        ///             All values were successfully removed from the set.
        ///     </para>
        /// </returns>
        [PreserveSig]
        public HRESULT DeleteAllItems();
        
        /// <summary>
        ///     SetUINT32 associates the given UINT32 value with the specified key.
        /// </summary>
        /// <param name="guidKey">
        ///     GUID specifying the key.
        /// </param>
        /// <param name="unValue">
        ///     Value of the property.
        /// </param>
        /// <returns>
        ///     <para>
        ///         S_OK.
        ///             Value was successfully set.
        ///     </para>
        ///     <para>
        ///         E_OUTOFMEMORY
        ///             Insufficient memory was available to create a new item in
        ///             the store.
        ///     </para>
        /// </returns>
        [PreserveSig] 
        public HRESULT SetUINT32(GUID* guidKey, System.UInt32 unValue);

        /// <summary>
        ///     SetUINT64 associates the given UINT64 value with the specified key.
        /// </summary>
        /// <param name="guidKey">
        ///     GUID specifying the key.
        /// </param>
        /// <param name="unValue">
        ///     Value of the property.
        /// </param>
        /// <returns>
        ///     <para>
        ///         S_OK.
        ///             Value was successfully set.
        ///     </para>
        ///     <para>
        ///         E_OUTOFMEMORY
        ///             Insufficient memory was available to create a new item in
        ///             the store.
        ///     </para>
        /// </returns>
        [PreserveSig] 
        public HRESULT SetUINT64(GUID* guidKey, System.UInt64 unValue);

        /// <summary>
        ///     SetDouble associates the given double value with the specified key.
        /// </summary>
        /// <param name="guidKey">
        ///     GUID specifying the key.
        /// </param>
        /// <param name="fValue">
        ///     Value of the property.
        /// </param>
        /// <returns>
        ///     <para>
        ///         S_OK.
        ///             Value was successfully set.
        ///     </para>
        ///     <para>
        ///         E_OUTOFMEMORY
        ///             Insufficient memory was available to create a new item in
        ///             the store.
        ///     </para>
        /// </returns>
        [PreserveSig]
        public HRESULT SetDouble(GUID* guidKey, double fValue);

        /// <summary>
        ///     SetGUID associates the given GUID value with the specified key.
        /// </summary>
        /// <param name="guidKey">
        ///     GUID specifying the key.
        /// </param>
        /// <param name="guidValue">
        ///     Value of the property.
        /// </param>
        /// <returns>
        ///     <para>
        ///         S_OK.
        ///             Value was successfully set.
        ///     </para>
        ///     <para>
        ///         E_OUTOFMEMORY
        ///             Insufficient memory was available to create a new item in
        ///             the store.
        ///     </para>
        /// </returns>
        [PreserveSig] 
        public HRESULT SetGUID(GUID* guidKey, GUID* guidValue);

        /// <summary>
        ///     SetString associates the given LPWSTR value with the specified key.
        /// </summary>
        /// <param name="guidKey">
        ///     GUID specifying the key.
        /// </param>
        /// <param name="wszValue">
        ///     Value of the property.  WszValue is presumed to be NULL-terminated.
        /// </param>
        /// <returns>
        ///     <para>
        ///         S_OK.
        ///             Value was successfully set.
        ///     </para>
        ///     <para>
        ///         E_OUTOFMEMORY
        ///             Insufficient memory was available to create a new item in
        ///             the store.
        ///     </para>
        /// </returns>
        /// <remarks>
        ///      Callee will allocate a new buffer and copy the given string, rather
        ///      than storing a reference to the string passed in.  Thus, caller is
        ///      responsible for freeing any memory which may have been allocated
        ///      for wszValue.
        /// </remarks>
        [PreserveSig]
        public HRESULT SetString(GUID* guidKey, System.Char* wszValue);

        /// <summary>
        ///     SetBlob associates the given data blob value with the specified key.
        /// </summary>
        /// <param name="guidKey">
        ///     GUID specifying the key.
        /// </param>
        /// <param name="pBuf">
        ///     Value of the property.
        /// </param>
        /// <param name="cbBufSize">
        ///     Size of the buffer specified by pBuf, in bytes.
        /// </param>
        /// <returns>
        ///     <para>
        ///         S_OK.
        ///             Value was successfully set.
        ///     </para>
        ///     <para>
        ///         E_OUTOFMEMORY
        ///             Insufficient memory was available to create a new item in
        ///             the store.
        ///     </para>
        /// </returns>
        /// <remarks>
        ///      Callee will allocate a new buffer and copy the given data blob, rather
        ///      than storing a reference to the string passed in.  Thus, caller is
        ///      responsible for freeing any memory which may have been allocated
        ///      for pBuf.
        /// </remarks>
        [PreserveSig]
        public HRESULT SetBlob(GUID* guidKey, System.Byte* pBuf, System.UInt32  cbBufSize);

        /// <summary>
        ///     SetUnknown associates the given IUnknown interface pointer value
        ///     with the specified key.
        /// </summary>
        /// <param name="guidKey">
        ///     GUID specifying the key.
        /// </param>
        /// <param name="pUnknown">
        ///     Value of the property.
        /// </param>
        /// <returns>
        ///     <para>
        ///         S_OK.
        ///             Value was successfully set.
        ///     </para>
        ///     <para>
        ///         E_OUTOFMEMORY
        ///             Insufficient memory was available to create a new item in
        ///             the store.
        ///     </para>
        /// </returns>
        [PreserveSig]
        public HRESULT SetUnknown(GUID* guidKey, void* pUnknown);

        /// <summary>
        ///     LockStore excludes other threads from accessing the set, until such
        ///     time as UnlockStore is called.
        /// </summary>
        /// <returns>
        ///     <para>
        ///         S_OK.
        ///             Store was successfully locked.
        ///     </para>
        /// </returns>
        [PreserveSig]
        public HRESULT LockStore();

        /// <summary>
        ///     UnlockStore allows other threads access to the set, if they were
        ///     previously denied access via LockStore.
        /// </summary>
        /// <returns>
        ///     <para>
        ///         S_OK.
        ///             Store was successfully unlocked.
        ///     </para>
        /// </returns>
        [PreserveSig] 
        public HRESULT UnlockStore();

        /// <summary>
        ///     GetCount retrieves the number of items currently in the set.
        /// </summary>
        /// <param name="pcItems">
        ///     Upon success, will contain the count of items in the set.
        /// </param>
        /// <returns>
        ///     <para>
        ///         S_OK.
        ///             The call was successful, and *pcItems contains the number of
        ///             values in the set.
        ///     </para>
        /// </returns>
        [PreserveSig] 
        public HRESULT GetCount(System.UInt32* pcItems);

        /// <summary>
        ///     GetItemByIndex retrieves the item at the specified index in the
        ///     store.
        /// </summary>
        /// <param name="unIndex">
        ///     Zero-based index of the item to retrieve.
        /// </param>
        /// <param name="pguidKey">
        ///     Upon success, contains the GUID key associated with this value.
        /// </param>
        /// <param name="pValue">
        ///     Upon success, contains the value at index unIndex of the set.
        /// </param>
        /// <returns>
        ///     <para>
        ///         S_OK.
        ///             The call was successful.
        ///     </para>
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         All values returned are copies of the actual value in the set.
        ///         Thus, the caller is responsible for freeing memory associated
        ///         with the value by calling PropVariantClear.
        ///     </para>
        ///     <para>
        ///         The order of items within the set is UNDEFINED, therefore the
        ///         caller should never expect to find the same value at the same
        ///         index except between matched LockStore and UnlockStore calls.
        ///     </para>
        /// </remarks>
        [PreserveSig]
        public HRESULT GetItemByIndex(
            System.UInt32 unIndex,
            GUID* pguidKey,
            PROPVARIANT* pValue      // can be NULL. If not NULL, when done must use PropVariantClear() to free
            );

        /// <summary>
        ///     CopyAllItems clones all items from the store to the specified store.
        /// </summary>
        /// <param name="pDest">
        ///     pointer to an IMFAttributes interface which will recieve all items
        ///     from this set.
        /// </param>
        /// <returns>
        ///     <para>
        ///         S_OK.
        ///             The call was successful.
        ///     </para>
        ///     <para>
        ///         E_FAIL.
        ///             Copying one or more items from the store to the destination
        ///             was unsuccessful.
        ///     </para>
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         Note that the pDest store must be unlocked in order to retrieve
        ///         or set items by key.
        ///     </para>
        ///     <para>
        ///         CopyAllItems is a destructive operation on the destination store.
        ///         In the event of a failure to copy all items, the state of the
        ///         pDest store is UNDEFINED.  Furthermore, any existing items in
        ///         the pDest store will be removed prior to copying items from the
        ///         source store.
        ///     </para>
        /// </remarks>
        [PreserveSig]
        public HRESULT CopyAllItems([IsPointerToCOMInterfaceType(typeof(IMFAttributes))] void* pDest);

    }
}