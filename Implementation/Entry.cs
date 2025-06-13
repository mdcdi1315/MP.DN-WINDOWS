
using MP;
using System;
using Microsoft.IO;
using DotNetResourcesExtensions;

namespace MusicPlayer
{
    internal static class Entry
    {
        private static FileInfo settingsfileinfo;
        private static System.String basedir;
        private static FileStream session;

        public static PlayerWindow Window;
        public static LoadingFormInstanceImpl WaitFormInstance;

        private static System.Int32 InitializeQuickPlayer(System.String arg, FileInfo settingsf)
        {
            System.Int32 sidx = arg.IndexOf(':');
            if (sidx == -1 || arg.Length <= 6) { MusicPlayerHelper.ShowErrorMessage("The file argument is not provided correctly."); return 5; }
            sidx++;
            FileInfo fi = new(arg.Substring(sidx).Trim('\"'));
            if (fi.Exists == false) { MusicPlayerHelper.ShowErrorMessage($"The file {fi.FullName} does not exist."); return 1342; }
            if (settingsf.Exists)
            {
                FileStream FS = null;
                try
                {
                    FS = settingsf.OpenRead();
                    MP.Settings.ReadFromCGI(FS);
                }
                catch { return 13670; }
                finally { FS?.Dispose(); }
            }
            else { MusicPlayerHelper.ShowErrorMessage("The Music Player App must have been already successfully initialized at once before using this feature!"); return 13671; }
            System.Threading.Thread mainth = new(() => {
                SingleFilePlayerWindow sfpw = null;
                MusicPlayerStream mps = null;
                try {
                    BamlWindowDictionaryLoader<SingleFilePlayerWindow> wdl = new(Settings.Global.Resources.GetByteArrayResource("SFPWindowBAML"));
                    sfpw = wdl.Window;
                    wdl = null;
                    mps = new(fi.OpenRead());
                    mps.SetStringAttribute("FileName", fi.FullName);
                    mps.SetBooleanAttribute(MusicPlayerStream.IsStreamOwnerProperty, true);
                    sfpw.InitSFPW(mps);
                    sfpw.ShowDialog();
                } catch (System.Exception e)
                {
                    MusicPlayerHelper.ShowErrorMessage($"Exception of type {e.GetType().FullName} occured:\n{e.Message}"); return;
                }
                finally { if (sfpw is not null && sfpw.IsActive) { sfpw.Close(); } if (mps is not null) { mps.Dispose(); mps = null; } }
            });
            mainth.TrySetApartmentState(System.Threading.ApartmentState.STA);
            mainth.Priority = System.Threading.ThreadPriority.Normal;
            mainth.Name = "[MP] Player Root Thread worker";
            mainth.Start();
            mainth.Join();
            return 0;
        }

