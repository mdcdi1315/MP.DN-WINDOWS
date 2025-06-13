namespace MP.Threading
{
    /// <summary>
    /// Defines an abstraction of the Music Player Cancellation tasks tokens.
    /// </summary>
    /// <typeparam name="T">The type to use that will be the unique id of the instance.</typeparam>
    public interface ICancellationToken<T>
    {
        /// <summary>
        /// Returns a value whether this token is still valid , based on the given unique id.
        /// </summary>
        /// <param name="unique">The unique id that the task has kept to ensure that the code running in a thread or scheduler still runs.</param>
        /// <returns>A value whether this instance is still valid.</returns>
        public System.Boolean IsValid(T unique);

        /// <summary>
        /// Invalidates the current token if possible.
        /// </summary>
        public void Invalidate();

        /// <summary>
        /// The unique id that this token actually holds.
        /// </summary>
        public T InstanceId { get; }
    }

}