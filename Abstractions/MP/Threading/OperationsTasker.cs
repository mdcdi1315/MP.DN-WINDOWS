
using System;
using MP.Collections;
using MP.Annotations;

namespace MP.Threading
{
    /// <summary>
    /// Manages and runs tasking operations on a seperate thread. <br />
    /// The requests are submitted to a queue where all the tasking operations are temporarily saved ,  <br />
    /// before these do actually run by the dispatch thread , which is also managed by an instance of this class. <br />
    /// Completely thread-safe.
    /// </summary>
    [RequiresNativeLayer]
    public sealed class OperationsTasker : IOperationsTasker
    {
        private WorkItemInternal current;
        private System.Int32 index;
        private System.Boolean running , run;
        private System.Threading.EventWaitHandle wh;
        private System.Threading.Thread background;
        private SingleLinkedListBasedQueue<WorkItemInternal> pendingoperations;
        private ArrayBasedList<OperationsTaskerWorkItemFailureData> faileddispatches;

        private sealed class WorkItemInternal : OperationsTaskerWorkItem
        {
            private System.Int32 fordinal;

            public WorkItemInternal(Delegate @delegate, System.Int32 ordinal , params object[] arguments) : base(@delegate, arguments)
            {
                fordinal = ordinal;
            }

            public System.Int32 Ordinal => fordinal;
        }

        /// <summary>
        /// Creates a new <see cref="OperationsTasker"/> class instance.
        /// </summary>
        public OperationsTasker()
        {
            background = null;
            pendingoperations = new(); // Private operations cache for each delegate that needs to be executed
            wh = new(false , System.Threading.EventResetMode.ManualReset);
            faileddispatches = new(5);
            running = false;
            run = false;
            index = 0;
        }

        /// <inheritdoc />
        public void Add(OperationsTaskerWorkItem operation)
        {
            if (operation is null) { return; }
            pendingoperations.Enqueue(new WorkItemInternal(operation.MethodToExecute, index++, arguments: operation.AdditionalMethodArguments));
            wh.Set();
        }
        
        private void BackgroundWorkerCode()
        {
            try {
                running = true;
                System.Int32 wqhash = 0;
                System.DateTime starttime = default;
                DebugProvider.WriteLine($"OPTasker: Launched operations dispatch thread with id {System.Threading.Thread.CurrentThread.ManagedThreadId} ...");
                while (run && pendingoperations is not null)
                {
                    if (pendingoperations.TryDequeue(out current))
                    {
                        try {
                            wqhash = current.GetHashCode();
                            DebugProvider.WriteLine($"OPTasker: Executing work action with code 0x{wqhash:x8}");
                            starttime = SystemInfo.Now;
                            current.MethodToExecute.Method.Invoke(current.MethodToExecute.Target, current.AdditionalMethodArguments);
                            DebugProvider.WriteLine("OPTasker: Work action finished successfully.");
                        } catch (System.Reflection.TargetInvocationException ex) {
                            // Throw the causing exception instead.
                            // Do not throw the exception now , throw it after the thread finishes completely.
                            // In Debug at the meantime, report the error in console.
                            DebugProvider.WriteLine($"OPTasker: A new exception was pushed into the exception queue. \nThis exception, and any other one that might occur later, will be filed to a crash report after the app is shut down. \nException: {ex.InnerException}");
                            if (current.MethodToExecute is not null) {
                                faileddispatches.Add(new(wqhash, starttime, SystemInfo.Now, ex.InnerException, current.MethodToExecute.Method , current.AdditionalMethodArguments));
                            } else {
                                faileddispatches.Add(new(wqhash, starttime, SystemInfo.Now, ex.InnerException , null , current.AdditionalMethodArguments));
                            }
                        } catch (Exception ex) {
                            if (ex is not NullReferenceException)
                            {
                                // Do not throw the exception now , throw it after the thread finishes completely.
                                // In Debug at the meantime, report the error in console.
                                DebugProvider.WriteLine($"OPTasker: A new exception was pushed into the exception queue. \nThis exception, and any other one that might occur later, will be filed to a crash report after the app is shut down. \nException: {ex}");
                                faileddispatches.Add(new(current is null ? 0 : current.GetHashCode(), starttime, SystemInfo.Now, ex));
                            }
                        }
                    } else {
                        DebugProvider.WriteLine("OPTasker: The dispatch work queue is now empty.");
                        wh.Reset();
                        DebugProvider.WriteLine("OPTasker: Suspending thread until a new dispatch operation occurs...");
                        System.Threading.WaitHandle.WaitAny(new System.Threading.WaitHandle[] { wh });
                    }
                }
                current = null;
            } catch (NullReferenceException e)
                when (e.ToString().Contains(nameof(pendingoperations)))
            { 
            
            } finally {
                running = false;
            }
            DebugProvider.WriteLine("OPTasker: Dispatch thread is now shutting down...");
        }

