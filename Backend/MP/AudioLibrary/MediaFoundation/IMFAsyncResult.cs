
using MP.ComInterop;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace MP.AudioLibrary.MediaFoundation
{
    /// <summary>
    ///     This interface is used to represent the result from an asynchronous operation. <br />
    ///     For most Media Foundation components and applications that need to 
    ///     create an <see cref="IMFAsyncResult"/> implementation, 
    ///     <see cref="MediaFoundationInterfacesFactory.CreateAsyncResult(object, IMFAsyncCallback, object)"/>
    ///     , which instantiates the MF implementation of this interface, will suffice. <br />
    ///     Any implementation of <see cref="IMFAsyncResult"/> must inherit from the
    ///     MFASYNCRESULT structure defined in mfapi.h
    /// </summary>
    [ComImport]
    [Guid(MediaFoundationInterfaceIds.IID_IMFAsyncResult)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe interface IMFAsyncResult
    {
        /// <summary>
        ///     Retrieves an IUnknown pointer to the state object associated with
        ///     the asynchronous operation, if any.
        ///     If there is no associated state, then *ppunkState is set to NULL.
        /// </summary>
        [PreserveSig]
        HRESULT GetState(/* IUnknown */ void** ppunkState);

        /// <summary>
        ///     Returns an HRESULT indicating the success or failure of the 
        ///     asynchronous operation
        /// </summary>
        [PreserveSig]
        HRESULT GetStatus();

        /// <summary>
        ///     Sets the HRESULT status code to indicate the success or failure
        ///     of the asynchronous operation.  
        /// </summary>
        [PreserveSig]
        HRESULT SetStatus(HRESULT hrStatus);

        /// <summary>
        ///     Retrieves an IUnknown pointer to the object associated with the
        ///     asynchronous operation, if any.
        ///     If there is no associated object, then *ppunkObject is set to NULL.
        /// </summary>
        [PreserveSig]
        HRESULT GetObject(/* IUnknown */ void** ppObject);

        /// <summary>
        ///     Returns an IUnknown pointer to the state object associated with
        ///     the asynchronous operation, if any, without incrementing its
        ///     reference count
        /// </summary>
        [PreserveSig]
        [return: MaybeNull]
        /* IUnknown */ void* GetStateNoAddRef();
    }
}