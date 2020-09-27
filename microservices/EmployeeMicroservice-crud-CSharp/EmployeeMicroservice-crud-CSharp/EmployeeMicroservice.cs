using System;

using RebelSoftware.MessageQueue;
using RebelSoftware.LoggingService;
using RebelSoftware.SerializationService;
using RebelSoftware.DataLayer;

namespace EmployeeMicroservice_crud_CSharp
{
    public class EmployeeMicroservice
    {
        private Logging.Logger _logger;
        private DocumentDb.DocumentDbRepository _documentDbRepository;

        EmployeeMicroservice(Logging.Logger logger, DocumentDb.DocumentDbRepository documentDbRepository)
        {
            _logger = logger;
            _documentDbRepository = documentDbRepository;
        }

        public Model.OperationResult InsertEmployee(Model.Employee employee)
        {
            try
            {
                _documentDbRepository.WriteToDb.Invoke(employee);
                return new Model.OperationResult(Model.ValidationResults.Success);
            }
            catch(Exception ex)
            {
                _logger.LogException.Invoke(ex);
                return new Model.OperationResult(Model.ValidationResults.UnknownError);
            }
        }
    }
}