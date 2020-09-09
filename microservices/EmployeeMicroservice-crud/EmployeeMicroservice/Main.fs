module Main

open System

open Model
open RebelSoftware.MessageQueue
open RebelSoftware.LoggingService
open RebelSoftware.SerializationService
open RebelSoftware.DataLayer


let insertEmployee (logger:Logging.Logger) (documentDbRepository:DocumentDb.DocumentDbRepository)  employee = 
    let operationResult = 
        try //this try/with does not cover anything related to RabbitMQ. If there's an issue with RabbitMQ, we can't write a response back, so we might as well let the app crash.
            employee
            |> documentDbRepository.WriteToDb  //todo: try/catch, handle/log error
            { ValidationResult = ValidationResults.Success }
        with
        | ex -> 
            logger.LogException ex |> ignore
            { ValidationResult = ValidationResults.UnknownError }
    operationResult

let onMessageReceived (serializationService:Serialization.SerializationService<Employee>) insertEmployeeFn message = 
    serializationService.DeserializeFromJson message
    |> insertEmployeeFn
    |> fun opResult -> opResult :> obj


[<EntryPoint>]
let main argv =
    let logger = Logging.createLogger ()
    let repository = DocumentDb.createDocumentDbRepository ()
    let serializationService = Serialization.createSerializationService ()
    try
        printfn "Employee microservice running"
        insertEmployee logger repository
        |> onMessageReceived serializationService
        |> MessageQueueServer.startMessageQueueListener serializationService
        0
    with
    | ex -> 
        logger.LogException ex |> ignore
        -1 //let the app exit. container orchestration should restart the microservice.
     // return an integer exit code