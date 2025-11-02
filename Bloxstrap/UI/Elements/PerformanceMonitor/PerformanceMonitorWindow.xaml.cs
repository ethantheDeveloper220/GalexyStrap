using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using Voidstrap.UI.Elements.Base;

namespace Voidstrap.UI.Elements.PerformanceMonitor
{
    public partial class PerformanceMonitorWindow : WpfUiWindow, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private DispatcherTimer? _monitorTimer;
        private PerformanceCounter? _cpuCounter;
        private PerformanceCounter? _ramCounter;
        private Process? _robloxProcess;
        
        private DateTime _sessionStartTime;
        private double _totalCpu;
        private int _cpuSamples;
        private long _peakMemory;
        
        private double _currentFps;
        private double _totalFps;
        private int _fpsSamples;
        private double _minFps = double.MaxValue;
        private double _maxFps;
        
        private int _currentPing;
        private long _totalPing;
        private int _pingSamples;

        private bool _alwaysOnTop;
        public bool AlwaysOnTop
        {
            get => _alwaysOnTop;
            set
            {
                _alwaysOnTop = value;
                OnPropertyChanged();
            }
        }

        public PerformanceMonitorWindow()
        {
            InitializeComponent();
            DataContext = this;
            InitializePerformanceCounters();
            AlwaysOnTop = App.Settings.Prop.PerformanceMonitorAlwaysOnTop;
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
                App.Logger.WriteLine("[PerformanceMonitor]", $"Failed to initialize performance counters: {ex.Message}");
            }
        }

        private void StartMonitoring_Click(object sender, RoutedEventArgs e)
        {
            StartMonitoring();
        }

        private void StopMonitoring_Click(object sender, RoutedEventArgs e)
        {
            StopMonitoring();
        }

        private void ResetStats_Click(object sender, RoutedEventArgs e)
        {
            ResetStatistics();
        }

        private void StartMonitoring()
        {
            if (_monitorTimer != null && _monitorTimer.IsEnabled)
                return;

            _sessionStartTime = DateTime.Now;
            _totalCpu = 0;
            _cpuSamples = 0;
            _peakMemory = 0;
            _totalFps = 0;
            _fpsSamples = 0;
            _minFps = double.MaxValue;
            _maxFps = 0;
            _totalPing = 0;
            _pingSamples = 0;

            _monitorTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(App.Settings.Prop.PerformanceMonitorUpdateInterval)
            };
            _monitorTimer.Tick += MonitorTimer_Tick;
            _monitorTimer.Start();

            StartButton.IsEnabled = false;
            StopButton.IsEnabled = true;
            
            StatusIndicator.Fill = new SolidColorBrush(Colors.LimeGreen);
            StatusText.Text = "Monitoring Active";

            App.Logger.WriteLine("[PerformanceMonitor]", "Monitoring started");
        }

        private void StopMonitoring()
        {
            if (_monitorTimer != null)
            {
                _monitorTimer.Stop();
                _monitorTimer = null;
            }

            StartButton.IsEnabled = true;
            StopButton.IsEnabled = false;
            
            StatusIndicator.Fill = new SolidColorBrush(Colors.Gray);
            StatusText.Text = "Monitoring Stopped";

            App.Logger.WriteLine("[PerformanceMonitor]", "Monitoring stopped");
        }

        private void ResetStatistics()
        {
            _totalCpu = 0;
            _cpuSamples = 0;
            _peakMemory = 0;
            _totalFps = 0;
            _fpsSamples = 0;
            _minFps = double.MaxValue;
            _maxFps = 0;
            _totalPing = 0;
            _pingSamples = 0;
            _sessionStartTime = DateTime.Now;

            SessionDurationText.Text = "00:00:00";
            AvgCpuText.Text = "0%";
            PeakMemoryText.Text = "0 MB";
            AvgFpsText.Text = "0";
            MinFpsText.Text = "0";
            MaxFpsText.Text = "0";
            AvgPingText.Text = "0 ms";
        }

        private void MonitorTimer_Tick(object? sender, EventArgs e)
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
                    CpuUsageText.Text = $"{cpuUsage:F1}%";
                    CpuProgressBar.Value = cpuUsage;
                    CpuDetailsText.Text = cpuUsage < 30 ? "Low Usage" : cpuUsage < 70 ? "Moderate Usage" : "High Usage";
                    
                    _totalCpu += cpuUsage;
                    _cpuSamples++;
                }

                // Update Memory
                if (_ramCounter != null)
                {
                    double availableMemory = _ramCounter.NextValue();
                    long totalMemory = GetTotalPhysicalMemory();
                    long usedMemory = totalMemory - (long)availableMemory;
                    
                    MemoryUsageText.Text = $"{usedMemory:N0} MB";
                    MemoryProgressBar.Value = (double)usedMemory / totalMemory * 100;
                    MemoryDetailsText.Text = $"{usedMemory:N0} MB / {totalMemory:N0} MB";
                    
                    if (usedMemory > _peakMemory)
                        _peakMemory = usedMemory;
                }

                // Find Roblox process
                FindRobloxProcess();

                // Update Roblox-specific metrics
                if (_robloxProcess != null && !_robloxProcess.HasExited)
                {
                    RobloxStatusText.Text = "✓ Roblox Running";
                    
                    // Simulate FPS (in real implementation, you'd read from Roblox memory or logs)
                    _currentFps = SimulateFPS();
                    FpsText.Text = $"{_currentFps:F0} FPS";
                    
                    _totalFps += _currentFps;
                    _fpsSamples++;
                    if (_currentFps < _minFps) _minFps = _currentFps;
                    if (_currentFps > _maxFps) _maxFps = _currentFps;
                    
                    AvgFpsText.Text = $"{(_totalFps / _fpsSamples):F0}";
                    MinFpsText.Text = $"{_minFps:F0}";
                    MaxFpsText.Text = $"{_maxFps:F0}";

                    // Simulate Ping
                    _currentPing = SimulatePing();
                    PingText.Text = $"{_currentPing} ms";
                    
                    _totalPing += _currentPing;
                    _pingSamples++;
                    AvgPingText.Text = $"{(_totalPing / _pingSamples)} ms";
                    PacketLossText.Text = "0%";
                }
                else
                {
                    RobloxStatusText.Text = "✗ Roblox Not Running";
                    FpsText.Text = "-- FPS";
                    PingText.Text = "-- ms";
                }

                // Update GPU (simulated - requires additional libraries for real GPU monitoring)
                double gpuUsage = SimulateGPU();
                GpuUsageText.Text = $"{gpuUsage:F1}%";
                GpuProgressBar.Value = gpuUsage;
                GpuDetailsText.Text = "GPU Monitoring (Simulated)";

                // Update Disk
                DiskUsageText.Text = "0 MB/s";
                DiskReadText.Text = "0 MB/s";
                DiskWriteText.Text = "0 MB/s";

                // Update Session Stats
                TimeSpan sessionDuration = DateTime.Now - _sessionStartTime;
                SessionDurationText.Text = $"{sessionDuration:hh\\:mm\\:ss}";
                AvgCpuText.Text = _cpuSamples > 0 ? $"{(_totalCpu / _cpuSamples):F1}%" : "0%";
                PeakMemoryText.Text = $"{_peakMemory:N0} MB";
            }
            catch (Exception ex)
            {
                App.Logger.WriteLine("[PerformanceMonitor]", $"Error updating metrics: {ex.Message}");
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
            // or parse Roblox logs. This is a simulation.
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

        protected override void OnClosing(CancelEventArgs e)
        {
            StopMonitoring();
            App.Settings.Prop.PerformanceMonitorAlwaysOnTop = AlwaysOnTop;
            base.OnClosing(e);
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
