namespace Backend

open System
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting

open RebelSoftware.LoggingService
open RebelSoftware.MessageQueueService
open RebelSoftware.SerializationService
open RebelSoftware.HttpService

type Startup() =

    // This method gets called by the runtime. Use this method to add services to the container.
    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
    member this.ConfigureServices(services: IServiceCollection) =
        ()

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    member this.Configure(app: IApplicationBuilder, env: IWebHostEnvironment) =
        let logger = Logging.createLogger ()
        let messageQueuer = MessageQueueing.createMessageQueuer ()
        let serializationService = Serialization.createSerializationService ()
        let getHttpService httpContext = 
            Http.createHttpServer httpContext
        let employeeValidator = Validation.getEmployeeValidator ()        

        if env.IsDevelopment() then
            app.UseDeveloperExceptionPage() |> ignore

        app.UseRouting() |> ignore
        
        app.UseEndpoints(fun endpoints ->
            endpoints.MapPost("/employee/create", fun context -> EmployeeController.create logger messageQueuer serializationService (getHttpService context) employeeValidator) |> ignore
            //endpoints.MapPost("/employee/create", fun context -> Employee.create_dummy context)
              //todo: why do we get a compile error here if this is not a lambda? Can't see why the 2nd param can't just be RandomNumbers.insertOne, instead of a lambda that calls RandomNumbers.insertOne
            //todo: use generic exception page, for when we catch an Exception while writing a response                 
            ) |> ignore  