
using MP.Annotations;
using System.Runtime.Versioning;

namespace MP.NativeInterop.Windows.COM
{
    /// <summary>
    /// The IStream interface lets you read and write data to stream objects. 
    /// Stream objects contain the data in a structured storage object, where storages provide the structure. 
    /// Simple data can be written directly to a stream but, most frequently, streams are elements nested within a storage object. 
    /// They are similar to standard files. <br /> <br />
    /// 
    /// The IStream interface defines methods similar to the MS-DOS FAT file functions.
    /// For example, each stream object has its own access rights and a seek pointer.
    /// The main difference between a DOS file and a stream object is that in the latter 
    /// case, streams are opened using an IStream interface pointer rather than a file handle. <br /> <br />
    /// 
    /// The methods in this interface present your object's data as a contiguous sequence of bytes that you can read or write. 
    /// There are also methods for committing and reverting changes on streams that are open in transacted mode and methods 
    /// for restricting access to a range of bytes in the stream. <br /> <br />
    /// 
    /// Streams can remain open for long periods of time without consuming file-system resources.
    /// The IUnknown::Release method is similar to a close function on a file. 
    /// Once released, the stream object is no longer valid and cannot be used.
    /// </summary>
    [SupportedOSPlatform(WindowsVersions._WIN32_WINNT_WIN2K)]
    [COMInterfaceGenerator("0000000c-0000-0000-C000-000000000046" , typeof(ISequentialStream))]
    public unsafe partial interface IStream
    {
        /// <summary>
        /// The <see cref="Seek"/> method changes the seek pointer to a new location. <br />
        /// The new location is relative to either the beginning of the stream, the end of the stream, or the current seek pointer.
        /// </summary>
        /// <param name="dlibMove">The displacement to be added to the location indicated by the <paramref name="dwOrigin"/> parameter.
        /// If <paramref name="dwOrigin"/> is <see cref="STREAM_SEEK.STREAM_SEEK_SET"/>, this is interpreted as an unsigned value rather than a signed value.</param>
        /// <param name="dwOrigin">The origin for the displacement specified in <paramref name="dlibMove"/>. 
        /// The origin can be the beginning of the file (<see cref="STREAM_SEEK.STREAM_SEEK_SET"/>), the current seek pointer (<see cref="STREAM_SEEK.STREAM_SEEK_CUR"/>), or the end of the file (<see cref="STREAM_SEEK.STREAM_SEEK_END"/>).
        /// For more information about values, see the <see cref="STREAM_SEEK"/> enumeration.</param>
        /// <param name="plibNewPosition">A pointer to the location where this method writes the value of the new seek pointer from the beginning of the stream. <br />
        /// You can set this pointer to <see langword="null"/>.
        /// In this case, this method does not provide the new seek pointer.</param>
        /// <returns>
        /// This method can return one of these values. <br />
        /// <list type="table">
        ///     <listheader>
        ///         <term>Return code</term>
        ///         <description>Description</description>
        ///     </listheader>
        ///     <item>
        ///         <term><see cref="CommonHResults.S_OK"/></term>
        ///         <description>The seek pointer was successfully adjusted.</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="CommonHResults.E_PENDING"/></term>
        ///         <description>Asynchronous Storage only: Part or all of the stream data is currently unavailable.</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="CommonHResults.STG_E_INVALIDPOINTER"/></term>
        ///         <description>Indicates that plibNewPosition points to invalid memory, because <paramref name="plibNewPosition"/> is not read.</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="CommonHResults.STG_E_INVALIDFUNCTION"/></term>
        ///         <description>The <paramref name="dwOrigin"/> parameter contains an invalid value, or the <paramref name="dlibMove"/> parameter contains a bad offset value. For example, the result of the seek pointer is a negative offset value.</description>
        ///     </item>
        /// </list>
        /// </returns>
        public HRESULT Seek(System.Int64 dlibMove, STREAM_SEEK dwOrigin, System.UInt64* plibNewPosition);

        /// <summary>
        /// The <see cref="SetSize"/> method changes the size of the stream object.
        /// </summary>
        /// <param name="libNewSize">Specifies the new size, in bytes, of the stream.</param>
        /// <returns>
        /// This method can return one of these values. <br />
        /// <list type="table">
        ///     <listheader>
        ///         <term>Return code</term>
        ///         <description>Description</description>
        ///     </listheader>
        ///     <item>
        ///         <term><see cref="CommonHResults.S_OK"/></term>
        ///         <description>The size of the stream object was successfully changed.</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="CommonHResults.E_PENDING"/></term>
        ///         <description>Asynchronous Storage only: Part or all of the stream's data is currently unavailable.</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="CommonHResults.STG_E_MEDIUMFULL"/></term>
        ///         <description>The stream size is not changed because there is no space left on the storage device.</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="CommonHResults.STG_E_INVALIDFUNCTION"/></term>
        ///         <description>
        ///             The value of the <paramref name="libNewSize"/> parameter is not supported by the implementation. 
        ///             Not all streams support greater than 232 bytes.
        ///             If a stream does not support more than 232 bytes, the high <see cref="System.UInt32"/> data type of <paramref name="libNewSize"/> must be zero. 
        ///             If it is nonzero, the implementation may return <see cref="CommonHResults.STG_E_INVALIDFUNCTION"/>. 
        ///             In general, COM-based implementations of the IStream interface do not support streams larger than 232 bytes.
        ///         </description>
        ///     </item>
        /// </list>
        /// </returns>
        public HRESULT SetSize(System.UInt64 libNewSize);

