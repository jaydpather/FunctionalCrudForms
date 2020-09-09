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
open RebelSoftware.SerializationService.Serialization


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

        let _serializationService =  createSerializationService<Employee> ()       

        let mutable _httpResponses = Unchecked.defaultof<string list>

        let writeHttpResponse str =
            let taskAction=  Action( fun () ->
                _httpResponses <- str :: _httpResponses)
            let task = new Task(taskAction)
            task.Start()
            task.Wait() //make sure the task is done by the time we reach assertions
            task            
        
        let mutable _httpServer = Unchecked.defaultof<HttpServer>

        let _defaultRequestBody = { FirstName = "Rajesh"; LastName = "Patel" }

        let createHttpServer readRequestBodyFn =
            _httpServer <- {
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
            fun () -> _defaultRequestBody |> serializeToJson
            |> createHttpServer 

        [<Test>]
        member this.ReturnsResponseFromMQWhenEmployeeIsValid() =
            let mockValidator = { 
                ValidateEmployee = fun employee -> { 
                    ValidationResult = ValidationResults.Success 
                } 
            }            
            EmployeeController.create _logger _messageQueuer _serializationService _httpServer mockValidator
            |> ignore
            Assert.AreEqual(1, _httpResponses.Length)
            Assert.True(_httpResponses.[0].Contains("response to")) //this means we got the response from the mock MQ service
            Assert.AreEqual(0, _logMessagesReceived.Length)

        [<Test>]
        member this.DoesNotSubmitToMQWhenEmployeeIsInValid() =
            let failureResults = [ ValidationResults.FirstNameBlank; ValidationResults.LastNameBlank; ValidationResults.UnknownError ] //note: each time we add a failure result, this list becomes out of date. //todo: find a way to fix this: use reflection to iterate over all ValidationResults? How do we filter out the Success cases and UI states (like Saving)?

            let testFn curFailureResult = 
                let opResult = { 
                    ValidationResult = curFailureResult
                } 
                let mockValidator = { 
                    ValidateEmployee = fun employee -> opResult
                }            
                EmployeeController.create _logger _messageQueuer _serializationService _httpServer mockValidator
                |> ignore
                
                Assert.AreEqual(1, _httpResponses.Length)
                Assert.False(_httpResponses.[0].Contains("response to")) //this means we did NOT get the response from the mock MQ service
                Assert.AreEqual(opResult |> serializeToJson, _httpResponses.[0]) //controller should write the OperationResult (w/ ValidationResult property) to HTTP response
                Assert.AreEqual(0, _logMessagesReceived.Length)

                _httpResponses <- [] //prepare for next loop iteration

            List.map testFn failureResults |> ignore

        [<Test>]
        member this.LogsMessageWhenExceptionThrown() =
            fun () -> "invalid JSON"
            |> createHttpServer
            
            let mockValidator = { 
                ValidateEmployee = fun employee -> { 
                    ValidationResult = ValidationResults.Success 
                } 
            }            
            EmployeeController.create _logger _messageQueuer _serializationService _httpServer mockValidator
            |> ignore
            Assert.AreEqual(1, _logMessagesReceived.Length)
            Assert.AreEqual(1, _httpResponses.Length)
            let expectedResponseObj = { ValidationResult = ValidationResults.UnknownError }
            let actualResponseObj = JsonConvert.DeserializeObject<OperationResult>(_httpResponses.[0])
            Assert.AreEqual(expectedResponseObj, actualResponseObj) //value equality, not physical equality

