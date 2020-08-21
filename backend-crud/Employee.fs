module Employee

open Microsoft.AspNetCore.Http
open RabbitMQ.Client
open RabbitMQ.Client.Events
open System.Collections.Concurrent
open System.Text
open System
open System.Collections.Generic
open System.IO
open Microsoft.FSharp.Control
open Model
open Validation
open System.Runtime.Serialization.Json

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
    let jsonSerializer = DataContractJsonSerializer(typedefof<Employee>)
    use stream:MemoryStream = new MemoryStream(System.Text.Encoding.ASCII.GetBytes jsonString)
    let employee = jsonSerializer.ReadObject(stream) :?> Employee
    employee

let publishToMsgQueue rpcInfo (jsonString:string) = 
    let msgBytes = Encoding.UTF8.GetBytes(jsonString)
    rpcInfo.channel.BasicPublish(exchange=String.Empty, routingKey="employee", basicProperties=rpcInfo.props, body=msgBytes) //todo: routing key from config file

    rpcInfo.channel.BasicConsume(consumer = rpcInfo.consumer, queue=rpcInfo.channel.QueueDeclare().QueueName, autoAck=true)

let createClient (requestJsonString:string) = 
    let employee = deserializeEmployeeFromJson requestJsonString
    let validationResult = Validation.validateFirstName employee.Name
    let rpcInfo = createRpcInfoObject ()
    
    match validationResult = ValidationResults.Success with 
        | true -> //todo: why do we need successValue? (it doesn't compile if you reference ValidationResults.Success in the pattern match)
            publishToMsgQueue rpcInfo requestJsonString |> ignore
            // let msgBytes = Encoding.UTF8.GetBytes(requestJsonString)
            // rpcInfo.channel.BasicPublish(exchange=String.Empty, routingKey="employee", basicProperties=rpcInfo.props, body=msgBytes)

            // rpcInfo.channel.BasicConsume(consumer = rpcInfo.consumer, queue=rpcInfo.channel.QueueDeclare().QueueName, autoAck=true) |> ignore
            
        | false -> rpcInfo.respQueue.Add("{Status:Failure}")

    rpcInfo 

//use this function to return a success value without microservices running
let create_dummy (httpContext:HttpContext) = 
    httpContext.Response.Headers.["Access-Control-Allow-Origin"] <- Microsoft.Extensions.Primitives.StringValues("*")
    httpContext.Response.WriteAsync("{Status:'Success'}")

//todo: look up RabbitMQ prod guidelines. (this code is based on the C# tutorial, which is not the best practice for prod)
//todo: return error status to client if write to Rabbit MQ failed, or if microservice failed, or isn't running
let create (httpContext:HttpContext) =
    use reader = new StreamReader(httpContext.Request.Body)
    let bodyTask = reader.ReadToEndAsync()
    bodyTask.Wait()
    let body = bodyTask.Result
    //let body = bodyTask.GetAwaiter().GetResult() 
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

    let rpcInfo = createClient body
    
    let responseString = rpcInfo.respQueue.Take()
    httpContext.Response.Headers.["Access-Control-Allow-Origin"] <- Microsoft.Extensions.Primitives.StringValues("*")
    httpContext.Response.WriteAsync(responseString)
    