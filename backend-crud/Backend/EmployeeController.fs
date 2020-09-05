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
open RebelSoftware.HttpService.Http

//use this function to return a success value without microservices running
let create_dummy (httpContext:HttpContext) = 
    httpContext.Response.Headers.["Access-Control-Allow-Origin"] <- Microsoft.Extensions.Primitives.StringValues("*")
    httpContext.Response.WriteAsync("{Status:'Success'}")



//todo: look up RabbitMQ prod guidelines. (this code is based on the C# tutorial, which is not the best practice for prod)
//todo: return error status to client if write to Rabbit MQ failed, or if microservice failed, or isn't running
//todo: shared logging layer between backend and microservices?
let create (logger:Logging.Logger) (messageQueuer:MessageQueueing.MessageQueuer) (httpServer:HttpServer) (employeeValidator:EmployeeValidator) =
    let getResponseStr requestJsonString opResult = 
        match opResult.ValidationResult = ValidationResults.Success with 
            | true ->
                messageQueuer.WriteMessageAndGetResponse requestJsonString
            | false -> 
                opResult :> obj
                |> httpServer.SerializeToJson

    try
        //UNIT TEST: 
        //  * when body contains valid Employee object, we write the response from the message queue. 
        //  * when body contains validation errors, we write the OperationResult w/ expected validation errors to the response 
        //  * when body contains invalid JSON, we write an OperationResult w/ UnknownError to the response, and call logger.LogException
        let requestJsonString = httpServer.ReadRequestBody ()
        requestJsonString
        |> httpServer.DeserializeEmployeeFromJson 
        |> employeeValidator.ValidateEmployee
        |> (getResponseStr requestJsonString)
        |> httpServer.WriteHttpResponse
    with
    | ex -> 
        logger.LogException ex |> ignore
        { ValidationResult = ValidationResults.UnknownError }
        |> JsonConvert.SerializeObject
        |> httpServer.WriteHttpResponse 

    