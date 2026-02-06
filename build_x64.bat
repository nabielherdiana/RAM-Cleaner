@echo off
echo ========================================
echo   RAM Cleaner Build Script (64-bit)
echo ========================================
echo.

set CSC64=%WINDIR%\Microsoft.NET\Framework64\v4.0.30319\csc.exe

if not exist "%CSC64%" (
    echo ERROR: C# compiler not found at:
    echo %CSC64%
    echo.
    echo Please ensure .NET Framework 4.0 is installed.
    pause
    exit /b 1
)

echo Building RAMCleaner_x64.exe...
echo.

"%CSC64%" /nologo /target:winexe /platform:x64 /out:RAMCleaner_x64.exe /win32manifest:app.manifest /reference:System.dll /reference:System.Drawing.dll /reference:System.Windows.Forms.dll /reference:Microsoft.VisualBasic.dll RAMCleaner.cs

if %ERRORLEVEL% EQU 0 (
    echo.
    echo ========================================
    echo   SUCCESS! Created: RAMCleaner_x64.exe
    echo ========================================
) else (
    echo.
    echo ========================================
    echo   BUILD FAILED!
    echo ========================================
)

echo.
pause
