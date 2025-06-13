
using System.Runtime.InteropServices;

namespace MP.Archiving
{
    [StructLayout(LayoutKind.Explicit , Pack = 1)]
    internal struct MPARCHATTRIB
    {
        [FieldOffset(0)]
        public AttributeValueType Type;

        [FieldOffset(2)]
        public System.Byte ID0;

        [FieldOffset(3)]
        public System.Byte ID1;

        [FieldOffset(4)]
        public System.Byte ID2;

        [FieldOffset(5)]
        public System.Byte ID3;

        [FieldOffset(6)]
        public MPARCHUINT AttributeLength;

        public unsafe System.String AttributeId
        {
            get {
                fixed (System.Byte* pref = &ID0)
                {
                    System.Byte* pid = pref;
                    System.Text.StringBuilder sb = new(4);
                    for (System.Int32 I = 0; I < 4; I++ , pid++) { sb.Append((*pid).ToChar()); }
                    return sb.ToString();
                }
            }
            set {
                if (value is null) { 
                    ID0 = 0;
                    ID1 = 0;
                    ID2 = 0;
                    ID3 = 0;
                    return;
                }
                if (value.Length != 4) { throw new System.ArgumentException("String must be exactly 4 ASCII characters."); }
                fixed (System.Byte* pref = &ID0)
                {
                    System.Byte* pid = pref;
                    for (System.Int32 I = 0; I < 4; I++ , pid++) 
                    {
                        *pid = value[I].ToByte();
                    }
                }
            }
        }
    }
}