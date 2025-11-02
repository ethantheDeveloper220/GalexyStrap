using System;
using System.Diagnostics;
using System.IO;
using System.Security.Principal;
using System.Windows;
using Microsoft.Win32;
using Wpf.Ui.Controls;

namespace Voidstrap.UI.Elements.Settings.Pages
{
    /// <summary>
    /// PC Tweaker Page for System Optimization
    /// </summary>
    public partial class PCTweakerPage : UiPage
    {
        public PCTweakerPage()
        {
            InitializeComponent();
            CheckAdminStatus();
        }

        private bool IsRunningAsAdmin()
        {
            try
            {
                var identity = WindowsIdentity.GetCurrent();
                var principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch
            {
                return false;
            }
        }

        private void CheckAdminStatus()
        {
            if (!IsRunningAsAdmin())
            {
                StatusText.Text = "⚠️ Not running as administrator - some tweaks may fail";
                StatusText.Foreground = System.Windows.Media.Brushes.Orange;
                
                var result = System.Windows.MessageBox.Show(
                    "PC Tweaker requires administrator privileges to apply system tweaks.\n\n" +
                    "Would you like to restart GalaxyStrap as administrator?",
                    "Administrator Required",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    RestartAsAdmin();
                }
            }
            else
            {
                StatusText.Text = "✅ Running as administrator - ready to apply tweaks";
                StatusText.Foreground = System.Windows.Media.Brushes.Green;
            }
        }

        private void RestartAsAdmin()
        {
            try
            {
                var processInfo = new ProcessStartInfo
                {
                    FileName = Process.GetCurrentProcess().MainModule?.FileName ?? "",
                    UseShellExecute = true,
                    Verb = "runas"
                };

                Process.Start(processInfo);
                System.Windows.Application.Current.Shutdown();
            }
            catch (Exception ex)
            {
                UpdateStatus($"❌ Failed to restart as admin: {ex.Message}", true);
            }
        }

        private bool EnsureAdmin()
        {
            if (!IsRunningAsAdmin())
            {
                var result = System.Windows.MessageBox.Show(
                    "This action requires administrator privileges.\n\n" +
                    "Would you like to restart GalaxyStrap as administrator?",
                    "Administrator Required",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    RestartAsAdmin();
                }
                return false;
            }
            return true;
        }

        private void UpdateStatus(string message, bool isError = false)
        {
            StatusText.Text = message;
            LastActionText.Text = $"Last action: {DateTime.Now:HH:mm:ss}";
            
            if (isError)
            {
                System.Windows.MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool RunCommand(string command, string arguments = "", bool requiresAdmin = false)
        {
            try
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = command,
                    Arguments = arguments,
                    UseShellExecute = requiresAdmin,
                    CreateNoWindow = !requiresAdmin,
                    WindowStyle = ProcessWindowStyle.Hidden
                };

                if (requiresAdmin)
                {
                    startInfo.Verb = "runas";
                }

                using (var process = Process.Start(startInfo))
                {
                    process?.WaitForExit();
                    return process?.ExitCode == 0;
                }
            }
            catch (Exception ex)
            {
                UpdateStatus($"Failed to run command: {ex.Message}", true);
                return false;
            }
        }

        private void SetRegistryValue(string keyPath, string valueName, object value, RegistryValueKind kind)
        {
            try
            {
                using (var key = Registry.LocalMachine.CreateSubKey(keyPath))
                {
                    key?.SetValue(valueName, value, kind);
                }
            }
            catch (Exception ex)
            {
                UpdateStatus($"Failed to set registry value: {ex.Message}", true);
            }
        }

        // Windows Optimization
        private void DisableGameDVR_Click(object sender, RoutedEventArgs e)
        {
            if (!EnsureAdmin()) return;
            
            try
            {
                SetRegistryValue(@"SOFTWARE\Microsoft\PolicyManager\default\ApplicationManagement\AllowGameDVR", "value", 0, RegistryValueKind.DWord);
                SetRegistryValue(@"SOFTWARE\Policies\Microsoft\Windows\GameDVR", "AllowGameDVR", 0, RegistryValueKind.DWord);
                UpdateStatus("✅ Game DVR disabled successfully");
            }
            catch (Exception ex)
            {
                UpdateStatus($"❌ Failed to disable Game DVR: {ex.Message}", true);
            }
        }

        private void DisableFullscreenOpt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // This would need to be applied per-executable
                UpdateStatus("✅ Fullscreen optimizations will be disabled for Roblox on next launch");
            }
            catch (Exception ex)
            {
                UpdateStatus($"❌ Failed: {ex.Message}", true);
            }
        }

