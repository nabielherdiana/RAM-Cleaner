using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Windows.Forms;

namespace RAMCleaner
{
    public class MainForm : Form
    {
        // Windows API imports
        [DllImport("kernel32.dll")]
        static extern bool SetProcessWorkingSetSize(IntPtr proc, int min, int max);

        [DllImport("kernel32.dll")]
        static extern IntPtr GetCurrentProcess();

        [DllImport("psapi.dll", SetLastError = true)]
        static extern bool EmptyWorkingSet(IntPtr hProcess);

        // UI Colors - Dark Theme
        private readonly Color bgColor = Color.FromArgb(26, 26, 46);
        private readonly Color cardColor = Color.FromArgb(22, 33, 62);
        private readonly Color accentColor = Color.FromArgb(100, 149, 237);
        private readonly Color textColor = Color.FromArgb(240, 240, 240);
        private readonly Color subTextColor = Color.FromArgb(150, 150, 170);
        private readonly Color successColor = Color.FromArgb(80, 200, 120);
        private readonly Color warningColor = Color.FromArgb(255, 193, 7);

        // UI Elements
        private Label titleLabel;
        private Label ramUsageLabel;
        private Label availableLabel;
        private Button cleanButton;
        private Label statusLabel;
        private ProgressBar progressBar;
        private Label adminLabel;
        private bool isAdmin;

        public MainForm()
        {
            isAdmin = IsRunAsAdmin();
            InitializeComponent();
            UpdateMemoryInfo();
        }

        private bool IsRunAsAdmin()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form settings
            this.Text = "RAM Cleaner";
            this.Size = new Size(340, 290);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = bgColor;
            this.Font = new Font("Segoe UI", 9F);

