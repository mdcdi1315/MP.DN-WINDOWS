
using System;
using MP.ComInterop;
using System.Windows.Forms;
using System.Collections.Generic;
using Microsoft.Win32.SafeHandles;

namespace MP.Dialogs
{
    /// <summary>
    /// Defines a file filter for all the MusicPlayer-defined file dialogs.
    /// </summary>
    public struct FileDialogFilter
    {
        public System.String FilterPattern;

        public System.String FilterDisplay;

        public FileDialogFilter(System.String pattern , System.String display)
        {
            FilterPattern = pattern; FilterDisplay = display;
        }

        public static FileDialogFilter GetFromWin32Filter(System.String filter)
        {
            if (System.String.IsNullOrEmpty(filter)) { throw new ArgumentNullException(nameof(filter)); }
            FileDialogFilter fltret = new();
            System.String[] ftokens = filter.Split('|');
            if (ftokens.Length != 2) { throw new ArgumentException("There is not a pipe character or are too many pipe characters on the string."); }
            fltret.FilterPattern = ftokens[1];
            fltret.FilterDisplay = ftokens[0];
            return fltret;
        }
    }

    /// <summary>
    /// Defines the base implementation class for all the MusicPlayer-defined file dialogs.
    /// </summary>
    public abstract class FileDialog
    {
        // While we would need more than 4 bytes to hold for the defined Boolean properties,
        // another 1 to hold the dialog result and another 1 to hold the dialog type , 
        // I use a bit flag field to reference all the above and using only 2 bytes for saving all these,
        // and have still space to reference another 6 boolean values.
        [Flags]
        private enum MPFileDialogFlags : System.UInt16
        {
            None = 0,
            Type_OpenFileDialog = 1,
            Type_SaveFileDialog = 2,
            Type_OpenFolderDialog = 4,
            // Reserve 8 and 16 for future file dialog types.
            Options_NoValidate = 32,
            Options_PathMustExist = 64,
            Options_FileMustExist = 128,
            Options_AllowMultiSelect = 256,
            // Bit data 512..4096 are unused.
            Dialog_ResultOk = 8192,
            Dialog_ResultCancel = 16384,
            Dialog_ExecutionCompleted = 32768
        }

        private System.Int32 fltidx;
        private MPFileDialogFlags flags;
        protected System.String[] retpaths;
        private List<FileDialogFilter> filters;
        private System.String startingfolderpath , title , defaultext;

        private FileDialog()
        {
            // Do not set any flags these will be done by the ctors to determine correct dialog
            flags = MPFileDialogFlags.None;
            retpaths = new System.String[1];
            filters = new(4);
            fltidx = 0;
            startingfolderpath = title = defaultext = null;
        }

        protected FileDialog(System.String guid) : this(guid , false) { }

        protected FileDialog(System.String guid , System.Boolean useopenfolderdialoglogic) : this()
        {
            if (guid == CommonInteropClsIds.CLSID_FileOpenDialog) {
                if (useopenfolderdialoglogic) {
                    flags = MPFileDialogFlags.Type_OpenFolderDialog;
                } else {
                    flags = MPFileDialogFlags.Type_OpenFileDialog;
                }
            } else if (guid == CommonInteropClsIds.CLSID_FileSaveDialog) {
                flags = MPFileDialogFlags.Type_SaveFileDialog;
            } else {
                throw new ArgumentException("No other GUID's are supported for now.");
            }
        }

        public void AddFilter(FileDialogFilter filter) => filters.Add(filter);

        public void ClearFilters() => filters.Clear();

        public System.Int32 FilterCount => filters.Count;

        public System.Int32 CurrentFilterIndex
        {
            get => fltidx;
            set {
                if (value < 0) { throw new ArgumentOutOfRangeException(nameof(CurrentFilterIndex), "Index must be a positive or zero value."); }
                if (value > filters.Count) { throw new ArgumentOutOfRangeException(nameof(CurrentFilterIndex), "Index was out of the filter list bounds."); }
                fltidx = value;
            }
        }

        public System.String Title
        {
            get => title;
            set => title = value;
        }

        public System.String StartupFolder
        {
            get => startingfolderpath;
            set => startingfolderpath = value;
        }

        public System.String DefaultFilterExtension
        {
            get => defaultext;
            set => defaultext = value;
        }

        public System.Boolean PerformNoValidation
        {
            get => flags.HasFlag(MPFileDialogFlags.Options_NoValidate);
            set {
                if (value) { flags |= MPFileDialogFlags.Options_NoValidate;  }
                else { flags &= ~MPFileDialogFlags.Options_NoValidate; }
            }
        }