        private static void InitMainWindow()
        {
            FileStream fsm = null;
            try {
                DebugProvider.WriteLine("Retrieveing settings...");
                System.Int32 erc = LoadSettingsFile(settingsfileinfo);
                if (erc != 0) {
                    DebugProvider.WriteLine("FAILED!");
                    WaitFormInstance?.Dispose();
                    Environment.Exit(erc);
                }
                DebugProvider.WriteLine("Settings information loaded successfully.");
                DebugProvider.WriteLine("Preparing RCU engine...");
                DebugProvider.WriteLine("RCU-Bootstrap: Bootstrapper v1.0.0.0. now initializing...");
                DebugProvider.WriteLine("RCU-Bootstrap: Preparing for new UI session...");
                if (Settings.Global.ResourceDictionary.Exists == false)
                {
                    WaitFormInstance?.Dispose();
                    MusicPlayerHelper.ShowErrorMessage("Cannot locate the Resources dictionary. The Music Player cannot load without them.\nPress 'OK' to exit.");
                    DebugProvider.WriteLine("RCU-Bootstrap: [FATAL] Cannot load because the Resource dictionary was not found. Exiting with code 4.");
                    System.Environment.Exit(4);
                }
                DebugProvider.WriteLine($"RCU-Bootstrap: Loading main resources data from {Settings.Global.ResourceDictionary.FullName}");
                try {
                    fsm = Settings.Global.ResourceDictionary.OpenRead();
                    Settings.Global.Resources = new DotNetResourceLoader(fsm);
                } catch (System.Exception ex) {
                    WaitFormInstance?.Dispose();
                    DebugProvider.WriteLine($"RCU-Bootstrap: [FATAL] Cannot load because the Resource dictionary was not loaded cleanly. Error information: {ex}\nExiting with code 4.");
                    MusicPlayerHelper.ShowErrorMessage("Music player failed to open: Resources are unavailable to the reader.\nPress \'OK\' to exit.");
                    System.Environment.Exit(4);
                }
                DebugProvider.WriteLine("RCU-Bootstrap: Creating loading form.");
                CreateLoadingForm();
                DebugProvider.WriteLine("RCU-Bootstrap: Waiting for engine to preload...");
                DebugProvider.WriteLine("Creating session lock file...");
                session = new(Path.Join(basedir, "session"), FileMode.CreateNew);
                System.Threading.Thread.Sleep(1000);
                DebugProvider.WriteLine("RCU-Bootstrap: Loading Engine caches...");
                erc = LoadCachesAndData();
                if (erc != 0) {
                    DebugProvider.WriteLine("FAILED!");
                    WaitFormInstance?.Dispose();
                    Environment.Exit(erc);
                }
                DebugProvider.WriteLine("UIManager: Initializing UI for new session.");
                System.Byte[] bt = Settings.Global.Resources.GetByteArrayResource("MainWindowBAML");
                DebugProvider.WriteLine("UIManager: UI load stage 1 ended.");
#if DEBUG
                ConsoleBackend.Title = Settings.Global.Resources.GetStringResource("DebugConsoleTitle");
#endif
                BamlWindowDictionaryLoader<PlayerWindow> wdl = new(bt);
                Window = wdl.Window;
                wdl = null;
                DebugProvider.WriteLine("UIManager: UI load stage 2 ended.");
                DebugProvider.WriteLine("RCU-Bootstrap: Loading Main Window WPF resources...");
                bt = Settings.Global.Resources.GetByteArrayResource("MainWindowTemplateDictionary");
                DebugProvider.WriteLine("UIManager: UI load stage 3 ended.");
                BamlResourceDictionaryLoader rdl = new(bt);
                Window.Resources = rdl.LoadedDictionary;
                rdl = null;
                DebugProvider.WriteLine("RCU-Bootstrap: WPF resource loading succeeded!!!");
                DebugProvider.WriteLine("UIManager: UI load stage 4 ended.");
                bt = null;
                DebugProvider.WriteLine("UIManager: UI load stage 5 ended.");
#if NET8_0
                DebugProvider.WriteLine("APP: Note: Fix for issue https://github.com/dotnet/wpf/issues/8911 has been effectively applied.");
                DebugProvider.WriteLine("APP: This fix is not needed for .NET 9+ but it is useful this to be noticed if this bug happens again.");
#endif
                DebugProvider.WriteLine("UIManager: UI load succeeded , entering the main message loop.");
                DebugProvider.WriteLine($"RCU-Bootstrap: Initial bootstrap and data preparation succeeded at {SystemInfo.Now} .");
                Window.ShowDialog();
                DebugProvider.WriteLine("UIManager: UI session seems to be finished , unloading.");
                // Specific issue for .NET 8 , see https://github.com/dotnet/wpf/issues/8911
                // Also see https://github.com/dotnet/wpf/issues/10171
                // This is needed because the app makes use of the ListView control.
                // However , keep this for future releases too as this acts as the Dispose() method.
                Window.Dispatcher.InvokeShutdown();
                DebugProvider.WriteLine("UIManager: UI resources were destroyed.");
                Window = null;
                DebugProvider.WriteLine("RCU: UI session ended successfully.");
            } catch (System.Exception e) {
                WaitFormInstance?.Dispose(); // Destroy wait window if it is still active
                switch (e)
                {
                    case System.Windows.Markup.XamlParseException especific:
                        MP.Dialogs.NativeMessageBox.Show($"UI error occured! Music Player UI failed to load cleanly. Exception data: {especific}", "UI Catastrophic error", MP.Dialogs.ButtonSelection.OK, MP.Dialogs.IconSelection.Error);
                        break;
                    default: // For all other cases just rethrow the exception
                        throw;
                }
            } finally {
                fsm?.Dispose();
                fsm = null;
            }
        }

