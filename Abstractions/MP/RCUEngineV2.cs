
using System;
using MP.Threading;
using MP.Collections;
using MP.Annotations;
using System.Threading;
using MP.ExceptionSystem;
using System.Collections.Generic;
using MP.Annotations.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;

namespace MP
{
    /// <summary>
    /// This is a rough sketch for the RCU Engine V2!!!! <br />
    /// Do not use it yet because it's implementation details may change.
    /// </summary>
    [Preliminary]
    public abstract class RCUEngineV2 : IAttributeable, IDisposable
    {
        private IOperationsTasker tasker;
        private HashSet<SendCommandDelegateV2> delegates;
        private Dictionary<System.String, System.Object> attributes;
        private Dictionary<System.String, Delegate> engine_functions;

        /// <summary>
        /// A builder class for defining initial functions, attributes and the operation dispatch tasker that the RCU engine will make use of.
        /// </summary>
        protected sealed class RCUEngineBuilder : IAttributeable
        {
            internal Dictionary<System.String, Delegate> engine_functions;
            internal Dictionary<System.String, System.Object> attributes;
            internal IOperationsTasker tasker;

            internal RCUEngineBuilder()
            {
                attributes = new(5);
                engine_functions = new(5);
            }

            /// <summary>
            /// Registers a function to be used by the referred instance.
            /// </summary>
            /// <typeparam name="T">The type of the function to register.</typeparam>
            /// <param name="name">The name of the function to be referenced when needed to.</param>
            /// <param name="function">The function reference to register.</param>
            public void RegisterFunction<T>(System.String name, T function)
                where T : Delegate
            {
                ArgumentNullException.ThrowIfNull(name);
                ArgumentNullException.ThrowIfNull(function);
                engine_functions.Add(name, function);
            }

            /// <summary>
            /// Registers the operations tasker implementation to use with the creating RCU engine implementation.
            /// </summary>
            /// <param name="tasker">The operation tasker to use.</param>
            /// <exception cref="ArgumentNullException"><paramref name="tasker"/> was <see langword="null"/>.</exception>
            public void RegisterTaskerImplementation(IOperationsTasker tasker)
            {
                ArgumentNullException.ThrowIfNull(tasker);
                this.tasker = tasker;
            }

            /// <inheritdoc />
            public object GetAttribute(string name)
            {
                ArgumentNullException.ThrowIfNullOrEmpty(name);
                try {
                    return attributes[name];
                } catch (KeyNotFoundException) {
                    throw new AttributeNotFoundException(name);
                }
            }

            /// <inheritdoc />
            public void SetAttribute(string name, object value)
            {
                ArgumentNullException.ThrowIfNullOrEmpty(name);
                attributes[name] = value;
            }
        }

        // Internal exception class for cases where a function is not found.
        // This is private so that the engine users and the public cannot catch this.
        private sealed class FunctionNotFoundException : BaseException
        {
            public FunctionNotFoundException(String name) : base($"The requested function is not found in the RCU engine.\nFunction name: {name}") { }
        }

        #region Initialization

        /// <summary>Constructs a new <see cref="RCUEngineV2"/> instance.</summary>
        /// <exception cref="RCUEngineInitializationException">The instance has failed to be created. See the inner exception that was caught for more information.</exception>
        [Throws(typeof(RCUEngineInitializationException))]
        public RCUEngineV2()
        {
            RCUEngineBuilder builder = new();
            try {
                Create(builder);
            } catch (Exception e) {
                throw new RCUEngineInitializationException(e);
            }
            tasker = builder.tasker ?? throw new RCUEngineInitializationException(new InvalidOperationException("No operations tasker was registered for this RCU engine instance."));
            attributes = builder.attributes;
            delegates = new(2, new DelegateEqualityComparer<SendCommandDelegateV2>());
            engine_functions = builder.engine_functions;
        }

