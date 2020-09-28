using System;

using NUnit.Framework;
using Moq;

using RebelSoftware.Logging;
using RebelSoftware.MessageQueue;
using RebelSoftware.Serialization;
using RebelSoftware.HttpService;
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
        private Validation.EmployeeValidator _employeeValidator;
        private EmployeeController _employeeController;

        [SetUp]
        public void Setup()
        {
            _logger = new Mock<ILoggingService>();
            _messageQueuer = new Mock<IMessageQueueService>();
            _serializationService = new Mock<ISerializationService>();
            _httpService = new Mock<IHttpService>();
            _employeeValidator = Validation.getEmployeeValidator();
            // _employeeValidator.ValidateEmployee += (employee) => {
            //     return new Model.OperationResult(Model.ValidationResults.Success);
            // };

            _employeeController = new EmployeeController(_logger.Object, _messageQueuer.Object, _serializationService.Object, _httpService.Object, _employeeValidator);
        }

        [Test]
        public void DummyTest()
        {
            
            _employeeController.Create();
            //logger.Object.LogException(new Exception());
            
            _logger.Verify(l => l.LogException(It.IsAny<Exception>()), Times.Once());
            //Assert.IsTrue(true);
        }
    }
}