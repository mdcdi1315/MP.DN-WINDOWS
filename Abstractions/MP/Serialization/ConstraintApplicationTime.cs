

namespace MP.Serialization
{
    /// <summary>
    /// Specifies when the specified constraint can be enforced.
    /// </summary>
    public enum ConstraintApplicationTime : System.Byte
    {
        /// <summary>
        /// Constraint is enforced during read-time (deserialization) only.
        /// </summary>
        Reading,
        /// <summary>
        /// Constraint is enforced during write-time (serialization) only.
        /// </summary>
        Writing,
        /// <summary>
        /// Constrains is enforced both times (That is like specifying somehow <see cref="Reading"/> and <see cref="Writing"/> times together).
        /// </summary>
        Both
    }
}