        /// <summary>
        /// Method definition called only once for setting up fields. <br />
        /// Additionally, the functions that you will use throughout the lifetime of your instance must be defined here as well.
        /// </summary>
        /// <param name="builder">The RCU engine builder class. Through this parameter you register methods and initialize attributes.</param>
        /// <remarks>
        /// During the time this method is called, the engine is still at UNINITIALIZED STATE. <br />
        /// That means, you cannot call any of the methods provided with this instance, including calling the <see cref="Initialize(out RCUEngineInitializationException)"/> method and registering events to the <see cref="SendCommand"/> event. <br />
        /// Calling such methods at this time, the results that they will return will be undefined and the exceptions that will throw are not known.
        /// </remarks>
        protected abstract void Create([DisallowNull] RCUEngineBuilder builder);

        /// <summary>
        /// Provides the implementation for initializing the RCU engine. Initialization happens after everything is ready for it <br />
        /// (such as, resources, global locks, files needed, data, etc.) <br />
        /// After this method returns, the operations tasker starts to dispatch operations.
        /// </summary>
        protected abstract void InitializeImpl();

        /// <summary>
        /// Initializes the RCU engine. <br />
        /// A value is returned whether the initialization succeeded.
        /// </summary>
        /// <param name="exception">The exception occured during initialization.</param>
        /// <returns>A value whether initialization is considered successfull. If not, an exception object will be filled at <paramref name="exception"/> parameter.</returns>
        public System.Boolean Initialize(out RCUEngineInitializationException exception)
        {
            ObjectDisposedException.ThrowIf(delegates is null, this);
            exception = null;
            try {
                InitializeImpl();
                tasker.Run();
            } catch (Exception e) {
                exception = new RCUEngineInitializationException(e);
            }
            return exception is null;
        }

        #endregion

        #region IAttributeable implementation

        /// <inheritdoc />
        /// <exception cref="ObjectDisposedException">The engine was disposed.</exception>
        public System.Object GetAttribute(System.String name)
        {
            if (System.String.IsNullOrEmpty(name)) { throw new ArgumentNullException(nameof(name), "Attribute name must not be the empty string."); }
            ObjectDisposedException.ThrowIf(attributes is null, this);
            try {
                return attributes[name];
            } catch (KeyNotFoundException) {
                throw new AttributeNotFoundException(name);
            }
        }

        /// <inheritdoc />
        /// <exception cref="ObjectDisposedException">The engine was disposed.</exception>
        public void SetAttribute(System.String name, System.Object value)
        {
            if (System.String.IsNullOrEmpty(name)) { throw new ArgumentNullException(nameof(name), "Attribute name must not be the empty string."); }
            ObjectDisposedException.ThrowIf(attributes is null, this);
            attributes[name] = value;
        }

        #endregion

        #region Event implementation

        /// <summary>
        /// Through this event the UI or the target where the engine connects to listens for new events and routes the data appropriately. <br />
        /// The event must have been attached to the target before calling <see cref="Initialize(out RCUEngineInitializationException)"/>;
        /// otherwise , data loss may occur.
        /// </summary>
        /// <exception cref="InvalidOperationException">An already existing send command delegate was attempted to be added for a second time.</exception>
        public event SendCommandDelegateV2 SendCommand
        {
            [Throws(typeof(InvalidOperationException))]
            add
            {
                try {
                    Monitor.Enter(delegates);
                    if (!delegates.Add(value)) {
                        throw new InvalidOperationException("Cannot register an existing send command delegate!");
                    }
                } finally {
                    Monitor.Exit(delegates);
                }
            }

            remove
            {
                try {
                    Monitor.Enter(delegates);
                    delegates.Remove(value);
                } finally {
                    Monitor.Exit(delegates);
                }
            }
        }

        // Internal method to cope with broadcasting the SendCommand event.
        private void BroadcastSendCommand(SendCommandDataEventArgs e)
        {
            try {
                // Acquire access on the delegate set.
                // This allows us that no delegates will be added or released during this broadcast.
                Monitor.Enter(delegates);
                foreach (SendCommandDelegateV2 d in delegates) { d.Invoke(this, e); }
            } finally {
                Monitor.Exit(delegates); // Release access after done with broadcasting
            }
        }

        /// <summary>
        /// Raises a command to the <see cref="SendCommand"/> event.
        /// </summary>
        /// <param name="type">The type of the command to pass.</param>
        protected void RaiseSendCommand(CommonSendCommandTypes type)
        {
            DebugProvider.WriteLine($"CoreMessageDispatcher: Dispatching command {type} to the target.");
            BroadcastSendCommand(new(type));
        }

