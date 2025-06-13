#include "Hosting.h"
#include <winreg.h>

typedef struct hostfxr_initialize_parameters INITPARAMS, *PINITPARAMS;

// Loads HostFXR and gets exports.
int GetHostFXR(LPWSTR path)
{
	mod = LoadLibraryExW(path , NULL , 8);
	if (mod == NULL) { return 1; }
	initfptr = (hostfxr_initialize_for_dotnet_command_line_fn)GetProcAddress(mod, "hostfxr_initialize_for_dotnet_command_line");
	if (initfptr == NULL) { return 2; }
	run_app = (hostfxr_run_app_fn)GetProcAddress(mod, "hostfxr_run_app");
	if (run_app == NULL) { return 3; }
	close_fptr = (hostfxr_close_fn)GetProcAddress(mod, "hostfxr_close");
	if (close_fptr == NULL) { return 4; }
	return 0;
}

// Registers app arguments to the runtime.
int RegisterAppArguments(int ARGC , char_t** ARGV , LPWSTR DOTNETPATH , LPWSTR HOSTPATH)
{
	runtimehand = NULL;
	PINITPARAMS params = (PINITPARAMS)SafeAllocate(sizeof(INITPARAMS));
	params->size = sizeof(INITPARAMS);
	params->dotnet_root = DOTNETPATH;
	params->host_path = HOSTPATH;
	if (initfptr(ARGC, ARGV, params, &runtimehand) != 0) { return 1; }
	SafeUnallocate(params);
	if (runtimehand == NULL) { return 1; }
	return 0;
}

PHOSTFXR_PATHS GetHostFXRPaths(LPWSTR directory)
{
	LPWSTR error;
	LPWSTR srcstring = FormatString(L"%s\\host\\fxr\\*" , directory);
	LPWIN32_FIND_DATAW fd = (LPWIN32_FIND_DATAW)SafeAllocate(sizeof(WIN32_FIND_DATAW));
	HANDLE hfd = FindFirstFileW(srcstring, fd);
	if (hfd == INVALID_HANDLE_VALUE) { goto G_EXIT_ERR; }
	int cnt = 0;
	while (FindNextFileW(hfd, fd)) {
		if (StringMatch(fd->cFileName, L"..") == FALSE && StringMatch(fd->cFileName, L".") == FALSE) { cnt++; }
	}
	FindClose(hfd);
	hfd = FindFirstFileW(srcstring, fd);
	if (hfd == INVALID_HANDLE_VALUE) { goto G_EXIT_ERR; }
	// Reinitialize find operation , and create the resulting structure.
	PHOSTFXR_PATHS ret = SafeAllocate(sizeof(HOSTFXR_PATHS));
	ret->Entries = SafeAllocate(cnt * sizeof(HOSTFXR_LIB_ENTRY));
	ret->Count = cnt;
	cnt = 0;
	while (FindNextFileW(hfd, fd)) 
	{
		if (StringMatch(fd->cFileName , L"..") == FALSE && StringMatch(fd->cFileName , L".") == FALSE) 
		{
			ret->Entries[cnt].Path = FormatString(L"%s\\host\\fxr\\%s\\hostfxr.dll", directory, fd->cFileName);
			ret->Entries[cnt].Version = CloneString(fd->cFileName);
			cnt++;
		}
	}
	FindClose(hfd);
	return ret;
G_EXIT_ERR:
	error = GetLastErrorMessage(); 
	PrintFormatted(L"The HostFXR enumeration failed: %s", error);
	UnallocateString(error);
	return (PHOSTFXR_PATHS)NULL;
}

void DestroyHostFXRPathsStruct(PHOSTFXR_PATHS paths)
{
	if (paths == NULL) { return; } // For pointer safety.
	for (int I = 0; I < paths->Count; I++)
	{
		// Unallocate each entry string pointer seperately.
		UnallocateString(paths->Entries[I].Path);
		UnallocateString(paths->Entries[I].Version);
	}
	// Free the array pointer itself
	SafeUnallocate(paths->Entries);
	SafeUnallocate(paths);
}

int GetCompatibleHostFXR(PHOSTFXR_PATHS paths, LPWSTR version)
{
	if (paths == NULL) { return -1; }
	PDECOMPOSED_VERSION ddc = GetDecomposedVersionFromString(version);
#if _DEBUG
	if (ddc == NULL)
	{
		DebugBreak();
	}
#endif
	if (ddc == NULL) { return -1; } 
	PDECOMPOSED_VERSION vcompare;
	for (int I = 0; I < paths->Count; I++)
	{
		vcompare = GetDecomposedVersionFromString(paths->Entries[I].Version);
		if (vcompare == NULL)
		{
#if _DEBUG
			DebugBreak();
#endif
			continue;
		}
		if (DecVersionIsGreaterThanOrEqualTo(vcompare, ddc))
		{
			SafeUnallocate(ddc);
			SafeUnallocate(vcompare);
			return I;
		}
		SafeUnallocate(vcompare);
	}
	SafeUnallocate(ddc);
	return -1;
}

PDOTNET_SYSTEMWIDE_INSTALLATION GetSystemWideDotNetInstallation(void)
{
	SYSTEM_INFO si;
	GetNativeSystemInfo(&si);
	LPWSTR pparch = (si.wProcessorArchitecture == PROCESSOR_ARCHITECTURE_IA64 || si.wProcessorArchitecture == PROCESSOR_ARCHITECTURE_AMD64) ? L"x64" : L"arm64";
	LPWSTR pformatted = FormatString(L"SOFTWARE\\dotnet\\Setup\\InstalledVersions\\%s\\sharedhost" , pparch);
	HKEY phk;
	LSTATUS ls = RegOpenKeyExW(HKEY_LOCAL_MACHINE, pformatted , 0 , GENERIC_READ, &phk);
	UnallocateString(pformatted);
	if (ls != ERROR_SUCCESS) { return NULL; }
	LPWSTR ppathdata = DefaultAllocateString();
	DWORD dused = TheoriticalUpperBound;
	ls = RegGetValueW(phk, NULL, L"Path", RRF_RT_REG_SZ, NULL, ppathdata, &dused);
	if (ls != ERROR_SUCCESS) { 
		UnallocateString(ppathdata);
		return NULL; 
	}
	LPWSTR pverdata = AllocateString(50);
	dused = 50;
	ls = RegGetValueW(phk, NULL, L"Version", RRF_RT_REG_SZ, NULL, pverdata, &dused);
	if (ls != ERROR_SUCCESS) {
		UnallocateString(pverdata);
		return NULL;
	}
	PDOTNET_SYSTEMWIDE_INSTALLATION pdt = SafeAllocate(sizeof(DOTNET_SYSTEMWIDE_INSTALLATION));
	pdt->FolderPath = CloneString(ppathdata);
	pdt->Version = CloneString(pverdata);
	UnallocateString(ppathdata);
	UnallocateString(pverdata);
	return pdt;
}

void DestroySystemWideInstallationStruct(PDOTNET_SYSTEMWIDE_INSTALLATION inst)
{
	if (inst == NULL) { return; }
	UnallocateString(inst->FolderPath);
	UnallocateString(inst->Version);
	SafeUnallocate(inst);
}

// Runs the specified app.
int RunApp(void)
{
 	return run_app(runtimehand);
}

// Disposes all the allocated data.
void DisposeRuntime(void)
{
	// Free routines were run , avoid doing the same again.
	if (mod == NULL) { return; }
	close_fptr(runtimehand);
	FreeLibrary(mod);
	mod = NULL;
}

LPVOID GetRuntimeHandle(void)
{
	return runtimehand;
}

