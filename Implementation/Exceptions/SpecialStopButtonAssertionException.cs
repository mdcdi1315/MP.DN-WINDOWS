
// Keep this into the MP namespace for compatibility.
// Sometime the base namespace of this exception should be MP.ExceptionSystem.

namespace MP
{
    /// <summary>
    /// When the player requests that the repeat or any else action that depends on the stop events 
    /// should not be called , all stop event handlers that detect this exception must return immediately and not execute any code.
    /// </summary>
    internal sealed class SpecialStopButtonAssertionException : System.Exception
    {
        public SpecialStopButtonAssertionException() : base() { }
    }
}