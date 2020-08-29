module Main

open System
open System.Collections.Generic
open System.Text

open RabbitMQ.Client
open MongoDB.Driver
open MongoDB.Bson
open Microsoft.FSharp.Reflection
open Newtonsoft.Json
open Serilog
open Model

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

let logError ex = 
    ex.ToString()
    |> Serilog.Log.Error

let startMsgQueueListener () = 
    let factory = ConnectionFactory()
    factory.HostName <- "localhost"
    use connection = factory.CreateConnection()
    use channel = connection.CreateModel()
    let queueResult = channel.QueueDeclare(queue = "employee", durable = false, exclusive = false, autoDelete = false, arguments = null)
    let consumer = QueueingBasicConsumer(channel)
    
    channel.BasicConsume(queue = "employee", autoAck = true, consumer = consumer) |> ignore
    let random = Random()
    while true do   
        let ea = consumer.Queue.Dequeue() //:> BasicDeliverEventArgs
        let body = ea.Body
        let props = ea.BasicProperties
        let replyProps = channel.CreateBasicProperties()
        replyProps.CorrelationId <- props.CorrelationId
        //todo 1:
        //  * move startMsgQueueListener to Message Queue layer
        //      * takes 1 param: business layer save function
        //  * extract the try/with into a new function: saveRecord
        //    * move into new business layer project
        //  * extract writeToMongo into new Data Layer
        //  * entry point (this file) passes the data layer func to bus layer, passes Bus Layer func to MessageQueueLayer
        let operationResult = 
            try //this try/with does not cover anything related to RabbitMQ. If there's an issue with RabbitMQ, we can't write a response back, so we might as well let the app crash.
                let message = Encoding.UTF8.GetString(body)
                let employeeObj = JsonConvert.DeserializeObject<Employee>(message)
                writeToMongo employeeObj //todo: try/catch, handle/log error
                { ValidationResult = ValidationResults.Success }
            with
            | ex -> 
                logError ex |> ignore
                { ValidationResult = ValidationResults.UnknownError }


        let responseString = JsonConvert.SerializeObject operationResult
        let responseBytes = Encoding.UTF8.GetBytes(responseString)
        let addr = PublicationAddress(exchangeName = "", exchangeType = ExchangeType.Direct, routingKey = props.ReplyTo)
        channel.BasicPublish(addr = addr, basicProperties = replyProps, body = responseBytes)

        //printfn "received %s" message
        //printfn "publishing: %s" responseString

[<EntryPoint>]
let main argv =
    try
        printfn "Employee microservice running"
        let foo = MessageQueueLayer.MessageQueueLayer.foo()
        printfn "foo %i" foo
        Log.Logger <- LoggerConfiguration().MinimumLevel.Debug()
                .WriteTo.File("logfile.log", rollingInterval = RollingInterval.Day)
                .CreateLogger()

        startMsgQueueListener ()
        0
    with
    | ex -> 
        logError ex |> ignore
        -1 //let the app exit. container orchestration should restart the microservice.
     // return an integer exit code