

using System;

namespace MP.PlaylistManagement.TagSupport.MP_TCF
{
    [Flags]
    internal enum ENTRY_FLAGS : ushort
    {
        None = 0,
        LastEntry = 1 << 0
    }
}