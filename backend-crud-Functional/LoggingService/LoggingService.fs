namespace RebelSoftware.LoggingService

open Serilog
open System

module Logging =
    let initializeLogging () = 
        Log.Logger <- LoggerConfiguration().MinimumLevel.Debug()
                .WriteTo.File("logfile.log", rollingInterval = RollingInterval.Day) //todo: caller needs to pass in name of log file, which they will get from config file
                .CreateLogger()

    let logException ex = 
        ex.ToString()
        |> Serilog.Log.Error
