module Employee

open Microsoft.AspNetCore.Http
open RabbitMQ.Client
open RabbitMQ.Client.Events
open System.Collections.Concurrent
open System.Text
open Newtonsoft.Json
open System
open System.Collections.Generic
open System.IO
open Microsoft.FSharp.Control
open Model
open Validation

open LoggingService
//todo: separate into message queue layer

type RPCInfo = {
    connection : IConnection;
    channel : IModel;
    consumer : EventingBasicConsumer;
    respQueue : BlockingCollection<string>;
    props : IBasicProperties;
}

let onMessageReceived (respQueue:BlockingCollection<string>) correlationId (ea:BasicDeliverEventArgs) = 
    let response = Encoding.UTF8.GetString(ea.Body)
    if(ea.BasicProperties.CorrelationId = correlationId) then
        respQueue.Add(response)

let createRpcInfoObject () = 
    let factory = ConnectionFactory()
    factory.HostName <- "localhost" //todo: does F# have object initializer? //todo: get host name from config file
    let respQueue = new BlockingCollection<string>()
    let connection = factory.CreateConnection()
    let channel = connection.CreateModel()
    let consumer = EventingBasicConsumer(channel)
    let props = channel.CreateBasicProperties();
    props.CorrelationId <- Guid.NewGuid().ToString()
    props.ReplyTo <- channel.QueueDeclare().QueueName
    consumer.Received.Add(onMessageReceived respQueue props.CorrelationId)
    
    {
        connection = connection;
        channel = channel;
        consumer = consumer;
        respQueue = respQueue;
        props = props;
    }

let deserializeEmployeeFromJson (jsonString:string) = 
    let employee = JsonConvert.DeserializeObject<Employee>(jsonString)
    employee

let publishToMsgQueue (jsonString:string) rpcInfo = 
    let msgBytes = Encoding.UTF8.GetBytes(jsonString)
    rpcInfo.channel.BasicPublish(exchange=String.Empty, routingKey="employee", basicProperties=rpcInfo.props, body=msgBytes) //todo: routing key from config file

    rpcInfo.channel.BasicConsume(consumer = rpcInfo.consumer, queue=rpcInfo.props.ReplyTo, autoAck=true)

//use this function to return a success value without microservices running
let create_dummy (httpContext:HttpContext) = 
    httpContext.Response.Headers.["Access-Control-Allow-Origin"] <- Microsoft.Extensions.Primitives.StringValues("*")
    httpContext.Response.WriteAsync("{Status:'Success'}")

let readRequestBody (httpContext:HttpContext) = 
    use reader = new StreamReader(httpContext.Request.Body)
    let bodyTask = reader.ReadToEndAsync()
    bodyTask.Wait()
    let body = bodyTask.Result
    body

let writeHttpResponse (httpContext:HttpContext) responseString = 
    httpContext.Response.Headers.["Access-Control-Allow-Origin"] <- Microsoft.Extensions.Primitives.StringValues("*")
    httpContext.Response.WriteAsync(responseString)

//todo: look up RabbitMQ prod guidelines. (this code is based on the C# tutorial, which is not the best practice for prod)
//todo: return error status to client if write to Rabbit MQ failed, or if microservice failed, or isn't running
let create (httpContext:HttpContext) =
    try
        let requestJsonString = readRequestBody httpContext
        (*
            * todo: confirm with a load test whether or not this gives more scalability than doing bodyTask.Wait() 
              * internet's advice: bodyTask.GetAwaiter is more scalable b/c it frees up the current thread to do other stuff
              * Jayd's opinion: bodyTask.Wait() is more scalable b/c it DOESN'T create a new thread
                * .NET creates a new thread for each request
                  * this means other users never have to wait for this thread
                  * since we're on the back end, there's nothing else for this thread to do. (unlike the UI, where the window appears to be frozen b/c it can't be dragged while the UI thread is blocked)
                  * creating more threads means the server spends more time switching between threads
                  * if I wanted to run some other code while waiting to read the stream, I would run that code in another thread
        *)

        //todo2: try/catch and Success/Error. could receive invalid JSON
        //todo2: shared logging layer between backend and microservices?
        //todo1: move these 3 lines into bus layer func
        let employee = deserializeEmployeeFromJson requestJsonString
        let opResult = Validation.validateEmployee employee
        let responseStr = 
            match opResult.ValidationResult = ValidationResults.Success with 
                | true -> //todo: why do we need successValue? (it doesn't compile if you reference ValidationResults.Success in the pattern match)
                    //todo2: publishToMsgQueue should have a try/catch wrapper and return a Success/Error
                    //todo2: this method returns a Success<RpcInfo> or Error<Exception>
                    let rpcInfo = createRpcInfoObject () //todo 1: create MQ Layer, startup injects function pointer
                    rpcInfo
                    |> publishToMsgQueue requestJsonString 
                    |> ignore
                    rpcInfo.respQueue.Take() //todo: figure out how to handle a timeout here. (e.g., if microservice isn't running). currently, the front end just says "saving..." and hangs
                | false -> 
                    JsonConvert.SerializeObject(opResult)

        writeHttpResponse httpContext responseStr
    with
    | ex -> 
        LoggingService.logException ex |> ignore
        { ValidationResult = ValidationResults.UnknownError }
        |> JsonConvert.SerializeObject
        |> writeHttpResponse httpContext 

    