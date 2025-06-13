
#include "Helpers.h"

const int TheoriticalUpperBound = 30000;
BOOL consoleenabled;
HANDLE stdouthand;
HANDLE procheap = NULL;

BOOL InitializeConsole(void)
{
	if (consoleenabled) { return TRUE; }
	consoleenabled = AllocConsole();
	if (stdouthand == NULL && consoleenabled)
	{
		stdouthand = GetStdHandle(STD_OUTPUT_HANDLE);
	}
	return consoleenabled;
}

void UninitializeConsole(void) { FreeConsole(); consoleenabled = FALSE; }

LPWSTR AllocateString(ULONGLONG length)
{
	if (length > 30000) { length = 30000; }
	ULONGLONG len = length * sizeof(WCHAR);
	LPVOID dt = SafeAllocate(len);
	SafeZeroMemory(dt , len);
	return (LPWSTR)dt;
}

LPWSTR DefaultAllocateString(void) {
	return AllocateString((ULONGLONG)TheoriticalUpperBound);
}

void UnallocateString(LPWSTR string)
{
	if (string == NULL) { return; }
	SafeUnallocate(string);
}

int StringLength(LPWSTR string)
{
	if (string == NULL) { return -1; }
	int I = 0;
	while (I < TheoriticalUpperBound && string[I] != L'\0') { I++; }
	return I;
}

LPWSTR FormatString(LPWSTR format, ...)
{
	LPWSTR result = AllocateString(TheoriticalUpperBound);
	va_list argList;
	va_start(argList, format);
	StringCchVPrintfW(result, TheoriticalUpperBound, format, argList);
	va_end(argList);
	free(argList);
	return result;
}

void AppendString(LPWSTR current, DWORD currentlength, LPWSTR append, DWORD appendlength)
{
	for (DWORD I = currentlength, J = 0; I < 30000 && J < appendlength; I++, J++)
	{
		current[I] = append[J];
		if (append[J] == L'\0') { break; }
	}
}

void CopyString2(LPWSTR original, LPWSTR result, ULONGLONG ub) {
	if (ub <= 0 || ub > TheoriticalUpperBound) { ub = TheoriticalUpperBound; }
	StringCchCopyW(result, ub, original);
}

void CopyString(LPWSTR original, LPWSTR result) {
	CopyString2(original, result, TheoriticalUpperBound);
}

LPWSTR CloneString(LPWSTR original)
{
	int len = StringLength(original) + 1;
	LPWSTR pret = AllocateString(len);
	StringCchCopyW(pret, len, original);
	return pret;
}

void Print(LPWSTR string)
{
	if (consoleenabled)
	{
		size_t length;
		if (StringCchLengthW(string, TheoriticalUpperBound, &length) != S_OK) { return; }
		WriteConsoleW(stdouthand, string, (DWORD)length, NULL, NULL);
	}
}

void PrintFormatted(LPWSTR format, ...)
{
	if (consoleenabled)
	{
		LPWSTR result = AllocateString(TheoriticalUpperBound);
		va_list argList;
		va_start(argList, format);
		StringCchVPrintfW(result, TheoriticalUpperBound, format, argList);
		va_end(argList);
		free(argList);
		size_t length;
		if (StringCchLengthW(result, TheoriticalUpperBound, &length) != S_OK) { return; }
		WriteConsoleW(stdouthand, result, (DWORD)length, NULL, NULL);
		UnallocateString(result);
	}
}

BOOL FileExists(LPWSTR path)
{
	WIN32_FIND_DATAW data;
	HANDLE EDT = FindFirstFileW(path, &data);
	if (EDT == NULL || EDT == INVALID_HANDLE_VALUE) {
		return FALSE;
	}
	else {
		FindClose(EDT);
		if (data.dwFileAttributes != -1) {
			return (data.dwFileAttributes & 0x10) == 0;
		}
	}
	return FALSE;
}

BOOL DirectoryExists(LPWSTR path)
{
	WIN32_FIND_DATAW data;
	HANDLE EDT = FindFirstFileW(path, &data);
	if (EDT == NULL || EDT == INVALID_HANDLE_VALUE) {
		return FALSE;
	} else {
		FindClose(EDT);
		if (data.dwFileAttributes != -1) {
			return data.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY;
		}
	}
	return FALSE;
}

BOOL SubDirectoryExists(LPWSTR basepath, LPWSTR fdname)
{
	LPWSTR subdir = FormatString(L"%s\\%s", basepath, fdname);
	BOOL ret = DirectoryExists(subdir);
	UnallocateString(subdir);
	return ret;
}

BOOL DeleteExistingFile(LPWSTR path)
{
	return DeleteFileW(path);
}

LPWSTR GetResourceString(int ID)
{
	LPWSTR result = AllocateString(980);
	LoadStringW(NULL, ID, result, 980);
	return result;
}

BOOL StringMatch(LPWSTR source, LPWSTR match)
{
	int slens = StringLength(source), slenm = StringLength(match);
	BOOL result = TRUE;
	for (int I = 0, J = 0; I < slens && J < slenm && I < TheoriticalUpperBound; I++, J++)
	{
		if (source[I] != match[J]) { result = FALSE; break; }
	}
	return result;
}

