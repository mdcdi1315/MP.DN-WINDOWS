

namespace MP.IO
{
    /// <summary>Seeking displacement constants.</summary>
    public enum SeekDisplacement
    {
        /// <summary>Specifies the beginning of the stream.</summary>
        Begin = System.IO.SeekOrigin.Begin,
        /// <summary>Specifies the current position within the stream.</summary>
        Current = System.IO.SeekOrigin.Current,
        /// <summary>Specifies the end of the stream.</summary>
        End = System.IO.SeekOrigin.End
    }
}