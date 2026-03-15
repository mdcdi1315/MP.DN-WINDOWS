namespace MP.Random
{
    /// <summary>
    /// Instances of the interface represent an algorithm that provides random numbers.
    /// </summary>
    public interface IRandomSource
    {
        /// <summary>
        /// Returns a new random number , ranging from 0 to <see cref="System.UInt64.MaxValue"/>.
        /// </summary>
        /// <returns>The newly forged random number.</returns>
        public System.UInt64 Next();

        /// <summary>
        /// Initializes the alrogithm with the specified seed.
        /// </summary>
        /// <param name="seed">The seed of the current instance. Can be any number inslusive to the <see cref="System.Int64"/> range</param>
        public void Init(System.Int64 seed);

        /// <summary>Gets the seed that the current algorithm is using.</summary>
        public System.Int64 Seed { get; }
    }
}
