
using MP.ComInterop;
using System.Runtime.InteropServices;

namespace MP.Dialogs
{
    public unsafe sealed class SaveFileDialog : FileDialog
    {
        public SaveFileDialog() : base(CommonInteropClsIds.CLSID_FileSaveDialog) { }

        protected override bool SpawnDialogSpecificImplementation(IFileDialog dialog, nint parenthandle)
        {
            var hr = dialog.Show(parenthandle);
            if (hr.SUCCEEDED)
            {
                System.IntPtr isli;
                hr = dialog.GetResult(&isli);
                if (hr.SUCCEEDED)
                {
                    IShellItem item = (IShellItem)Marshal.GetObjectForIUnknown(isli);
                    System.Char* dpname;
                    hr = item.GetDisplayName(SIGDN.SIGDN_DESKTOPABSOLUTEPARSING, &dpname);
                    if (hr.FAILED)
                    {
                        Marshal.ReleaseComObject(item);
                        throw hr.MappingException;
                    }
                    retpaths[0] = new System.String(dpname);
                    Interop.Ole32.CoTaskMemFree(dpname);
                    Marshal.ReleaseComObject(item);
                    return true;
                } else {
                    return false;
                }
            } else {
                return false;
            }
        }
    }
}