        /// <inheritdoc />
        public void Run()
        {
            // Check whether the dispatch thread is already running.
            if (running) { return; } // Exit if that is true
            background = new(BackgroundWorkerCode);
            background.IsBackground = true;
            background.Name = "[MP] Backrground actions running thread";
            background.TrySetApartmentState(System.Threading.ApartmentState.MTA);
            run = true;
            background.Start();
        }

        /// <inheritdoc />
        public System.Boolean IsRunning => background is not null && running;

        /// <summary>
        /// Gets the current running operation that the thread manages.
        /// </summary>
        public OperationsTaskerWorkItem RunningWorkItem => current;

        /// <summary>
        /// Gets the number of queued operations waiting to be run.
        /// </summary>
        public System.Int32 QueuedWorkItems => pendingoperations.Count;

        /// <summary>
        /// Gets the number of failed operations for the lifetime of this instance. <br />
        /// A failed operation is considered any operation that has thrown an unhandled exception.
        /// </summary>
        public System.Int32 FailedWorkItemsCount => faileddispatches.Count;

        /// <summary>
        /// Gets an enumerable that contains information about the failed operations in the dispatch work queue.
        /// </summary>
        public System.Collections.Generic.IEnumerable<OperationsTaskerWorkItemFailureData> FailedWorkItems => faileddispatches;

        /// <summary>
        /// Temporarily joins the executing thread with the calling thread until the current operation reaches the end.
        /// </summary>
        public void JoinCurrentOperation()
        {
            WorkItemInternal lc = null;
            if (current is null) { return; }
            do
            {
                System.Threading.Interlocked.Exchange(ref lc, current);
                System.Threading.Thread.Sleep(10);
            } while (lc is not null && lc.Ordinal == current.Ordinal);
        }

        /// <summary>
        /// Causes any scheduled methods to run to be discarded.
        /// </summary>
        public void ClearCurrentWorkItemQueue() => pendingoperations?.Clear();

        /// <summary>
        /// Causes the dispatch thread to be destroyed, after the current method has completed. <br />
        /// Note that you can re-create a new dispatch thread by just calling the <see cref="Run"/> method. <br />
        /// You just use this to stop the thread and dispose this object now or to temporarily stop dispatching.
        /// </summary>
        public void Stop()
        {
            if (background is not null)
            {
                run = false;
                do { wh?.Set(); } while (background.Join(70) == false);
                background = null;
            }
        }

        /// <summary>
        /// Causes the dispatch thread to be destroyed, after all the methods in the dispatch queue have been completed. <br />
        /// Note that you can re-create a new dispatch thread by just calling the <see cref="Run"/> method. <br />
        /// You just use this to stop the thread and dispose this object now or to temporarily stop dispatching. <br />
        /// This call will block until the dispatch thread terminates.
        /// </summary>
        public void StopAndProcessAll()
        {
            if (background is not null)
            {
                while (pendingoperations is not null && pendingoperations.Count > 0) { System.Threading.Thread.Sleep(30); }
                run = false;
                do { wh?.Set(); } while (background.Join(70) == false);
                background = null;
            }
        }

        /// <summary>
        /// Disposes this <see cref="OperationsTasker"/> instance after ensuring that all dispatch commands were run.
        /// </summary>
        public void Dispose()
        {
            if (wh is null) { return; }
            StopAndProcessAll(); // Stop the background thread.
            // Now run the disposal code...
            if (pendingoperations is not null)
            {
                pendingoperations.Clear();
                pendingoperations = null;
            }
            if (faileddispatches is not null)
            {
                faileddispatches.Clear();
                faileddispatches = null;
            }
            wh?.Dispose();
            wh = null;
            background = null;
            current = null;
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Finalizer.
        /// </summary>
        ~OperationsTasker() => Dispose();
    }

}