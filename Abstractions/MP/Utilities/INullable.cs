

namespace MP.Utilities
{
    /// <summary>
    /// Defined by a structure that can represent an empty value , although that structures cannot be null.
    /// </summary>
    public interface INullable
    {
        /// <summary>
        /// Gets a value whether the current instance data are effectively empty.
        /// </summary>
        public System.Boolean IsNull { get; }
    }
}