
using System;
using System.Threading;
using System.Runtime.Versioning;
using MP.Annotations.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.ConstrainedExecution;

namespace MP.Threading
{
    /// <summary>
    /// Provides a <see cref="Thread"/>-like implementation that is cancellable.
    /// </summary>
    public sealed class CancellableThread : CriticalFinalizerObject
    {
        private readonly Thread thread;
        private readonly Details details;
        
        private sealed class Details : ICancellableThreadDetails
        {
            private bool cancel;
            private object user_object;
            private readonly CancellableThreadStart ts;

            public Details(object user_object, CancellableThreadStart thread_start)
            {
                cancel = false;
                ts = thread_start;
                this.user_object = user_object;
            }

            public void Cancel() => cancel = true;

            public bool ShouldCancel => cancel;

            public object AdditionalData
            {
                get => user_object;
                set => user_object = value;
            }

            public void ThreadCode() => ts.Invoke(this);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="CancellableThread"/> class from the given thread start object that represents the methods to execute once the thread is started.
        /// </summary>
        /// <param name="thread_start">The thread start delegate to invoke once the thread is started.</param>
        [Throws(typeof(ArgumentNullException))]
        public CancellableThread(CancellableThreadStart thread_start)
        {
            ArgumentNullException.ThrowIfNull(thread_start);
            details = new(null, thread_start);
            thread = new(new ThreadStart(details.ThreadCode));
        }

        /// <summary>
        /// Gets or sets the name of thread. <br />
        /// If <see langword="null"/>, it means that a name has not been assigned yet to the thread.
        /// </summary>
        [MaybeNull]
        public string Name
        {
            get => thread.Name;
            [Throws(typeof(InvalidOperationException))]
            set => thread.Name = value;
        }

        /// <summary>
        /// Gets or sets a value whether or not a thread is a background thread.
        /// </summary>
        /// <exception cref="ThreadStateException">Attempted to change this value while the thread is running.</exception>
        public bool IsBackground
        {
            get => thread.IsBackground;
            [Throws(typeof(ThreadStateException))]
            set => thread.IsBackground = value;
        }

        /// <summary>
        /// Gets a value containing the states of the current thread.
        /// </summary>
        public ThreadState ThreadState => thread.ThreadState;

        /// <summary>
        /// Gets or sets a value indicating the scheduling priority of the current thread.
        /// </summary>
        public ThreadPriority Priority
        {
            [Throws(typeof(ThreadStateException), typeof(ArgumentException))]
            get => thread.Priority;
            [Throws(typeof(ThreadStateException), typeof(ArgumentException))]
            set => thread.Priority = value;
        }

        /// <summary>
        /// Gets a unique identifier for the current managed thread.
        /// </summary>
        /// <returns>An integer that represents a unique identifier for this managed thread.</returns>
        public int ManagedThreadId => thread.ManagedThreadId;

        /// <summary>
        /// Gets an <see cref="System.Threading.ExecutionContext"/> object that contains information about the various contexts of the current thread.
        /// </summary>
        /// <returns>An <see cref="System.Threading.ExecutionContext"/> object that consolidates context information for the current thread.</returns>
        [MaybeNull]
        public ExecutionContext ExecutionContext => thread.ExecutionContext;

        /// <summary>
        /// Interrupts a thread that is in the <see cref="ThreadState.WaitSleepJoin"/> thread state.
        /// </summary>
        [Throws]
        public void Interrupt() => thread.Interrupt();

        /// <summary>
        /// Blocks the calling thread until the thread represented by this instance terminates, while continuing to perform standard COM and SendMessage pumping.
        /// </summary>
        [Throws(typeof(ThreadStateException), typeof(ThreadInterruptedException))]
        public void Join() => thread.Join();

        /// <summary>
        /// Blocks the calling thread until the thread represented by this instance terminates or the specified time elapses, while continuing to perform standard COM and SendMessage pumping.
        /// </summary>
        /// <param name="timeout_ms">The number of milliseconds to wait for the thread to time out.</param>
        /// <returns><see langword="true"/> if the thread has been terminated; <see langword="false"/> if the thread has not been terminated after the amount of time specified by the <paramref name="timeout_ms"/> parameter has elapsed.</returns>
        /// <exception cref="ThreadStateException">The thread has not been started.</exception>
        /// <exception cref="ThreadInterruptedException">The thread was interrupted while waiting.</exception>
        /// <exception cref="ArgumentException"><paramref name="timeout_ms"/> is less than -1 (<see cref="Timeout.Infinite"/>).</exception>
        /// <exception cref="ArgumentOutOfRangeException">The value of <paramref name="timeout_ms"/> is negative and is not equal to <see cref="Timeout.Infinite"/> in milliseconds.</exception>
        [Throws(
            typeof(ArgumentException),
            typeof(ThreadStateException), 
            typeof(ThreadInterruptedException),
            typeof(ArgumentOutOfRangeException)
        )]
        public bool Join(int timeout_ms) => thread.Join(timeout_ms);
 
