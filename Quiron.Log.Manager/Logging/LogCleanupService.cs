using Microsoft.Extensions.Hosting;

namespace Quiron.Log.Manager.Logging
{
    public class LogCleanupService(string logSys = "logs", int days = 30) : BackgroundService
    {
        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    CleanupOldLogs();
                }
                catch { }

                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }
        }

        private void CleanupOldLogs()
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var logDirectory = Path.Combine(currentDirectory, logSys);

            if (Directory.Exists(logDirectory))
            {
                var files = Directory.GetFiles(logDirectory, "*.txt")
                                 .Select(f => new FileInfo(f))
                                 .OrderByDescending(f => f.CreationTime)
                                 .ToList();

                var filesToDelete = files.Skip(days);
                foreach (var file in filesToDelete)
                {
                    try
                    {
                        file.Delete();
                    }
                    catch { }
                }
            }
        }
    }
}