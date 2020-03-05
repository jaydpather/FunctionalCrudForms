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
    bodyTask.Wait() //todo: research F# await syntax
    let body = bodyTask.Result

    let rpcInfo = createClient body
    
    let responseString = rpcInfo.respQueue.Take()
    httpContext.Response.Headers.["Access-Control-Allow-Origin"] <- Microsoft.Extensions.Primitives.StringValues("*")
    httpContext.Response.WriteAsync(responseString)
    