        /// <summary>
        /// The <see cref="CopyTo"/> method copies a specified number of bytes from the current seek pointer in the stream to the current seek pointer in another stream.
        /// </summary>
        /// <param name="pstm">A pointer to the destination stream. The stream pointed to by <paramref name="pstm"/> can be a new stream or a clone of the source stream.</param>
        /// <param name="cb">The number of bytes to copy from the source stream.</param>
        /// <param name="pcbRead">
        ///     A pointer to the location where this method writes the actual number of bytes read from the source. 
        ///     You can set this pointer to <see langword="null"/>. 
        ///     In this case, this method does not provide the actual number of bytes read.
        /// </param>
        /// <param name="pcbWritten">
        ///     A pointer to the location where this method writes the actual number of bytes written to the destination. 
        ///     You can set this pointer to <see langword="null"/>.
        ///     In this case, this method does not provide the actual number of bytes written.
        /// </param>
        /// <returns>
        /// This method can return one of these values. <br />
        /// <list type="table">
        ///     <listheader>
        ///         <term>Return code</term>
        ///         <description>Description</description>
        ///     </listheader>
        ///     <item>
        ///         <term><see cref="CommonHResults.S_OK"/></term>
        ///         <description>The stream object was successfully copied.</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="CommonHResults.E_PENDING"/></term>
        ///         <description>Asynchronous Storage only: Part or all of the data to be copied is currently unavailable.</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="CommonHResults.STG_E_INVALIDPOINTER"/></term>
        ///         <description>The value of one of the pointer parameters is invalid.</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="CommonHResults.STG_E_MEDIUMFULL"/></term>
        ///         <description>The stream is not copied because there is no space left on the storage device.</description>
        ///     </item>
        /// </list>
        /// </returns>
        public HRESULT CopyTo(
            /* [annotation][unique][in] */
            [IsPointerToCOMInterfaceType(typeof(IStream))] void* pstm,
            /* [in] */ System.UInt64 cb,
            /* [annotation] */
            System.UInt64* pcbRead,
            /* [annotation] */
            System.UInt64* pcbWritten);

        /// <summary>
        /// The <see cref="Commit"/> method ensures that any changes made to a stream object open in transacted mode are reflected in the parent storage. <br />
        /// If the stream object is open in direct mode, <see cref="Commit"/> has no effect other than flushing all memory buffers to the next-level storage object. <br />
        /// The COM compound file implementation of streams does not support opening streams in transacted mode.
        /// </summary>
        /// <param name="grfCommitFlags">Controls how the changes for the stream object are committed. See the <see cref="STGC"/> enumeration for a definition of these values.</param>
        /// <returns>
        /// This method can return one of these values. <br />
        /// <list type="table">
        ///     <listheader>
        ///         <term>Return code</term>
        ///         <description>Description</description>
        ///     </listheader>
        ///     <item>
        ///         <term><see cref="CommonHResults.S_OK"/></term>
        ///         <description>Changes to the stream object were successfully committed to the parent level.</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="CommonHResults.E_PENDING"/></term>
        ///         <description>Asynchronous Storage only: Part or all of the data to be copied is currently unavailable.</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="CommonHResults.STG_E_MEDIUMFULL"/></term>
        ///         <description>The commit operation failed due to lack of space on the storage device.</description>
        ///     </item>
        /// </list>
        /// </returns>
        public HRESULT Commit(STGC grfCommitFlags);

        /// <summary>
        /// The Revert method discards all changes that have been made to a transacted stream since the last <see cref="Commit"/> call.
        /// On streams open in direct mode and streams using the COM compound file implementation of <see cref="Revert"/>, this method has no effect.
        /// </summary>
        /// <returns>
        /// This method can return one of these values. <br />
        /// <list type="table">
        ///     <listheader>
        ///         <term>Return code</term>
        ///         <description>Description</description>
        ///     </listheader>
        ///     <item>
        ///         <term><see cref="CommonHResults.S_OK"/></term>
        ///         <description>The stream was successfully reverted to its previous version.</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="CommonHResults.E_PENDING"/></term>
        ///         <description>Asynchronous Storage only: Part or all of the data to be copied is currently unavailable.</description>
        ///     </item>
        /// </list>
        /// </returns>
        /// <remarks>The <see cref="Revert"/> method discards changes made to a transacted stream since the last commit operation.</remarks>
        public HRESULT Revert();

