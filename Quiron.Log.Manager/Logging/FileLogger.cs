using Microsoft.Extensions.Logging;

namespace Quiron.Log.Manager.Logging
{
    public class FileLogger(string logSys, string userName = "Logs") : ILogger
    {
        private static readonly Lock _lock = new();

        public IDisposable? BeginScope<TState>(TState state) => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception
            , Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel)) return;

            var logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{logLevel}] - {eventId.Id}|{eventId.Name}|{userName} - {formatter(state, exception)}{Environment.NewLine}";
            this.CreateLogDirectory(logMessage);
        }

        public void Log(LogLevel logLevel, EventId eventId, string message)
        {
            if (!IsEnabled(logLevel)) return;

            var logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{logLevel}] - {eventId.Id}|{eventId.Name}|{userName} - {message}{Environment.NewLine}";
            this.CreateLogDirectory(logMessage);
        }

        public void Log(LogLevel logLevel, string message)
        {
            if (!IsEnabled(logLevel)) return;

            var logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{logLevel}] - {0}| |{userName} - {message}{Environment.NewLine}";
            this.CreateLogDirectory(logMessage);
        }

        private void CreateLogDirectory(string logMessage)
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var logDirectory = Path.Combine(currentDirectory, logSys);
            var logFilePath = Path.Combine(logDirectory, $"log-{DateTime.Now:yyyy-MM-dd}.txt");

            Directory.CreateDirectory(logDirectory);

            lock (_lock)
            {
                File.AppendAllText(logFilePath, logMessage);
            }
        }
    }
}