void ShowMessageBox(LPWSTR message)
{
	LPWSTR title = GetResourceString(IDS_HostTitle);
	MessageBoxW(NULL, message, title, 0);
	UnallocateString(title);
}

BOOL ShowQuestionMessageBox(LPWSTR message)
{
	LPWSTR title = GetResourceString(IDS_HostTitle);
	BOOL ret = MessageBoxW(NULL, message, title, MB_TASKMODAL | MB_ICONQUESTION | MB_YESNO) == IDYES;
	UnallocateString(title);
	return ret;
}

void ShowErrorMessageBox(LPWSTR message)
{
	LPWSTR title = GetResourceString(IDS_HostTitle);
	MessageBoxW(NULL, message, title, MB_TASKMODAL | MB_ICONERROR);
	UnallocateString(title);
}

LPWSTR GetLastErrorMessage()
{
	LPWSTR ret = AllocateString(10000);
	FormatMessageW(FORMAT_MESSAGE_FROM_SYSTEM, 0, GetLastError(), 1033, ret, 10000, NULL);
	return ret;
}

BOOL RunProcess(LPWSTR CommandLine)
{
	LPSTARTUPINFOW si = (LPSTARTUPINFOW)SafeAllocate(sizeof(STARTUPINFOW));
	si->cb = sizeof(STARTUPINFOW);
	si->lpTitle = L"PPI_MP-DOTNET8";
	LPPROCESS_INFORMATION pi = (LPPROCESS_INFORMATION)SafeAllocate(sizeof(PROCESS_INFORMATION));
	BOOL ret = CreateProcessW(NULL, CommandLine, NULL, NULL, FALSE, CREATE_NEW_PROCESS_GROUP, NULL, NULL, si, pi);
	LPDWORD dt = (LPDWORD)SafeAllocate(sizeof(DWORD));
	if (pi != NULL) {
		do {
			GetExitCodeProcess(pi->hProcess, dt);
			Sleep(300);
		} while (*dt == 259);
		if (pi->hProcess != NULL) { CloseHandle(pi->hProcess); }
		if (pi->hThread != NULL) { CloseHandle(pi->hThread); }
		SafeUnallocate(pi);
	}
	SafeUnallocate(si);
	if (*dt != 0) { ret = FALSE; }
	SafeUnallocate(dt);
	return ret;
}

HANDLE CreateProcessHeap(void)
{
	if (procheap != NULL) { return procheap; }
	procheap = HeapCreate(0, 5000, 0);
	if (procheap == NULL) { 
		MessageBoxW(NULL, L"Failed to create the private heap object for the process. MP Host must exit.", L"MP Allocation Error!", MB_TASKMODAL | MB_ICONERROR); 
		ExitProcess(277); 
	}
	return procheap;
}

void DestroyProcessHeap(void)
{
	if (procheap != NULL)
	{
		HeapDestroy(procheap);
	}
}

LPVOID SafeAllocate(ULONGLONG size)
{
	LPVOID ptr = HeapAlloc(CreateProcessHeap(), HEAP_ZERO_MEMORY, size);
	if (ptr == NULL) {  
		MessageBoxW(NULL, L"Failed to allocate a memory block for the process. MP Host must exit.", L"MP Allocation Error!", MB_TASKMODAL | MB_ICONERROR);
		ExitProcess(277);
	}
	return ptr;
}

void SafeUnallocate(LPVOID fptr)
{
	if (fptr == NULL) { return; }
	if (HeapFree(CreateProcessHeap(), 0, fptr) == FALSE)
	{
		MessageBoxW(NULL, L"Failed to unallocate a memory block for the process. MP Host must exit.", L"MP Allocation Error!", MB_TASKMODAL | MB_ICONERROR);
		ExitProcess(277);
	}
}

void SafeZeroMemory(LPVOID ptr, SIZE_T size)
{
	if (ptr == NULL) { return; } // If NULL do not do anything
	if (size < 0) { return; } // If negative do not do anything
	LPBYTE ppb = (LPBYTE)ptr;
	while (ppb < ((LPBYTE)ptr + size))
	{
		*ppb = 0;
		ppb++;
	}
}

void PauseConsoleInput(void)
{
	if (consoleenabled)
	{
		Print(L"Press Enter to continue...");
		LPVOID buf = SafeAllocate(1);
		DWORD nch;
		ReadConsoleW(GetStdHandle(STD_INPUT_HANDLE), buf, 1, &nch, NULL);
		SafeUnallocate(buf);
	}
}

LPWSTR GetDirectoryFromPath(LPWSTR path)
{
	DWORD charlength = StringLength(path);
	LPWSTR result = AllocateString(charlength);
	int position = -1;
	for (int I = charlength - 1; I >= 0; I--)
	{
		if (path[I] == '\\') { position = I; break; }
	}
	if (position == -1) { 
		CopyString2(path, result, charlength);
		return result;
	}
	
	for (int I = 0; I < position; I++)
	{
		result[I] = path[I];
	}
	return result;
}