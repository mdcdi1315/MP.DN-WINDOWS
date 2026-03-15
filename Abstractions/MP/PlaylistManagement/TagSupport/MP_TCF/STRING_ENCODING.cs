

namespace MP.PlaylistManagement.TagSupport.MP_TCF
{
    internal enum STRING_ENCODING : ushort
    {
        UTF16_LE = 0,
        UTF16_BE = 1 << 0,
        ASCII = 1 << 1,
        UTF32_LE = 1 << 2,
        UTF32_BE = 1 << 3
    }
}