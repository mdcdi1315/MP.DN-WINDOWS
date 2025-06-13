
#include "Network.h"

const LPWSTR FAILURE_NETWORK = L"Network API";
const LPWSTR FAILURE_FILEAPI = L"File API";

LPWSTR Internal_RetrieveLastError(LPWSTR failtype)
{
	LPWSTR msg1 = AllocateString(10000);
	FormatMessageW(FORMAT_MESSAGE_FROM_SYSTEM, 0, GetLastError(), 1033, msg1, 10000, NULL);
	LPWSTR msg2 = DefaultAllocateString();
	LPWSTR ret;
	LPDWORD erc = SafeAllocate(sizeof(DWORD));
	LPDWORD strlen = SafeAllocate(sizeof(DWORD));
	*strlen = TheoriticalUpperBound;
	if (InternetGetLastResponseInfoW(erc , msg2 , strlen) == TRUE) {
		ret = FormatString(L"%s: %s\nLast Response Info: (%d) %s" , failtype , msg1 , erc , msg2);
	} else {
		ret = FormatString(L"%s: %s", failtype, msg1);
	}
	SafeUnallocate(erc);
	SafeUnallocate(strlen);
	UnallocateString(msg1);
	UnallocateString(msg2);
	return ret;
}

// Initializes the Internet backend. 
// If an error was found , then the return value is not NULL.
LPWSTR InitializeNetwork(void)
{
	handle_one = InternetOpenW(L"Music Player on .NET 8 Native Host - mdcdi1315", INTERNET_OPEN_TYPE_DIRECT, L"", NULL, 0);
	if (handle_one == NULL) { return Internal_RetrieveLastError(FAILURE_NETWORK); }
	return NULL;
}

LPWSTR Request(LPWSTR url)
{
	reqhandle = InternetOpenUrlW(handle_one, url, L"", 0, 0, (DWORD_PTR)NULL);
	if (reqhandle == NULL) { return Internal_RetrieveLastError(FAILURE_NETWORK); }
	return NULL;
}

LPWSTR DownloadFileTo(LPWSTR path)
{
	LPWSTR current = AllocateString(10004);
	current[0] = '\\';
	current[1] = '\\';
	current[2] = '?';
	current[3] = '\\';
	AppendString(current, 4, path, StringLength(path) + 1);
	HANDLE hfile = CreateFileW(current, GENERIC_WRITE, 0, NULL, CREATE_ALWAYS, FILE_ATTRIBUTE_NORMAL, NULL);
	if (hfile == INVALID_HANDLE_VALUE) { 
		UnallocateString(current);
		return Internal_RetrieveLastError(FAILURE_FILEAPI);
	}
	UnallocateString(current);
	void* buffer = SafeAllocate(4096);
	LPDWORD br = SafeAllocate(sizeof(DWORD));
	while (InternetReadFile(reqhandle, buffer, 4096, br) == TRUE)
	{
		if (*br == 0) { break; } // Download completed
		if (WriteFile(hfile, buffer, *br, NULL, NULL) == FALSE)
		{
			SafeUnallocate(buffer);
			SafeUnallocate(br);
			CloseHandle(hfile);
			return Internal_RetrieveLastError(FAILURE_FILEAPI);
		}
	}
	SafeUnallocate(buffer);
	SafeUnallocate(br);
	CloseHandle(hfile);
	return NULL;
}

void TerminateTransaction(void)
{
	if (reqhandle != NULL) {
		InternetCloseHandle(reqhandle);
	}
	if (handle_one != NULL) {
		InternetCloseHandle(handle_one);
	}
}
