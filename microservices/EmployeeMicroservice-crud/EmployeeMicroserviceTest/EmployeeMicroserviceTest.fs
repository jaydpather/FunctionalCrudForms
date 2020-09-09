namespace RebelSoftware.UnitTests.MicroserviceTests

open System

open NUnit.Framework

open Model
open RebelSoftware.MessageQueue
open RebelSoftware.LoggingService.Logging
open RebelSoftware.SerializationService
open RebelSoftware.DataLayer.DocumentDb
open RebelSoftware.Microservice

module MainTest = 
    [<TestFixture>]
    type MainTest () =
        //todo: reusable code for common mocking, like logger
        let mutable _logMessagesReceived = Unchecked.defaultof<string list>
        let mutable _logger = Unchecked.defaultof<Logger>

        let mutable _dbMessagesWritten = Unchecked.defaultof<string list>
        let mutable _documentDbRepository = Unchecked.defaultof<DocumentDbRepository>

        [<SetUp>]
        member this.Setup() = 
            _logMessagesReceived <- []
            _logger <- { 
                LogException = fun ex -> 
                    _logMessagesReceived <- ex.ToString() :: _logMessagesReceived 
            }

            _dbMessagesWritten <- []
            _documentDbRepository <- {
                WriteToDb = fun object -> 
                    _dbMessagesWritten <- object.ToString() :: _dbMessagesWritten
            }

        [<Test>]
        member this.InsertEmployee_Success() = 
            let opResult = 
                { FirstName = "John"; LastName = "Patel" }
                |> EmployeeMicroservice.insertEmployee _logger _documentDbRepository 

            Assert.AreEqual(ValidationResults.Success, opResult.ValidationResult)            
            Assert.AreEqual(1, _dbMessagesWritten.Length)
            Assert.IsTrue(_dbMessagesWritten.[0].Contains("John"))
            Assert.IsTrue(_dbMessagesWritten.[0].Contains("Patel"))
            Assert.AreEqual(0, _logMessagesReceived.Length)

        [<Test>]
        member this.InsertEmployee_DbException() = 
            _documentDbRepository <- {
                WriteToDb = fun object -> 
                    Exception("db call fails")
                    |> raise
            }

            let opResult = 
                { FirstName = "John"; LastName = "Patel" }
                |> EmployeeMicroservice.insertEmployee _logger _documentDbRepository 

            Assert.AreEqual(ValidationResults.UnknownError, opResult.ValidationResult)            
            Assert.AreEqual(0, _dbMessagesWritten.Length)
            Assert.AreEqual(1, _logMessagesReceived.Length)
