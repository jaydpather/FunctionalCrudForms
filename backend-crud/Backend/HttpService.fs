namespace RebelSoftware.HttpService

open System.IO
open Microsoft.AspNetCore.Http
open Newtonsoft.Json

open Model

module Http =
    type HttpServer = {
        WriteHttpResponse : string -> System.Threading.Tasks.Task;
        ReadRequestBody : unit -> string;
        DeserializeEmployeeFromJson : string -> Employee; //todo: generic type param
        SerializeToJson : obj -> string;
    }

    let private deserializeEmployeeFromJson (jsonString:string) = 
        let employee = JsonConvert.DeserializeObject<Employee>(jsonString)
        employee

    let private serializeToJson object =
        let jsonString = JsonConvert.SerializeObject object
        jsonString

    let private readRequestBody (httpContext:HttpContext) = 
        use reader = new StreamReader(httpContext.Request.Body)
        let bodyTask = reader.ReadToEndAsync()
        bodyTask.Wait()
        let body = bodyTask.Result
        body

    let private writeHttpResponse (httpContext:HttpContext) responseString = 
        httpContext.Response.Headers.["Access-Control-Allow-Origin"] <- Microsoft.Extensions.Primitives.StringValues("*")
        httpContext.Response.WriteAsync(responseString)    

    let createHttpServer (httpContext:HttpContext) = 
        { 
            WriteHttpResponse = fun responseStr -> writeHttpResponse httpContext responseStr;
            ReadRequestBody = fun () -> readRequestBody httpContext;
            DeserializeEmployeeFromJson = fun jsonString -> deserializeEmployeeFromJson jsonString;
            SerializeToJson = serializeToJson;
        }