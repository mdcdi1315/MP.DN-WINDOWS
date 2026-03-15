

namespace MP.TagReading.Flac
{
    internal sealed class SharedConstants
    {
        // The unfortunate case here is that the metadata block header is encoded to 4 bytes , 
        // and all members are encoded to a bit level.
        public const int MetadataHeaderBytes = 4;
        public const int TypeMemberBits = 7;
        public const int IsLastMemberBits = 1;
        public const int LengthMemberBits = 24;
    }
}