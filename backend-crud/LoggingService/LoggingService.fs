namespace RebelSoftware.LoggingService

open Serilog
open System

module Logging =
    
    type Logger = {
        LogException : Exception -> unit
    }

    let private initializeLogging () = 
        Log.Logger <- LoggerConfiguration().MinimumLevel.Debug()
                .WriteTo.File("logfile.log", rollingInterval = RollingInterval.Day) //todo: caller needs to pass in name of log file, which they will get from config file
                .CreateLogger()

    let private logException ex = 
        ex.ToString()
        |> Serilog.Log.Error

    let createLogger () = 
        initializeLogging()
        { LogException = logException }
