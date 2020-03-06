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

let createClient (requestJsonString:string) = 
    let factory = ConnectionFactory()
    factory.HostName <- "localhost" //todo: does F# have object initializer?
    let respQueue = new BlockingCollection<string>()
    let connection = factory.CreateConnection()
    let channel = connection.CreateModel()
    let replyQueueName = channel.QueueDeclare().QueueName
    let consumer = EventingBasicConsumer(channel)
    let props = channel.CreateBasicProperties();
    props.CorrelationId <- Guid.NewGuid().ToString()
    props.ReplyTo <- replyQueueName
    consumer.Received.Add(onMessageReceived respQueue props.CorrelationId)
    
    let msgBytes = Encoding.UTF8.GetBytes(requestJsonString)

    channel.BasicPublish(exchange=String.Empty, routingKey="employee", basicProperties=props, body=msgBytes)

    channel.BasicConsume(consumer = consumer, queue=replyQueueName, autoAck=true) |> ignore

    let rpcInfo = {
        connection = connection;
        channel = channel;
        consumer = consumer;
        respQueue = respQueue;
        props = props;
    }
    rpcInfo

//todo: look up RabbitMQ prod guidelines. (this code is based on the C# tutorial, which is not the best practice for prod)
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
    