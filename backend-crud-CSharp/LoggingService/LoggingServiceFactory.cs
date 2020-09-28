using System;

namespace RebelSoftware.Logging
{
    public interface ILoggingService
    {
        void LogException(Exception ex);
    }

    public static class LoggingServiceFactory
    {
        public static ILoggingService CreateLoggingService()
        {
            var logger = new Logger();
            logger.InitializeLogging();
            return logger;
        }
    }
}