        public HRESULT LockRegion(
            /* [in] */ System.UInt64 libOffset,
            /* [in] */ System.UInt64 cb,
            /* [in] */ LOCKTYPE dwLockType);

        public HRESULT UnlockRegion(
            /* [in] */ System.UInt64 libOffset,
            /* [in] */ System.UInt64 cb,
            /* [in] */ LOCKTYPE dwLockType);

        /// <summary>
        /// The <see cref="Stat"/> method retrieves the <see cref="STATSTG"/> structure for this stream.
        /// </summary>
        /// <param name="pstatstg">Pointer to a <see cref="STATSTG"/> structure where this method places information about this stream object.</param>
        /// <param name="grfStatFlag">Specifies that this method does not return some of the members in the <see cref="STATSTG"/> structure, thus saving a memory allocation operation. Values are taken from the <see cref="STATFLAG"/> enumeration.</param>
        /// <returns>
        /// This method can return one of these values.
        /// <list type="table">
        ///     <listheader>
        ///         <term>Return code</term>
        ///         <description>Description</description>
        ///     </listheader>
        ///     <item>
        ///         <term><see cref="CommonHResults.S_OK"/></term>
        ///         <description>The <see cref="STATSTG"/> structure was successfully returned at the specified location.</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="CommonHResults.E_PENDING"/></term>
        ///         <description>Asynchronous Storage only: Part or all of the data to be copied is currently unavailable.</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="CommonHResults.STG_E_ACCESSDENIED"/></term>
        ///         <description>The caller does not have enough permissions for accessing statistics for this storage object.</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="CommonHResults.STG_E_INSUFFICIENTMEMORY"/></term>
        ///         <description>The <see cref="STATSTG"/> structure was not returned due to a lack of memory.</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="CommonHResults.STG_E_INVALIDFLAG"/></term>
        ///         <description>The value for the <paramref name="grfStatFlag"/> parameter is not valid.</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="CommonHResults.STG_E_INVALIDPOINTER"/></term>
        ///         <description>The <paramref name="pstatstg"/> pointer is not valid.</description>
        ///     </item>
        /// </list>
        /// </returns>
        /// <remarks>
        /// <see cref="Stat"/> retrieves a pointer to the <see cref="STATSTG"/> structure that contains information about this open stream. <br />
        /// When this stream is within a structured storage and IStorage::EnumElements is called, it creates an enumerator object with the IEnumSTATSTG interface on it, which can be called to enumerate the storages and streams through the <see cref="STATSTG"/> structures associated with each of them.
        /// </remarks>
        public HRESULT Stat(STATSTG* pstatstg, STATFLAG grfStatFlag);

        /// <summary>
        /// The <see cref="Clone"/> method creates a new stream object with its own seek pointer that references the same bytes as the original stream.
        /// </summary>
        /// <param name="ppstm">When successful, pointer to the location of an <see cref="IStream"/> pointer to the new stream object. If an error occurs, this parameter is <see langword="null"/>.</param>
        /// <returns>
        /// This method can return one of these values.
        /// <list type="table">
        ///     <listheader>
        ///         <term>Return code</term>
        ///         <description>Description</description>
        ///     </listheader>
        ///     <item>
        ///         <term><see cref="CommonHResults.S_OK"/></term>
        ///         <description>The stream was successfully cloned.</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="CommonHResults.E_PENDING"/></term>
        ///         <description>Asynchronous Storage only: Part or all of the data to be copied is currently unavailable.</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="CommonHResults.STG_E_INSUFFICIENTMEMORY"/></term>
        ///         <description>The stream was not cloned due to a lack of memory.</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="CommonHResults.STG_E_INVALIDPOINTER"/></term>
        ///         <description>The <paramref name="ppstm"/> pointer is not valid.</description>
        ///     </item>
        /// </list>
        /// </returns>
        /// <remarks>
        /// The <see cref="Clone"/> method creates a new stream object for accessing the same bytes but using a separate seek pointer.  <br />
        /// The new stream object sees the same data as the source-stream object. <br />
        /// Changes written to one object are immediately visible in the other. Range locking is shared between the stream objects. <br /> <br />
        /// 
        /// The initial setting of the seek pointer in the cloned stream instance is the same as the current setting of the seek pointer in the original stream at the time of the clone operation.
        /// </remarks>
        public HRESULT Clone([IsPointerToCOMInterfaceType(typeof(IStream))] void** ppstm);
    }
}
