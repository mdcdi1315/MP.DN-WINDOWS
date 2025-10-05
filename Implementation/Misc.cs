using System;
using System.Drawing;
using MP.Graphics.Imaging;
using MP.GamepadBackend;
using MP.AudioLibrary.MMDevice;
using System.Collections.Generic;
using DotNetResourcesExtensions;
using MP.Graphics.Imaging.WindowsBitmap;

namespace MP
{
    public static class MusicPlayerHelper
    {
        private static System.IntPtr mainwinhandle;

        private static System.IntPtr HandleInit()
            => new System.Windows.Interop.WindowInteropHelper(MusicPlayer.Entry.Window).Handle;

        public static void InitializeMainWindowHandle()
        {
            mainwinhandle = MusicPlayer.Entry.Window.Dispatcher.Invoke(new Func<System.IntPtr>(HandleInit));
        }

        // Wraps a given string description and wraps it so as to be visible in a window.
        public static System.String WrapStringBy(System.String str, System.Int32 wraplen)
        {
            if (System.String.IsNullOrEmpty(str)) { return System.String.Empty; }
            if (wraplen <= 0) { throw new ArgumentOutOfRangeException(nameof(wraplen), "Wrap length must be a positive number."); }
            System.Text.StringBuilder sb = new(str.Length);
            foreach (var s in str.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries))
            {
                if (s.Length > wraplen) {
                    sb.Append(s.Remove(wraplen));
                    sb.Append('\n');
                    System.String stemp = s.Substring(wraplen);
                    if (stemp.Length <= wraplen)
                    {
                        sb.Append(stemp);
                        sb.Append('\n');
                        continue;
                    }
                    while (stemp.Length > wraplen)
                    {
                        sb.Append(stemp.Remove(wraplen));
                        sb.Append('\n');
                        stemp = stemp.Substring(wraplen);
                    }
                } else {
                    sb.Append(s);
                    sb.Append('\n');
                }
            }
            return sb.ToString();
        }

        public static System.IntPtr WindowHandle => mainwinhandle;

        public static System.Windows.Forms.ReadOnlyWindowHandle WinFormsHandle => new(mainwinhandle);

        private static MMDevice ShowDialogAndRet(IEnumerable<MMDevice> eps)
        {
            SelectDeviceWindow SDW = new(eps);
            try {
                MusicPlayer.Entry.WaitFormInstance?.Close();
                if (SDW.ShowDialog(WinFormsHandle) == System.Windows.Forms.DialogResult.Cancel) { return null; }
                return SDW.SelectedDevice;
            } finally {
                SDW?.Dispose();
                SDW = null;
            }
        }

        public static MMDevice GetSuitableDevice()
        {
            var mmdevenum = new MMDeviceEnumerator();
            IList<MMDevice> eps = null;
            try {
                eps = mmdevenum.EnumAudioEndpoints(EDataFlow.Render, DEVICE_STATE.ACTIVE);
                // The first time the player must always ask from the user the device!
                if (System.String.IsNullOrEmpty(Settings.Global.AudioDeviceId))
                {
                    return ShowDialogAndRet(eps);
                }
                MMDevice fd = null;
                foreach (var dv in eps)
                {
                    if (dv.ID == Settings.Global.AudioDeviceId) {
                        fd = dv;
                        continue;
                    }
                    dv.Dispose();
                }
                // If the device was not found , we are ought to show the dialog
                if (fd is null) {
                    return ShowDialogAndRet(eps);
                }
                return fd;
            } finally {
                mmdevenum.Dispose();
                mmdevenum = null;
            }
        }

        public static MMDevice GetSuitableDeviceNoInteraction()
        {
            if (System.String.IsNullOrEmpty(Settings.Global.AudioDeviceId)) { goto G_EXIT; }
            var mmdevenum = new MMDeviceEnumerator();
            try {
                if (mmdevenum.TryGetDevice(Settings.Global.AudioDeviceId , out var dv)) {
                    return dv;
                } else {
                    goto G_EXIT;
                }
            } finally {
                mmdevenum.Dispose();
                mmdevenum = null;
            }
        G_EXIT:
            ShowErrorResourceMessage("Error_AudioDevMustBeSelectedFromMainApp");
            return null;
        }