        /// <summary>
        /// Raises a command to the <see cref="SendCommand"/> event.
        /// </summary>
        /// <param name="type">The type of the command to pass.</param>
        /// <param name="metadata">Additional metadata to send with the command.</param>
        /// <exception cref="ArgumentNullException"><paramref name="metadata"/> was <see langword="null"/>.</exception>
        protected void RaiseSendCommand(CommonSendCommandTypes type, CommandMetadata metadata)
        {
            if (metadata is null) { throw new ArgumentNullException(nameof(metadata), "[API ERROR] The metadata parameter is empty. Maybe you intended to do something else?"); }
            DebugProvider.WriteLine($"CoreMessageDispatcher: Dispatching command {type} to the target.");
            BroadcastSendCommand(new(type) { Metadata = metadata });
        }

        /// <summary>
        /// Raises a message command to the <see cref="SendCommand"/> event.
        /// </summary>
        /// <param name="type">The type of the command to pass.</param>
        /// <param name="message">The message to pass along.</param>
        protected void RaiseSendMessageCommand(CommonSendCommandTypes type, System.String message)
        {
            DebugProvider.WriteLine($"CoreMessageDispatcher: Dispatching command {type} to the target.");
            BroadcastSendCommand(new(type) { MessageData = message });
        }

        /// <summary>Gets a command from a user interaction or from code.</summary>
        /// <param name="cmd">The command data to be processed by the engine implementation.</param>
        public abstract void RecieveCommand(RecieveCommand cmd);

        #endregion

        #region Engine Functions implementation

        /// <summary>
        /// Invokes a previously registered function with no arguments.
        /// </summary>
        /// <param name="name">The name of the function to invoke.</param>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> was null.</exception>
        /// <exception cref="ArgumentException">A parameter count mismatch was detected.</exception>
        [Throws(typeof(ArgumentNullException) , typeof(ArgumentException))]
        protected void InvokeFunction(String name)
        {
            try {
                tasker.Add(engine_functions[name]);
            } catch (KeyNotFoundException) {
                throw new FunctionNotFoundException(name);
            }
        }

        /// <summary>
        /// Invokes a previously registered function with one argument.
        /// </summary>
        /// <param name="name">The name of the function to invoke.</param>
        /// <param name="p">The function parameter to pass.</param>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> was null.</exception>
        /// <exception cref="ArgumentException">A parameter count mismatch was detected.</exception>
        [Throws(typeof(ArgumentNullException), typeof(ArgumentException))]
        protected void InvokeFunction<T>(String name, T p)
        {
            try {
                tasker.Add(engine_functions[name], p);
            } catch (KeyNotFoundException) {
                throw new FunctionNotFoundException(name);
            }
        }

        /// <summary>
        /// Invokes a previously registered function with two arguments.
        /// </summary>
        /// <param name="name">The name of the function to invoke.</param>
        /// <param name="p1">The first function parameter to pass.</param>
        /// <param name="p2">The second function parameter to pass.</param>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> was null.</exception>
        /// <exception cref="ArgumentException">A parameter count mismatch was detected.</exception>
        [Throws(typeof(ArgumentNullException), typeof(ArgumentException))]
        protected void InvokeFunction<T1 , T2>(String name, T1 p1 , T2 p2)
        {
            try {
                tasker.Add(engine_functions[name], p1 , p2);
            } catch (KeyNotFoundException) {
                throw new FunctionNotFoundException(name);
            }
        }

        /// <summary>
        /// Invokes a previously registered function with three arguments.
        /// </summary>
        /// <param name="name">The name of the function to invoke.</param>
        /// <param name="p1">The first function parameter to pass.</param>
        /// <param name="p2">The second function parameter to pass.</param>
        /// <param name="p3">The third function parameter to pass.</param>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> was null.</exception>
        /// <exception cref="ArgumentException">A parameter count mismatch was detected.</exception>
        [Throws(typeof(ArgumentNullException), typeof(ArgumentException))]
        protected void InvokeFunction<T1, T2, T3>(String name , T1 p1, T2 p2, T3 p3)
        {
            try {
                tasker.Add(engine_functions[name], p1, p2, p3);
            } catch (KeyNotFoundException) {
                throw new FunctionNotFoundException(name);
            }
        }

