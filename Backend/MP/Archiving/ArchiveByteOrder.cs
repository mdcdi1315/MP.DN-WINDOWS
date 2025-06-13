

namespace MP.Archiving
{
    /// <summary>
    /// Defines the byte order under which all the byte-order dependent numeric data are defined as.
    /// </summary>
    internal enum ArchiveByteOrder : System.Byte
    {
        Invalid = 0,
        LittleEndian,
        BigEndian
    }
}