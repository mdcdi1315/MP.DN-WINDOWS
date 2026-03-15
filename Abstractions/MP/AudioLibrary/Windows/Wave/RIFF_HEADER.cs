using MP.IO.DataStructuring;
using MP.IO.DataStructuring.Generation;

namespace MP.AudioLibrary.Windows.Wave
{
    [DataStructureGenerator]
    internal partial struct RIFF_HEADER : IDataStructure
    {
        [FixedString(4)]
        public System.String ID;

        [FieldEndianess(Endianess.Little)]
        public System.UInt32 FileSize;

        [FixedString(4)]
        public System.String Type;
    }
}