        /// <summary>
        /// Blocks the calling thread until the thread represented by this instance terminates or the specified time elapses, while continuing to perform standard COM and SendMessage pumping.
        /// </summary>
        /// <param name="timeout">A <see cref="TimeSpan"/> set to the amount of time to wait for the thread to terminate.</param>
        /// <returns>true if the thread terminated; false if the thread has not terminated after the amount of time specified by the <paramref name="timeout"/> parameter has elapsed.</returns>
        /// <exception cref="ThreadStateException">The caller attempted to join a thread that is in the <see cref="ThreadState.Unstarted"/> state.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The value of <paramref name="timeout"/> is negative and is not equal to <see cref="Timeout.Infinite"/> in milliseconds, or is greater than <see cref="Int32.MaxValue"/> milliseconds.</exception>
        [Throws(
            typeof(ThreadStateException), 
            typeof(ArgumentOutOfRangeException)
        )]
        public bool Join(TimeSpan timeout) => thread.Join(timeout);

        /// <summary>
        /// Turns off automatic cleanup of runtime callable wrappers (RCW) for the current thread.
        /// </summary>
        public void DisableComObjectEagerCleanup() => thread.DisableComObjectEagerCleanup();

        /// <summary>
        /// Returns an <see cref="ApartmentState"/> value indicating the apartment state.
        /// </summary>
        /// <returns>One of the <see cref="ApartmentState"/> values indicating the apartment state of the managed thread. The default is <see cref="ApartmentState.Unknown"/>.</returns>
        public ApartmentState GetApartmentState() => thread.GetApartmentState();

        /// <summary>Sets the apartment state of a thread before it is started.</summary>
        /// <param name="state">The new apartment state.</param>
        /// <returns>true if the apartment state is set; otherwise, false.</returns>
        /// <exception cref="ArgumentException"><paramref name="state"/> is not a valid apartment state.</exception>
        /// <exception cref="PlatformNotSupportedException">.NET Core and .NET 5+ only: In all cases on macOS and Linux.</exception>
        /// <exception cref="ThreadStateException">The thread was started and has terminated, or the call is not being made from the thread's context while the thread is running.</exception>
        [UnsupportedOSPlatform("macos")]
        [UnsupportedOSPlatform("linux")]
        [Throws(typeof(PlatformNotSupportedException), typeof(ArgumentException) , typeof(ThreadStateException))]
        public bool TrySetApartmentState(ApartmentState state) => thread.TrySetApartmentState(state);

        /// <summary>
        /// Causes the operating system to change the state of the cancellable thread to <see cref="ThreadState.Running"/>.
        /// </summary>
        /// <param name="user_object">Additional user object to pass to the thread's code. Can be <see langword="null"/> as well.</param>
        /// <exception cref="ThreadStateException">The thread has already been started.</exception>
        /// <exception cref="OutOfMemoryException">There is not enough memory available to start this thread.</exception>
        [Throws(typeof(ThreadStateException), typeof(OutOfMemoryException))]
        public void Start([AllowNull] System.Object user_object)
        {
            if (thread.ThreadState == ThreadState.Unstarted) { details.AdditionalData = user_object; }
            thread.Start();
        }

        /// <summary>
        /// Causes the operating system to change the state of the cancellable thread to <see cref="ThreadState.Running"/>.
        /// </summary>
        /// <exception cref="ThreadStateException">The thread has already been started.</exception>
        /// <exception cref="OutOfMemoryException">There is not enough memory available to start this thread.</exception>
        [Throws(typeof(ThreadStateException), typeof(OutOfMemoryException))]
        public void Start() => Start(null);

        /// <summary>
        /// Attempts to cancel the running thread. <br />
        /// Running this while the thread is in unstarted state is invalid.
        /// </summary>
        /// <exception cref="ThreadStateException">Attempted to cancel a thread that was never started.</exception>
        [Throws(typeof(ThreadStateException))]
        public void Cancel()
        {
            if (thread.ThreadState == ThreadState.Unstarted) {
                throw new ThreadStateException("Cannot cancel an unstarted thread!");
            }
            details.Cancel();
        }

        /// <summary>Returns a hash code for the current thread.</summary>
        /// <returns>An integer hash code value.</returns>
        public override int GetHashCode() => thread.GetHashCode();
    }
}