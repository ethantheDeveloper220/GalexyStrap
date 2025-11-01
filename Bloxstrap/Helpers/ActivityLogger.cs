using System.Windows.Media;

namespace Voidstrap.Helpers
{
    public class LogEntry
    {
        public string Timestamp { get; set; } = "";
        public LogLevel Level { get; set; }
        public string Message { get; set; } = "";
        
        public Brush BackgroundBrush => Level switch
        {
            LogLevel.Error => new SolidColorBrush(Color.FromArgb(20, 255, 0, 0)),
            LogLevel.Warning => new SolidColorBrush(Color.FromArgb(20, 255, 165, 0)),
            LogLevel.Success => new SolidColorBrush(Color.FromArgb(20, 0, 255, 0)),
            _ => Brushes.Transparent
        };

        public Brush LevelBrush => Level switch
        {
            LogLevel.Error => new SolidColorBrush(Color.FromRgb(220, 53, 69)),
            LogLevel.Warning => new SolidColorBrush(Color.FromRgb(255, 193, 7)),
            LogLevel.Success => new SolidColorBrush(Color.FromRgb(40, 167, 69)),
            LogLevel.Info => new SolidColorBrush(Color.FromRgb(23, 162, 184)),
            _ => new SolidColorBrush(Color.FromRgb(108, 117, 125))
        };
    }

    public enum LogLevel
    {
        Debug,
        Info,
        Success,
        Warning,
        Error
    }

    public static class ActivityLogger
    {
        public static event Action<LogEntry>? LogAdded;
        private static readonly object _lock = new object();

        public static void Log(string message, LogLevel level = LogLevel.Info)
        {
            lock (_lock)
            {
                var entry = new LogEntry
                {
                    Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                    Level = level,
                    Message = message
                };

                LogAdded?.Invoke(entry);
            }
        }

        // Convenience methods
        public static void LogInfo(string message) => Log(message, LogLevel.Info);
        public static void LogSuccess(string message) => Log(message, LogLevel.Success);
        public static void LogWarning(string message) => Log(message, LogLevel.Warning);
        public static void LogError(string message) => Log(message, LogLevel.Error);
        public static void LogDebug(string message) => Log(message, LogLevel.Debug);
    }
}
