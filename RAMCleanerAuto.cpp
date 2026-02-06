#include <windows.h>
#include <stdio.h>
#include <psapi.h>
#include <conio.h>

// Link with psapi.lib
#pragma comment(lib, "psapi.lib")

void SetWorkingSet(DWORD processID, int* success, int* failed)
{
    HANDLE hProcess = OpenProcess(PROCESS_QUERY_LIMITED_INFORMATION | 
        PROCESS_VM_READ | PROCESS_SET_LIMITED_INFORMATION | PROCESS_SET_QUOTA,
        FALSE, processID);

    if (NULL != hProcess)
    {
        if (SetProcessWorkingSetSize(hProcess, (SIZE_T)-1, (SIZE_T)-1)) {
            (*success)++;
        } else {
            (*failed)++;
        }
        CloseHandle(hProcess);
    }
    else
    {
        (*failed)++;
    }
}

void PrintMemoryInfo(const char* label, double val, const char* unit, double total, const char* totalUnit) {
    if (total > 0) {
        printf("%s %.2f %s / %.2f %s\n", label, val, unit, total, totalUnit);
    } else {
        printf("%s %.2f %s\n", label, val, unit);
    }
}

int main()
{
    SetConsoleTitle("RAM Cleaner");
    
    // Header
    printf("========================================\n");
    printf("           RAM Cleaner v1.1\n");
    printf("========================================\n\n");
    
    // Get memory before
    MEMORYSTATUSEX memBefore;
    memBefore.dwLength = sizeof(MEMORYSTATUSEX);
    GlobalMemoryStatusEx(&memBefore);
    
    double availableBefore = (double)memBefore.ullAvailPhys / (1024.0 * 1024.0);
    double totalMB = (double)memBefore.ullTotalPhys / (1024.0 * 1024.0);
    
    PrintMemoryInfo("RAM Before:", (totalMB - availableBefore) / 1024.0, "GB", totalMB / 1024.0, "GB");
    printf("\nCleaning RAM...\n");
    
    // Start timer using simple tick count for minimal overhead
    DWORD start = GetTickCount();
    
    // Get all processes
    DWORD aProcesses[2048], cbNeeded, cProcesses;
    int success = 0, failed = 0;

    if (EnumProcesses(aProcesses, sizeof(aProcesses), &cbNeeded))
    {
        cProcesses = cbNeeded / sizeof(DWORD);

        for (unsigned int i = 0; i < cProcesses; i++)
        {
            if (aProcesses[i] != 0)
            {
                SetWorkingSet(aProcesses[i], &success, &failed);
            }
        }
    }

    // End timer
    DWORD end = GetTickCount();
    DWORD duration = end - start;

    // Get memory after
    Sleep(500);
    MEMORYSTATUSEX memAfter;
    memAfter.dwLength = sizeof(MEMORYSTATUSEX);
    GlobalMemoryStatusEx(&memAfter);
    
    double availableAfter = (double)memAfter.ullAvailPhys / (1024.0 * 1024.0);
    double freedMB = availableAfter - availableBefore;

    // Results
    printf("\n");
    printf("========================================\n");
    printf("              COMPLETE!\n");
    printf("========================================\n\n");
    
    PrintMemoryInfo("RAM After: ", (totalMB - availableAfter) / 1024.0, "GB", totalMB / 1024.0, "GB");
    printf("Processes cleaned: %d\n", success);
    printf("Time: %lu ms\n\n", duration);

    if (freedMB > 0)
    {
        if (freedMB > 1024)
            printf("Memory freed: %.2f GB\n", freedMB / 1024.0);
        else
            printf("Memory freed: %.2f MB\n", freedMB);
    }
    else
    {
        printf("RAM optimized (no significant change)\n");
    }

    printf("\nTip: Open Task Manager (Ctrl+Shift+Esc) to verify\n\n");
    printf("Press any key to exit...");
    _getch();
    
    return 0;
}