        public static System.Windows.Media.SolidColorBrush ToBrush(this System.Drawing.Color color)
            => new(System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B));

        /// <summary>
        /// Returns a new <see cref="System.Windows.Media.ImageSource"/> from the specified image resource from the specified resource loader.
        /// </summary>
        /// <param name="ldr">The resource loader to load the image from.</param>
        /// <param name="resourcename">The name of the image to load.</param>
        /// <returns>The XAML equivalent image.</returns>
        public static System.Windows.Media.ImageSource LoadXamlImage(this IResourceLoader ldr , System.String resourcename)
        {
            System.IO.Stream STR = null;
            System.Windows.Media.Imaging.BitmapImage IMG = new();
            try {
                STR = new Microsoft.IO.MemoryStream(ldr.GetByteArrayResource(resourcename));
                STR.Position = 0;
                IMG.BeginInit();
                IMG.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
                IMG.CreateOptions = System.Windows.Media.Imaging.BitmapCreateOptions.None;
                IMG.StreamSource = STR;
                IMG.EndInit();
                IMG.Freeze();
            } finally {
                STR?.Dispose();
            }
            return IMG;
        }

        public static Bitmap LoadIconResourceAndOrdinal(this IResourceLoader ldr, System.String resourcename, System.Int32 ordinal)
        {
            System.IO.Stream temps = null , temp2 = null;
            RawIconReader rir = null;
            RawBitmapReader rbr = null;
            try
            {
                temps = new Microsoft.IO.MemoryStream(ldr.GetByteArrayResource(resourcename));
                rir = new RawIconReader(temps);
                temp2 = rir.GetBitmapStream(ordinal);
                temps.Dispose();
                rir.Dispose();
                rbr = RawBitmapReader.RawBitmapIconDecode(temp2);
                return rbr.ToBitmap();
            } finally { 
                temps?.Dispose();
                temps = null;
                temp2?.Dispose();
                temp2 = null;
                rir?.Dispose();
                rir = null;
                rbr?.Dispose();
                rbr = null;
            }
        }

        public static Bitmap LoadNormalBitmap(this IResourceLoader ldr , System.String resourcename)
        {
            System.IO.Stream STR = null;
            try {
                STR = new Microsoft.IO.MemoryStream(ldr.GetByteArrayResource(resourcename));
                return new(STR);
            } finally {
                STR?.Dispose();
                STR = null;
            }
        }

        /// <summary>
        /// Converts the <see cref="System.Drawing.Imaging.Metafile"/> to a valid XAML image so that it
        /// can be used in <see cref="System.Windows.Controls.Image.Source"/> property..
        /// </summary>
        /// <param name="metafile">The metafile to convert.</param>
        /// <returns>The XAML metafile image.</returns>
        public static System.Windows.Media.ImageSource ToXamlBitmap(this System.Drawing.Imaging.Metafile metafile)
        {
            System.IO.Stream STR;
            System.Windows.Media.Imaging.BitmapImage IMG = new();
            STR = new Microsoft.IO.MemoryStream();
            try { metafile.Save(STR, metafile.RawFormat); } catch { STR?.Dispose(); throw; }
            STR.Position = 0;
            IMG.BeginInit();
            IMG.StreamSource = STR;
            IMG.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
            IMG.CreateOptions = System.Windows.Media.Imaging.BitmapCreateOptions.DelayCreation;
            IMG.EndInit();
            IMG.Freeze();
            return IMG;
        }

        public static System.String GetSize(System.Int64 length)
        {
            System.Int64 num = length;
            System.Int64 nump = 0;
            System.Byte I = 0;
            System.String format = "B";
            while (num > 0)
            {
                I++;
                nump = num;
                num = num / 1024;
            }
            switch (I)
            {
                case 0:
                    // Returns Empty because the file size is 0.
                    return "Empty";
                case 1:
                    // Directly yield execution to format string.
                    break;
                case 2:
                    format = "KB";
                    break;
                case 3:
                    format = "MB";
                    break;
                case 4:
                    format = "GB";
                    break;
                case 5:
                    format = "TB";
                    break;
                default:
                    return "<N/A>";
            }
            return System.String.Format("{0} {1}", nump, format);
        }

        public static System.String GetNameOnly(this Microsoft.IO.FileInfo info)
        {
            System.Int32 occ = info.Name.LastIndexOf('.');
            if (occ == -1)
            {
                return info.Name;
            }
            else
            {
                return info.Name.Remove(occ);
            }
        }

        public static void ShowMessage(System.String message)
        {
            using (var dlg = new MP.Dialogs.NewGenMessageBox())
            {
                dlg.Text = message;
                dlg.Title = "Notice";
                dlg.Buttons = Dialogs.ButtonSelection.OK;
                dlg.SelectedIcon = Dialogs.IconSelection.Info;
                dlg.ShowDialog(mainwinhandle);
                _ = dlg.ReturnedButton;
            }
        }

        public static void ShowErrorMessage(System.String message)
        {
            using (var dlg = new MP.Dialogs.NewGenMessageBox())
            {
                dlg.Text = message;
                dlg.Title = "Notice";
                dlg.Buttons = Dialogs.ButtonSelection.OK;
                dlg.SelectedIcon = Dialogs.IconSelection.Error;
                dlg.ShowDialog(mainwinhandle);
                _ = dlg.ReturnedButton;
            }
        }

        public static void ShowResourceMessage(System.String resname) => ShowMessage(Settings.Global.Resources.GetStringResource(resname));

        public static void ShowResourceMessage(System.String resname , params System.Object[] format) 
            => ShowMessage(System.String.Format(Settings.Global.Resources.GetStringResource(resname) , format));

        public static void ShowResourceMessage(System.String resname, GamepadReader reader)
        {
            using (var dlg = new MP.Dialogs.NewGenMessageBox(reader))
            {
                dlg.Text = Settings.Global.Resources.GetStringResource(resname);
                dlg.Title = "Notice";
                dlg.Buttons = Dialogs.ButtonSelection.OK;
                dlg.SelectedIcon = Dialogs.IconSelection.Info;
                dlg.ShowDialog(mainwinhandle);
                _ = dlg.ReturnedButton;
            }
        }

        public static void ShowResourceMessage(System.String resname, GamepadReader reader, params System.Object[] format)
        {
            using (var dlg = new MP.Dialogs.NewGenMessageBox(reader))
            {
                dlg.Text = System.String.Format(Settings.Global.Resources.GetStringResource(resname), format);
                dlg.Title = "Notice";
                dlg.Buttons = Dialogs.ButtonSelection.OK;
                dlg.SelectedIcon = Dialogs.IconSelection.Info;
                dlg.ShowDialog(mainwinhandle);
                _ = dlg.ReturnedButton;
            }
        }

        public static void ShowErrorResourceMessage(System.String resname) => ShowErrorMessage(Settings.Global.Resources.GetStringResource(resname));

        public static void ShowErrorResourceMessage(System.String resname , GamepadReader reader)
        {
            using (var dlg = new MP.Dialogs.NewGenMessageBox(reader))
            {
                dlg.Text = Settings.Global.Resources.GetStringResource(resname);
                dlg.Title = "Error";
                dlg.Buttons = Dialogs.ButtonSelection.OK;
                dlg.SelectedIcon = Dialogs.IconSelection.Error;
                dlg.ShowDialog(mainwinhandle);
                _ = dlg.ReturnedButton;
            }
        }

        public static void ShowErrorResourceMessage(System.String resname, GamepadReader reader , params System.Object[] format)
        {
            using (var dlg = new MP.Dialogs.NewGenMessageBox(reader))
            {
                dlg.Text = System.String.Format(Settings.Global.Resources.GetStringResource(resname), format);
                dlg.Title = "Error";
                dlg.Buttons = Dialogs.ButtonSelection.OK;
                dlg.SelectedIcon = Dialogs.IconSelection.Error;
                dlg.ShowDialog(mainwinhandle);
                _ = dlg.ReturnedButton;
            }
        }

        public static void ShowErrorResourceMessage(System.String resname, params System.Object[] format)
             => ShowErrorMessage(System.String.Format(Settings.Global.Resources.GetStringResource(resname), format));

        public static System.Boolean ShowQuestionMessage(System.String message)
        {
            using (var dlg = new MP.Dialogs.NewGenMessageBox())
            {
                dlg.Text = message;
                dlg.Title = "Notice";
                dlg.Buttons = Dialogs.ButtonSelection.YesNo;
                dlg.SelectedIcon = Dialogs.IconSelection.Question;
                dlg.ShowDialog(mainwinhandle);
                return dlg.ReturnedButton == Dialogs.ButtonReturned.Yes;
            }
        }

        public static System.Boolean ShowResourceQuestionMessage(System.String resname, params System.Object[] format) =>
            ShowQuestionMessage(System.String.Format(Settings.Global.Resources.GetStringResource(resname) , format));
    }

    public sealed class BamlResourceDictionaryLoader
    {
        private System.Windows.ResourceDictionary rdd;

        public BamlResourceDictionaryLoader(System.Byte[] bamldata)
        {
            System.IO.Stream strm = null;
            System.Windows.Baml2006.Baml2006Reader reader = null;
            try {
                strm = new Microsoft.IO.MemoryStream(bamldata);
                reader = new(strm);
                rdd = System.Windows.Markup.XamlReader.Load(reader) as System.Windows.ResourceDictionary;
            } finally {
                reader?.Close();
                reader = null;
                strm?.Dispose();
                strm = null;
            }
        }
        
        public BamlResourceDictionaryLoader(System.IO.Stream strm)
        {
            System.Windows.Baml2006.Baml2006Reader reader = new(strm);
            try {
                rdd = System.Windows.Markup.XamlReader.Load(reader) as System.Windows.ResourceDictionary;
            } finally {
                reader.Close();
                reader = null;
            }
        }

        public System.Windows.ResourceDictionary LoadedDictionary => rdd;
    }

    public sealed class BamlWindowDictionaryLoader<T>
        where T : System.Windows.Window
    {
        private T window;

        public BamlWindowDictionaryLoader(System.Byte[] bamldata)
        {
            System.IO.Stream strm = null;
            System.Xaml.XamlReaderSettings xrs = new();
            xrs.LocalAssembly = typeof(T).Assembly;
            System.Windows.Baml2006.Baml2006Reader reader = null;
            try {
                strm = new Microsoft.IO.MemoryStream(bamldata);
                reader = new(strm , xrs);
                window = System.Windows.Markup.XamlReader.Load(reader) as T;
            } finally {
                reader?.Close();
                reader = null;
                strm?.Dispose();
                strm = null;
            }
        }

        public T Window => window;
    }
}