
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

    public sealed class InvalidSettingClassDefinitionLayoutException : SettingsTreeBaseException
    {
        public InvalidSettingClassDefinitionLayoutException(System.String msg) : base(msg) { }
    }

    public sealed class NotASettingsTreeException : SettingsTreeBaseException
    {
        public NotASettingsTreeException(System.Type type) : base($"The class type {type.FullName} is not a valid Settings Tree definition because it does not define the SettingsTreeLayoutClassAttribute.") { }
    }

    public sealed class SettingNotFoundException : SettingsTreeBaseException
    {
        private System.String settingname;

        public SettingNotFoundException(System.String name) : base($"The specified setting is not part of the settings tree , or it was excluded during the build process. \nSetting Name: {name}") { settingname = name; }

        public System.String Name => settingname;
    }
}