        private static void CreateLoadingForm()
        {
            WaitFormInstance = new();
            WaitFormInstance.StartAsync();
        }

        private static System.Boolean VerifyPaths()
        {
            DebugProvider.WriteLine("Verifying that the paths defined in the settings file are correct...");
            System.String n1 = basedir;
            if (n1.Length > 1 && n1[n1.Length - 1] == '/' || n1[n1.Length - 1] == '\\')
            {
                n1 = n1.Remove(n1.Length - 1);
            }
            System.String n2 = Settings.Global.BaseDataDirectory.Parent.FullName;
            if (n2 != n1)
            {
                DebugProvider.WriteLine($"Base app loading directory is not correct: {n2}. Expected to be instead {n1}.");
                return false;
            }
            DebugProvider.WriteLine("Settings file is correct and can be used.");
            return true;
        }

        private static System.Int32 LoadSettingsFile(FileInfo FI)
        {
            FileStream FS = null;
            if (FI.Exists == false)
            {
                DebugProvider.WriteLine("Load flags are: NEW_SESSION CREATE_NEW_SETTINGS");
                try
                {
                    FS = FI.Create();
                    MP.Settings.Global = MP.Settings.GetDefault();
                    MP.Settings.WriteAsCGI(FS);
                } catch (System.Exception e) {
                    DebugProvider.WriteLine($"[FATAL] Writing the settings file failed: {e}");
                    return 13672; 
                }
                finally { FS?.Dispose(); }
                MP.Settings.Global.PlaylistsDirectory.Create();
            } else {
                DebugProvider.WriteLine("Load flags are: NEW_SESSION READ_SETTINGS");
                try {
                    FS = FI.OpenRead();
                    MP.Settings.ReadFromCGI(FS);
                    if (VerifyPaths() == false)
                    {
                        Settings.PatchBrokenPaths();
                        DebugProvider.WriteLine("Re-verifying that the paths are not broken again.");
                        if (VerifyPaths() == false)
                        {
                            MP.Dialogs.NativeMessageBox.Show("Settings file is corrupt! The settings file could not be patched successfully. Error occured." , "Cannot Patch Corrupt Settings File" , MP.Dialogs.ButtonSelection.OK , MP.Dialogs.IconSelection.Error);
                            return 13675;
                        }
                    }
                } catch (System.Exception e) {
                    DebugProvider.WriteLine($"[FATAL] Reading the settings file failed: {e}");
                    return 13670; 
                }
                finally { FS?.Dispose(); }
            }
            DebugProvider.WriteLine("Done loading settings!");
            return 0;
        }

        private static System.Int32 LoadCachesAndData()
        {
            FileStream FS = null;
            DebugProvider.WriteLine("Creating cache files if these do not exist...");
            if (Settings.Global.IconCachesMapFile.Exists == false)
            {
                DebugProvider.WriteLine("Writing empty icon cache map...");
                MP.Caches.PlaylistIconCache.IconCacheWriter wr = null;
                try
                {
                    FS = Settings.Global.IconCachesMapFile.OpenWrite();
                    wr = new(FS);
                    wr.WriteEmpty();
                }
                catch { return 13670; }
                finally { wr?.Dispose(); FS?.Dispose(); }
            }
            DebugProvider.WriteLine("Creating data directories...");
            Settings.Global.IconCachesDirectory.Create();
            Settings.Global.TempDirectory.Create();
            Settings.Global.PlaylistsDirectory.Create();
            Settings.Global.TempDirectory.Create();
            Settings.Global.OptionalFeaturesDirectory.Create();
            NativeLibraryLoader.AddDLLSearchDirectory(Settings.Global.OptionalFeaturesDirectory.FullName);
            DebugProvider.WriteLine("Data directories were created.");
            return 0;
        }

