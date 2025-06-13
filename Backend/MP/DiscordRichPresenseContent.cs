
using System;
using System.Collections.Generic;

namespace MP
{
    using DGSDK;

    /// <summary>
    /// Monitors errors and information from the Discord API.
    /// </summary>
    public sealed class DiscordTraceMonitor : DebugSource
    {
        public const System.String TraceMonitorSourceName = "DiscordRP-System";

        public DiscordTraceMonitor() { SourceName = TraceMonitorSourceName; }

        internal void DGSDK_LOG(LogLevel lvl , System.String message)
        {
            System.String sev = lvl switch { 
                LogLevel.Error => "ERROR",
                LogLevel.Warn => "WARNING",
                LogLevel.Info => "INFO",
                LogLevel.Debug => "DEBUG",
                _ => "INFO"
            };
            WriteLogLine($"[{sev}] {message}");
        }

        internal void RunCallbacksResultError(Result result)
        {
            DGSDK_LOG(LogLevel.Warn , $"RunCallbacks failed! Code: {result}");
        }

        internal void InitializationError(Result result)
        {
            DGSDK_LOG(LogLevel.Error, $"Initialization failed on the target with result code {result}. No Discord services will be provided for this instance.");
        }

        internal void UpdateActivityInfo(Result result)
        {
            if (result != Result.Ok) {
                DGSDK_LOG(LogLevel.Error, $"Activity update failed on the target with result code {result}. This activity was not registered in Discord Client.");
            } else {
                DGSDK_LOG(LogLevel.Info, "Activity update successfully dispatched.");
            }
        }

        internal void SdkNotFoundError(System.Exception e)
        {
            DGSDK_LOG(LogLevel.Error, $"Cannot locate the Discord SDK, or the SDK cannot be loaded. \nUnderlying Exception: {e}");
        }
    }

    /// <summary>
    /// Activity update information.
    /// </summary>
    public struct DiscordUpdateInfoData
    {
        public ActivityTimestamps Timestamps;
        public ActivityAssets Assets;
        public System.String State;
        public System.String Details;
        public System.Boolean IsInstancedSession;
        // Internal field. Instruct the dispatching thread to register the logging callback.
        internal System.Boolean UpdateLogCallbackD;
    }

    /// <summary>
    /// Provides a thread-safe environment for providing Rich Presense to Discord clients.
    /// </summary>
    public sealed class DiscordRichPresense : IDisposable
    {
        private Discord instance;
        private System.Int64 appid;
        private System.Boolean valid;
        private DiscordTraceMonitor dtm;
        private ActivityManager richpresmgr;
        private Stack<DiscordUpdateInfoData> dps;
        private System.Threading.Thread dispatchwq;

        public DiscordRichPresense(System.Int64 AppID)
        {
            valid = false;
            dps = new(10);
            appid = AppID;
            instance = null;
            dtm = null;
            richpresmgr = null;
            dispatchwq = new(DispatchingCode);
            // I do not think that it calls any COM code but you never know what can happen...
            dispatchwq.TrySetApartmentState(System.Threading.ApartmentState.STA);
            dispatchwq.Priority = System.Threading.ThreadPriority.BelowNormal;
            dispatchwq.Name = "[MP] Discord Connection Dispatch Thread";
        }

        private static void DispatchingCode_ClearActivityHandler(Result ret) { }

        private void DispatchingCode_UpdateInfo(Result ret) => dtm?.UpdateActivityInfo(ret);

        private void DispatchingCode()
        {
            try {
                // P2: Corresponds to NoRequireDiscord.
                instance = new(appid, 1);
                // Get and init the activity manager.
                richpresmgr = instance.GetActivityManager();
            } catch (ResultException e) {
                instance?.Dispose();
                dtm?.InitializationError(e.Result);
                dtm?.Disable();
                return;
            // The below two exceptions are only about the instance constructor.
            } catch (DllNotFoundException e) {
                dtm?.SdkNotFoundError(e);
                dtm?.Disable();
                return;
            } catch (EntryPointNotFoundException e) {
                dtm?.SdkNotFoundError(e);
                dtm?.Disable();
                return;
            }
            if (dtm is not null) { instance.SetLogHook(LogLevel.Debug, new(dtm.DGSDK_LOG)); }
            // Destroy APPID for safety reasons.
            appid = 0;
            // Inform at logging level that Discord SDK is ready.
            dtm?.DGSDK_LOG(LogLevel.Info, $"Discord SDK was successfully initialized on thread {dispatchwq.ManagedThreadId} .");
            valid = true;
            while (valid) {
                if (dps.Count == 0) { 
                    System.Threading.Thread.Sleep(98); 
                } else {
                    DiscordUpdateInfoData upd = dps.Pop();
                    if (upd.UpdateLogCallbackD) {
                        instance.SetLogHook(LogLevel.Debug, new(dtm.DGSDK_LOG));
                    } else {
                        richpresmgr.UpdateActivity(new() {
                            Instance = upd.IsInstancedSession,
                            Assets = upd.Assets,
                            Party = default,
                            Secrets = default,
                            Timestamps = upd.Timestamps,
                            State = upd.State,
                            Details = upd.Details
                        }, new(DispatchingCode_UpdateInfo));
                    }
                }
                try {
                    // Finally , run the callbacks to flush/update data.
                    instance.RunCallbacks();
                } catch (ResultException ex) {
                    if (ex.Result == Result.Ok) { continue; }
                    if (ex.Result == Result.NotRunning) {
                        valid = false;
                        dtm?.DGSDK_LOG(LogLevel.Info, "The Discord Client seems detached. There is no need to update anymore , so the dispatching thread will be destroyed.");
                        break;
                    }
                    dtm?.RunCallbacksResultError(ex.Result);
                    // This is a non-fatal error so the loop may continue running.
                }
            }
            // We are not interested whether the activity will be cleared successfully.
            richpresmgr.ClearActivity(new(DispatchingCode_ClearActivityHandler));
            // Detach activity manager.
            richpresmgr = null;
            // Must be freed from the same thread that created the object.
            instance.Dispose();
            instance = null;
        }

        /// <summary>
        /// Gets a value whether the app is connected to the Discord Client.
        /// </summary>
        public System.Boolean Connected => valid;

        /// <summary>
        /// Attempts to start the dispatching thread , if possible.
        /// </summary>
        public void TryRun() => dispatchwq?.Start();

        public void UpdateInfo(DiscordUpdateInfoData data)
        {
            data.UpdateLogCallbackD = false;
            dps?.Push(data);
        }

        public void RegisterLogsListener(DiscordTraceMonitor tm)
        {
            if (dispatchwq is null) { return; }
            dtm?.Disable();
            dtm = null;
            DebugProvider.RemoveSource(DiscordTraceMonitor.TraceMonitorSourceName);
            dtm = tm;
            dtm.Enable();
            if (valid) {
                UpdateInfo(new() { UpdateLogCallbackD = true });
            }
        }

        /// <summary>
        /// Waits for the dispatching thread to complete all of it's commands , and then disposes the API and subsequently exits.
        /// </summary>
        public void Dispose()
        {
            if (dispatchwq is null) { return; }
            valid = false;
            dispatchwq?.Join();
            dispatchwq = null;
            dps?.Clear();
            dps = null;
            dtm?.Disable();
            DebugProvider.RemoveSource(DiscordTraceMonitor.TraceMonitorSourceName);
            dtm = null;
        }
    }
}