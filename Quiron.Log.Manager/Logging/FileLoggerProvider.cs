using Microsoft.Extensions.Logging;

namespace Quiron.Log.Manager.Logging
{
    public class FileLoggerProvider(string logSys = "logs", string userName = "System") 
        : ILoggerProvider
    {
        public ILogger CreateLogger(string categoryName)
        {
            return new FileLogger(logSys, userName);
        }

        protected virtual void Dispose(bool disposing) { }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}