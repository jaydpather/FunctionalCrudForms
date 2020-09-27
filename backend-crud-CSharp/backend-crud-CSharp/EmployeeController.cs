using RebelSoftware.LoggingService;
using RebelSoftware.MessageQueueService;
using RebelSoftware.SerializationService;
using RebelSoftware.HttpService;

namespace backend_crud_CSharp
{
    public class EmployeeController
    {
        private Logging.Logger _logger;
        private MessageQueueing.MessageQueuer _messageQueuer;
        private Serialization.SerializationService<Model.Employee> _serializationService;
        private Validation.EmployeeValidator _employeeValidator;
        private IHttpService _httpService;
        public EmployeeController(Logging.Logger logger, MessageQueueing.MessageQueuer messageQueuer, Serialization.SerializationService<Model.Employee> serializationService, IHttpService httpService, Validation.EmployeeValidator employeeValidator)
        {
            _logger = logger;
            _messageQueuer = messageQueuer;
            _serializationService = serializationService;
            _employeeValidator = employeeValidator;
            _httpService = httpService;
        }
    }
}