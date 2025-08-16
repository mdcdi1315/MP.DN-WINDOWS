
using System;

namespace MP.SettingsTree
{
    /// <summary>
    /// Defines the base exception for all the Settings Tree exceptions. <br />
    /// You should not derive from this class; it is only intended for the Settings Tree infrastracture.
    /// </summary>
    public abstract class SettingsTreeBaseException : ExceptionSystem.BaseException
    {
        /// <summary>
        /// Creates a default instance of the <see cref="SettingsTreeBaseException"/> class.
        /// </summary>
        public SettingsTreeBaseException() : base() { }

        /// <summary>
        /// Creates a new instance of the <see cref="SettingsTreeBaseException"/> class and with the specified 
        /// detailed error message.
        /// </summary>
        /// <param name="message">The error message to show.</param>
        public SettingsTreeBaseException(System.String message) : base(message) { }

        /// <summary>
        /// Creates a new instance of the <see cref="SettingsTreeBaseException"/> class, with the specified 
        /// detailed error message and the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message to show.</param>
        /// <param name="innerException">The exception that is the cause of this exception.</param>
        public SettingsTreeBaseException(System.String message, Exception innerException) : base(message, innerException) { }
    }

    /// <summary>
    /// The exception that is thrown when an invalid layout of a Settings Tree class was encountered.
    /// </summary>
    public class InvalidSettingClassDefinitionLayoutException : SettingsTreeBaseException
    {
        /// <summary>
        /// Creates a new instance of the <see cref="InvalidSettingClassDefinitionLayoutException"/>, 
        /// specifying the specific reason why this exception instance is created.
        /// </summary>
        /// <param name="msg">The message specifying the specific reason why this exception is created.</param>
        public InvalidSettingClassDefinitionLayoutException(System.String msg) : base(msg) { }

        /// <summary>
        /// Creates a new instance of the <see cref="InvalidSettingClassDefinitionLayoutException"/>, 
        /// specifying the specific reason why this exception instance is created, and the exception that caused this exception to be thrown.
        /// </summary>
        /// <param name="msg">The message specifying the specific reason why this exception is created.</param>
        /// <param name="inner">The culprit exception for this exception to be thrown.</param>
        public InvalidSettingClassDefinitionLayoutException(System.String msg, Exception inner) : base(msg, inner) { }
    }

    /// <summary>
    /// The exception that is thrown when the object's class is not decorated with the <see cref="SettingsTreeLayoutClassAttribute"/>.
    /// </summary>
    public sealed class NotASettingsTreeException : InvalidSettingClassDefinitionLayoutException
    {
        /// <summary>
        /// Creates a new instance of the <see cref="NotASettingsTreeException"/>, specifying the type of the object
        /// that caused this exception to be created.
        /// </summary>
        /// <param name="type">The type of the object not decorated with the <see cref="SettingsTreeLayoutClassAttribute"/>.</param>
        public NotASettingsTreeException(Type type) : base($"The class type {type.FullName} is not a valid Settings Tree definition because it does not define the SettingsTreeLayoutClassAttribute.") { }
    }

    /// <summary>
    /// The exception that is thrown when requesting a setting that does not exist.
    /// </summary>
    public sealed class SettingNotFoundException : SettingsTreeBaseException
    {
        private System.String settingname;

        /// <summary>
        /// Creates a new instance of the <see cref="SettingNotFoundException"/> class,
        /// specifying the ID of the setting that was not found.
        /// </summary>
        /// <param name="name">The ID of the setting that was not found.</param>
        public SettingNotFoundException(System.String name) : base($"The specified setting is not part of the settings tree , or it was excluded during the build process. \nSetting Name: {name}") { settingname = name; }

        /// <summary>
        /// The ID of the setting that was not found.
        /// </summary>
        public System.String Name => settingname;
    }

    /// <summary>
    /// The exception that is thrown when attempting to retrieve a resource via the <see cref="IResourcesProvider"/>
    /// interface, but that resource was not found.
    /// </summary>
    public sealed class ResourceNotFoundException : SettingsTreeBaseException
    {
        private System.String rn;

        /// <summary>
        /// Constructs a new instance of the <see cref="ResourceNotFoundException"/> class with the specified resource that was not found.
        /// </summary>
        /// <param name="resourcename">The name of the resource that was not found.</param>
        public ResourceNotFoundException(System.String resourcename)
            : base($"The specified resource was not found.\nResource name: {resourcename}")
                => rn = resourcename;

        /// <summary>
        /// Gets the name of the resource that was not found.
        /// </summary>
        public System.String ResourceName => rn;
    }
}