        /// <summary>
        /// Invokes a previously registered function with any number of arguments.
        /// </summary>
        /// <param name="name">The name of the function to invoke.</param>
        /// <param name="arguments">The arguments to pass to the underlying function.</param>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> was null.</exception>
        /// <exception cref="ArgumentException">A parameter count mismatch was detected.</exception>
        [Throws(typeof(ArgumentNullException), typeof(ArgumentException))]
        protected void InvokeFunction(String name , params System.Object[] arguments)
        {
            try {
                tasker.Add(engine_functions[name], arguments: arguments);
            } catch (KeyNotFoundException) {
                throw new FunctionNotFoundException(name);
            }
        }

        /// <summary>
        /// Returns a previously declared function delegate for this RCU engine instance. <br />
        /// Useful for when you want to call this delegate many times in a code excerpt.
        /// </summary>
        /// <typeparam name="T">The type of the function delegate to retrieve. Must match exactly.</typeparam>
        /// <param name="name">The name of the function delegate to retrieve.</param>
        /// <returns>The function delegate, cast to <typeparamref name="T"/> type.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> was null.</exception>
        /// <exception cref="InvalidCastException"><typeparamref name="T"/> does not correspond to the actual function delegate signature.</exception>
        [Throws(typeof(ArgumentNullException) , typeof(InvalidCastException))]
        protected T GetFunctionDelegate<T>(String name)
            where T : Delegate
        {
            ArgumentNullException.ThrowIfNullOrEmpty(name);
            try {
                return (T)engine_functions[name];
            } catch (KeyNotFoundException) {
                throw new FunctionNotFoundException(name);
            }
        }

        /// <summary>
        /// Mutates a function previously registered in the RCU engine function cache. <br />
        /// This method is useful for intercepting events and information from the engine,
        /// but introduces the cost and risk of programming it incorrectly, and/or allowing an attacker to 
        /// gain control. <br />
        /// To alleviate this, engine implementations that do not require this mutation should override this
        /// method and throw directly instead.
        /// </summary>
        /// <typeparam name="T">The type of the delegate to replace with the current one. This must be the exact same type as the previously registered one.</typeparam>
        /// <param name="name">The name of the function to mutate. This must be an existing and 'mutatable' function.</param>
        /// <param name="value">The new implementation of the function, effectively mutating the old one. When this new delegate will be into effect in all the places is unknown.</param>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> and/or <paramref name="value"/> were <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="name"/> was the empty string ("").</exception>
        /// <exception cref="InvalidCastException"><typeparamref name="T"/> type was not matching with the existing function signature.</exception>
        [Throws(
            typeof(ArgumentException),
            typeof(ArgumentNullException),
            typeof(InvalidCastException)
            )]
        protected virtual void MutateFunction<T>(String name, T value)
            where T : Delegate
        {
            ArgumentNullException.ThrowIfNullOrEmpty(name);
            ArgumentNullException.ThrowIfNull(value);
            if (engine_functions.TryGetValue(name , out var func))
            {
                // OK, we found the method to mutate.
                if (func.GetType() != typeof(T)) {
                    // Invalid mutation attempt, throw.
                    throw new InvalidCastException("The function delegate signature must match with the older function signature delegate.\n" +
                        $"Actual type: {typeof(T).FullName}, expected: {func.GetType().FullName}");
                } else {
                    engine_functions[name] = value;
                }
            } else {
                throw new FunctionNotFoundException(name);
            }
        }

        #endregion

        #region External operations tasker interface

        /// <summary>
        /// Registers an external function to be queued and executed by the engine's operations tasker.
        /// </summary>
        /// <typeparam name="T">The type of the function to register.</typeparam>
        /// <param name="function">The function reference to be executed</param>
        /// <param name="arguments">The function's arguments.</param>
        /// <exception cref="ArgumentNullException"><paramref name="function"/> was <see langword="null"/>.</exception>
        /// <exception cref="ObjectDisposedException">The engine was disposed.</exception>
        [Throws(typeof(ArgumentNullException) , typeof(ObjectDisposedException))]
        public void RegisterExecutable<T>(T function, params System.Object[] arguments)
            where T : Delegate
        {
            ObjectDisposedException.ThrowIf(tasker is null, this);
            tasker.Add(function, arguments: arguments);
        }

