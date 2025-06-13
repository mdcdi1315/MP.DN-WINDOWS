
namespace MP
{
    using Threading;

    // A special cancellation token for the rotating thread runs.
    internal sealed class RotatingThreadCancellationToken : ICancellationToken<System.Int32>
    {
        private System.Int32 assign;
        private System.Boolean run;

        /// <summary>
        /// Checks whether the thread is valid , otherwise terminate the thread.
        /// </summary>
        /// <param name="unique">The assigned thread number to check for validity.</param>
        /// <returns>A value whether the thread must continue to run.</returns>
        public System.Boolean IsValid(System.Int32 unique) => assign == unique && run;

        /// <summary>
        /// Invalidates the current thread.
        /// </summary>
        public void Invalidate() => run = false;

        /// <summary>
        /// This is meant to be saved by the thread so as to check whether it is invalidated.
        /// </summary>
        public System.Int32 InstanceId => assign;

        private RotatingThreadCancellationToken()
        {
            assign = 0;
            run = false;
        }

        public RotatingThreadCancellationToken(RotatingThreadCancellationToken previous)
        {
            previous ??= new();
            assign = previous.assign + 1;
            if (assign == System.Int32.MaxValue) { assign = 0; }
            run = true;
        }
    }

    internal sealed class OperationsTasksCancellationToken : ICancellationToken<System.Byte>
    {
        private System.Byte assign;
        private System.Boolean run;

        public OperationsTasksCancellationToken(byte assign)
        {
            this.assign = assign;
            run = true;
        }

        public byte InstanceId => assign;

        public void Invalidate() => run = false;

        public bool IsValid(byte unique) => assign == unique && run;
    }
}
