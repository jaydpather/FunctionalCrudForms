﻿module Main

open System
open System.Collections.Generic
open System.Text

open RabbitMQ.Client
open MongoDB.Driver
open MongoDB.Bson
open Microsoft.FSharp.Reflection
open Newtonsoft.Json

type SuccessStatus = {
    Status: string;
}

type Employee = {
    Name: string;
}


type OperationResult = {
    ValidationResult: int;
}

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
    let database = client.GetDatabase("FunctionalCrudForms")
    let collection = database.GetCollection<BsonDocument>("Employee");
    //let record = { Value = 456 }
    let document = 
        record 
        |> convertToDictionary 
        |> BsonDocument

    collection.InsertOne(document) 
        |> ignore

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
        let message = Encoding.UTF8.GetString(body)

        let employeeObj = JsonConvert.DeserializeObject<Employee>(message)
        writeToMongo employeeObj


        let status = { ValidationResult = 0 } //todo: reference ValidationResults class from Model project of Fable folder. for now, assume 0 will always mean success
        let responseString = JsonConvert.SerializeObject status //"{\"data\":" + responseValue.ToString() + "}"
        // let validationResult = 0
        // let responseString = String.Format("{{ValidationResult:{0}}}", validationResult)
        // let status = { Status = "Success" }
        // let responseString = status.ToJson() //"{\"data\":" + responseValue.ToString() + "}"
        let responseBytes = Encoding.UTF8.GetBytes(responseString)
        let addr = PublicationAddress(exchangeName = "", exchangeType = ExchangeType.Direct, routingKey = props.ReplyTo)
        channel.BasicPublish(addr = addr, basicProperties = replyProps, body = responseBytes)

        printfn "received %s" message
        printfn "publishing: %s" responseString
    
[<EntryPoint>]
let main argv =
    printfn "Employee microservice running"
    let foo = MessageQueueLayer.MessageQueueLayer.foo()
    printfn "foo %i" foo
    startMsgQueueListener ()
    0 // return an integer exit code