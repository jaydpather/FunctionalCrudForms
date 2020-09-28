using System;

using Serilog;

namespace RebelSoftware.Logging
{
    //todo: rename Logger to LoggingService
    internal class Logger : ILoggingService 
    {
        internal void InitializeLogging()
        {
            Log.Logger = new LoggerConfiguration().MinimumLevel.Debug().WriteTo.File("logfile.log", rollingInterval: RollingInterval.Day).CreateLogger();
        }

        public void LogException(Exception ex)
        {
            Serilog.Log.Error(ex.ToString());
        }
    }
}
