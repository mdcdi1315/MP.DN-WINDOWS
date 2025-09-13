
using System;
using MP.ComInterop;
using MP.Annotations;
using System.Runtime.InteropServices;

namespace MP.AudioLibrary.MediaFoundation
{
    public static unsafe class IMFAsyncCallbackExtensions
    {
        public static HRESULT Invoke(this IMFAsyncCallback callback , IMFAsyncResult result)
        {
            ArgumentNullException.ThrowIfNull(result);
            return callback.Invoke(Marshal.GetIUnknownForObject(result).ToPointer());
        }

        public static HRESULT InvokeInvokeFromNativeCode([IsPointerToCOMInterfaceType(typeof(IMFAsyncCallback))] void* pobject , IMFAsyncResult result)
        {
            ArgumentNullException.ThrowIfNull(pobject);
            ArgumentNullException.ThrowIfNull(result);
            delegate* unmanaged<void*, HRESULT> ptoinvoke = (delegate* unmanaged<void*, HRESULT>)ComMarshalling.GetComFunctionPointer(pobject, 4);
            return ptoinvoke(Marshal.GetIUnknownForObject(result).ToPointer());
        }

        public static HRESULT InvokeInvokeFromNativeCode(
            [IsPointerToCOMInterfaceType(typeof(IMFAsyncCallback))] void* pobject, 
            [IsPointerToCOMInterfaceType(typeof(IMFAsyncResult))] void* presult)
        {
            ArgumentNullException.ThrowIfNull(pobject);
            ArgumentNullException.ThrowIfNull(presult);
            delegate* unmanaged<void*, HRESULT> ptoinvoke = (delegate* unmanaged<void*, HRESULT>)ComMarshalling.GetComFunctionPointer(pobject, 4);
            return ptoinvoke(presult);
        }
    }
}