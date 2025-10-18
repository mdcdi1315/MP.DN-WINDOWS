using System;
using System.Collections.Generic;

namespace MP.Graphics.Windowing
{
    /// <summary>
    /// Represents the window dispatcher, a class for dynamically running executables during rendering passes.
    /// </summary>
    public sealed class WindowDispatcher
    {
        private sealed class DelegateData
        {
            public System.Delegate Delegate;
            public System.Object[] Arguments;

            public DelegateData() { }
        }

        private Queue<DelegateData> delegates;
        private List<DelegateData> delegatesineverytick;

        private static bool ErrorHandler_DummyTarget(Exception ex) => false;

        /// <summary>
        /// Creates an empty instance of the <see cref="WindowDispatcher"/> class.
        /// </summary>
        public WindowDispatcher() { 
            delegates = new Queue<DelegateData>();
            delegatesineverytick = new List<DelegateData>();
            ErrorHandler = new(ErrorHandler_DummyTarget);
        }

        /// <summary>
        /// Internal implementation. Dequeues all the enqueued delegates, and runs all the methods that must be run in every loop pass. <br />
        /// If an exception occurs, the error handler at <see cref="ErrorHandler"/> event is fired. <br />
        /// If that handler returns false, it fails and throws the exception to the rendering loop (thus being a hard error)
        /// </summary>
        internal void RunDispatches()
        {
            DelegateData dlg;
            while (delegates.TryDequeue(out dlg))
            {
                Delegate d = dlg.Delegate;
                try {
                    if (d is null) { continue; }
                    d.DynamicInvoke(dlg.Arguments);
                } catch (Exception ex) {
                    if (!ErrorHandler(ex)) {
                        throw;
                    }
                }
            }
            foreach (var dl in delegatesineverytick) 
            {
                Delegate d = dl.Delegate;
                try
                {
                    if (d is null) { continue; }
                    d.DynamicInvoke(dl.Arguments);
                }
                catch (Exception ex)
                {
                    if (!ErrorHandler(ex))
                    {
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Registers an action to be executed in the next executables pass in the window rendering loop.
        /// </summary>
        /// <param name="action">The action to register</param>
        public void RegisterAction(Action action) => delegates.Enqueue(new DelegateData { Delegate = action, Arguments = null });

        /// <summary>
        /// Registers a method to be called once in the next executables pass in the window rendering loop.
        /// </summary>
        /// <typeparam name="TDLG">The method's type to be called</typeparam>
        /// <param name="delegate">The method reference to be called</param>
        /// <param name="arguments">The arguments to pass to the method when it will be called</param>
        public void RegisterMethod<TDLG>(TDLG @delegate , params System.Object[] arguments)
            where TDLG : Delegate
        => delegates.Enqueue(new DelegateData() { Delegate = @delegate, Arguments = arguments });

        /// <summary>
        /// Registers a method to be called in every pass in the window rendering loop.
        /// </summary>
        /// <typeparam name="TDLG">The method's type to be called</typeparam>
        /// <param name="delegate">The method reference to be called</param>
        /// <param name="arguments">The arguments to pass to the method when it will be called</param>
        public void RegisterMethodInEveryFrame<TDLG>(TDLG @delegate , params System.Object[] arguments)
            where TDLG : Delegate
        => delegatesineverytick.Add(new DelegateData() { Delegate = @delegate, Arguments = arguments });

        /// <summary>
        /// Registers an action to be called in every pass in the window rendering loop.
        /// </summary>
        /// <param name="action">The action to register</param>
        public void RegisterActionInEveryFrame(Action action) => delegatesineverytick.Add(new DelegateData() { Delegate = action, Arguments = null });

        /// <summary>
        /// Removes a registered method that was scheduled to be executed in every rendering pass.
        /// </summary>
        /// <typeparam name="TDLG">The delegate type to remove it's method.</typeparam>
        /// <param name="delegate">The method handle to be removed from the list</param>
        /// <returns>A value indicating success or not. If <paramref name="delegate"/> is null, <see langword="false"/> is instead returned.</returns>
        public System.Boolean RemoveMethodInEveryTickPhase<TDLG>(TDLG @delegate)
            where TDLG : Delegate
        {
            if (@delegate is null) { return false; }
            for (int I = delegatesineverytick.Count - 1; I > -1; I--) {
                if (delegatesineverytick[I].Delegate == @delegate) {
                    delegatesineverytick.RemoveAt(I);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Through this event the caller can register an error handler (and thus avoiding to throw the exception)
        /// </summary>
        public event WindowDispatcherErrorHandlerCallback ErrorHandler; 
    }
}
