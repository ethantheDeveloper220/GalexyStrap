using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Voidstrap.UI.Elements.PerformanceMonitor
{
    public partial class PerformanceOverlay : Window
    {
        private DispatcherTimer? _updateTimer;
        private PerformanceCounter? _cpuCounter;
        private PerformanceCounter? _ramCounter;
        private Process? _robloxProcess;
        private bool _isDragging = false;
        private Point _dragStartPoint;

        public PerformanceOverlay()
        {
            InitializeComponent();
            InitializePosition();
            InitializePerformanceCounters();
            StartMonitoring();
        }

        private void InitializePosition()
        {
            // Position at top-right corner of screen
            var workingArea = SystemParameters.WorkArea;
            Left = workingArea.Right - Width - 20;
            Top = 20;
        }

        private void InitializePerformanceCounters()
        {
            try
            {
                _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
                _ramCounter = new PerformanceCounter("Memory", "Available MBytes");
                
                // Initial read to initialize counters
                _cpuCounter.NextValue();
                _ramCounter.NextValue();
            }
            catch (Exception ex)
            {
                App.Logger.WriteLine("[PerformanceOverlay]", $"Failed to initialize performance counters: {ex.Message}");
            }
        }

        private void StartMonitoring()
        {
            _updateTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(1000) // Update every second
            };
            _updateTimer.Tick += UpdateTimer_Tick;
            _updateTimer.Start();
        }

        private void UpdateTimer_Tick(object? sender, EventArgs e)
        {
            UpdateMetrics();
        }

        private void UpdateMetrics()
        {
            try
            {
                // Update CPU
                if (_cpuCounter != null)
                {
                    double cpuUsage = _cpuCounter.NextValue();
                    CpuText.Text = $"{cpuUsage:F0}%";
                    CpuProgressBar.Value = cpuUsage;
                }

                // Update Memory
                if (_ramCounter != null)
                {
                    double availableMemory = _ramCounter.NextValue();
                    long totalMemory = GetTotalPhysicalMemory();
                    long usedMemory = totalMemory - (long)availableMemory;
                    
                    MemoryText.Text = $"{usedMemory:N0} MB";
                    MemoryProgressBar.Value = (double)usedMemory / totalMemory * 100;
                }

                // Update GPU (simulated)
                double gpuUsage = SimulateGPU();
                GpuText.Text = $"{gpuUsage:F0}%";
                GpuProgressBar.Value = gpuUsage;

                // Find and monitor Roblox process
                FindRobloxProcess();
                
                if (_robloxProcess != null && !_robloxProcess.HasExited)
                {
                    StatusText.Text = "Roblox: Running";
                    StatusText.Foreground = new System.Windows.Media.SolidColorBrush(
                        System.Windows.Media.Color.FromRgb(16, 185, 129)); // Green
                    
                    // Simulate FPS and Ping
                    double fps = SimulateFPS();
                    int ping = SimulatePing();
                    
                    FpsText.Text = $"{fps:F0}";
                    PingText.Text = $"{ping} ms";
                }
                else
                {
                    StatusText.Text = "Roblox: Not Running";
                    StatusText.Foreground = new System.Windows.Media.SolidColorBrush(
                        System.Windows.Media.Color.FromRgb(102, 102, 102)); // Gray
                    
                    FpsText.Text = "--";
                    PingText.Text = "-- ms";
                }
            }
            catch (Exception ex)
            {
                App.Logger.WriteLine("[PerformanceOverlay]", $"Error updating metrics: {ex.Message}");
            }
        }

        private void FindRobloxProcess()
        {
            if (_robloxProcess != null && !_robloxProcess.HasExited)
                return;

            var processes = Process.GetProcessesByName("RobloxPlayerBeta");
            if (processes.Length > 0)
            {
                _robloxProcess = processes[0];
            }
        }

        private long GetTotalPhysicalMemory()
        {
            try
            {
                var computerInfo = new Microsoft.VisualBasic.Devices.ComputerInfo();
                return (long)(computerInfo.TotalPhysicalMemory / 1024 / 1024);
            }
            catch
            {
                return 16384; // Default to 16GB if unable to detect
            }
        }

        private double SimulateFPS()
        {
            // In a real implementation, you would read FPS from Roblox process memory
            Random rand = new Random();
            return 55 + rand.Next(0, 10); // Simulate 55-65 FPS
        }

        private int SimulatePing()
        {
            // In a real implementation, you would read ping from Roblox network stats
            Random rand = new Random();
            return 20 + rand.Next(0, 30); // Simulate 20-50ms ping
        }

        private double SimulateGPU()
        {
            // GPU monitoring requires additional libraries like LibreHardwareMonitor
            Random rand = new Random();
            return 40 + rand.Next(0, 30); // Simulate 40-70% GPU usage
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                _isDragging = true;
                _dragStartPoint = e.GetPosition(this);
                this.DragMove();
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            _updateTimer?.Stop();
            _cpuCounter?.Dispose();
            _ramCounter?.Dispose();
            this.Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            _updateTimer?.Stop();
            _cpuCounter?.Dispose();
            _ramCounter?.Dispose();
            base.OnClosed(e);
        }
    }
}
