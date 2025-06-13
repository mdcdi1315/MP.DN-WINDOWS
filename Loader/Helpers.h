#pragma once

#include "framework.h"
#include "resource.h"
#include <strsafe.h>

// Maximum string length for all the MP Host functions.
extern const int TheoriticalUpperBound;

BOOL InitializeConsole(void);

void UninitializeConsole(void);

LPWSTR AllocateString(ULONGLONG length);

LPWSTR DefaultAllocateString(void);

void UnallocateString(LPWSTR string);

int StringLength(LPWSTR string);

/// <summary>
/// Creates and formats a string given the other arguments.
/// The returned string must be freed using the UnallocateString function.
/// </summary>
/// <param name="format">The format string to apply.</param>
/// <param name="">The format arguments.</param>
/// <returns>The formatted string.</returns>
LPWSTR FormatString(LPWSTR format, ...);

void AppendString(LPWSTR current, DWORD currentlength, LPWSTR append, DWORD appendlength);

void CopyString2(LPWSTR original, LPWSTR result, ULONGLONG ub);

void CopyString(LPWSTR original, LPWSTR result);

/// <summary>
/// Clones the current string pointer contents to a new string pointer.
/// You must free the returned string by using the UnallocateString function.
/// </summary>
/// <param name="original">The string pointer to copy.</param>
/// <returns>The cloned string.</returns>
LPWSTR CloneString(LPWSTR original);

void Print(LPWSTR string);

void PrintFormatted(LPWSTR format, ...);

void PauseConsoleInput(void);

BOOL StringMatch(LPWSTR source, LPWSTR match);

/// <summary>
/// Allocates and returns an RC resource string.
/// The resource string has a limitation of 980 characters in length.
/// </summary>
/// <param name="ID">The string ID to retrieve.</param>
/// <returns>The retrieved resource string. Must be freed by using the UnallocateString function.</returns>
LPWSTR GetResourceString(int ID);

void ShowMessageBox(LPWSTR message);

BOOL ShowQuestionMessageBox(LPWSTR message);

void ShowErrorMessageBox(LPWSTR message);

// Gets the last error message from a native Windows call.
// The returned string must be unallocated by using the UnallocateString function.
LPWSTR GetLastErrorMessage();

// Launches a new process with the specified command line.
// A boolean return value indicates whether the process has exited successfully or not.
BOOL RunProcess(LPWSTR CommandLine);

// Memory Heap Manipulation

HANDLE CreateProcessHeap(void);

void DestroyProcessHeap(void);

/// <summary>
/// SafeAllocate safely allocates memory blocks for you , without worrying about any errors that might happen
/// during allocation. If such an error occurs, the function guarantees to terminate the program.
/// </summary>
/// <param name="size">The number of bytes to allocate.</param>
/// <returns>The allocated memory.</returns>
LPVOID SafeAllocate(ULONGLONG size);

/// <summary>
/// SafeUnallocate safely unallocates memory blocks previously allocated by the SafeAllocate function.
/// If an error occurs, the function guarantees to terminate the program.
/// </summary>
/// <param name="fptr">The memory to unallocate.</param>
void SafeUnallocate(LPVOID fptr);

/// <summary>
/// SafeZeroMemory safely zeroizes the returned memory pointer from SafeAllocate.
/// </summary>
/// <param name="ptr">The memory block to be zeroized.</param>
/// <param name="size">The memory block's length in bytes to be zeroized.</param>
void SafeZeroMemory(LPVOID ptr, SIZE_T size);

// End Memory Heap Manipulation

// File Access API

/// <summary>
/// Gets the parent directory from a file path.
/// The new string returned must be unallocated by using the UnallocateString function.
/// </summary>
/// <param name="path">The file path to get it's parent directory. Can point to either a file or a directory.</param>
/// <returns>The parent directory of the current file/directory path</returns>
LPWSTR GetDirectoryFromPath(LPWSTR path);

BOOL FileExists(LPWSTR path);

BOOL DirectoryExists(LPWSTR path);

BOOL SubDirectoryExists(LPWSTR basepath, LPWSTR fdname);

BOOL DeleteExistingFile(LPWSTR path);

// End File Access API