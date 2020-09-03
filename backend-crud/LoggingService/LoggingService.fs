namespace LoggingService

open Serilog

module LoggingService =
    let initializeLogging () = 
        Log.Logger <- LoggerConfiguration().MinimumLevel.Debug()
                .WriteTo.File("logfile.log", rollingInterval = RollingInterval.Day)
                .CreateLogger()

    let logException ex = 
        ex.ToString()
        |> Serilog.Log.Error