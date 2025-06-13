
using System;
using MP.WindowsInterop;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace MP.ComInterop
{
    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct STATSTG
    {
        [FieldOffset(0)]
        private System.Char* PWCSNAME;

        [FieldOffset(8)]
        public STGTY Type;

        [FieldOffset(16)]
        public System.UInt64 StreamSize;

        [FieldOffset(24)]
        private Interop.FILETIME ModTime;

        [FieldOffset(32)]
        private Interop.FILETIME CrtTime;

        [FieldOffset(40)]
        private Interop.FILETIME LaTime;

        [FieldOffset(48)]
        public System.UInt32 StreamMode;

        [FieldOffset(52)]
        public LOCKTYPE LockTypesSupported;

        [FieldOffset(56)]
        public GUID CLSID;

        [FieldOffset(72)]
        public System.UInt32 StateBitsNotSupported;

        [FieldOffset(76)]
        public System.UInt32 RSVD;

        public static STATSTG CreateStruct(System.String name = null)
        {
            if (name is null) { name = System.String.Empty; }
            System.UInt32 size = ((name.Length + 1) * sizeof(System.Char)).ToUInt32();
            void* pmem = Interop.Ole32.CoTaskMemAlloc(size);
            if (pmem is null) {
                throw new OutOfMemoryException("Allocation failed for the name member of the STATSTG structure!!!");
            }
            fixed (System.Char* pn = name) { Unsafe.CopyBlockUnaligned(pmem, pn, size); }
            STATSTG ret = new();
            ret.PWCSNAME = (System.Char*)pmem;
            ret.CLSID = GUID.Empty;
            ret.LaTime = Interop.FILETIME.Now;
            return ret;
        }

        public static STATSTG CreateStructWithoutName()
        {
            STATSTG ret = new();
            ret.PWCSNAME = null;
            ret.CLSID = GUID.Empty;
            ret.LaTime = Interop.FILETIME.Now;
            return ret;
        }

        public System.String Name => new(PWCSNAME);

        public DateTime ModificationTime
        {
            readonly get => ModTime.ToDateTimeUtc();
            set => ModTime = Interop.FILETIME.FromDateTime(value);
        }

        public DateTime CreationTime
        {
            readonly get => CrtTime.ToDateTimeUtc();
            set => CrtTime = Interop.FILETIME.FromDateTime(value);
        }

        public DateTime LastAccessTime
        {
            readonly get => LaTime.ToDateTimeUtc(); 
            set => LaTime = Interop.FILETIME.FromDateTime(value);
        }
    }
}

