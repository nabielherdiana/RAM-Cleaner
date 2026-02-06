using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace RAMCleanerAuto
{
    class Program
    {
        [DllImport("kernel32.dll")]
        static extern bool SetProcessWorkingSetSize(IntPtr proc, int min, int max);

        [DllImport("kernel32.dll")]
        static extern IntPtr GetCurrentProcess();

        [DllImport("psapi.dll", SetLastError = true)]
        static extern bool EmptyWorkingSet(IntPtr hProcess);

        static bool IsRunAsAdmin()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        static double GetAvailableMemoryMB()
        {
            try
            {
                var pc = new PerformanceCounter("Memory", "Available MBytes");
                return pc.NextValue();
            }
            catch { return 0; }
        }

        static void Main(string[] args)
        {
            Console.Title = "RAM Cleaner";
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("========================================");
            Console.WriteLine("           RAM Cleaner v1.0");
            Console.WriteLine("========================================");
            Console.ResetColor();
            Console.WriteLine();

            // Check admin status
            bool isAdmin = IsRunAsAdmin();
            if (isAdmin)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[Administrator Mode]");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("[User Mode - Limited]");
            }
            Console.ResetColor();
            Console.WriteLine();

            // Get memory before
            double beforeMB = GetAvailableMemoryMB();
            Console.WriteLine("Starting RAM cleanup...");
            Console.WriteLine();

            // Clean RAM
            int cleaned = 0;
            int skipped = 0;
            Process[] processes = Process.GetProcesses();

            foreach (Process proc in processes)
            {
                try
                {
                    Console.Write(String.Format("\rProcessing: {0}                    ", proc.ProcessName.Length > 20 ? proc.ProcessName.Substring(0, 20) : proc.ProcessName));
                    IntPtr handle = proc.Handle;
                    if (SetProcessWorkingSetSize(handle, -1, -1))
                    {
                        cleaned++;
                    }
                }
                catch (System.ComponentModel.Win32Exception)
                {
                    skipped++;
                }
                catch { }
            }

            // Clean own process
            try { EmptyWorkingSet(GetCurrentProcess()); } catch { }

            // Wait for memory stats to update
            System.Threading.Thread.Sleep(500);

            // Get memory after
            double afterMB = GetAvailableMemoryMB();
            double freedMB = afterMB - beforeMB;

            // Show results
            Console.WriteLine("\r                                        ");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("========================================");
            Console.WriteLine("              COMPLETE!");
            Console.WriteLine("========================================");
            Console.ResetColor();
            Console.WriteLine();

            Console.WriteLine(String.Format("Processes cleaned: {0}", cleaned));
            if (skipped > 0)
                Console.WriteLine(String.Format("Processes skipped: {0}", skipped));
            Console.WriteLine();

            if (freedMB > 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                if (freedMB > 1024)
                    Console.WriteLine(String.Format("Memory freed: {0:F2} GB", freedMB / 1024));
                else
                    Console.WriteLine(String.Format("Memory freed: {0:F0} MB", freedMB));
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine("RAM optimized (no significant change)");
            }

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("Tip: Open Task Manager (Ctrl+Shift+Esc) to verify");
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey(true);
        }
    }
}
