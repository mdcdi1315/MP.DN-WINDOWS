
using MP.ExtensibilitySystem;

namespace MP.ExtSystemApi
{
    public static class DefaultExtSystemRequests
    {
        /// <summary>
        /// Called to all extensions to provide a <see cref="MainMenuButtonRegistrationInfo"/> structure, <br />
        /// which it can be used for the extensions to register their own UI's.
        /// </summary>
        public const SystemRequestType RegisterMainMenuButton = (SystemRequestType)1;
        /// <summary>
        /// Once <see cref="RegisterMainMenuButton"/> has been dispatched, this request is called, and as it's 
        /// own argument it is passed the contents of <see cref="MainMenuButtonRegistrationInfo.Tag"/> field. <br />
        /// The stated field allows the particular extension that had registered the button to understand that it should return a <see cref="System.Windows.Forms.Form"/> object
        /// so that the app can later invoke and show the form to the user.
        /// </summary>
        public const SystemRequestType InvokeMainMenuButton = (SystemRequestType)2;
        /// <summary>
        /// Called to all extensions to provide a <see cref="MainMenuButtonRegistrationInfo"/> structure when the user
        /// right-clicks an audio track in order to show additional information that may be required.
        /// </summary>
        public const SystemRequestType RegisterRightClickOptionButton = (SystemRequestType)3;
        /// <summary>
        /// Once <see cref="RegisterRightClickOptionButton"/> has been dispatched, this request is called when an extension button is pressed, and as it's 
        /// own argument it is passed a <see cref="RightClickOptionButtonData"/> auto-filled by MP. <br />
        /// The stated field allows the particular extension that had registered the button to understand that it should return a <see cref="System.Windows.Forms.Form"/> object
        /// so that the app can later invoke and show the form to the user.
        /// </summary>
        public const SystemRequestType InvokeRightClickOptionButton = (SystemRequestType)4;
        /// <summary>
        /// Useful for registering codecs when a new player instance is created. <br />
        /// A <see cref="AbstractPropertyStream"/> is passed as the request argument, and it must be returned back a <see cref="CodecProbeRequestResult"/> object. <br />
        /// For Windows only, the <see cref="AbstractPropertyStream"/> object recieved also implements the <see cref="ComInterop.IStream"/> interface. <br />
        /// </summary>
        public const SystemRequestType GetAudioStream = (SystemRequestType)5;
        
    }
}