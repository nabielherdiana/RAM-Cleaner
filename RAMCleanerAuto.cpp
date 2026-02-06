#include <iostream>
#include <windows.h>
#include <tchar.h>
#include <psapi.h>
#include <chrono>
#include <conio.h>

void SetWorkingSet(DWORD processID, int& success, int& failed)
{
    HANDLE hProcess = OpenProcess(PROCESS_QUERY_LIMITED_INFORMATION | 
        PROCESS_VM_READ | PROCESS_SET_LIMITED_INFORMATION | PROCESS_SET_QUOTA,
        FALSE, processID);

    if (NULL != hProcess)
    {
        if (SetProcessWorkingSetSize(hProcess, -1, -1) == 1) {
            success++;
        } else {
            failed++;
        }
        CloseHandle(hProcess);
    }
    else
    {
        failed++;
    }
}

MEMORYSTATUSEX GetMemoryInfo()
{
    MEMORYSTATUSEX memInfo;
    memInfo.dwLength = sizeof(MEMORYSTATUSEX);
    GlobalMemoryStatusEx(&memInfo);
    return memInfo;
}

int main()
{
    SetConsoleTitle(TEXT("RAM Cleaner"));
    
    // Header
    std::cout << "========================================" << std::endl;
    std::cout << "           RAM Cleaner v1.0" << std::endl;
    std::cout << "========================================" << std::endl;
    std::cout << std::endl;
    
    // Get memory before
    MEMORYSTATUSEX memBefore = GetMemoryInfo();
    double availableBefore = memBefore.ullAvailPhys / (1024.0 * 1024.0);
    double totalMB = memBefore.ullTotalPhys / (1024.0 * 1024.0);
    
    std::cout << "RAM Before: " << (totalMB - availableBefore) / 1024.0 << " GB / " << totalMB / 1024.0 << " GB" << std::endl;
    std::cout << std::endl;
    std::cout << "Cleaning RAM..." << std::endl;
    
    // Start timer
    auto start = std::chrono::steady_clock::now();
    
    // Get all processes
    DWORD aProcesses[1024], cbNeeded, cProcesses;
    int success = 0, failed = 0;

    if (EnumProcesses(aProcesses, sizeof(aProcesses), &cbNeeded))
    {
        cProcesses = cbNeeded / sizeof(DWORD);

        for (unsigned int i = 0; i < cProcesses; i++)
        {
            if (aProcesses[i] != 0)
            {
                SetWorkingSet(aProcesses[i], success, failed);
            }
        }
    }

    // End timer
    auto end = std::chrono::steady_clock::now();
    auto duration = std::chrono::duration_cast<std::chrono::milliseconds>(end - start).count();

    // Get memory after
    Sleep(500);
    MEMORYSTATUSEX memAfter = GetMemoryInfo();
    double availableAfter = memAfter.ullAvailPhys / (1024.0 * 1024.0);
    double freedMB = availableAfter - availableBefore;

    // Results
    std::cout << std::endl;
    std::cout << "========================================" << std::endl;
    std::cout << "              COMPLETE!" << std::endl;
    std::cout << "========================================" << std::endl;
    std::cout << std::endl;
    
    std::cout << "RAM After:  " << (totalMB - availableAfter) / 1024.0 << " GB / " << totalMB / 1024.0 << " GB" << std::endl;
    std::cout << "Processes cleaned: " << success << std::endl;
    std::cout << "Time: " << duration << " ms" << std::endl;
    std::cout << std::endl;

    if (freedMB > 0)
    {
        if (freedMB > 1024)
            std::cout << "Memory freed: " << freedMB / 1024.0 << " GB" << std::endl;
        else
            std::cout << "Memory freed: " << freedMB << " MB" << std::endl;
    }
    else
    {
        std::cout << "RAM optimized (no significant change)" << std::endl;
    }

    std::cout << std::endl;
    std::cout << "Tip: Open Task Manager (Ctrl+Shift+Esc) to verify" << std::endl;
    std::cout << std::endl;
    std::cout << "Press any key to exit...";
    _getch();
    
    return 0;
}
