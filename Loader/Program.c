
#include "framework.h"
#include "resource.h"
#include <strsafe.h>
#include "Hosting.h"
#include "Helpers.h"
#include "Network.h"
#include "Constants.h"

// Enable Debug messages in Release builds.
// Has no effect in Debug builds.
//#define SHOW_DEBUG_MSG 1

#ifdef _DEBUG
    #undef SHOW_DEBUG_MSG
#endif

LPWSTR processpathdir;
LPWSTR hostfxr_path;
LPWSTR runtimepath;
BOOL SharedInitialization;

BOOL InstallDotNetRuntime(void);

void Debug_PrintAllPaths(PHOSTFXR_PATHS paths);

int ProgramStep_EnsureCustomDotNet(void);

BOOL ProgramStep_FindSystemWideInstallation(void);

int ProgramStep_EnsureSessionFile(void);

int APIENTRY wWinMain(_In_ HINSTANCE hInstance,
    _In_opt_ HINSTANCE hPrevInstance,
    _In_ LPWSTR    lpCmdLine,
    _In_ int       nCmdShow)
{
#if _DEBUG || SHOW_DEBUG_MSG
    InitializeConsole();
#endif // _DEBUG
    Print(L"Initializing...\n");
    SharedInitialization = FALSE;
    int erc;
    LPWSTR ppc = DefaultAllocateString();
    hostfxr_path = NULL;
    LPWSTR dotnet = NULL;
    GetModuleFileNameW(NULL , ppc , TheoriticalUpperBound);
    Print(L"Music player On .NET 8 - Native Invoker version 1.0.1.6\n");
    PrintFormatted(L"Host file path: %s\n" , ppc);
    processpathdir = GetDirectoryFromPath(ppc);
    UnallocateString(ppc);
    PrintFormatted(L"Command Line: %s\n", lpCmdLine);
    PrintFormatted(L"Working Directory: %s\n" , processpathdir);
#if _DEBUG || SHOW_DEBUG_MSG
    Print(L"Since we are on a Debug build, additionally setting the COREHOST_TRACE environment variable to log .NET init messages...\n");
    SetEnvironmentVariableW(L"COREHOST_TRACE", L"1");
    SetEnvironmentVariableW(L"COREHOST_TRACEFILE", L"Mp-Host-RuntimeTrace.txt");
#endif
    LPWSTR temp = FormatString(L"%s\\%s" , processpathdir , MusicPlayerDLLNAME);
    if (FileExists(temp) == FALSE) {
        ShowErrorMessageBox(L"The Music Player implementation does not exist. Check whether the file MusicPlayer-impl.dll does exist.");
        erc = 10;
        goto G_EXIT;
    }
    if ((erc = ProgramStep_EnsureSessionFile()) != 0)
    {
        goto G_EXIT;
    }
    // First, check for the system-wide installation.
    // If that has something to give to us, use it.
    if (ProgramStep_FindSystemWideInstallation())
    {
        goto G_INSTALL_SUCC_CNT;
    }
    if ((erc = ProgramStep_EnsureCustomDotNet()) != 0)
    {
        goto G_EXIT;
    }
    PHOSTFXR_PATHS paths;
G_INSTALL_SUCC_CNT:
    paths = GetHostFXRPaths(runtimepath);
    // Keep .NET path if we are about to use it
    dotnet = CloneString(runtimepath);
    UnallocateString(runtimepath);
    if (paths == NULL || paths->Count == 0)
    {
        if (SharedInitialization)
        {
            Print(L"Possibly corrupted installation, falling back to app-independent installation...");
            SharedInitialization = FALSE;
            if ((erc = ProgramStep_EnsureCustomDotNet()) != 0)
            {
                goto G_EXIT;
            }
        }
        Print(L"HostFXR probing failed.\nEither the runtime is not installed properly , or there is a filesystem denial.\n");
        ShowErrorMessageBox(L"Cannot initialize the app because there is a malformed .NET installation.\nReinstall the private .NET installation by deleting the Runtime directory inside the app's directory , and retry.");
        DestroyHostFXRPathsStruct(paths);
        UnallocateString(dotnet);
        erc = 11;
        goto G_EXIT;
    }
    Debug_PrintAllPaths(paths);
    int pph = GetCompatibleHostFXR(paths, L"8.0.0"); // The app requires .NET 8 at least
    if (pph == -1)
    {
        if (SharedInitialization)
        {
            Print(L"Could not find a usuable HostFXR here. Falling back...\n");
            SharedInitialization = FALSE;
            if ((erc = ProgramStep_EnsureCustomDotNet()) != 0)
            {
                goto G_EXIT;
            }
        }
        Print(L"Could not find a usuable HostFXR in any way. Exiting...\n");
        DestroyHostFXRPathsStruct(paths);
        UnallocateString(dotnet);
        erc = 10;
        goto G_EXIT;
    }
    PrintFormatted(L"Picking HostFXR of version %s with path %s.\n" , paths->Entries[pph].Version , paths->Entries[pph].Path);
    hostfxr_path = CloneString(paths->Entries[pph].Path);
    DestroyHostFXRPathsStruct(paths);
    if ((erc = GetHostFXR(hostfxr_path)) != 0) {
        PrintFormatted(L"HostFXR initialization from path %s failed. (Code %d).\n", hostfxr_path, erc);
        ShowErrorMessageBox(L"Could not initialize the runtime. Runtime instantiation failed.\nPress 'OK' to exit.");
        goto G_EXIT;
    }
    char_t** args = SafeAllocate(3 * sizeof(LPVOID));
    args[0] = temp;
    args[1] = L"--token-handle=1374";
    args[2] = lpCmdLine;
    PrintFormatted(L"Executing app %s with:\n", temp);
    PrintFormatted(L".NET Path: %s\nHostFXR PATH: %s\n" , dotnet , hostfxr_path);
    PrintFormatted(L"Arguments:\nARG[0]: %s\nARG[1]: %s\nARG[2]: %s\n" , args[0] , args[1] , args[2]);
    Print(L"Preparing...\n");
    if ((erc = RegisterAppArguments(3, args , dotnet , hostfxr_path)) != 0) {
        UnallocateString(dotnet);
        SafeUnallocate(args);
        PrintFormatted(L"Failed to pass the CoreCLR arguments. Method failed with code %d.\n" , erc);
        ShowErrorMessageBox(L"Runtime does not have the required information to instantiate.\nRuntime instantiation failed.\nPress 'OK' to exit.");
        goto G_EXIT;
    }
#if _DEBUG
    PrintFormatted(L"Loading Runtime at initialized location %p...\n", GetRuntimeHandle());
#else
    Print(L"Loading Runtime...\n");
#endif // _DEBUG
    SafeUnallocate(args);
    UnallocateString(dotnet);
    Print(L"Invoking app code...\n");
#ifdef SHOW_DEBUG_MSG
    UninitializeConsole();
#endif
    erc = RunApp();
    PrintFormatted(L"The app completed execution and exited with code %d.\n" , erc);
G_EXIT:
    DisposeRuntime();
    UnallocateString(temp);
    UnallocateString(processpathdir);
    UnallocateString(hostfxr_path);
#if _DEBUG || SHOW_DEBUG_MSG
    PauseConsoleInput();
    UninitializeConsole();
#endif // _DEBUG
    return erc;
}

