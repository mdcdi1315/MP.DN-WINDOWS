using System;
using System.Collections.Generic;
using MP.Annotations.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;

namespace MP.Threading
{
    /// <summary>
    /// Manages and runs operations one-by-one , as the RCU engine dictates to run it's commands. <br />
    /// Provided into this library as an abstraction and implementation for reference and why not, to use it in your code.
    /// </summary>
    public interface IOperationsTasker : IDisposable
    {
        /// <summary>
        /// Queues the specified method to be executed by the tasker.
        /// </summary>
        /// <param name="methoditem">The method to queue and execute into the operation tasker.</param>
        /// <exception cref="ArgumentNullException"><paramref name="methoditem"/> was <see langword="null"/>.</exception>
        public void Add(OperationsTaskerWorkItem methoditem);

        /// <summary>
        /// Causes the tasker to start dequeueing cached operations. <br />
        /// After all the operations have been completed , the tasker does not stop running , but it waits for new ones once they occur.
        /// </summary>
        public void Run();

        /// <summary>
        /// Gets a value whether this tasker object is active; <br />
        /// that is, the <see cref="Run"/> method has been called on the instance.
        /// </summary>
        public System.Boolean IsRunning { get; }

        /// <summary>
        /// Gets the work item currently executed by the current operations tasker instance.
        /// </summary>
        [MaybeNull]
        public OperationsTaskerWorkItem RunningWorkItem { get; }

        /// <summary>
        /// Gets the number of tasker work items waiting to be run.
        /// </summary>
        public System.Int32 QueuedWorkItems { get; }

        /// <summary>
        /// Gets the number of failed work items for the lifetime of this instance. <br />
        /// A failed work item is considered any work item that has thrown an unhandled exception.
        /// </summary>
        public System.Int32 FailedWorkItemsCount { get; }

        /// <summary>
        /// Gets an enumerable that contains information about the failed work items in the current instance.
        /// </summary>
        public IEnumerable<OperationsTaskerWorkItemFailureData> FailedWorkItems { 
            [return: MaybeReturnEmptyCollectionButNeverNull]
            get;
        }

        /// <summary>
        /// Temporarily joins the tasker thread with the calling thread until the current work item reaches it's end.
        /// </summary>
        public void JoinCurrentOperation();

        /// <summary>
        /// Causes any queued work items to be effectively discarded. <br />
        /// Note: The current work item will still execute.
        /// </summary>
        public void ClearCurrentWorkItemQueue();

        /// <summary>
        /// Causes the tasker thread to be destroyed, after the current method has completed. <br />
        /// Note that you can re-create a new tasker thread by just calling the <see cref="Run"/> method. <br />
        /// You just use this to stop the thread and dispose this object now or to temporarily stop executing work items.
        /// </summary>
        public void Stop();

        /// <summary>
        /// Causes the tasker thread to be destroyed, after all the methods in the work item queue have been completed. <br />
        /// Note that you can re-create a new tasker thread by just calling the <see cref="Run"/> method. <br />
        /// You just use this to stop the thread and dispose this object now or to temporarily stop executing work items. <br />
        /// This call will block until the tasker thread terminates.
        /// </summary>
        public void StopAndProcessAll();
    }
}
