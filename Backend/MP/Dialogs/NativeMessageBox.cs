
namespace MP.Dialogs
{
    public static class NativeMessageBox
    {
        public static ButtonReturned Show(System.String text , System.String title , ButtonSelection buttons , IconSelection icon)
            => Show(Interop.User32.GetForegroundWindow(), text , title, buttons , icon);

        public static ButtonReturned Show(System.IntPtr parent , System.String text , System.String title , ButtonSelection buttons , IconSelection icon)
        {
            Interop.User32.MessageBoxFlags flg = Interop.User32.MessageBoxFlags.MB_OK;
            switch (buttons)
            {
                case ButtonSelection.OK:
                    flg |= Interop.User32.MessageBoxFlags.MB_OK;
                    break;
                case ButtonSelection.YesNo:
                    flg |= Interop.User32.MessageBoxFlags.MB_YESNO;
                    break;
                case ButtonSelection.OKCancel:
                    flg |= Interop.User32.MessageBoxFlags.MB_OKCANCEL;
                    break;
                case ButtonSelection.YesNoCancel:
                    flg |= Interop.User32.MessageBoxFlags.MB_YESNOCANCEL;
                    break;
                case ButtonSelection.RetryCancel:
                    flg |= Interop.User32.MessageBoxFlags.MB_RETRYCANCEL;
                    break;
                default:
                    throw new System.ArgumentException($"Invalid option for the buttons selection: {buttons}");
            }
            switch (icon)
            {
                case IconSelection.Question:
                    flg |= Interop.User32.MessageBoxFlags.MB_ICONQUESTION;
                    break;
                case IconSelection.Info:
                case IconSelection.Info2:
                    flg |= Interop.User32.MessageBoxFlags.MB_ICONINFORMATION;
                    break;
                case IconSelection.Warning:
                    flg |= Interop.User32.MessageBoxFlags.MB_ICONWARNING;
                    break;
                case IconSelection.Error:
                    flg |= Interop.User32.MessageBoxFlags.MB_ICONHAND;
                    break;
                case IconSelection.None:
                    break;
                default:
                    throw new System.ArgumentException($"Invalid option for the icon selection {icon}");
            }
            Interop.User32.MessageBoxButton ret = Interop.User32.MessageBox(parent, text, title, flg);
            return ret switch {
                Interop.User32.MessageBoxButton.IDOK => ButtonReturned.OK,
                Interop.User32.MessageBoxButton.IDCANCEL => ButtonReturned.Cancel,
                Interop.User32.MessageBoxButton.IDABORT => ButtonReturned.Abort,
                Interop.User32.MessageBoxButton.IDRETRY => ButtonReturned.Retry,
                Interop.User32.MessageBoxButton.IDYES => ButtonReturned.Yes,
                Interop.User32.MessageBoxButton.IDNO => ButtonReturned.No,
                Interop.User32.MessageBoxButton.Error => throw new ExceptionSystem.NativeWindowsException(),
                _ => ButtonReturned.None,
            };
        }
    
        public static ButtonReturned Show(System.Windows.Forms.IWin32Window parent , System.String text , System.String title , ButtonSelection buttons , IconSelection icon)
            => Show(parent.Handle, text, title, buttons, icon);
    }
}