

namespace MP.ExceptionSystem
{
    /// <summary>
    /// Thrown when an specified <see cref="EnvironmentVariable"/> does not exist.
    /// </summary>
    public sealed class EnvironmentVariableNotFoundException : BaseException
    {
        private System.String name;

        /// <summary>
        /// Creates a new instance of the <see cref="EnvironmentVariableNotFoundException"/> class
        /// with the specified variable name for which the lookup failed.
        /// </summary>
        /// <param name="envname">The name of the environment variable for which the lookup was failed.</param>
        public EnvironmentVariableNotFoundException(System.String envname)
            : base ($"The specified environment variable was not found. \nEnvironment variable name: {envname}")
        {
            name = envname;
        }

        /// <summary>
        /// Gets the name of the environment variable that the lookup was failed with.
        /// </summary>
        public System.String Name => name;
    }
}