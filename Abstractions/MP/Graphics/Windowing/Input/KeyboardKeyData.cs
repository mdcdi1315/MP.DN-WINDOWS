
namespace MP.Graphics.Windowing.Input
{
    /// <summary>
    /// Provides information about a pressed key in the keyboard.
    /// </summary>
    public readonly struct KeyboardKeyData
    {
        /// <summary>
        /// Defines the keyboard key that had it's state changed.
        /// </summary>
        public readonly KeyboardKeyCode Code;

        /// <summary>Defines the state of the key.</summary>
        public readonly KeyState State;

        /// <summary>
        /// Defines the Unicode key mapping for the key pressed in the keyboard, if any. <br />
        /// If not any the \u0000 character should be returned from this field.
        /// </summary>
        public readonly char Key;

        /// <summary>
        /// Constructs a new <see cref="KeyboardKeyData"/> structure from the specified key code and key state.
        /// </summary>
        /// <param name="code">The key code that it's state was changed.</param>
        /// <param name="state">The state that the key specified in <paramref name="code"/> was entered into.</param>
        public KeyboardKeyData(KeyboardKeyCode code, KeyState state)
        {
            Key = '\0';
            Code = code;
            State = state;
        }

        /// <summary>
        /// Constructs a new <see cref="KeyboardKeyData"/> structure from the specified key code and key state.
        /// </summary>
        /// <param name="code">The key code that it's state was changed.</param>
        /// <param name="state">The state that the key specified in <paramref name="code"/> was entered into.</param>
        /// <param name="key">The character corresponding to the key pressed for the currently selected language, if any.</param>
        public KeyboardKeyData(KeyboardKeyCode code, KeyState state, char key)
        {
            Key = key;
            Code = code;
            State = state;
        }
    }
}