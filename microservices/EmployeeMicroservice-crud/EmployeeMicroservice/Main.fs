module Main

open System

open Model
open RebelSoftware.MessageQueue
open RebelSoftware.LoggingService
open RebelSoftware.SerializationService
open RebelSoftware.DataLayer
open RebelSoftware.Microservice

let onMessageReceived (serializationService:Serialization.SerializationService<Employee>) insertEmployeeFn message = 
    serializationService.DeserializeFromJson message
    |> insertEmployeeFn
    |> fun opResult -> opResult :> obj


[<EntryPoint>]
let main argv =
    let logger = Logging.createLogger ()
    
    try
        printfn "Employee microservice running"
        let repository = DocumentDb.createDocumentDbRepository ()
        let serializationService = Serialization.createSerializationService ()
        EmployeeMicroservice.insertEmployee logger repository
        |> onMessageReceived serializationService
        |> MessageQueueServer.startMessageQueueListener serializationService
        0
    with
    | ex -> 
        logger.LogException ex |> ignore
        -1 //let the app exit. container orchestration should restart the microservice.
     // return an integer exit code