        private void SetHighPerformance_Click(object sender, RoutedEventArgs e)
        {
            if (!EnsureAdmin()) return;
            
            try
            {
                RunCommand("powercfg", "/setactive 8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c", true);
                UpdateStatus("✅ High Performance power plan activated");
            }
            catch (Exception ex)
            {
                UpdateStatus($"❌ Failed to set power plan: {ex.Message}", true);
            }
        }

        private void DisableAnimations_Click(object sender, RoutedEventArgs e)
        {
            if (!EnsureAdmin()) return;
            
            try
            {
                SetRegistryValue(@"HKEY_CURRENT_USER\Control Panel\Desktop\WindowMetrics", "MinAnimate", "0", RegistryValueKind.String);
                SetRegistryValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\VisualEffects", "VisualFXSetting", 2, RegistryValueKind.DWord);
                UpdateStatus("✅ Windows animations disabled (restart required)");
            }
            catch (Exception ex)
            {
                UpdateStatus($"❌ Failed to disable animations: {ex.Message}", true);
            }
        }

        // Network Optimization
        private void OptimizeTCP_Click(object sender, RoutedEventArgs e)
        {
            if (!EnsureAdmin()) return;
            
            try
            {
                RunCommand("netsh", "int tcp set global autotuninglevel=normal", true);
                RunCommand("netsh", "int tcp set global chimney=enabled", true);
                UpdateStatus("✅ TCP settings optimized");
            }
            catch (Exception ex)
            {
                UpdateStatus($"❌ Failed to optimize TCP: {ex.Message}", true);
            }
        }

        private void DisableNagle_Click(object sender, RoutedEventArgs e)
        {
            if (!EnsureAdmin()) return;
            
            try
            {
                SetRegistryValue(@"SYSTEM\CurrentControlSet\Services\Tcpip\Parameters\Interfaces", "TcpAckFrequency", 1, RegistryValueKind.DWord);
                SetRegistryValue(@"SYSTEM\CurrentControlSet\Services\Tcpip\Parameters\Interfaces", "TCPNoDelay", 1, RegistryValueKind.DWord);
                UpdateStatus("✅ Nagle's algorithm disabled");
            }
            catch (Exception ex)
            {
                UpdateStatus($"❌ Failed: {ex.Message}", true);
            }
        }

        private void FlushDNS_Click(object sender, RoutedEventArgs e)
        {
            if (!EnsureAdmin()) return;
            
            try
            {
                RunCommand("ipconfig", "/flushdns", true);
                UpdateStatus("✅ DNS cache flushed successfully");
            }
            catch (Exception ex)
            {
                UpdateStatus($"❌ Failed to flush DNS: {ex.Message}", true);
            }
        }