        private static System.Int32 SaveSettingsAndExit(FileInfo FI)
        {
            FileStream FS = null;
            DebugProvider.WriteLine("Saving settings snapshot...");
            try
            {
                FS = FI.Open(FileMode.Create);
                MP.Settings.WriteAsCGI(FS);
            } catch { return 13673; } finally { FS?.Dispose(); }
            return 0;
        }

        [MTAThread]
        public static System.Int32 Main(System.String[] args)
        {
            try {
                // Possibly the below log line is never written to the log...
                DebugProvider.WriteLine("Starting...");
                // Create and register the Windows system information layer.
                SystemInfo.RegisterPlatformLayer(new SystemInfo_Windows());
                // When in debug mode, select base directory to be the location where .NET loaded
                // this assembly (So that developers can further debug the app if they want to)
#if DEBUG
                basedir = System.AppDomain.CurrentDomain.BaseDirectory;
#else
                basedir = SystemInfo.CurrentProcessDirectory;
#endif
                System.Int32 erc;
                if (File.Exists(Path.Join(basedir, "session"))) { return 13671; }
                settingsfileinfo = new(Path.Join(basedir, "settings"));
                for (System.Int32 I = 0; I < args.Length; I++)
                {
                    if (args[I].StartsWith("-file:")) { return InitializeQuickPlayer(args[I], settingsfileinfo); }
                }
                if (DebugProvider.IsSupported)
                {
                    DebugProvider.AddSink(new ConsoleSink());
                }
                DebugProvider.WriteLine($"Music Player On .NET 8 , mdcdi1315 , Version {AppInfo.Version}");
                DebugProvider.WriteLine("Preparing...");
                if (DebugProvider.IsSupported)
                {
                    DebugProvider.AddSink(new LogFileSink(Path.Join(basedir, "mp.log")));
                    DebugProvider.WriteLine("Initialized log file!");
                }
                if (System.Diagnostics.Debugger.IsAttached)
                {
#if DEBUG
                    DebugProvider.WriteLine("APP: WARN: DEBUGGER DETECTED! Preparing environment for the debugger.");
                    //System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
#else
                    MP.Dialogs.NativeMessageBox.Show("Attempted to invoke the program through the debugger. This operation is disallowed on Release builds." , "Invalid attempt" , MP.Dialogs.ButtonSelection.OK , MP.Dialogs.IconSelection.Error);
                    return 13669;
#endif
                }
                DebugProvider.WriteLine("Injecting RCU bootstrapper and UI thread...");
                System.Threading.Thread MAIN = new(new System.Threading.ThreadStart(InitMainWindow));
                MAIN.TrySetApartmentState(System.Threading.ApartmentState.STA);
                MAIN.Priority = System.Threading.ThreadPriority.AboveNormal;
                MAIN.Name = "[MP] Player Root Thread worker";
                MAIN.Start();
                DebugProvider.WriteLine($"Engine Thread injected successfully and was started at {SystemInfo.Now} .");
                MAIN.Join();
                DebugProvider.WriteLine("Engine seems now unloaded, closing the app...");
                erc = SaveSettingsAndExit(settingsfileinfo);
                DebugProvider.WriteLine("Deleting remaining code paths...");
                Settings.Global = null;
                Window = null;
                basedir = null;
                DebugProvider.WriteLine("Dropping GC collection routine to ensure correct cleanup...");
                GC.Collect(2, GCCollectionMode.Forced, true);
                if (erc != 0) { return erc; }
                System.Threading.Thread.Sleep(30);
            } finally {
                WaitFormInstance?.Dispose();
                if (session is not null)
                {
                    System.String sess = session.Name;
                    session.Close();
                    session.Dispose();
                    try { File.Delete(sess); } catch { }
                    sess = null;
                }
            }
            DebugProvider.WriteLine("Exiting cleanly.");
            DebugProvider.CleanSinks();
            if (DebugProvider.IsSupported) {
                ConsoleBackend.WriteConsoleLine("Program finished execution with code 0. \nPress Enter to exit...");
                ConsoleBackend.ReadConsole();
                ConsoleBackend.Destroy();
            }
            return 0;
        }
    }
}