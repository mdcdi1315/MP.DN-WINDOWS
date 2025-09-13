
using MP.ComInterop;
using System.Runtime.InteropServices;

namespace MP.Dialogs
{
    public unsafe sealed class OpenFolderDialog : FileDialog
    {
        public OpenFolderDialog() : base(CommonInteropClsIds.CLSID_FileOpenDialog , true) { }

        protected override bool SpawnDialogSpecificImplementation(IFileDialog dialog, nint parenthandle)
        {
            IFileOpenDialog opendlg = dialog as IFileOpenDialog;
            FILEOPENDIALOGOPTIONS opts;
            dialog.GetOptions(&opts);
            var hr = dialog.Show(parenthandle);
            if (hr.SUCCEEDED)
            {
                if (opts.HasFlag(FILEOPENDIALOGOPTIONS.FOS_ALLOWMULTISELECT))
                {
                    System.IntPtr ilst;
                    hr = opendlg.GetResults(&ilst);
                    IShellItemArray array = (IShellItemArray)Marshal.GetObjectForIUnknown(ilst);
                    System.UInt32 items;
                    if (array.GetCount(&items).FAILED) { return false; }
                    retpaths = new System.String[items];
                    System.IntPtr islp;
                    IShellItem item;
                    for (System.UInt32 I = 0; I < items; I++)
                    {
                        array.GetItemAt(I, &islp);
                        item = (IShellItem)Marshal.GetObjectForIUnknown(islp);
                        System.Char* dpname;
                        hr = item.GetDisplayName(SIGDN.SIGDN_DESKTOPABSOLUTEPARSING, &dpname);
                        if (hr.FAILED)
                        {
                            Marshal.ReleaseComObject(item);
                            Marshal.ReleaseComObject(array);
                            throw hr.CreateException();
                        }
                        retpaths[I] = new System.String(dpname);
                        Interop.Ole32.CoTaskMemFree(dpname);
                        Marshal.ReleaseComObject(item);
                    }
                    Marshal.ReleaseComObject(array);
                    return true;
                }
                else
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
                            throw hr.CreateException();
                        }
                        retpaths[0] = new System.String(dpname);
                        Interop.Ole32.CoTaskMemFree(dpname);
                        Marshal.ReleaseComObject(item);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
        }
    }
}