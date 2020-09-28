using System;

using NUnit.Framework;
using Moq;

using RebelSoftware.Logging;
using RebelSoftware.MessageQueue;
using RebelSoftware.Serialization;
using RebelSoftware.HttpService;
using RebelSoftware.EmployeeLogic;
using backend_crud_CSharp;

namespace backend_crud_CSharpTest
{
    [TestFixture]
    public class EmployeeControllerTest
    {
        private Mock<ILoggingService> _logger;
        private Mock<IMessageQueueService> _messageQueuer;
        private Mock<ISerializationService> _serializationService;
        private Mock<IHttpService> _httpService;
        private Mock<IEmployeeLogicService> _employeeLogicService;
        private EmployeeController _employeeController;

        [SetUp]
        public void Setup()
        {
            _logger = new Mock<ILoggingService>();
            _messageQueuer = new Mock<IMessageQueueService>();
            _serializationService = new Mock<ISerializationService>();
            _httpService = new Mock<IHttpService>();
            _employeeLogicService = new Mock<IEmployeeLogicService>();

            _employeeController = new EmployeeController(_logger.Object, _messageQueuer.Object, _serializationService.Object, _httpService.Object, _employeeLogicService.Object);
        }

        [Test]
        public void Create_WritesResponseWhenNoExceptionThrown()
        {
            _employeeLogicService.Setup(x => x.ValidateEmployee(It.IsAny<Model.Employee>())).Returns(new Model.OperationResult(Model.ValidationResults.Success));

            _employeeController.Create();
            
            _httpService.Verify(h => h.WriteHttpResponse(It.IsAny<string>()), Times.Once());
            _logger.Verify(l => l.LogException(It.IsAny<Exception>()), Times.Never());
        }
    }
}