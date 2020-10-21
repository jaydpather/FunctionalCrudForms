module EmployeeController

open Microsoft.AspNetCore.Http
open Newtonsoft.Json
open System.Collections.Generic
open System.IO
open Microsoft.FSharp.Control
open Model
open Validation

open RebelSoftware.LoggingService
open RebelSoftware.MessageQueueService
open RebelSoftware.Serialization
open RebelSoftware.HttpService
open RebelSoftware.Logic

//use this function to return a success value without microservices running
let create_dummy (httpContext:HttpContext) = 
    httpContext.Response.Headers.["Access-Control-Allow-Origin"] <- Microsoft.Extensions.Primitives.StringValues("*")
    httpContext.Response.WriteAsync("{Status:'Success'}")


let handleException httpCtx ex = 
    try
        Logging.logException ex |> ignore //logging could fail due to full hard drive, or something
    finally    
        ()
    { ValidationResult = ValidationResults.UnknownError }

let rec handleOutput output = 
    match output with 
    | Output.MqWaitResponse(object) -> 
        object 
        |> Json.serialize
        |> MessageQueueing.writeMessageAndGetResponse
    | Output.MqWaitAndCall(object, fn) -> 
        object
        |> Json.serialize
        |> MessageQueueing.writeMessageAndGetResponse
        |> fn
        |> handleOutput
    | Output.JsonResponse(object) ->
        object
        |> Json.serialize        
    | Output.LogFatalError(ex) -> //todo: test this case
        ex
        |> handleException
        |> Json.serialize
    | Output.MqOutput(objMQ, objHttp) -> 
        System.NotImplementedException ()
        |> raise 
//*  implement test insert_invalidInput
//0. remove Output.LogFatal - we already catch exceptions in the controller
//2. implement MqWaitAndCall
//3. use decorator pattern for reusable try/catch block (or .NET, if it lets you write to response)

//todo: look up RabbitMQ prod guidelines. (this code is based on the C# tutorial, which is not the best practice for prod)
//todo: return error status to client if write to Rabbit MQ failed, or if microservice failed, or isn't running
//todo: shared logging layer between backend and microservices?
let create httpCtx =
    try
        Http.readRequestBody httpCtx
        |> Employee.insertAndCall
        |> handleOutput
        |> Http.writeHttpResponse httpCtx        
    with
    | ex -> 
        handleException httpCtx ex
        |> Json.serialize
        |> Http.writeHttpResponse httpCtx

    