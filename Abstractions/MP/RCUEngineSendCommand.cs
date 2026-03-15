
using System;

namespace MP
{
    /// <summary>
    /// Defines common send command types. More can be defined as constants in a static class by the user.
    /// </summary>
    public enum CommonSendCommandTypes : System.Byte
    {
        /// <summary>
        /// This field should only and only be sent when the unload operation has begun.
        /// This instructs the player UI to not listen to any new events.
        /// (Effectively detaching it from the <see cref="RCUEngineV2.SendCommand"/> event).
        /// </summary>
        IgnoreFutureRequests = 0,
        /// <summary>Optional event to refresh the player UI.</summary>
        Refresh,
        /// <summary>A hard failure was occured by the previously recieved event <see cref="CommonRecieveCommandTypes.HardFailAppMustClose"/>. This exists so that it can be re-routed to reach the target.</summary>
        HardFailGracefulExitRequested,
        /// <summary>Clears the player screen if applicable.</summary>
        ClearPlayerScreen,
        /// <summary>Loads the player screen if applicable.</summary>
        LoadPlayerScreen,
        /// <summary>Clears the exploration screen if applicable.</summary>
        ClearExplorationScreen,
        /// <summary>Loads the exploration screen if applicable.</summary>
        LoadExplorationScreen,
        /// <summary>Prepares the target for loading a player instance.</summary>
        AttachPlayerEvents,
        /// <summary>Unloads a previously loaded player instance.</summary>
        DetachPlayerEvents,
        /// <summary>Throws an error or informational message to the user directly.</summary>
        ThrowMessage,
        /// <summary>Deprecated , will be removed in a subsequent release</summary>
        [Annotations.DeprecatedMayBeRemoved("1.0.1.0")]
        ThrowTitleMessage, // Deprecated, use instead the ThrowWaitMessage command.
        /// <summary>Deprecated , will be removed in a subsequent release</summary>
        [Annotations.DeprecatedMayBeRemoved("1.0.1.0")]
        ClearTitleMessage, // Deprecated, use instead the ClearWaitMessage command.
        /// <summary>Updates the selected index of the current track.</summary>
        UpdateSelectedIndex,
        /// <summary>
        /// This is a controller command that must be executed on the behalf of the UI. <br />
        /// The specific command to execute is sent to MessageData string.
        /// </summary>
        ControllerCommand,
        /// <summary>
        /// Spawns and displays a window wait message.
        /// </summary>
        ThrowWaitMessage,
        /// <summary>
        /// Clears the window presented with <see cref="ThrowWaitMessage"/>.
        /// </summary>
        ClearWaitMessage,
    }

    /// <summary>
    /// Sends command data when a command leaves the RCU Engine.
    /// </summary>
    public sealed class SendCommandDataEventArgs : EventArgs
    {
        /// <summary>
        /// The specific command type sent for the event.
        /// </summary>
        public CommonSendCommandTypes CommandType;
        /// <summary>
        /// Any message data that must be carried on.
        /// </summary>
        public System.String MessageData;
        /// <summary>
        /// Additional metadata that must be passed to the call.
        /// </summary>
        public CommandMetadata Metadata;

        /// <summary>
        /// Creates an empty send command event arguments class initialized with the specified command type to be sent.
        /// </summary>
        /// <param name="commandType">The command type to be sent to the target.</param>
        public SendCommandDataEventArgs(CommonSendCommandTypes commandType)
        {
            CommandType = commandType;
            MessageData = System.String.Empty;
            Metadata = null;
        }
    }

    /// <summary>
    /// Defines the event delegate function signature.
    /// </summary>
    /// <param name="sender">The engine that the command was sent through.</param>
    /// <param name="e">The additional command data to send.</param>
    public delegate void SendCommandDelegate(RCUEngineV2 sender, SendCommandDataEventArgs e);
}