module Main

open System
open System.Collections.Generic


open RabbitMQ.Client
open MongoDB.Driver
open MongoDB.Bson
open Microsoft.FSharp.Reflection

open Serilog
open Model

open RebelSoftware.MessageQueue
open RebelSoftware.LoggingService
open System.Text
open Newtonsoft.Json

let convertToDictionary (record) =
    seq {
        //todo: load test to see how slow Reflection is
        //  * cache FSharpType.GetRecordFields and check how fast it is
        //  * compare to hardcoded conversion to dictionary
        for prop in FSharpType.GetRecordFields(record.GetType()) -> 
        prop.Name, prop.GetValue(record)
    } |> dict

let writeToMongo record = 
    let client = MongoClient()
    let database = client.GetDatabase("FunctionalCrudForms") //todo: get db name from config file
    let collection = database.GetCollection<BsonDocument>("Employee"); //todo: get collection name from config file
    
    let document = 
        record 
        |> convertToDictionary 
        |> BsonDocument

    collection.InsertOne(document) 
        |> ignore

let onMessageReceived (logger:Logging.Logger) message = 
    let operationResult = 
        try //this try/with does not cover anything related to RabbitMQ. If there's an issue with RabbitMQ, we can't write a response back, so we might as well let the app crash.
            let employeeObj = JsonConvert.DeserializeObject<Employee>(message)
            writeToMongo employeeObj //todo: try/catch, handle/log error
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
        logger
        |> onMessageReceived 
        |> MessageQueueServer.startMessageQueueListener 
        0
    with
    | ex -> 
        logger.LogException ex |> ignore
        -1 //let the app exit. container orchestration should restart the microservice.
     // return an integer exit code