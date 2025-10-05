namespace MP.Graphics.Windowing.Input
{
    /// <summary>
    /// Defines a convenient mapping from keyboard buttons to key codes. <br />
    /// All input API's should translate values based on this enumeration members.
    /// </summary>
    public enum KeyboardKeyCode : System.Byte
    {
        /// <summary>
        /// Defines a dummy value, meaning that the value retrieved from the system cannot be mapped to one of this enumeration's members.
        /// </summary>
        Unmapped,
        /// <summary>Defines the ESC button.</summary>
        Escape,
        /// <summary>Defines the BACKSPACE button.</summary>
        BackSpace,
        /// <summary>Defines the ENTER button.</summary>
        Enter,
        /// <summary>Defines the INSERT button.</summary>
        Insert,
        /// <summary>Defines the DELETE button.</summary>
        Delete,
        /// <summary>Defines the TAB button.</summary>
        Tab,
        /// <summary>Defines the left SHIFT button.</summary>
        LeftShift,
        /// <summary>Defines the right SHIFT button.</summary>
        RightShift,
        /// <summary>Defines the left CONTROL button.</summary>
        LeftControl,
        /// <summary>Defines the right CONTROL button.</summary>
        RightControl,
        /// <summary>Defines the left ALT button.</summary>
        LeftAlt,
        /// <summary>Defines the right ALT button.</summary>
        RightAlt,
        /// <summary>Defines the HOME button.</summary>
        Home,
        /// <summary>Defines the END button.</summary>
        End,
        /// <summary>Defines the PAGE UP button.</summary>
        PageUp,
        /// <summary>Defines the PAGE DOWN button.</summary>
        PageDown,
        /// <summary>Defines the SPACE button.</summary>
        Space,
        /// <summary>Defines the PAUSE button.</summary>
        Pause,
        /// <summary>Defines the PRINT SCREEN button.</summary>
        PrintScreen,
        /// <summary>Defines the up arrow button.</summary>
        ArrowUp,
        /// <summary>Defines the down arrow button.</summary>
        ArrowDown,
        /// <summary>Defines the left arrow button.</summary>
        ArrowLeft,
        /// <summary>Defines the right arrow button.</summary>
        ArrowRight,
        /// <summary>Defines the numeric '1' button.</summary>
        One,
        /// <summary>Defines the numeric '2' button.</summary>
        Two,
        /// <summary>Defines the numeric '3' button.</summary>
        Three,
        /// <summary>Defines the numeric '4' button.</summary>
        Four,
        /// <summary>Defines the numeric '5' button.</summary>
        Five,
        /// <summary>Defines the numeric '6' button.</summary>
        Six,
        /// <summary>Defines the numeric '7' button.</summary>
        Seven,
        /// <summary>Defines the numeric '8' button.</summary>
        Eight,
        /// <summary>Defines the numeric '9' button.</summary>
        Nine,
        /// <summary>Defines the numeric '0' button.</summary>
        Zero,
        /// <summary>Defines the letter 'A' button.</summary>
        A,
        /// <summary>Defines the letter 'B' button.</summary>
        B,
        /// <summary>Defines the letter 'C' button.</summary>
        C,
        /// <summary>Defines the letter 'D' button.</summary>
        D,
        /// <summary>Defines the letter 'E' button.</summary>
        E,
        /// <summary>Defines the letter 'F' button.</summary>
        F,
        /// <summary>Defines the letter 'G' button.</summary>
        G,
        /// <summary>Defines the letter 'H' button.</summary>
        H,
        /// <summary>Defines the letter 'I' button.</summary>
        I,
        /// <summary>Defines the letter 'J' button.</summary>
        J,
        /// <summary>Defines the letter 'K' button.</summary>
        K,
        /// <summary>Defines the letter 'L' button.</summary>
        L,
        /// <summary>Defines the letter 'M' button.</summary>
        M,
        /// <summary>Defines the letter 'N' button.</summary>
        N,
        /// <summary>Defines the letter 'O' button.</summary>
        O,
        /// <summary>Defines the letter 'P' button.</summary>
        P,
        /// <summary>Defines the letter 'Q' button.</summary>
        Q,
        /// <summary>Defines the letter 'R' button.</summary>
        R,
        /// <summary>Defines the letter 'S' button.</summary>
        S,
        /// <summary>Defines the letter 'T' button.</summary>
        T,
        /// <summary>Defines the letter 'U' button.</summary>
        U,
        /// <summary>Defines the letter 'V' button.</summary>
        V,
        /// <summary>Defines the letter 'W' button.</summary>
        W,
        /// <summary>Defines the letter 'X' button.</summary>
        X,
        /// <summary>Defines the letter 'Y' button.</summary>
        Y,
        /// <summary>Defines the letter 'Z' button.</summary>
        Z,
        /// <summary>Defines the (') button.</summary>
        Apostrophe,
        /// <summary>Defines the (,) button.</summary>
        Comma,
        /// <summary>Defines the (-) button.</summary>
        Minus,
        /// <summary>Defines the (.) button.</summary>
        Period,
        /// <summary>Defines the (;) button.</summary>
        SemiColon,
        /// <summary>Defines the (=) button.</summary>
        Equal,
        /// <summary>Defines the ([) button.</summary>
        LeftBracket,
        /// <summary>Defines the (]) button.</summary>
        RightBracket,
        /// <summary>Defines the (\) button.</summary>
        BackSlash,
        /// <summary>Defines the (`) button.</summary>
        GraveAccent,
        /// <summary>Defines the FUNCTION 1 button.</summary>
        F1,
        /// <summary>Defines the FUNCTION 2 button.</summary>
        F2,
        /// <summary>Defines the FUNCTION 3 button.</summary>
        F3,
        /// <summary>Defines the FUNCTION 4 button.</summary>
        F4,
        /// <summary>Defines the FUNCTION 5 button.</summary>
        F5,
        /// <summary>Defines the FUNCTION 6 button.</summary>
        F6,
        /// <summary>Defines the FUNCTION 7 button.</summary>
        F7,
        /// <summary>Defines the FUNCTION 8 button.</summary>
        F8,
        /// <summary>Defines the FUNCTION 9 button.</summary>
        F9,
        /// <summary>Defines the FUNCTION 10 button.</summary>
        F10,
        /// <summary>Defines the FUNCTION 11 button.</summary>
        F11,
        /// <summary>Defines the FUNCTION 12 button.</summary>
        F12,
        /// <summary>Defines the FUNCTION 13 button.</summary>
        F13,
        /// <summary>Defines the FUNCTION 14 button.</summary>
        F14,
        /// <summary>Defines the FUNCTION 15 button.</summary>
        F15,
        /// <summary>Defines the FUNCTION 16 button.</summary>
        F16,
        /// <summary>Defines the FUNCTION 17 button.</summary>
        F17,
        /// <summary>Defines the FUNCTION 18 button.</summary>
        F18,
        /// <summary>Defines the FUNCTION 19 button.</summary>
        F19,
        /// <summary>Defines the FUNCTION 20 button.</summary>
        F20,
        /// <summary>Defines the FUNCTION 21 button.</summary>
        F21,
        /// <summary>Defines the FUNCTION 22 button.</summary>
        F22,
        /// <summary>Defines the FUNCTION 23 button.</summary>
        F23,
    }
}