            // Title
            titleLabel = new Label()
            {
                Text = "RAM Cleaner",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = textColor,
                AutoSize = false,
                Size = new Size(320, 40),
                Location = new Point(10, 10),
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(titleLabel);

            // Admin status label
            adminLabel = new Label()
            {
                Text = isAdmin ? "[Administrator]" : "[User Mode]",
                Font = new Font("Segoe UI", 8F),
                ForeColor = isAdmin ? successColor : warningColor,
                AutoSize = false,
                Size = new Size(320, 16),
                Location = new Point(10, 45),
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(adminLabel);

            // Card panel for memory info
            Panel cardPanel = new Panel()
            {
                BackColor = cardColor,
                Size = new Size(300, 70),
                Location = new Point(20, 68)
            };
            this.Controls.Add(cardPanel);

            // RAM Usage
            ramUsageLabel = new Label()
            {
                Font = new Font("Segoe UI", 11F),
                ForeColor = textColor,
                AutoSize = false,
                Size = new Size(280, 25),
                Location = new Point(10, 12),
                TextAlign = ContentAlignment.MiddleCenter
            };
            cardPanel.Controls.Add(ramUsageLabel);

            // Available RAM
            availableLabel = new Label()
            {
                Font = new Font("Segoe UI", 10F),
                ForeColor = subTextColor,
                AutoSize = false,
                Size = new Size(280, 22),
                Location = new Point(10, 38),
                TextAlign = ContentAlignment.MiddleCenter
            };
            cardPanel.Controls.Add(availableLabel);

            // Clean Button
            cleanButton = new Button()
            {
                Text = "CLEAN RAM",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                Size = new Size(200, 45),
                Location = new Point(70, 150),
                FlatStyle = FlatStyle.Flat,
                BackColor = accentColor,
                ForeColor = Color.White,
                Cursor = Cursors.Hand
            };
            cleanButton.FlatAppearance.BorderSize = 0;
            cleanButton.Click += CleanButton_Click;
            cleanButton.MouseEnter += (s, e) => cleanButton.BackColor = Color.FromArgb(120, 169, 255);
            cleanButton.MouseLeave += (s, e) => cleanButton.BackColor = accentColor;
            this.Controls.Add(cleanButton);

            // Progress Bar
            progressBar = new ProgressBar()
            {
                Size = new Size(300, 3),
                Location = new Point(20, 205),
                Style = ProgressBarStyle.Continuous,
                Visible = false
            };
            this.Controls.Add(progressBar);

            // Status Label
            statusLabel = new Label()
            {
                Font = new Font("Segoe UI", 9F),
                ForeColor = successColor,
                AutoSize = false,
                Size = new Size(300, 40),
                Location = new Point(20, 212),
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(statusLabel);

            this.ResumeLayout(false);
        }

        private void UpdateMemoryInfo()
        {
            try
            {
                var pc = new PerformanceCounter("Memory", "Available MBytes");
                double availableMB = pc.NextValue();
                
                ulong totalMemory = new Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory;
                double totalGB = totalMemory / (1024.0 * 1024.0 * 1024.0);
                double availableGB = availableMB / 1024.0;
                double usedGB = totalGB - availableGB;
                
                ramUsageLabel.Text = String.Format("RAM Usage: {0:F1} GB / {1:F1} GB", usedGB, totalGB);
                availableLabel.Text = String.Format("Available: {0:F1} GB ({1:F0}%)", availableGB, availableGB/totalGB*100);
            }
            catch
            {
                ramUsageLabel.Text = "RAM Usage: Calculating...";
                availableLabel.Text = "";
            }
        }

        private double GetAvailableMemoryMB()
        {
            try
            {
                var pc = new PerformanceCounter("Memory", "Available MBytes");
                return pc.NextValue();
            }
            catch
            {
                return 0;
            }
        }

        private async void CleanButton_Click(object sender, EventArgs e)
        {
            cleanButton.Enabled = false;
            cleanButton.Text = "Cleaning...";
            progressBar.Visible = true;
            progressBar.Value = 0;
            statusLabel.Text = "";
            statusLabel.ForeColor = subTextColor;
            
            double beforeMB = GetAvailableMemoryMB();
            int cleaned = 0;
            int skipped = 0;

            await System.Threading.Tasks.Task.Run(() =>
            {
                Process[] processes = Process.GetProcesses();
                int total = processes.Length;
                int current = 0;

                foreach (Process proc in processes)
                {
                    try
                    {
                        current++;
                        int progress = (int)((current / (float)total) * 100);
                        this.Invoke((Action)(() => {
                            progressBar.Value = Math.Min(progress, 100);
                            statusLabel.Text = String.Format("Processing: {0}", proc.ProcessName);
                        }));

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

                // Clean our own process
                try
                {
                    EmptyWorkingSet(GetCurrentProcess());
                }
                catch { }
            });

            // Calculate freed memory
            System.Threading.Thread.Sleep(500);
            double afterMB = GetAvailableMemoryMB();
            double freedMB = afterMB - beforeMB;
            
            progressBar.Value = 100;
            progressBar.Visible = false;
            
            cleanButton.Text = "CLEAN RAM";
            cleanButton.Enabled = true;
            
            UpdateMemoryInfo();

            // Show result
            string resultText;
            if (freedMB > 0)
            {
                statusLabel.ForeColor = successColor;
                if (freedMB > 1024)
                    resultText = String.Format("Freed: {0:F2} GB ({1} procs)", freedMB/1024, cleaned);
                else
                    resultText = String.Format("Freed: {0:F0} MB ({1} procs)", freedMB, cleaned);
            }
            else
            {
                statusLabel.ForeColor = subTextColor;
                resultText = String.Format("Done! ({0} procs cleaned)", cleaned);
            }

            resultText += "\nCheck Task Manager (Ctrl+Shift+Esc)";
            statusLabel.Text = resultText;

            // Show Windows notification
            ShowNotification(freedMB, cleaned);
        }

        private void ShowNotification(double freedMB, int cleaned)
        {
            try
            {
                NotifyIcon notifyIcon = new NotifyIcon();
                notifyIcon.Icon = SystemIcons.Information;
                notifyIcon.Visible = true;
                
                string title = "RAM Cleaner - Complete!";
                string message;
                
                if (freedMB > 1024)
                    message = String.Format("Successfully freed {0:F2} GB RAM\n{1} processes cleaned", freedMB/1024, cleaned);
                else if (freedMB > 0)
                    message = String.Format("Successfully freed {0:F0} MB RAM\n{1} processes cleaned", freedMB, cleaned);
                else
                    message = String.Format("{0} processes cleaned", cleaned);

                message += "\n\nPress Ctrl+Shift+Esc to open Task Manager";

                notifyIcon.BalloonTipTitle = title;
                notifyIcon.BalloonTipText = message;
                notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
                notifyIcon.ShowBalloonTip(5000);

                // Clean up after delay
                Timer timer = new Timer();
                timer.Interval = 6000;
                timer.Tick += (s, ev) => {
                    notifyIcon.Visible = false;
                    notifyIcon.Dispose();
                    timer.Stop();
                    timer.Dispose();
                };
                timer.Start();
            }
            catch { }
        }

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
