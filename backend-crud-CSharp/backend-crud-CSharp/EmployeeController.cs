using System;

using RebelSoftware.MessageQueueService;
using RebelSoftware.HttpService;

using RebelSoftware.Logging;
using RebelSoftware.Serialization;

namespace backend_crud_CSharp
{
    public class EmployeeController
    {
        private ILoggingService _logger;
        private MessageQueueing.MessageQueuer _messageQueuer;
        private ISerializationService _serializationService;
        private Validation.EmployeeValidator _employeeValidator;
        private IHttpService _httpService;
        public EmployeeController(ILoggingService logger, MessageQueueing.MessageQueuer messageQueuer, ISerializationService serializationService, IHttpService httpService, Validation.EmployeeValidator employeeValidator)
        {
            _logger = logger;
            _messageQueuer = messageQueuer;
            _serializationService = serializationService;
            _employeeValidator = employeeValidator;
            _httpService = httpService;
        }

        private string GetResponseString(string requestJsonString, Model.OperationResult operationResult)
        {
            string retVal = null;

            if(operationResult.ValidationResult == Model.ValidationResults.Success)
            {
                retVal = _messageQueuer.WriteMessageAndGetResponse.Invoke(requestJsonString);
            }
            else
            {
                retVal = _serializationService.SerializeToJson<Model.OperationResult>(operationResult);
            }
            
            return retVal;
        }

        public void Create()
        {
            try
            {
                var requestJsonString = _httpService.ReadRequestBody();
                var employee = _serializationService.DeserializeFromJson<Model.Employee>(requestJsonString);
                var operationResult =  _employeeValidator.ValidateEmployee.Invoke(employee);
                var responseStr = GetResponseString(requestJsonString, operationResult);
                _httpService.WriteHttpResponse(responseStr);
            }
            catch(Exception ex)
            {
                _logger.LogException(ex);
                var operationResult = new Model.OperationResult(Model.ValidationResults.UnknownError);
                var responseStr = _serializationService.SerializeToJson<Model.OperationResult>(operationResult);
                _httpService.WriteHttpResponse(responseStr);
            }
        }
    }
}