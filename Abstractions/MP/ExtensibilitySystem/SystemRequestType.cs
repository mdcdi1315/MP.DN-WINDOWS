

namespace MP.ExtensibilitySystem
{
    /// <summary>
    /// When a request is performed on the extensibility engine, the engine must call
    /// all the loaded extensions to perform the request. 
    /// This empty enumeration should be extended by the app authors to include custom requests that should happen.
    /// </summary>
    public enum SystemRequestType : System.UInt32
    {
        /// <summary>Reserved value. When this is passed to the engine, the engine should not take any action.</summary>
        None
    }
}