        /// <summary>
        /// Gets the functions that have failed execution. Useful for fileing these in a crash report.
        /// </summary>
        /// <exception cref="ObjectDisposedException">The engine was disposed.</exception>
        public IEnumerable<OperationsTaskerWorkItemFailureData> FailedExecutables => (tasker ?? throw new ObjectDisposedException(GetType().FullName)).FailedWorkItems;

        /// <summary>
        /// This calls in the <see cref="IOperationsTasker.JoinCurrentOperation"/> method. <br />
        /// It is important to not call this from a method that is executed on the engine's operations tasker!!!
        /// </summary>
        protected void WaitUntilCurrentOperationCompletion() => tasker.JoinCurrentOperation();

        #endregion

        /// <summary>
        /// Gets the current playlist that the engine does currently manage. <br />
        /// Can be null if no playlist is selected (at least at the time the property getter was invoked).
        /// </summary>
        [MaybeNull]
        public abstract PlaylistManagement.Playlist Playlist { get; }

        #region Uninitialization

        /// <summary>
        /// Gets a value whether this RCU engine instance is shutting down. <br />
        /// While in this state, no <see cref="SendCommand"/> events must be performed.
        /// </summary>
        public abstract System.Boolean IsShuttingDown { get; }

        /// <summary>
        /// Prepares the shutdown sequence. <br />
        /// Should be called as soon as the user has requested to exit.
        /// </summary>
        public abstract void PrepareShutdown();

        /// <summary>
        /// Does the exactly reverse of what <see cref="Initialize"/> does; destroys the engine instance. <br />
        /// Called by the <see cref="Dispose"/> method. <br />
        /// This method can throw any exception.
        /// </summary>
        protected abstract void Uninitialize();

        /// <summary>
        /// Unloads the engine , when an explicit request for shutdown has been done before. <br />
        /// Note: It is up to the implementer's responsibility to properly implement the stated logic, and this method <br />
        /// will test the <see cref="IsShuttingDown"/> property to see whether execution can continue.
        /// </summary>
        /// <exception cref="RCUEngineUninitializationException">The engine could not be uninitialized, either due to a user code error or a fatal error in the engine implementation.</exception>
        [Throws(typeof(RCUEngineUninitializationException))]
        public void Dispose()
        {
            try {
                // Lock on the object to ensure Dispose is called once sequentially for all threads.
                // If the lock is not acquired, we know that at least a thread tries to dispose, so let it complete...
                // Additionally, the object must be able to be disposed to continue. This is checked with the IsShuttingDown property.
                if (! (IsShuttingDown || Monitor.TryEnter(this))) { return; }
                // Now call our uninitializer.
                Uninitialize();

                // Process events of the operations tasker object.
                if (tasker is not null)
                {
                    // First step is to process all the tasker operations so far.
                    tasker.StopAndProcessAll();
                    // Then, dispose the tasker.
                    tasker.Dispose();
                    // Finally destroy the object reference, we are ready to continue disposing other stuff.
                    tasker = null;
                }

                // Clear attribute store 
                attributes?.Clear();
                attributes = null;
                // Check whether we can access the delegates collection, and remove them all.
                if (delegates is not null)
                {
                    try {
                        Monitor.Enter(delegates);
                        delegates.Clear();
                    } finally { 
                        // Exceptions specially called by the above lines should always ensure that the lock is released.
                        Monitor.Exit(delegates);
                    }
                    delegates = null; // Now clear the object. This will also cause NRE's or ANE's in SendCommand additions or removals.
                }
                // Now remove all the registered functions
                engine_functions?.Clear();
                engine_functions = null;
                // If we reached here all good!
            } catch (Exception e) {
                // Exception occurred, wrap it into an RCUEngineUninitializationException object and throw that instead.
                throw new RCUEngineUninitializationException(e);
            } finally {
                Monitor.Exit(this); // Release the lock we acquired above...
                GC.SuppressFinalize(this); // If deriving classes do provide a finalizer method...
            }
        }

        #endregion
    }
}