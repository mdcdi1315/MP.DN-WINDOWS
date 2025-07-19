
using MP.ExceptionSystem;
using System.Diagnostics.CodeAnalysis;

namespace MP.ExtensibilitySystem
{
    /// <summary>
    /// Defines the base class where all extensibility modules must derive from so that the engine can send messages to them.
    /// </summary>
    public abstract class ExtensibilitySystemExtension
    {
        /// <summary>
        /// A custom field to indicate the last exception occured by <see cref="GetService"/>. <br />
        /// If no exceptions occured on the last <see cref="GetService"/> call, then this should be <see langword="null"/>.
        /// </summary>
        public BaseException LastException;

        /// <summary>
        /// Contains a reference to the current extensibility engine settings object. <br />
        /// Through this reference, the extensions can specify settings that alter their behavior. <br />
        /// This is filled in by the engine itself; it cannot be modified during the object's lifetime.
        /// </summary>
        public readonly IAttributeable Settings;

        /// <summary>
        /// Constructs a new <see cref="ExtensibilitySystemExtension"/> class instance,
        /// with the specified settings object that contains the settings defined for the current object.
        /// </summary>
        /// <param name="extsettings">The settings object reference to pass to this extension.</param>
        /// <exception cref="System.ArgumentNullException">The settings object reference was <see langword="null"/>.</exception>
        protected ExtensibilitySystemExtension(IAttributeable extsettings)
        {
            if (extsettings is null) {
                throw new System.ArgumentNullException(nameof(extsettings));
            }
            Settings = extsettings;
        }

        /// <summary>
        /// Called by the engine when it is ready to load this extension. <br />
        /// In your code, you must provide initialization tasks and register your required extensions to the engine. 
        /// </summary>
        /// <remarks>
        /// This code should throw exceptions on failed initialization tasks. <br />
        /// This allows the engine, and finally, the developer to understand what is going wrong.
        /// </remarks>
        public abstract void OnLoad();

        /// <summary>
        /// Called by the engine when it is ready to unload this extension. <br />
        /// In this stage you must destroy the extension and free all the internal state that is holding.
        /// </summary>
        /// <remarks>
        /// This code should throw exceptions on failed uninitialization tasks. <br />
        /// This allows the engine, and finally, the developer to understand what is going wrong.
        /// </remarks>
        public abstract void OnUnload();

        /// <summary>
        /// Called by the extensibility engine when a request is performed. <br />
        /// This is and the primary way of communicating with the app in general. <br />
        /// The engine provides the request and any additional data required for the request,
        /// and the extension should return through the <paramref name="service"/> parameter the requested service object. <br />
        /// If the extension cannot handle the particular request, it should return <see langword="false"/>.
        /// </summary>
        /// <param name="type">The request type that the engine expects this extension should handle</param>
        /// <param name="data">Additional data that might be required so that the request can execute.</param>
        /// <param name="service">The object that is associated with the request and it is it's result.</param>
        /// <returns>A value whether the current extension can handle the request provided via the <paramref name="type"/> parameter.</returns>
        /// <remarks>
        /// This code <strong>SHOULD NOT THROW EXCEPTIONS</strong>. <br />
        /// You should instead report exceptions regarding this request through the field <see cref="LastException"/>, and the method
        /// should return <see langword="false"/>. <br />
        /// Note that, if it returns <see langword="false"/> and the <see cref="LastException"/> is <see langword="null"/>, then the extension just cannot handle this request.
        /// </remarks>
        public abstract System.Boolean GetService(SystemRequestType type, System.Object data , [NotNullWhen(true)] out System.Object service);

        /// <summary>
        /// Called by the extensibility engine to determine whether this extension can be safely loaded. <br />
        /// The engine must pass in the versioning information, plus it must assure that those information are not null in any way.
        /// </summary>
        /// <param name="verinfo">The versioning information that the extension should test to find compatibility.</param>
        /// <returns><see langword="true"/> when this extension is supported by the provided versioning information; otherwise it should return <see langword="false"/>,</returns>
        public abstract System.Boolean IsSupported([NotNull] EngineVersioningInformation verinfo);
    }
}