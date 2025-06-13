

namespace MP.Threading
{
    /// <summary>
    /// Represents a method that failed execution. <br />
    /// Usually this represents any exception thrown from the method itself.
    /// </summary>
    public sealed class OperationsTaskerWorkItemFailureData
    {
        private System.Object[] argumentvalues;
        private System.Reflection.MethodInfo methodfailed;
        private System.Int32 workqueueitemhash;
        private System.DateTime starttime, failuretime;
        private System.Exception occuredexception;

        /// <summary>
        /// Initializes a new <see cref="OperationsTaskerWorkItemFailureData" /> class instance from the specified error information.
        /// </summary>
        /// <param name="workwqhash">A pseudo-unique hash code of the item which it was failed</param>
        /// <param name="start">The time when the method was started execution</param>
        /// <param name="failedat">The time when the method failed</param>
        /// <param name="exception">The exception that was thrown by the method.</param>
        /// <param name="faillingmethod">A <see cref="System.Reflection.MethodInfo"/> instance which represents the method that was failed.</param>
        /// <param name="argspassed">Additional argument values passed to the method.</param>
        public OperationsTaskerWorkItemFailureData(System.Int32 workwqhash, System.DateTime start, System.DateTime failedat, System.Exception exception, System.Reflection.MethodInfo faillingmethod = null, System.Object[] argspassed = null)
        {
            workqueueitemhash = workwqhash;
            starttime = start;
            failuretime = failedat;
            occuredexception = exception;
            methodfailed = faillingmethod;
            argumentvalues = argspassed;
        }

        /// <summary>
        /// A pseudo-unique hash code of the item which it was failed. <br />
        /// Auto-filled by the operations tasker; it is not necessary to implement this in your code.
        /// </summary>
        public System.Int32 WorkQueueItemHash => workqueueitemhash;

        /// <summary>
        /// The exact time when the method was invoked by the tasker.
        /// </summary>
        public System.DateTime StartTime => starttime;

        /// <summary>
        /// The exact time when the tasker discovered that the method was failed.
        /// </summary>
        public System.DateTime FailureTime => failuretime;

        /// <summary>
        /// The exception that was thrown by the method
        /// </summary>
        public System.Exception Exception => occuredexception;

        /// <summary>
        /// The method's information, as specified by the Reflection API's.
        /// </summary>
        public System.Reflection.MethodInfo FailedMethod => methodfailed;

        /// <summary>
        /// The arguments passed to the method. <br />
        /// May be null or empty.
        /// </summary>
        public System.Object[] Arguments => argumentvalues;
    }
}