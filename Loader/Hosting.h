#pragma once

#include "VersionHelper.h"
#include "framework.h"
#include "resource.h"
#include "Helpers.h"

hostfxr_initialize_for_dotnet_command_line_fn initfptr;
hostfxr_run_app_fn run_app;
hostfxr_close_fn close_fptr;
hostfxr_handle runtimehand;
HMODULE mod;

// A structure that defines a HostFXR library entry.
// The structure returns information about the library version and it's fully qualified path.
typedef struct hostfxr_library_entry {
	LPWSTR Version;
	LPWSTR Path;
} HOSTFXR_LIB_ENTRY, *PHOSTFXR_LIB_ENTRY;

// A simple structure that defines an array of found HostFXR library entries.
// You can use the Count member to use a for loop to easily iterate between the entries.
typedef struct hostfxr_paths_definition {
	int Count;
	HOSTFXR_LIB_ENTRY* Entries;
} HOSTFXR_PATHS , *PHOSTFXR_PATHS;

// A structure for returning information from the system where the shared .NET installation is located to.
typedef struct dotnet_systemwide_installation {
	LPWSTR Version;
	LPWSTR FolderPath;
} DOTNET_SYSTEMWIDE_INSTALLATION , *PDOTNET_SYSTEMWIDE_INSTALLATION;

int GetHostFXR(LPWSTR path);

int RegisterAppArguments(int ARGC, char_t** ARGV , LPWSTR DOTNETPATH , LPWSTR HOSTPATH);

int RunApp(void);

// Gets a handful of valid .NET HostFXR paths to use.
// If the function returns successfully , use the DestroyHostFXRPathsStruct to destroy the returned structure.
PHOSTFXR_PATHS GetHostFXRPaths(LPWSTR exedirectory);

// Destroys a structure instance returned by GetHostFXRPaths function.
// You should always call this method to free such a structure;
// Just calling directly other freeing method might fail or will fail
// to release all the information that the structure holds.
void DestroyHostFXRPathsStruct(PHOSTFXR_PATHS paths);

/// <summary>
/// Returns the index of the passed PHOSTFXR_PATHS structure pointer with the specified installation version required to run the app you are about to run.
/// </summary>
/// <param name="paths">The structure pointer to pass to find for a valid .NET installation.</param>
/// <param name="version">The required .NET version to run the app.</param>
/// <returns>The compatible HostFXR installation that can be used. -1 if no such installation can be found.</returns>
int GetCompatibleHostFXR(PHOSTFXR_PATHS paths, LPWSTR version);

/// <summary>
/// Gets the shared .NET runtime installation, if it can be proven that it does exist.
/// </summary>
/// <returns>The shared .NET runtime installation, if that does exist; otherwise, NULL.</returns>
PDOTNET_SYSTEMWIDE_INSTALLATION GetSystemWideDotNetInstallation(void);

/// <summary>
/// Destroys a PDOTNET_SYSTEMWIDE_INSTALLATION structure pointer if GetSystemWideDotNetInstallation succeeds.
/// </summary>
/// <param name="inst">The structure pointer to destroy.</param>
void DestroySystemWideInstallationStruct(PDOTNET_SYSTEMWIDE_INSTALLATION inst);

// Destroys the runtime-associated information.
// Must be called after you have finished using it.
void DisposeRuntime(void);

// Under request , the return value must contain the current instance of the runtime.
LPVOID GetRuntimeHandle(void);