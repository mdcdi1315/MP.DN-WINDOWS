
using MP.ComInterop;
using MP.WindowsInterop;
using System.Runtime.InteropServices;

namespace MP.AudioLibrary.MediaFoundation
{
    internal readonly unsafe struct IMFMediaBufferNative : IMFMediaBuffer
    {
        private readonly NativelyHeldObject* obj;

        private readonly struct NativelyHeldObject
        {
            public readonly VirtualTable* Table; // Set indirectly through casting
        }

        public IMFMediaBufferNative(void* p_object) => obj = (NativelyHeldObject*)p_object;

        public IMFMediaBufferNative(System.IntPtr p_object) => obj = (NativelyHeldObject*)p_object;

        public readonly void* ToNative() => obj;

        [StructLayout(LayoutKind.Sequential)]
        public struct VirtualTable
        {
            public delegate* unmanaged[Stdcall]<void*, GUID*, void**, HRESULT> QueryInterface;

            public delegate* unmanaged[Stdcall]<void*, System.UInt32> AddRef;

            public delegate* unmanaged[Stdcall]<void*, System.UInt32> Release;

            public delegate* unmanaged[Stdcall]<void*, System.Byte**, System.UInt32*, System.UInt32*, HRESULT> Lock;

            public delegate* unmanaged[Stdcall]<void*, HRESULT> Unlock;

            public delegate* unmanaged[Stdcall]<void*, System.UInt32*, HRESULT> GetCurrentLength;

            public delegate* unmanaged[Stdcall]<void*, System.UInt32, HRESULT> SetCurrentLength;

            public delegate* unmanaged[Stdcall]<void*, System.UInt32*, HRESULT> GetMaxLength;
        }

        public readonly HRESULT Lock(byte** ppbBuffer, uint* pcbMaxLength, uint* pcbCurrentLength) => obj->Table->Lock(obj,  ppbBuffer, pcbMaxLength, pcbCurrentLength);

        public readonly HRESULT Unlock() => obj->Table->Unlock(obj);

        public readonly HRESULT GetCurrentLength(uint* pcbCurrentLength) => obj->Table->GetCurrentLength(obj, pcbCurrentLength);

        public readonly HRESULT SetCurrentLength(uint cbCurrentLength) => obj->Table->SetCurrentLength(obj, cbCurrentLength);

        public readonly HRESULT GetMaxLength(uint* pcbMaxLength) => obj->Table->GetMaxLength(obj, pcbMaxLength);

        public readonly System.UInt32 AddRef() => obj->Table->AddRef(obj);

        public readonly System.UInt32 Release() => obj->Table->Release(obj);
    }
}