
using MP.Annotations;
using System.Runtime.InteropServices;

namespace MP.AudioLibrary.MediaFoundation
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct MFT_OUTPUT_DATA_BUFFER
    {
        public System.UInt32 StreamID; // [in] which stream is this for
        [IsPointerToCOMInterfaceType(typeof(IMFSample))]
        public void* pSample;    // [in/out] can be NULL
        public MFT_OUTPUT_DATA_BUFFER_FLAGS Status;   // [out] MFT_OUTPUT_DATA_BUFFER_FLAGS (INCOMPLETE, etc.)
        [IsPointerToCOMInterfaceType(typeof(IMFCollection))]
        public void* pEvents; // [out] Can be NULL.  Zero or more events produced by the MFT
    }
}