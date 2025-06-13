namespace MP
{
    /// <summary>
    /// Defines a log source, for example a component that logs multiple messages.
    /// </summary>
    public abstract class DebugSource
    {
        private System.Boolean active;
        /// <summary>
        /// Gets or sets a unique name to identify this debug source among others.
        /// </summary>
        protected internal System.String SourceName;

        /// <summary>
        /// Creates a new debug source instance.
        /// </summary>
        protected DebugSource() { 
            active = true; 
            SourceName = GetType().Name;
        }

        /// <summary>
        /// Gets a value whether this debug source is currently active.
        /// </summary>
        public System.Boolean IsActive => active;

        /// <summary>
        /// Enables this debug source instance, after being temporarily disabled with <see cref="Disable"/>.
        /// </summary>
        public void Enable() => active = true;
        
        /// <summary>
        /// Disables temporarily this debug source instance.
        /// </summary>
        public void Disable() => active = false;

        /// <summary>
        /// Writes log text to the underlying debugging provider.
        /// </summary>
        /// <param name="msg">The text to write as log.</param>
        protected void WriteLog(System.String msg)
        {
            if (active) {
                DebugProvider.SourceLog(this , msg);
            }
        }

        /// <summary>
        /// Writes a log line to the underlying debugging provider.
        /// </summary>
        /// <param name="msg">The log line to write.</param>
        protected void WriteLogLine(System.String msg) => WriteLog($"{msg}\n");
    }
}