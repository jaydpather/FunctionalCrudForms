module Main

open System

open Model
open RebelSoftware.MessageQueue
open RebelSoftware.LoggingService
open RebelSoftware.DataLayer
open Newtonsoft.Json


let onMessageReceived (logger:Logging.Logger) (documentDbRepository:DocumentDb.DocumentDbRepository)  message = 
    let operationResult = 
        try //this try/with does not cover anything related to RabbitMQ. If there's an issue with RabbitMQ, we can't write a response back, so we might as well let the app crash.
            let employeeObj = JsonConvert.DeserializeObject<Employee>(message)
            employeeObj :> obj
            |> documentDbRepository.WriteToDb  //todo: try/catch, handle/log error
            { ValidationResult = ValidationResults.Success }
        with
        | ex -> 
            logger.LogException ex |> ignore
            { ValidationResult = ValidationResults.UnknownError }
    operationResult


[<EntryPoint>]
let main argv =
    let logger = Logging.createLogger ()
    try
        printfn "Employee microservice running"
        DocumentDb.createDocumentDbRepository ()
        |> onMessageReceived logger
        |> MessageQueueServer.startMessageQueueListener 
        0
    with
    | ex -> 
        logger.LogException ex |> ignore
        -1 //let the app exit. container orchestration should restart the microservice.
     // return an integer exit code