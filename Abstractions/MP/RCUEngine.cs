

using System;
using System.Collections.Generic;

namespace MP
{
    /// <summary>
    /// Defines the RCU Engine, which is the core of the Music Player. <br />
    /// This class definition is the pure stripped abstraction that can be embedded anywhere. <br />
    /// What is the RCU Engine? <br />
    /// -&gt; The engine is in it's entirety a state machine that processes requests 
    /// from the user (either those are programmatic or are from interaction) ,
    /// routes appropriately for each platform the required command sequences 
    /// to run and then schedules these to be run when the engine has finished 
    /// from previous requests. <br />
    /// While a request is running the engine must issue commands back to the interaction consumer 
    /// (either this is a UI or a programmatic interface) and these must be appropriately handled by the target.<br />
    /// Note that the engine should only manage one request at a time; that allows it to 
    /// become thread independent and assures that all commands will correctly be issued, 
    /// and that it's state will not be corrupted. <br />
    /// -&gt; The engine is optimized for doing large playback sessions reliably, no matter how many
    /// playlists, tracks or modifications will happen to it's state. <br />
    /// -&gt; It is called 'Runtime Compiled Units' since the engine must be aware for all compiled IL command sequences
    /// that should be run under a specific engine state.
    /// </summary>
    public abstract class RCUEngine : IDisposable , IAttributeable
    {
        private Dictionary<System.String, System.Object> attributes;

        // A dummy function command handler in order to construct an instance of the SendCommand event properly.
        // If i was kept to use the lambda function , that would allocate 24 bytes which could be used for sth else.
        private static void DummySendCommandHandler(System.Object send, SendCommandDataEventArgs e) { }

        /// <summary>
        /// Creates a new RCU Engine instance. <br />
        /// When the object is created only creation code must be run. <br />
        /// See the <see cref="Create"/> method for more info.
        /// </summary>
        public RCUEngine() 
        {
            SendCommand = new(DummySendCommandHandler);
            attributes = new(5);
            Create();
        }

        /// <summary>
        /// This is called by the constructor to create any additional fields that you define and they need initialization routines
        /// that must be run independently from the <see cref="Initialize"/> method.
        /// </summary>
        protected abstract void Create();

        /// <summary>
        /// In this method you must provide all the initialization code that is required to be run. <br />
        /// Note that this method should be run lazily, or at any demanded time. Do not depend on when this method will actually run.
        /// </summary>
        /// <returns>A value whether initialization succeeded. On a single failed call, all subsequent calls must fail.</returns>
        public abstract System.Boolean Initialize();

        /// <summary>
        /// Raises a command to the <see cref="SendCommand"/> event.
        /// </summary>
        /// <param name="type">The type of the command to pass.</param>
        protected void RaiseSendCommand(CommonSendCommandTypes type)
        {
            DebugProvider.WriteLine($"CoreMessageDispatcher: Dispatching command {type} to the target.");
            SendCommand.Invoke(this, new(type));
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
            SendCommand.Invoke(this, new(type) { Metadata = metadata });
        }

        /// <summary>
        /// Raises a message command to the <see cref="SendCommand"/> event.
        /// </summary>
        /// <param name="type">The type of the command to pass.</param>
        /// <param name="message">The message to pass along.</param>
        protected void RaiseSendMessageCommand(CommonSendCommandTypes type , System.String message)
        {
            DebugProvider.WriteLine($"CoreMessageDispatcher: Dispatching command {type} to the target.");
            SendCommand.Invoke(this, new(type) { MessageData = message });
        }

        /// <summary>Gets a command from a user interaction or from code.</summary>
        /// <param name="cmd">The command data to be processed by the engine implementation.</param>
        public abstract void RecieveCommand(RecieveCommand cmd);

        /// <summary>
        /// Through this event the UI or the target listens for new events and routes the data appropriately. <br />
        /// The event must have been attached to the target before calling <see cref="Initialize()"/>;
        /// otherwise , data loss may occur.
        /// </summary>
        public event SendCommandDelegate SendCommand;

        /// <summary>
        /// Gets a value whether this RCU engine instance is shutting down. <br />
        /// While in this state, no <see cref="SendCommand"/> events must be performed.
        /// </summary>
        public abstract System.Boolean IsShuttingDown { get; }

        /// <summary>
        /// Gets the currently playing track from the current playlist. You might not use this property,
        /// especially when you are working with games which you must concurrently play sounds.
        /// </summary>
        public abstract System.Int32 CurrentTrackIndex { get; }

        /// <summary>
        /// Gets the currently activated playlist that the engine manages.
        /// </summary>
        public abstract IPlaylist Playlist { get; }

        /// <summary>
        /// Gets the tag from the current playing file , if there is any.
        /// </summary>
        public SavedDataTag GetTag() => Playlist.GetTagFromFile(Playlist.TracksContained[CurrentTrackIndex]);

        /// <inheritdoc />
        public System.Object GetAttribute(System.String name)
        {
            if (System.String.IsNullOrEmpty(name)) { throw new ArgumentNullException(nameof(name), "Attribute name must not be the empty string."); }
            try {
                return attributes[name];
            } catch (KeyNotFoundException) {
                throw new ExceptionSystem.AttributeNotFoundException(name);
            }
        }

        /// <inheritdoc />
        public void SetAttribute(System.String name, System.Object value)
        {
            if (System.String.IsNullOrEmpty(name)) { throw new ArgumentNullException(nameof(name), "Attribute name must not be the empty string."); }
            attributes[name] = value;
        }

        /// <summary>
        /// Prepares the shutdown sequence. <br />
        /// Should be called as soon as the user has requested to exit.
        /// </summary>
        public abstract void PrepareShutdown();

        /// <summary>
        /// Does the exactly reverse of what <see cref="Initialize"/> does; destroys the engine instance. <br />
        /// Called by the <see cref="Dispose"/> method.
        /// </summary>
        /// <returns><see langword="true"/> when uninitialization succeeded; otherwise it failed and it must return <see langword="false"/>.</returns>
        protected abstract System.Boolean Uninitialize();

        /// <summary>
        /// Unloads the player backend , when an explicit request for shutdown has been done before. <br />
        /// Note: It is up to the implementer's responsibility to properly implement the stated logic.
        /// </summary>
        public void Dispose()
        {
            if (Uninitialize())
            {
                // Clear attribute store 
                attributes?.Clear();
                attributes = null;
                // Clean all SendCommand event references.
                foreach (var method in SendCommand.GetInvocationList()) 
                {
                    SendCommand -= method as SendCommandDelegate;
                }
                GC.SuppressFinalize(this);
            }
        }
    }
}