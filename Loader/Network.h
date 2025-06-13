#pragma once

#include "framework.h"
#include "Helpers.h"
#include <wininet.h>

HINTERNET handle_one;
HINTERNET reqhandle;

LPWSTR InitializeNetwork(void);

LPWSTR Request(LPWSTR url);

LPWSTR DownloadFileTo(LPWSTR path);

void TerminateTransaction(void);