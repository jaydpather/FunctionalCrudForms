namespace RebelSoftware.HttpService

open System.IO
open Microsoft.AspNetCore.Http
open Newtonsoft.Json

open Model

module Http =
    let readRequestBody (httpContext:HttpContext) = 
        (*
            * todo: confirm with a load test whether bodyTask.GetAwaiter() is more or less scalable than bodyTask.Wait() 
              * internet's advice: bodyTask.GetAwaiter is more scalable b/c it frees up the current thread to do other stuff
              * Jayd's opinion: bodyTask.Wait() is more scalable b/c it DOESN'T create a new thread
                * .NET creates a new thread for each request
                  * this means other users never have to wait for this thread
                  * since we're on the back end, there's nothing else for this thread to do. (unlike the UI, where the window appears to be frozen b/c it can't be dragged while the UI thread is blocked)
                  * creating more threads means the server spends more time switching between threads
                  * if I wanted to run some other code while waiting to read the stream, I would run that code in another thread
        *)
        use reader = new StreamReader(httpContext.Request.Body)
        let bodyTask = reader.ReadToEndAsync()
        bodyTask.Wait()
        let body = bodyTask.Result
        body

    let writeHttpResponse (httpContext:HttpContext) responseString = 
        httpContext.Response.Headers.["Access-Control-Allow-Origin"] <- Microsoft.Extensions.Primitives.StringValues("*")
        httpContext.Response.WriteAsync(responseString)    
