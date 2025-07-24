using Microsoft.Extensions.Logging;

namespace Quiron.Log.Manager.Logging
{
    public class FileLoggerProvider(string logSys = "logs", bool isParent = false, string userName = "System") 
        : ILoggerProvider
    {
        public ILogger CreateLogger(string categoryName)
        {
            return new FileLogger(logSys, isParent, userName);
        }

        protected virtual void Dispose(bool disposing) { }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}