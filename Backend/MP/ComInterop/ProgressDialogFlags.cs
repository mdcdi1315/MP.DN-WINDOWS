


using System;

namespace MP.ComInterop
{
    [Flags]
    public enum ProgressDialogFlags : System.UInt32
    {
        /// <summary>default normal progress dlg behavior</summary>
        PROGDLG_NORMAL = 0x00000000,
        /// <summary>the dialog is modal to its hwndParent (default is modeless)</summary>
        PROGDLG_MODAL = 0x00000001,
        /// <summary>automatically updates the "Line3" text with the "time remaining" (you cant call SetLine3 if you pass this!)</summary>
        PROGDLG_AUTOTIME = 0x00000002,
        /// <summary>we dont show the "time remaining" if this is set. We need this if dwTotal < dwCompleted for sparse files</summary>
        PROGDLG_NOTIME = 0x00000004,
        /// <summary>Do not have a minimize button in the caption bar.</summary>
        PROGDLG_NOMINIMIZE = 0x00000008,
        /// <summary>Don't display the progress bar</summary>
        PROGDLG_NOPROGRESSBAR = 0x00000010,
        /// <summary>Use marquee progress (comctl32 v6 required)</summary>
        PROGDLG_MARQUEEPROGRESS = 0x00000020,
        /// <summary>No cancel button (operation cannot be canceled) (use sparingly)</summary>
        PROGDLG_NOCANCEL = 0x00000040
    }
}