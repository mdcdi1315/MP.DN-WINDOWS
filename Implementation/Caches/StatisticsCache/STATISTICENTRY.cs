

using System.Runtime.InteropServices;

namespace MP.Caches.StatisticsCache
{
    [StructLayout(LayoutKind.Explicit , Size = 12)]
    internal struct STATISTICENTRY
    {
        [FieldOffset(0)]
        public StatisticType Type;

        [FieldOffset(1)]
        public System.Byte RSVD0;

        [FieldOffset(2)]
        public System.UInt32 NameLength;

        [FieldOffset(6)]
        public System.UInt32 Size;

        [FieldOffset(10)]
        public System.UInt16 RSVD1;
    }
}