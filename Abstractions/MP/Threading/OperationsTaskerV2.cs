
using System;
using MP.Annotations;
using MP.Collections;
using System.Threading;
using System.Collections.Generic;

namespace MP.Threading
{
    /// <summary>
    /// A newer and revised implementation of the <see cref="OperationsTasker"/> class.
    /// </summary>
    [RequiresNativeLayer]
    public sealed class OperationsTaskerV2 : IOperationsTasker, ISyncronized
    {
        private Thread thread;
        private object opq_lck; // Operation queue lock.
        private volatile bool run;
        // A semaphore is used now anymore as the lock agent for the tasking thread.
        private SemaphoreSlim lck;
        private volatile OperationsTaskerWorkItem current;
        // We do not care to have access to all the operations,
        // we just want to store them and process them.
        // As such, we can store them in this queue implementation instead.
        private readonly SingleLinkedListBasedQueue<OperationsTaskerWorkItem> operation_queue;
        private readonly ArrayBasedList<OperationsTaskerWorkItemFailureData> failed_dispatches;

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationsTaskerV2"/> class.
        /// </summary>
        public OperationsTaskerV2()
        {
            run = false;
            lck = new(0, 1);
            thread = null;
            current = null;
            opq_lck = new();
            operation_queue = new();
            failed_dispatches = new();
        }

        private void ThreadWorker()
        {
            System.Int32 wqhash = 0;
            System.DateTime starttime = default;
            DebugProvider.WriteLine($"OPTasker: Launched operations dispatch thread with id {Thread.CurrentThread.ManagedThreadId} ...");

        process:
            do {
                Monitor.Enter(opq_lck);
                try {
                    OperationsTaskerWorkItem it;
                    if (!operation_queue.TryDequeue(out it)) { break; }
                    current = it;
                } finally {
                    Monitor.Exit(opq_lck);
                }

                try {
                    wqhash = current.GetHashCode();
                    DebugProvider.WriteLine($"OPTasker: Executing work action with code 0x{wqhash:x8}");
                    starttime = SystemInfo.Now;
                    current.MethodToExecute.Method.Invoke(current.MethodToExecute.Target, current.AdditionalMethodArguments);
                    DebugProvider.WriteLine("OPTasker: Work action finished successfully.");
                } catch (System.Reflection.TargetInvocationException tie) {
                    // Throw the causing exception instead.
                    // Do not throw the exception now , throw it after the thread finishes completely.
                    // In Debug at the meantime, report the error in console.
                    DebugProvider.WriteLine($"OPTasker: The work action with code 0x{wqhash:x8} FAILED.");
                    DebugProvider.WriteLine($"OPTasker: Exception: {tie.InnerException}");
                    if (current.MethodToExecute is not null) {
                        failed_dispatches.Add(new(wqhash, starttime, SystemInfo.Now, tie.InnerException, current.MethodToExecute.Method, current.AdditionalMethodArguments));
                    } else {
                        failed_dispatches.Add(new(wqhash, starttime, SystemInfo.Now, tie.InnerException, null, current.AdditionalMethodArguments));
                    }
                }
            } while (run);

            if (run) {
                DebugProvider.WriteLine("OPTasker: The dispatch work queue is now empty.");
                DebugProvider.WriteLine("OPTasker: Suspending thread until a new dispatch operation occurs...");

                // Enter semaphore acquire state - sleep the thread indefinitely.
                lck.Wait();
            }

            // If we have unlocked because at least one dispatching operation was added, process it. 
            // Otherwise, terminate.
            if (run) { goto process; }

            DebugProvider.WriteLine($"OPTasker: Destroying operations tasker thread with ID {Thread.CurrentThread.ManagedThreadId}.");
        }

        /// <inheritdoc />
        public bool IsRunning => run;

        /// <inheritdoc />
        public int QueuedWorkItems => operation_queue.Count;

        /// <inheritdoc />
        public OperationsTaskerWorkItem RunningWorkItem => current;

        /// <inheritdoc />
        public int FailedWorkItemsCount => failed_dispatches.Count;

        /// <inheritdoc />
        public IEnumerable<OperationsTaskerWorkItemFailureData> FailedWorkItems => failed_dispatches;

        /// <inheritdoc />
        public void Add(OperationsTaskerWorkItem methoditem)
        {
            ObjectDisposedException.ThrowIf(lck is null, this);
            Monitor.Enter(opq_lck);
            try {
                operation_queue.Enqueue(methoditem);
                if (lck.CurrentCount == 0) { lck.Release(); }
            } finally {
                Monitor.Exit(opq_lck);
            }
        }

        /// <inheritdoc />
        public void ClearCurrentWorkItemQueue()
        {
            ObjectDisposedException.ThrowIf(lck is null, this);
            Monitor.Enter(opq_lck);
            try {
                operation_queue.Clear();
            } finally {
                Monitor.Exit(opq_lck);
            }
        }

        /// <inheritdoc />
        public void JoinCurrentOperation()
        {
            ObjectDisposedException.ThrowIf(lck is null, this);
            OperationsTaskerWorkItem c = current;
            while (run && ReferenceEquals(c, current)) { Thread.Sleep(10); }
        }

        /// <inheritdoc />
        public void Run()
        {
            ObjectDisposedException.ThrowIf(lck is null, this);
            if (run) { return; }
            run = true;

            thread = new(new ThreadStart(ThreadWorker));

            thread.IsBackground = true;
            thread.Priority = ThreadPriority.BelowNormal;
            thread.Name = "[MP] Operations Tasker V2 Thread";

            thread.Start();
        }

        /// <inheritdoc />
        public void Stop()
        {
            ObjectDisposedException.ThrowIf(lck is null, this);
            run = false;
            // If the thread is in the sleeping state, let's cause it to wake up.
            if (lck.CurrentCount == 0) { lck.Release(); }
            thread?.Join();
            thread = null;
        }

        /// <inheritdoc />
        public void StopAndProcessAll()
        {
            ObjectDisposedException.ThrowIf(lck is null, this);
            if (thread is null) {
                // Simply clean. Do not even acquire the lock!
                operation_queue.Clear();
            } else if (operation_queue.Count > 0) {
                // We will wait until the semaphore comes to a locked state.
                while (lck.CurrentCount != 0) { Thread.Sleep(20); }
                // Now we can stop the thread safely.
                run = false;
                thread.Join();
                thread = null;
            }
        }

        /// <summary>
        /// Disposes this <see cref="OperationsTaskerV2"/> object. <br />
        /// Note that this method call is fully thread-safe.
        /// </summary>
        public void Dispose()
        {
            Monitor.Enter(this);
            try {
                if (thread is not null)
                {
                    // If our thread instance is alive we have to ensure that it aknownledges that it will be invalidated in a bit.
                    run = false;

                    Monitor.Enter(opq_lck);
                    try {
                        // Let's cleanup the operation queue to ensure that we are OK with that.
                        operation_queue.Clear();
                    } finally {
                        Monitor.Exit(opq_lck);
                    }

                    if (thread.IsAlive) {
                        // If the thread is in the sleeping state, let's cause it to wake up.
                        if (lck.CurrentCount == 0) { lck.Release(); }
                        // Wait for the thread to complete
                        thread.Join();
                    }

                    thread = null;
                }

                // Now destroy the semaphore as well.
                lck?.Dispose();
                lck = null;

                // Release lock object used for the dispatch queue
                opq_lck = null;
            } finally {
                Monitor.Exit(this);
            }
        }
    }
}