int ProgramStep_EnsureCustomDotNet(void)
{
    if (SubDirectoryExists(processpathdir, DotnetDirectoryName) == FALSE) {
        if (ShowQuestionMessageBox(L"The .NET Runtime is not installed for this installation. Install now locally?\nPress 'yes' to start installation , or use 'no' to exit."))
        {
            if (InstallDotNetRuntime()) {
                runtimepath = FormatString(L"%s\\Runtime", processpathdir);
                return 0;
            }
            else {
                return 24;
            }
        }
        return 10;
    }
    return 0;
}

BOOL ProgramStep_FindSystemWideInstallation(void)
{
    Print(L"Attempting to find a system-wide installation...\n");
    PDOTNET_SYSTEMWIDE_INSTALLATION systemwide = GetSystemWideDotNetInstallation();
    if (systemwide != NULL)
    {
        Print(L"Found a .NET system-wide installation. Attempting to use that.\n");
        PrintFormatted(L".NET system-wide installation version: %s\n", systemwide->Version);
        runtimepath = CloneString(systemwide->FolderPath);
        DestroySystemWideInstallationStruct(systemwide);
        SharedInitialization = TRUE;
        return TRUE;
    }
    Print(L"No system-wide installation found, falling back...\n");
    return FALSE;
}

int ProgramStep_EnsureSessionFile(void)
{
    // Before continuing , check whether the last session failed by the presense of the session file.
    // If yes , show an appropriate message.
    LPWSTR sessionfile = FormatString(L"%s\\session", processpathdir);
    if (FileExists(sessionfile)) {
        if (ShowQuestionMessageBox(L"The last Music Player session failed due to an error.\nAre you sure that you want to initialize the session although that your information might have been corrupted?\nPress 'yes' to continue , or use 'no' to exit.")) {
            DeleteExistingFile(sessionfile);
        }
        else {
            UnallocateString(sessionfile);
            return 22;
        }
    }
    UnallocateString(sessionfile);
    return 0;
}