        public System.Boolean CheckPath
        {
            get => flags.HasFlag(MPFileDialogFlags.Options_PathMustExist);
            set
            {
                if (value) { flags |= MPFileDialogFlags.Options_PathMustExist; }
                else { flags &= ~MPFileDialogFlags.Options_PathMustExist; }
            }
        }

        public System.Boolean CheckFilePath
        {
            get => flags.HasFlag(MPFileDialogFlags.Options_FileMustExist);
            set
            {
                if (value) { flags |= MPFileDialogFlags.Options_FileMustExist; }
                else { flags &= ~MPFileDialogFlags.Options_FileMustExist; }
            }
        }

        public System.Boolean MultiSelect
        {
            get => flags.HasFlag(MPFileDialogFlags.Options_AllowMultiSelect);
            set
            {
                if (value) { flags |= MPFileDialogFlags.Options_AllowMultiSelect; }
                else { flags &= ~MPFileDialogFlags.Options_AllowMultiSelect; }
            }
        }

        public System.String[] FilePaths => retpaths;

        /// <summary>
        /// Adds dialog filters to the current instance. 
        /// Each filter definition is seperated from each other by using newlines.
        /// </summary>
        /// <param name="str">The filters to add to the dialog.</param>
        /// <exception cref="ArgumentNullException"><paramref name="str"/> parameter was null or represented the empty string.</exception>
        public void AddMultipleFiltersFromString(System.String str)
        {
            if (System.String.IsNullOrEmpty(str)) { throw new ArgumentNullException(nameof(str)); }
            System.String[] filters = str.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries);
            foreach (var flt in filters) 
            {
                this.filters.Add(FileDialogFilter.GetFromWin32Filter(flt));
            }
        }

        /// <summary>
        /// Override this method in your custom code to provide the creation, shutdown and save of the 
        /// selected dialog items.
        /// </summary>
        /// <param name="dialog"> 
        /// The native COM object of this instance. <br />
        /// Cast it appropriately to an <see cref="IFileOpenDialog"/> or <see cref="IFileSaveDialog"/> interface instance. <br />
        /// You <strong>MUST NOT</strong> free the instance returned by this parameter. Otherwise an <see cref="AccessViolationException"/> may occur.
        /// </param>
        /// <param name="parenthandle">The parent window handle. Can also be 0.</param>
        /// <returns><see langword="true"/> when you have processed the data successfully; otherwise <see langword="false"/> to indicate cancellation or failure.</returns>
        protected abstract System.Boolean SpawnDialogSpecificImplementation(IFileDialog dialog, System.IntPtr parenthandle);

        /// <summary>
        /// Spawns a new file dialog specified by the window that it will act as the parent for the current dialog instance.
        /// </summary>
        /// <param name="window">The window that it will act as the parent for this dialog. Can also be <see langword="null"/>.</param>
        /// <returns>A value whether at least a file or a folder has been successfully selected.</returns>
        public System.Boolean SpawnDialog(IWin32Window window)
            => SpawnDialog(window is null ? IntPtr.Zero : window.Handle);

        /// <summary>
        /// Spawns a new file dialog specified by the window handle that will act as the parent for the current dialog instance.
        /// </summary>
        /// <param name="hwndparent">The HWND value of the parent window. Can also be <see cref="IntPtr.Zero"/>.</param>
        /// <returns>A value whether at least a file or a folder has been successfully selected.</returns>
        public System.Boolean SpawnDialog(System.IntPtr hwndparent = 0)
        {
            if (flags.HasFlag(MPFileDialogFlags.Dialog_ExecutionCompleted)) { return true; }
            System.Threading.Thread td = new(SpawnDialog_ThreadCode);
            td.TrySetApartmentState(System.Threading.ApartmentState.STA);
            td.Name = InternalResources.MP_DIALOGS_FILEDIALOGABSTRACT_THREADNAME;
            td.Start(hwndparent); // Must pass the HWND of the parent thru the thread
            td.Join(); // wait until the thread has finished
            return flags.HasFlag(MPFileDialogFlags.Dialog_ResultOk);
        }

