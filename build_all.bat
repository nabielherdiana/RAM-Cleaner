@echo off
echo ========================================
echo   RAM Cleaner - Build Both Versions
echo ========================================
echo.

set CSC32=%WINDIR%\Microsoft.NET\Framework\v4.0.30319\csc.exe
set CSC64=%WINDIR%\Microsoft.NET\Framework64\v4.0.30319\csc.exe

echo [1/2] Building 32-bit version...
"%CSC32%" /nologo /target:winexe /platform:x86 /out:RAMCleaner_x86.exe /win32manifest:app.manifest /reference:System.dll /reference:System.Drawing.dll /reference:System.Windows.Forms.dll /reference:Microsoft.VisualBasic.dll RAMCleaner.cs

if %ERRORLEVEL% EQU 0 (
    echo       OK - RAMCleaner_x86.exe
) else (
    echo       FAILED - 32-bit build
)

echo.
echo [2/2] Building 64-bit version...
"%CSC64%" /nologo /target:winexe /platform:x64 /out:RAMCleaner_x64.exe /win32manifest:app.manifest /reference:System.dll /reference:System.Drawing.dll /reference:System.Windows.Forms.dll /reference:Microsoft.VisualBasic.dll RAMCleaner.cs

if %ERRORLEVEL% EQU 0 (
    echo       OK - RAMCleaner_x64.exe
) else (
    echo       FAILED - 64-bit build
)

echo.
echo ========================================
echo   Build Complete!
echo ========================================
echo.
echo Files created:
dir /b *.exe 2>nul
echo.
pause
