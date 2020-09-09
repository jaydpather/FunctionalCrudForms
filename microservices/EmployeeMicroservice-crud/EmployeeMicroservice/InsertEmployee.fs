namespace RebelSoftware.Microservice

open Model
open RebelSoftware.MessageQueue
open RebelSoftware.LoggingService
open RebelSoftware.SerializationService
open RebelSoftware.DataLayer


module EmployeeMicroservice = 
    let insertEmployee (logger:Logging.Logger) (documentDbRepository:DocumentDb.DocumentDbRepository)  employee = 
        let operationResult = 
            try //this try/with does not cover anything related to RabbitMQ. If there's an issue with RabbitMQ, we can't write a response back, so we might as well let the app crash.
                employee
                |> documentDbRepository.WriteToDb  //todo: try/catch, handle/log error
                { ValidationResult = ValidationResults.Success }
            with
            | ex -> 
                logger.LogException ex |> ignore
                { ValidationResult = ValidationResults.UnknownError }
        operationResult