BOOL InstallDotNetRuntime(void)
{
    InitializeConsole();
    unsigned char malloccmd = FALSE;
    Print(L"Initializing network...\n");
    LPWSTR error = InitializeNetwork();
    if (error != NULL) { malloccmd = TRUE; goto G_ErrorExit; }
    LPWSTR dnurl = GetResourceString(MP_HOST_DNDOWNLOAD_STRING);
    PrintFormatted(L"Preparing to download %s...\n", dnurl);
    error = Request(dnurl);
    UnallocateString(dnurl);
    if (error != NULL) { malloccmd = TRUE; goto G_ErrorExit; }
    LPWSTR dotnet_install = FormatString(L"%s\\dotnet-install.ps1" , processpathdir);
    PrintFormatted(L"Final download path is '%s'...\n" , dotnet_install);
    Print(L"Downloading...\n");
    error = DownloadFileTo(dotnet_install);
    if (error != NULL) { malloccmd = TRUE; UnallocateString(dotnet_install); goto G_ErrorExit; }
    Print(L"Executing Streamed Install...\n");
    TerminateTransaction();
    LPWSTR dotnet_runtime_path = FormatString(L"%s\\%s" , processpathdir , DotnetDirectoryName);
    LPWSTR temp = FormatString(L"powershell.exe -File \"%s\" -Channel 8.0 -Quality GA -Runtime windowsdesktop -InstallDir \"%s\"", dotnet_install, dotnet_runtime_path);
    PrintFormatted(L"Command to run is: \'%s\'\n", temp);
    BOOL result = RunProcess(temp);
    UnallocateString(temp);
    temp = FormatString(L"powershell.exe -File \"%s\" -Channel 8.0 -Quality GA -Runtime dotnet -InstallDir \"%s\"", dotnet_install, dotnet_runtime_path);
    PrintFormatted(L"Command to run is: \'%s\'\n", temp);
    result |= RunProcess(temp);
    UnallocateString(temp);
    UnallocateString(dotnet_install);
    UnallocateString(dotnet_runtime_path);
    if (result == FALSE) { malloccmd = TRUE; error = GetLastErrorMessage(); goto G_ErrorExit; }
    Print(L"Music Player On .NET 8 is ready. In 4 seconds , the app will be launched.\n");
    Sleep(4000);
#ifndef _DEBUG
    UninitializeConsole();
#endif // !_DEBUG
    return TRUE;
G_ErrorExit:
    TerminateTransaction();
    PrintFormatted(L"Error occured: %s\nThe app will exit.\n" , error);
    if (malloccmd) { UnallocateString(error); }
    return FALSE;
}

void Debug_PrintAllPaths(PHOSTFXR_PATHS paths)
{
#if _DEBUG || SHOW_DEBUG_MSG
    Print(L"Printing all usuable HostFXR's: \n");
    if (paths == NULL) {
        Print(L"No usuable paths were found. \n");
        return;
    }
    for (int I = 0; I < paths->Count; I++)
    {
        PrintFormatted(L"HostFXR with path: %s, version %s\n" , paths->Entries[I].Path , paths->Entries[I].Version);
    }
#endif
}