        // Thread code to prepare the dialog and then call the overriden SpawnDialogSpecificImplementation method.
        private unsafe void SpawnDialog_ThreadCode(System.Object objhandle)
        {
            IFileDialog dialog = null;
            if (flags.HasFlag(MPFileDialogFlags.Type_OpenFileDialog) ||
                flags.HasFlag(MPFileDialogFlags.Type_OpenFolderDialog))
            {
                dialog = ComMarshalling.GetClassInstanceAsInterface<IFileOpenDialog>(new(CommonInteropClsIds.CLSID_FileOpenDialog), CLSCTX.CLSCTX_ALL);
            } else if (flags.HasFlag(MPFileDialogFlags.Type_SaveFileDialog))
            {
                dialog = ComMarshalling.GetClassInstanceAsInterface<IFileSaveDialog>(new(CommonInteropClsIds.CLSID_FileSaveDialog), CLSCTX.CLSCTX_ALL);
            }
            FILEOPENDIALOGOPTIONS opts = FILEOPENDIALOGOPTIONS.None;
            if (flags.HasFlag(MPFileDialogFlags.Type_OpenFolderDialog))
            {
                // Both flags must have been set so that an Open Folder dialog can succeed.
                opts |= FILEOPENDIALOGOPTIONS.FOS_PICKFOLDERS | FILEOPENDIALOGOPTIONS.FOS_FILEMUSTEXIST;
            }
            if (flags.HasFlag(MPFileDialogFlags.Options_NoValidate))
            {
                opts |= FILEOPENDIALOGOPTIONS.FOS_NOVALIDATE;
            }
            if (flags.HasFlag(MPFileDialogFlags.Options_AllowMultiSelect))
            {
                opts |= FILEOPENDIALOGOPTIONS.FOS_ALLOWMULTISELECT;
            }
            if (flags.HasFlag(MPFileDialogFlags.Options_PathMustExist))
            {
                opts |= FILEOPENDIALOGOPTIONS.FOS_PATHMUSTEXIST;
            }
            if (flags.HasFlag(MPFileDialogFlags.Options_FileMustExist))
            {
                opts |= FILEOPENDIALOGOPTIONS.FOS_FILEMUSTEXIST;
            }
            dialog.SetOptions(opts);
            dialog.SetFileTypeIndex(fltidx.ToUInt32());
            if (title is not null)
            {
                fixed (System.Char* ptitle = title)
                    dialog.SetTitle(ptitle);
            }
            if (defaultext is not null)
            {
                fixed (System.Char* pdefext = defaultext)
                    dialog.SetDefaultExtension(pdefext);
            }
            var hrt = Interop.Shell32.SHCreateItemFromParsingName(startingfolderpath, Interop.GUID.FromString(CommonInteropClsIds.IID_IShellItem), out var nativeinterface);
            if (hrt.SUCCEEDED) {
                dialog.SetFolder(nativeinterface);
                ComMarshalling.Release(nativeinterface);
                nativeinterface = null;
            }
            COMDLG_FILTERSPEC[] filters = new COMDLG_FILTERSPEC[this.filters.Count];
            List<SafeLibcMemoryHandle> memhandles = new(filters.Length * 2);
            for (System.Int32 I = 0; I < filters.Length; I++) 
            {
                filters[I] = new();
                SafeLibcMemoryHandle temp = this.filters[I].FilterPattern.ToNativeUnicodeString();
                filters[I].Filter = (System.Char*)temp.MemoryPointer;
                memhandles.Add(temp);
                temp = null;
                temp = this.filters[I].FilterDisplay.ToNativeUnicodeString();
                filters[I].FriendlyName = (System.Char*)temp.MemoryPointer;
                memhandles.Add(temp);
                temp = null;
            }
            if (filters.Length > 0) {
                fixed (COMDLG_FILTERSPEC* specs = filters)
                {
                    var hr = dialog.SetFileTypes(filters.Length.ToUInt32(), specs);
                    if (hr.FAILED) {
                        ComMarshalling.ReleaseInteropObject(dialog);
                        foreach (var smem in memhandles)
                        {
                            smem.Dispose();
                        }
                        memhandles.Clear();
                        throw hr.MappingException;
                    }
                }
                filters = null;
            }
            try {
                if (SpawnDialogSpecificImplementation(dialog, (System.IntPtr)objhandle)) {
                    flags |= MPFileDialogFlags.Dialog_ResultOk;
                } else {
                    flags |= MPFileDialogFlags.Dialog_ResultCancel;
                }
                flags |= MPFileDialogFlags.Dialog_ExecutionCompleted;
            } catch {
                // First release the dialog object, then throw the internal exception found
                ComMarshalling.ReleaseInteropObject(dialog);
                throw;
            } finally {
                flags |= MPFileDialogFlags.Dialog_ExecutionCompleted;
                foreach (var smem in memhandles)
                {
                    smem.Dispose();
                }
                memhandles.Clear();
            }
            ComMarshalling.ReleaseInteropObject(dialog);
        }
    }
}
