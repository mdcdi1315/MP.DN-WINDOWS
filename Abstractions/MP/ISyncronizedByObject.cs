namespace MP
{
    /// <summary>
    /// Specialization for the <see cref="ISyncronized"/> interface for classes that use syncronization (namely lock objects) objects to perform thread safety.
    /// </summary>
    public interface ISyncronizedByObject : ISyncronized
    {
        /// <summary>
        /// Gets the object that is used to ensure thread safety.
        /// </summary>
        public object SyncObject { get; }
    }
}