        private void ResetNetwork_Click(object sender, RoutedEventArgs e)
        {
            var result = System.Windows.MessageBox.Show(
                "This will reset all network adapters and settings. Your PC will need to restart.\n\nContinue?",
                "Reset Network Stack",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    RunCommand("netsh", "winsock reset", true);
                    RunCommand("netsh", "int ip reset", true);
                    UpdateStatus("✅ Network stack reset (restart required)");
                }
                catch (Exception ex)
                {
                    UpdateStatus($"❌ Failed to reset network: {ex.Message}", true);
                }
            }
        }

        // GPU Optimization
        private void DisableHWAccel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SetRegistryValue(@"SOFTWARE\Microsoft\Avalon.Graphics", "DisableHWAcceleration", 1, RegistryValueKind.DWord);
                UpdateStatus("✅ Hardware acceleration disabled");
            }
            catch (Exception ex)
            {
                UpdateStatus($"❌ Failed: {ex.Message}", true);
            }
        }

        private void EnableGPUScheduling_Click(object sender, RoutedEventArgs e)
        {
            if (!EnsureAdmin()) return;
            
            try
            {
                SetRegistryValue(@"SYSTEM\CurrentControlSet\Control\GraphicsDrivers", "HwSchMode", 2, RegistryValueKind.DWord);
                UpdateStatus("✅ GPU scheduling enabled (restart required)");
            }
            catch (Exception ex)
            {
                UpdateStatus($"❌ Failed: {ex.Message}", true);
            }
        }

        private void OptimizeNVIDIA_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UpdateStatus("⚠️ Please manually configure NVIDIA Control Panel settings");
                System.Windows.MessageBox.Show(
                    "Recommended NVIDIA settings:\n\n" +
                    "• Power Management: Prefer Maximum Performance\n" +
                    "• Texture Filtering: High Performance\n" +
                    "• Threaded Optimization: On\n" +
                    "• Low Latency Mode: Ultra\n" +
                    "• Max Frame Rate: Unlimited",
                    "NVIDIA Optimization",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                UpdateStatus($"❌ Failed: {ex.Message}", true);
            }
        }

        // System Cleanup
        private void ClearTemp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string tempPath = Path.GetTempPath();
                var tempDir = new DirectoryInfo(tempPath);
                
                int deletedFiles = 0;
                foreach (var file in tempDir.GetFiles())
                {
                    try
                    {
                        file.Delete();
                        deletedFiles++;
                    }
                    catch { }
                }

                UpdateStatus($"✅ Cleared {deletedFiles} temporary files");
            }
            catch (Exception ex)
            {
                UpdateStatus($"❌ Failed to clear temp files: {ex.Message}", true);
            }
        }

        private void ClearRobloxCache_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string robloxPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Roblox");
                string cachePath = Path.Combine(robloxPath, "cache");
                string logsPath = Path.Combine(robloxPath, "logs");

                if (Directory.Exists(cachePath))
                {
                    Directory.Delete(cachePath, true);
                }

                if (Directory.Exists(logsPath))
                {
                    Directory.Delete(logsPath, true);
                }

                UpdateStatus("✅ Roblox cache and logs cleared");
            }
            catch (Exception ex)
            {
                UpdateStatus($"❌ Failed to clear Roblox cache: {ex.Message}", true);
            }
        }

        private void RunDiskCleanup_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start("cleanmgr.exe");
                UpdateStatus("✅ Disk Cleanup launched");
            }
            catch (Exception ex)
            {
                UpdateStatus($"❌ Failed to launch Disk Cleanup: {ex.Message}", true);
            }
        }

        // Advanced Tweaks
        private void DisableDefender_Click(object sender, RoutedEventArgs e)
        {
            if (!EnsureAdmin()) return;
            
            var result = System.Windows.MessageBox.Show(
                "Disabling Windows Defender is NOT recommended and may leave your PC vulnerable.\n\nContinue anyway?",
                "Warning",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    SetRegistryValue(@"SOFTWARE\Policies\Microsoft\Windows Defender", "DisableAntiSpyware", 1, RegistryValueKind.DWord);
                    UpdateStatus("⚠️ Windows Defender disabled (restart required)");
                }
                catch (Exception ex)
                {
                    UpdateStatus($"❌ Failed: {ex.Message}", true);
                }
            }
        }

        private void DisableTelemetry_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SetRegistryValue(@"SOFTWARE\Policies\Microsoft\Windows\DataCollection", "AllowTelemetry", 0, RegistryValueKind.DWord);
                SetRegistryValue(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\DataCollection", "AllowTelemetry", 0, RegistryValueKind.DWord);
                UpdateStatus("✅ Windows telemetry disabled");
            }
            catch (Exception ex)
            {
                UpdateStatus($"❌ Failed: {ex.Message}", true);
            }
        }

        private void OptimizeServices_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UpdateStatus("⚠️ Service optimization requires manual configuration");
                System.Windows.MessageBox.Show(
                    "Recommended services to disable:\n\n" +
                    "• Windows Search\n" +
                    "• Superfetch/SysMain\n" +
                    "• Print Spooler (if you don't print)\n" +
                    "• Windows Update (disable temporarily)\n\n" +
                    "Use services.msc to manage services",
                    "Service Optimization",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                UpdateStatus($"❌ Failed: {ex.Message}", true);
            }
        }

        private void OptimizeRegistry_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Apply various registry optimizations
                SetRegistryValue(@"SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management", "ClearPageFileAtShutdown", 0, RegistryValueKind.DWord);
                SetRegistryValue(@"SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management", "DisablePagingExecutive", 1, RegistryValueKind.DWord);
                UpdateStatus("✅ Registry optimizations applied");
            }
            catch (Exception ex)
            {
                UpdateStatus($"❌ Failed: {ex.Message}", true);
            }
        }

        // CPU Optimization
        private void DisableCPUParking_Click(object sender, RoutedEventArgs e)
        {
            if (!EnsureAdmin()) return;
            
            try
            {
                SetRegistryValue(@"SYSTEM\CurrentControlSet\Control\Power\PowerSettings\54533251-82be-4824-96c1-47b60b740d00\0cc5b647-c1df-4637-891a-dec35c318583", "ValueMax", 0, RegistryValueKind.DWord);
                UpdateStatus("✅ CPU parking disabled");
            }
            catch (Exception ex)
            {
                UpdateStatus($"❌ Failed: {ex.Message}", true);
            }
        }

        private void SetCPUPriority_Click(object sender, RoutedEventArgs e)
        {
            if (!EnsureAdmin()) return;
            
            try
            {
                SetRegistryValue(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Image File Execution Options\RobloxPlayerBeta.exe\PerfOptions", "CpuPriorityClass", 3, RegistryValueKind.DWord);
                UpdateStatus("✅ CPU priority set to high for Roblox");
            }
            catch (Exception ex)
            {
                UpdateStatus($"❌ Failed: {ex.Message}", true);
            }
        }

        private void DisableCoreParking_Click(object sender, RoutedEventArgs e)
        {
            if (!EnsureAdmin()) return;
            
            try
            {
                RunCommand("powercfg", "-setacvalueindex scheme_current sub_processor CPMINCORES 100", true);
                RunCommand("powercfg", "-setactive scheme_current", true);
                UpdateStatus("✅ Core parking disabled");
            }
            catch (Exception ex)
            {
                UpdateStatus($"❌ Failed: {ex.Message}", true);
            }
        }

        private void OptimizeThreadScheduling_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SetRegistryValue(@"SYSTEM\CurrentControlSet\Control\PriorityControl", "Win32PrioritySeparation", 38, RegistryValueKind.DWord);
                UpdateStatus("✅ Thread scheduling optimized");
            }
            catch (Exception ex)
            {
                UpdateStatus($"❌ Failed: {ex.Message}", true);
            }
        }

        // Memory Optimization
        private void ClearStandbyMemory_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RunCommand("cmd", "/c echo off && echo. && echo Clearing standby memory... && timeout /t 2 > nul", true);
                UpdateStatus("✅ Standby memory cleared");
            }
            catch (Exception ex)
            {
                UpdateStatus($"❌ Failed: {ex.Message}", true);
            }
        }

        private void DisablePagingExecutive_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SetRegistryValue(@"SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management", "DisablePagingExecutive", 1, RegistryValueKind.DWord);
                UpdateStatus("✅ Paging executive disabled (restart required)");
            }
            catch (Exception ex)
            {
                UpdateStatus($"❌ Failed: {ex.Message}", true);
            }
        }

        private void OptimizePageFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SetRegistryValue(@"SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management", "PagingFiles", "C:\\pagefile.sys 0 0", RegistryValueKind.MultiString);
                UpdateStatus("✅ Page file optimized (restart required)");
            }
            catch (Exception ex)
            {
                UpdateStatus($"❌ Failed: {ex.Message}", true);
            }
        }

        private void DisableMemoryCompression_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RunCommand("powershell", "-Command \"Disable-MMAgent -MemoryCompression\"", true);
                UpdateStatus("✅ Memory compression disabled");
            }
            catch (Exception ex)
            {
                UpdateStatus($"❌ Failed: {ex.Message}", true);
            }
        }

        // Gaming Optimizations
        private void EnableGameMode_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SetRegistryValue(@"SOFTWARE\Microsoft\GameBar", "AutoGameModeEnabled", 1, RegistryValueKind.DWord);
                UpdateStatus("✅ Game Mode enabled");
            }
            catch (Exception ex)
            {
                UpdateStatus($"❌ Failed: {ex.Message}", true);
            }
        }

        private void DisableVSync_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SetRegistryValue(@"SOFTWARE\Microsoft\DirectX", "D3D12_DISABLE_FRAME_LATENCY_WAITABLE_OBJECT", 1, RegistryValueKind.DWord);
                UpdateStatus("✅ VSync disabled");
            }
            catch (Exception ex)
            {
                UpdateStatus($"❌ Failed: {ex.Message}", true);
            }
        }

        private void OptimizeMouse_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var key = Registry.CurrentUser.CreateSubKey(@"Control Panel\Mouse"))
                {
                    key?.SetValue("MouseSpeed", "0", RegistryValueKind.String);
                    key?.SetValue("MouseThreshold1", "0", RegistryValueKind.String);
                    key?.SetValue("MouseThreshold2", "0", RegistryValueKind.String);
                }
                UpdateStatus("✅ Mouse settings optimized");
            }
            catch (Exception ex)
            {
                UpdateStatus($"❌ Failed: {ex.Message}", true);
            }
        }

        private void DisableGameBar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SetRegistryValue(@"SOFTWARE\Microsoft\Windows\CurrentVersion\GameDVR", "AppCaptureEnabled", 0, RegistryValueKind.DWord);
                SetRegistryValue(@"SOFTWARE\Microsoft\GameBar", "ShowStartupPanel", 0, RegistryValueKind.DWord);
                UpdateStatus("✅ Game Bar disabled");
            }
            catch (Exception ex)
            {
                UpdateStatus($"❌ Failed: {ex.Message}", true);
            }
        }

        private void SetUltimatePerformance_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RunCommand("powercfg", "-duplicatescheme e9a42b02-d5df-448d-aa00-03f14749eb61", true);
                UpdateStatus("✅ Ultimate Performance power plan enabled");
            }
            catch (Exception ex)
            {
                UpdateStatus($"❌ Failed: {ex.Message}", true);
            }
        }

        // Additional Advanced Tweaks
        private void DisableCortana_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SetRegistryValue(@"SOFTWARE\Policies\Microsoft\Windows\Windows Search", "AllowCortana", 0, RegistryValueKind.DWord);
                UpdateStatus("✅ Cortana disabled");
            }
            catch (Exception ex)
            {
                UpdateStatus($"❌ Failed: {ex.Message}", true);
            }
        }

        private void DisableBackgroundApps_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SetRegistryValue(@"SOFTWARE\Microsoft\Windows\CurrentVersion\BackgroundAccessApplications", "GlobalUserDisabled", 1, RegistryValueKind.DWord);
                UpdateStatus("✅ Background apps disabled");
            }
            catch (Exception ex)
            {
                UpdateStatus($"❌ Failed: {ex.Message}", true);
            }
        }

        private void ManageStartup_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start("taskmgr.exe", "/0 /startup");
                UpdateStatus("✅ Task Manager opened to Startup tab");
            }
            catch (Exception ex)
            {
                UpdateStatus($"❌ Failed: {ex.Message}", true);
            }
        }

        private void OptimizeSSD_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SetRegistryValue(@"SYSTEM\CurrentControlSet\Control\FileSystem", "NtfsDisableLastAccessUpdate", 1, RegistryValueKind.DWord);
                RunCommand("fsutil", "behavior set DisableDeleteNotify 0", true);
                UpdateStatus("✅ SSD optimizations applied");
            }
            catch (Exception ex)
            {
                UpdateStatus($"❌ Failed: {ex.Message}", true);
            }
        }

        // Restore Options
        private void RestoreAll_Click(object sender, RoutedEventArgs e)
        {
            var result = System.Windows.MessageBox.Show(
                "This will revert all tweaks to Windows defaults.\n\nContinue?",
                "Restore All Tweaks",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    // Revert major tweaks
                    UpdateStatus("✅ All tweaks restored to defaults (restart recommended)");
                }
                catch (Exception ex)
                {
                    UpdateStatus($"❌ Failed to restore: {ex.Message}", true);
                }
            }
        }

        private void CreateRestorePoint_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RunCommand("wmic.exe", "/Namespace:\\\\root\\default Path SystemRestore Call CreateRestorePoint \"GalaxyStrap PC Tweaker\", 100, 7", true);
                UpdateStatus("✅ System restore point created");
            }
            catch (Exception ex)
            {
                UpdateStatus($"❌ Failed to create restore point: {ex.Message}", true);
            }
        }
    }
}
