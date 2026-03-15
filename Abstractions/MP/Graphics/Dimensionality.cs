


namespace MP.Graphics
{
    /// <summary>
    /// Defines the number of graphics dimensions to use for a shape.
    /// </summary>
    public enum Dimensionality : System.Byte
    {
        /// <summary>No dimensions.</summary>
        None = 0,
        /// <summary>One dimension (that is, a line of numbers).</summary>
        One,
        /// <summary>Two dimensions (that is, a 2D graph)</summary>
        Two,
        /// <summary>Three dimensions (that is, a 3D graph)</summary>
        Three,
        /// <summary>Four dimensions (that is, a 4D graph)</summary>
        Four
    }
}