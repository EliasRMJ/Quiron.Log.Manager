using Microsoft.Extensions.Logging;
using System.Text;

namespace Quiron.Log.Manager.Logging
{
    public class FileLogger(string logSys, string userName = "system") : ILogger
    {
        private static readonly Lock _lock = new();

        public IDisposable? BeginScope<TState>(TState state) => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception
            , Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel)) return;

            var userLog = userName;
            var eventName = eventId.Name;
            if (!string.IsNullOrWhiteSpace(eventName))
            {
                var eventInfo = eventName.Split('#');
                eventName = eventInfo[0];
                if (eventInfo.Length > 1)
                    userLog = $"#{eventInfo[1]}";
            }

            var logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{logLevel}] - {eventId.Id}|{eventName}|{userLog} - {formatter(state, exception)}{Environment.NewLine}";
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
                File.AppendAllText(logFilePath, logMessage, new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));
            }
        }
    }
}