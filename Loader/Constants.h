#pragma once

#include "framework.h"

// Defines common constants for the Loader project.
// Changing one string requires to change it's equivalent size constant too!

extern const LPWSTR MusicPlayerDLLNAME = (LPWSTR)L"MusicPlayer-impl.dll";
extern const LPWSTR HostFXRRelativePath = (LPWSTR)L"Runtime\\hostfxr.dll";
extern const LPWSTR DotnetDirectoryName = (LPWSTR)L"Runtime";
extern const size_t SIZE_DotnetDirectoryName = 8;
extern const size_t SIZE_HostFXRRelativePath = 20;
extern const size_t SIZE_MusicPlayerDLLNAME = 21;