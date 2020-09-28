using System;

using RebelSoftware.EmployeeLogic;
using RebelSoftware.HttpService;
using RebelSoftware.Logging;
using RebelSoftware.Serialization;
using RebelSoftware.MessageQueue;

namespace backend_crud_CSharp
{
    public class EmployeeController
    {
        private ILoggingService _logger;
        private IMessageQueueService _messageQueueService;
        private ISerializationService _serializationService;
        private IEmployeeLogicService _employeeLogicService;
        private IHttpService _httpService;
        public EmployeeController(ILoggingService logger, IMessageQueueService messageQueueService, ISerializationService serializationService, IHttpService httpService, IEmployeeLogicService employeeLogicService)
        {
            _logger = logger;
            _messageQueueService = messageQueueService;
            _serializationService = serializationService;
            _employeeLogicService = employeeLogicService;
            _httpService = httpService;
        }

        private string GetResponseString(string requestJsonString, Model.OperationResult operationResult)
        {
            string retVal = null;

            if(operationResult.ValidationResult == Model.ValidationResults.Success)
            {
                retVal = _messageQueueService.WriteMessageAndGetResponse(requestJsonString);
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
                var operationResult =  _employeeLogicService.ValidateEmployee(employee);
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