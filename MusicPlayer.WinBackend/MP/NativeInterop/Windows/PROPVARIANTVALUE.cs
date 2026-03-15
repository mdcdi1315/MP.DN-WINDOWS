
using System;
using System.Runtime.InteropServices;

namespace MP.NativeInterop.Windows
{
    [StructLayout(LayoutKind.Explicit)]
    public struct PROPVARIANTVALUE
    {
        /// <summary>
        /// cVal.
        /// </summary>
        [FieldOffset(0)]
        public sbyte cVal;
        /// <summary>
        /// bVal.
        /// </summary>
        [FieldOffset(0)]
        public byte bVal;
        /// <summary>
        /// iVal.
        /// </summary>
        [FieldOffset(0)]
        public short iVal;
        /// <summary>
        /// uiVal.
        /// </summary>
        [FieldOffset(0)]
        public ushort uiVal;
        /// <summary>
        /// lVal.
        /// </summary>
        [FieldOffset(0)]
        public int lVal;
        /// <summary>
        /// ulVal.
        /// </summary>
        [FieldOffset(0)]
        public uint ulVal;
        /// <summary>
        /// intVal.
        /// </summary>
        [FieldOffset(0)]
        public int intVal;
        /// <summary>
        /// uintVal.
        /// </summary>
        [FieldOffset(0)]
        public uint uintVal;
        /// <summary>
        /// hVal.
        /// </summary>
        [FieldOffset(0)]
        public long hVal;
        /// <summary>
        /// uhVal.
        /// </summary>
        [FieldOffset(0)]
        public ulong uhVal;
        /// <summary>
        /// fltVal.
        /// </summary>
        [FieldOffset(0)]
        public float fltVal;
        /// <summary>
        /// dblVal.
        /// </summary>
        [FieldOffset(0)]
        public double dblVal;
        //VARIANT_BOOL boolVal;
        /// <summary>
        /// boolVal.
        /// </summary>
        [FieldOffset(0)]
        public short boolVal;
        /// <summary>
        /// scode.
        /// </summary>
        [FieldOffset(0)]
        public int scode;
        //CY cyVal;
        /// <summary>
        /// Date time.
        /// </summary>
        [FieldOffset(0)]
        internal FILETIME filetime;
        //CLSID* puuid;
        //CLIPDATA* pclipdata;
        //BSTR bstrVal;
        //BSTRBLOB bstrblobVal;
        /// <summary>
        /// Binary large object.
        /// </summary>
        [FieldOffset(0)]
        public BLOB blobVal;
        //LPSTR pszVal;
        /// <summary>
        /// Pointer value.
        /// </summary>
        [FieldOffset(0)]
        public IntPtr pointerValue; //LPWSTR 
        /// <summary>
        /// Currency value.
        /// </summary>
        [FieldOffset(0)]
        public CURRENCY cy;
    }
}