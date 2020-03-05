namespace Backend

open System
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting

type Startup() =

    // This method gets called by the runtime. Use this method to add services to the container.
    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
    member this.ConfigureServices(services: IServiceCollection) =
        ()

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    member this.Configure(app: IApplicationBuilder, env: IWebHostEnvironment) =
        if env.IsDevelopment() then
            app.UseDeveloperExceptionPage() |> ignore

        app.UseRouting() |> ignore
        
        app.UseEndpoints(fun endpoints ->
             endpoints.MapGet("/employee/create", fun context -> Employee.create context) //todo: why do we get a compile error here if this is not a lambda? Can't see why the 2nd param can't just be RandomNumbers.insertOne, instead of a lambda that calls RandomNumbers.insertOne
            // endpoints.MapGet("/employee/create", fun (context:HttpContext) -> 
            //     printfn("test") |> ignore
            //     context.Response.Headers.["Access-Control-Allow-Origin"] <- Microsoft.Extensions.Primitives.StringValues("*")
            //     context.Response.WriteAsync("{\"status\":\"success\" }"
            //         ))
                |> ignore
            ) |> ignore    