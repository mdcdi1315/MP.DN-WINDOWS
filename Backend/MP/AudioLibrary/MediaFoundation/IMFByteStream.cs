
using MP.ComInterop;
using MP.Annotations;
using MP.WindowsInterop;
using System.Runtime.InteropServices;

namespace MP.AudioLibrary.MediaFoundation
{
    /// <summary>
    /// Represents a byte stream from some data source, which might be a local file, a network file, or some other source.  <br />
    /// The <see cref="IMFByteStream"/> interface supports the typical stream operations, such as reading, writing, and seeking.
    /// </summary>
    [ComImport]
    [Guid(MediaFoundationInterfaceIds.IID_IMFByteStream)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe interface IMFByteStream
    {
        /// <summary>Retrieves the characteristics of the byte stream.</summary>
        /// <param name="pdwCapabilities">Receives a bitwise OR of zero or more flags. See the <see cref="MFBYTESTREAM_CAPABILITIES"/> enumeration over which flags are supported and when.</param>
        /// <returns>If this method succeeds, it returns <see cref="CommonHResults.S_OK"/>. Otherwise, it returns an <see cref="HRESULT"/> error code.</returns>
        [PreserveSig]
        public HRESULT GetCapabilities(MFBYTESTREAM_CAPABILITIES* pdwCapabilities);

        /// <summary>
        ///     GetLength gets the length of the stream
        /// </summary>
        /// <param name="pqwLength">
        ///     Pointer to a variable that will receive the length of the stream.
        /// </param>
        /// <returns>
        ///     <para>
        ///         MF_E_BYTESTREAM_UNKNOWN_LENGTH:
        ///             The stream length is unknown (can happen in HTTP scenarios)
        ///     </para>
        /// </returns>
        [PreserveSig]
        public HRESULT GetLength(System.UInt64* pqwLength);

        /// <summary>Sets the length of the stream.</summary>
        /// <param name="qwLength">Length of the stream in bytes.</param>
        /// <returns>If this method succeeds, it returns <see cref="CommonHResults.S_OK"/>. Otherwise, it returns an <see cref="HRESULT"/> error code.</returns>
        [PreserveSig]
        public HRESULT SetLength(System.UInt64 qwLength);

        /// <summary>Retrieves the current read or write position in the stream.</summary>
        /// <param name="pqwPosition">Receives the current position, in bytes.</param>
        /// <returns>If this method succeeds, it returns <see cref="CommonHResults.S_OK"/>. Otherwise, it returns an <see cref="HRESULT"/> error code.</returns>
        [PreserveSig]
        public HRESULT GetCurrentPosition(System.UInt64* pqwPosition);

        /// <summary>Sets the current read or write position.</summary>
        /// <param name="qwPosition">New position in the stream, as a byte offset from the start of the stream.</param>
        /// <returns>
        /// The method returns an <see cref="HRESULT"/>. <br /> 
        /// Possible values include, but are not limited to, those in the following table. <br /> <br />
        /// <list type="table">
        ///     <listheader>
        ///         <term>Return code</term>
        ///         <description>Description</description>
        ///     </listheader>
        ///     <item>
        ///         <term><see cref="CommonHResults.S_OK"/></term>
        ///         <description>The method succeeded.</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="CommonHResults.E_INVALIDARG"/></term>
        ///         <description>Invalid argument.</description>
        ///     </item>
        /// </list>
        /// </returns>
        /// <remarks>
        /// If the new position is larger than the length of the stream, the method returns <see cref="CommonHResults.E_INVALIDARG"/>. <br /> <br />
        /// <strong>Implementation notes</strong>: 
        /// This method should update the current position in the stream by setting the current position to the value passed in to the <paramref name="qwPosition"/> parameter. 
        /// Other methods that can update the current position are <see cref="Read"/>, <see cref="BeginRead"/>, <see cref="Write"/>, <see cref="BeginWrite"/>, and <see cref="Seek"/>.
        /// </remarks>
        [PreserveSig]
        public HRESULT SetCurrentPosition(System.UInt64 qwPosition);

        /// <summary>Queries whether the current position has reached the end of the stream.</summary>
        /// <param name="pfEndOfStream">Receives the value <see cref="BOOL.TRUE"/> if the end of the stream has been reached, or <see cref="BOOL.FALSE"/> otherwise.</param>
        /// <returns>If this method succeeds, it returns <see cref="CommonHResults.S_OK"/>. Otherwise, it returns an <see cref="HRESULT"/> error code.</returns>
        [PreserveSig]
        public HRESULT IsEndOfStream(BOOL* pfEndOfStream);

        /// <summary>Reads data from the stream.</summary>
        /// <param name="pb">Pointer to a buffer that receives the data. The caller must allocate the buffer.</param>
        /// <param name="cb">Size of the buffer in bytes.</param>
        /// <param name="pcbRead">Receives the number of bytes that are copied into the buffer. This parameter cannot be <see langword="null"/>.</param>
        /// <returns>If this method succeeds, it returns <see cref="CommonHResults.S_OK"/>. Otherwise, it returns an <see cref="HRESULT"/> error code.</returns>
        /// <remarks>
        /// This method reads at most cb bytes from the current position in the stream and copies them into the buffer provided by the caller.
        /// The number of bytes that were read is returned in the <paramref name="pcbRead"/> parameter. 
        /// The method does not return an error code on reaching the end of the file, so the application should check the value in <paramref name="pcbRead"/> after the method returns. <br /> <br />
        /// 
        /// This method is synchronous. It blocks until the read operation completes. <br /> <br />
        /// 
        /// <strong>Implementation notes</strong>: 
        /// This method should update the current position in the stream by adding the number of bytes that were read, which is specified by the value returned in the <paramref name="pcbRead"/> parameter, to the current position.
        /// Other methods that can update the current position are <see cref="Read"/>, <see cref="Write"/>, <see cref="BeginWrite"/>, <see cref="Seek"/>, and <see cref="SetCurrentPosition"/>.
        /// </remarks>
        [PreserveSig]
        public HRESULT Read(System.Byte* pb, System.UInt32 cb, System.UInt32* pcbRead);

        /// <summary>Begins an asynchronous read operation from the stream.</summary>
        /// <param name="pb">Pointer to a buffer that receives the data. The caller must allocate the buffer.</param>
        /// <param name="cb"></param>
        /// <param name="pCallback">Pointer to the <see cref="IMFAsyncCallback"/> interface of a callback object. The caller must implement this interface.</param>
        /// <param name="punkState">
        /// Pointer to the IUnknown interface of a state object, defined by the caller.
        /// This parameter can be <see langword="null"/>. 
        /// You can use this object to hold state information. 
        /// The object is returned to the caller when the callback is invoked.
        /// </param>
        /// <returns>If this method succeeds, it returns <see cref="CommonHResults.S_OK"/>. Otherwise, it returns an <see cref="HRESULT"/> error code.</returns>
        /// <remarks>
        /// When all of the data has been read into the buffer, the callback object's <see cref="IMFAsyncCallback.Invoke"/> method is called. 
        /// At that point, the application should call <see cref="EndRead"/> to complete the asynchronous request. <br /> <br />
        /// 
        /// Do not read from, write to, free, or reallocate the buffer while an asynchronous read is pending. <br /> <br />
        /// 
        /// <strong>Implementation notes</strong>: 
        /// This method should update the current position in the stream by adding the number of bytes that will be read, which is specified by the value returned in the pcbRead parameter, to the current position. 
        /// Other methods that can update the current position are <see cref="BeginRead"/>, <see cref="Write"/>, <see cref="BeginWrite"/>, <see cref="Seek"/>, and <see cref="SetCurrentPosition"/>.
        /// </remarks>
        [PreserveSig]
        public HRESULT BeginRead(
            System.Byte* pb,
            System.UInt32 cb,
            [IsPointerToCOMInterfaceType(typeof(IMFAsyncCallback))] void* pCallback,
            /* IUnknown* */ void* punkState);

        /// <summary>Completes an asynchronous read operation.</summary>
        /// <param name="pResult">Pointer to the <see cref="IMFAsyncResult"/> interface. Pass in the same pointer that your callback object received in the <see cref="IMFAsyncCallback.Invoke"/> method.</param>
        /// <param name="pcbRead">Receives the number of bytes that were read.</param>
        /// <returns>If this method succeeds, it returns <see cref="CommonHResults.S_OK"/>. Otherwise, it returns an <see cref="HRESULT"/> error code.</returns>
        /// <remarks>Call this method after the <see cref="BeginRead"/> method completes asynchronously.</remarks>
        [PreserveSig]
        public HRESULT EndRead(
            [IsPointerToCOMInterfaceType(typeof(IMFAsyncResult))] void* pResult,
            System.UInt32* pcbRead);

        /// <summary>Writes data to the stream.</summary>
        /// <param name="pb">Pointer to a buffer that contains the data to write</param>
        /// <param name="cb">Size of the buffer in bytes.</param>
        /// <param name="pcbWritten">Receives the number of bytes that are written.</param>
        /// <returns>If this method succeeds, it returns <see cref="CommonHResults.S_OK"/>. Otherwise, it returns an <see cref="HRESULT"/> error code.</returns>
        /// <remarks>
        /// This method writes the contents of the <paramref name="pb"/> buffer to the stream, starting at the current stream position. 
        /// The number of bytes that were written is returned in the <paramref name="pcbWritten"/> parameter. <br /> <br />
        /// 
        /// This method is synchronous. It blocks until the write operation completes. <br /> <br />
        /// 
        /// <strong>Implementation notes</strong>: 
        /// This method should update the current position in the stream by adding the number of bytes that were written to the stream, which is specified by the value returned in the <paramref name="pcbWritten"/>, to the current position offset. <br /> <br />
        /// 
        /// Other methods that can update the current position are <see cref="Read"/>, <see cref="BeginRead"/>, <see cref="BeginWrite"/>, <see cref="Seek"/>, and <see cref="SetCurrentPosition"/>.
        /// </remarks>
        [PreserveSig]
        public HRESULT Write(System.Byte* pb, System.UInt32 cb, System.UInt32* pcbWritten);

        /// <summary>Begins an asynchronous write operation to the stream.</summary>
        /// <param name="pb">Pointer to a buffer containing the data to write.</param>
        /// <param name="cb">Size of the buffer in bytes.</param>
        /// <param name="pCallback">Pointer to the <see cref="IMFAsyncCallback"/> interface of a callback object. The caller must implement this interface.</param>
        /// <param name="punkState">
        /// Pointer to the IUnknown interface of a state object, defined by the caller. 
        /// This parameter can be <see langword="null"/>. 
        /// You can use this object to hold state information. 
        /// The object is returned to the caller when the callback is invoked.
        /// </param>
        /// <returns>If this method succeeds, it returns <see cref="CommonHResults.S_OK"/>. Otherwise, it returns an <see cref="HRESULT"/> error code.</returns>
        /// <remarks>
        /// When all of the data has been written to the stream, the callback object's <see cref="IMFAsyncCallback.Invoke"/> method is called. 
        /// At that point, the application should call <see cref="EndWrite"/> to complete the asynchronous request. <br /> <br />
        /// 
        /// Do not reallocate, free, or write to the buffer while an asynchronous write is still pending. <br /> <br />
        /// 
        /// <strong>Implementation notes</strong>: 
        /// This method should update the current position in the stream by adding the number of bytes that will be written to the stream, which is specified by the value returned in the pcbWritten, to the current position. 
        /// Other methods that can update the current position are <see cref="Read"/>, <see cref="BeginRead"/>, <see cref="Write"/>, <see cref="Seek"/>, and <see cref="SetCurrentPosition"/>.
        /// </remarks>
        [PreserveSig]
        public HRESULT BeginWrite(
            System.Byte* pb,
            System.UInt32 cb,
            [IsPointerToCOMInterfaceType(typeof(IMFAsyncCallback))] void* pCallback,
            /* IUnknown* */ void* punkState);

        /// <summary>Completes an asynchronous write operation.</summary>
        /// <param name="pResult">Pointer to the <see cref="IMFAsyncResult"/> interface. Pass in the same pointer that your callback object received in the <see cref="IMFAsyncCallback.Invoke"/> method.</param>
        /// <param name="pcbWritten">Receives the number of bytes that were written.</param>
        /// <returns>If this method succeeds, it returns <see cref="CommonHResults.S_OK"/>. Otherwise, it returns an <see cref="HRESULT"/> error code.</returns>
        /// <remarks>Call this method when the <see cref="BeginWrite"/> method completes asynchronously.</remarks>
        [PreserveSig]
        public HRESULT EndWrite(
            [IsPointerToCOMInterfaceType(typeof(IMFAsyncResult))] void* pResult,
            System.UInt32* pcbWritten);

        /// <summary>Moves the current position in the stream by a specified offset.</summary>
        /// <param name="SeekOrigin">Specifies the origin of the seek as a member of the <see cref="MFBYTESTREAM_SEEK_ORIGIN"/> enumeration. The offset is calculated relative to this position.</param>
        /// <param name="llSeekOffset">Specifies the new position, as a byte offset from the seek origin.</param>
        /// <param name="dwSeekFlags">Specifies zero or more flags. For a list of defined flags, see the <see cref="MFBYTESTREAM_SEEK_FLAGS"/> enumeration.</param>
        /// <param name="pqwCurrentPosition">Receives the new position after the seek.</param>
        /// <returns>If this method succeeds, it returns <see cref="CommonHResults.S_OK"/>. Otherwise, it returns an <see cref="HRESULT"/> error code.</returns>
        /// <remarks>
        /// <strong>Implementation notes</strong>:
        /// This method should update the current position in the stream by adding the <paramref name="llSeekOffset"/> to the seek <paramref name="SeekOrigin"/> position. 
        /// This should be the same value passed back in the <paramref name="pqwCurrentPosition"/> parameter. 
        /// Other methods that can update the current position are <see cref="Read"/>, <see cref="BeginRead"/>, <see cref="Write"/>, <see cref="BeginWrite"/>, and <see cref="SetCurrentPosition"/>.
        /// </remarks>
        [PreserveSig]
        public HRESULT Seek(
            MFBYTESTREAM_SEEK_ORIGIN SeekOrigin,
            System.Int64 llSeekOffset,
            MFBYTESTREAM_SEEK_FLAGS dwSeekFlags,
            System.UInt64* pqwCurrentPosition);

        /// <summary>
        /// Clears any internal buffers used by the stream. 
        /// If you are writing to the stream, the buffered data is written to the underlying file or device.
        /// </summary>
        /// <remarks>If the byte stream is read-only, this method has no effect.</remarks>
        /// <returns>If this method succeeds, it returns <see cref="CommonHResults.S_OK"/>. Otherwise, it returns an <see cref="HRESULT"/> error code.</returns>
        [PreserveSig]
        public HRESULT Flush();

        /// <summary>
        /// Closes the stream and releases any resources associated with the stream, such as sockets or file handles. 
        /// This method also cancels any pending asynchronous I/O requests.
        /// </summary>
        /// <returns>If this method succeeds, it returns <see cref="CommonHResults.S_OK"/>. Otherwise, it returns an <see cref="HRESULT"/> error code.</returns>
        [PreserveSig]
        public HRESULT Close();
    }
}