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

        //this function was purposely dup'd from HttpServer. todo: place this in a base class for all tests
        let deserializeEmployeeFromJson (jsonString:string) = 
            let employee = JsonConvert.DeserializeObject<Employee>(jsonString)
            employee

        //this function was purposely dup'd from HttpServer. todo: place this in a base class for all tests
        let serializeToJson object =
            let jsonString = JsonConvert.SerializeObject object
            jsonString

        let mutable _httpResponses = Unchecked.defaultof<string list>

        let writeHttpResponse str =
            let taskAction=  Action( fun () ->
                _httpResponses <- str :: _httpResponses)
            let task = new Task(taskAction)
            task.Start()
            task.Wait() //make sure the task is done by the time we reach assertions
            task            
        
        let mutable _httpServer = {
            DeserializeEmployeeFromJson = deserializeEmployeeFromJson;
            SerializeToJson = serializeToJson;
            WriteHttpResponse = writeHttpResponse;
            ReadRequestBody = fun () -> { FirstName = "Rajesh"; LastName = "Patel" } |> serializeToJson;
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
        member this.ReturnsResponseFromMQWhenEmployeeIsValid() =
            let mockValidator = { 
                ValidateEmployee = fun employee -> { 
                    ValidationResult = ValidationResults.Success 
                } 
            }            
            EmployeeController.create _logger _messageQueuer _httpServer mockValidator
            |> ignore
            Assert.AreEqual(1, _httpResponses.Length)
            Assert.True(_httpResponses.[0].Contains("response to")) //this means we got the response from the mock MQ service

        [<Test>]
        member this.DoesNotSubmitToMQWhenEmployeeIsInValid() =
            let failureResults = [ ValidationResults.FirstNameBlank; ValidationResults.LastNameBlank; ValidationResults.UnknownError ] //note: each time we add a failure result, this list becomes out of date

            let testFn curFailureResult = 
                let opResult = { 
                    ValidationResult = curFailureResult
                } 
                let mockValidator = { 
                    ValidateEmployee = fun employee -> opResult
                }            
                EmployeeController.create _logger _messageQueuer _httpServer mockValidator
                |> ignore
                
                Assert.AreEqual(1, _httpResponses.Length)
                Assert.False(_httpResponses.[0].Contains("response to")) //this means we did NOT get the response from the mock MQ service
                Assert.AreEqual(opResult |> serializeToJson, _httpResponses.[0])
                _httpResponses <- []

            List.map testFn failureResults |> ignore
