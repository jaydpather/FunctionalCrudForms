using System;
using System.Collections.Generic;

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
            InstantiateMocks();

            _employeeController = new EmployeeController(_logger.Object, _messageQueuer.Object, _serializationService.Object, _httpService.Object, _employeeLogicService.Object);
        }

        private void InstantiateMocks()
        {
            _logger = new Mock<ILoggingService>();
            _messageQueuer = new Mock<IMessageQueueService>();
            _serializationService = new Mock<ISerializationService>();
            _httpService = new Mock<IHttpService>();
            _employeeLogicService = new Mock<IEmployeeLogicService>();
        }

        [Test]
        public void Create_CallsMQWhenEmployeeIsValid()
        {
            _employeeLogicService.Setup(x => x.ValidateEmployee(It.IsAny<Model.Employee>())).Returns(new Model.OperationResult(Model.ValidationResults.Success));

            _employeeController.Create();
            
            _messageQueuer.Verify(m => m.WriteMessageAndGetResponse(It.IsAny<string>()), Times.Once());
            _httpService.Verify(h => h.WriteHttpResponse(It.IsAny<string>()), Times.Once());
            _logger.Verify(l => l.LogException(It.IsAny<Exception>()), Times.Never());
        }

        [Test]
        public void Create_DoesNotCallMQWhenEmployeeIsInvalid()
        {
            var failureResults = new int[]{ Model.ValidationResults.FirstNameBlank, Model.ValidationResults.LastNameBlank, Model.ValidationResults.UnknownError};

            foreach(var curFailureResult in failureResults)
            {
                InstantiateMocks(); //need to redo this so that our calls to Verify will work. (without this line, we'll count call from previous loop iterations)
                _employeeLogicService = new Mock<IEmployeeLogicService>();
                _employeeLogicService.Setup(x => x.ValidateEmployee(It.IsAny<Model.Employee>())).Returns(new Model.OperationResult(curFailureResult));
                _employeeController = new EmployeeController(_logger.Object, _messageQueuer.Object, _serializationService.Object, _httpService.Object, _employeeLogicService.Object);

                _employeeController.Create();

                _messageQueuer.Verify(m => m.WriteMessageAndGetResponse(It.IsAny<string>()), Times.Never());
                _httpService.Verify(h => h.WriteHttpResponse(It.IsAny<string>()), Times.Once());
                _logger.Verify(l => l.LogException(It.IsAny<Exception>()), Times.Never());
            }

        }

        [Test]
        public void Create_LogsMessageWhenExceptionThrown()
        {
            _employeeLogicService.Setup(x => x.ValidateEmployee(It.IsAny<Model.Employee>())).Throws(new Exception());

            _employeeController.Create();
            
            _messageQueuer.Verify(m => m.WriteMessageAndGetResponse(It.IsAny<string>()), Times.Never());
            _logger.Verify(l => l.LogException(It.IsAny<Exception>()), Times.Once());
            _httpService.Verify(h => h.WriteHttpResponse(It.IsAny<string>()), Times.Once());
        }
    }
}