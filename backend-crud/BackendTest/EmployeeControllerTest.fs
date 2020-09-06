namespace RebelSoftware.UnitTests

open System
open System.Threading.Tasks

open NUnit.Framework
open Newtonsoft.Json

open Model
open Validation
open RebelSoftware.LoggingService.Logging
open RebelSoftware.MessageQueueService.MessageQueueing
open RebelSoftware.HttpService.Http


module ControllerTests =
    [<TestFixture>]
    type EmployeeControllerTest () =
        let mutable _logMessagesReceived = Unchecked.defaultof<string list>
        let mutable _logger = Unchecked.defaultof<Logger>

        let mutable _messageQueue = Unchecked.defaultof<string list>
        let mutable _messageQueuer = Unchecked.defaultof<MessageQueuer>

        let mutable _httpResponses = Unchecked.defaultof<string list>
        let mutable _httpServer = Unchecked.defaultof<HttpServer>  

        //this function was purposely dup'd from HttpServer. todo: place this in a base class for all tests
        let deserializeEmployeeFromJson (jsonString:string) = 
            let employee = JsonConvert.DeserializeObject<Employee>(jsonString)
            employee

        //this function was purposely dup'd from HttpServer. todo: place this in a base class for all tests
        let serializeToJson object =
            let jsonString = JsonConvert.SerializeObject object
            jsonString

        let writeHttpResponse str =
            let taskAction=  Action( fun () ->
                _httpResponses <- str :: _httpResponses)
            let task = new Task(taskAction)
            task.Start()
            task            

        let createMockHttpServer readRequestBodyFn = {
            DeserializeEmployeeFromJson = deserializeEmployeeFromJson;
            SerializeToJson = serializeToJson;
            WriteHttpResponse = writeHttpResponse;
            ReadRequestBody = readRequestBodyFn;
        }

        [<SetUp>]
        member this.Setup() = 
            _logMessagesReceived <- []
            _logger <- { 
                LogException = fun ex -> 
                    _logMessagesReceived <- ex.ToString() :: _logMessagesReceived 
            }

            _messageQueue <- []
            _messageQueuer <- {
                WriteMessageAndGetResponse = fun str ->
                    _messageQueue <- str :: _messageQueue
                    sprintf "response to %s" str
            }

            _httpResponses <- []

        [<Test>]
        member this.TestMethodPassing() =
            let mockRequestBody = { ValidationResult = ValidationResults.Success } |> serializeToJson
            let httpServer = 
                fun () -> mockRequestBody
                |> createMockHttpServer 
            let mockValidator = { 
                ValidateEmployee = fun employee -> { 
                    ValidationResult = ValidationResults.Success 
                } 
            }            
            EmployeeController.create _logger _messageQueuer httpServer mockValidator
            |> ignore

            Assert.IsTrue(true)
    
