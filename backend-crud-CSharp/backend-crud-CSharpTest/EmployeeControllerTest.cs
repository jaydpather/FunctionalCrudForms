using System;

using NUnit.Framework;
using Moq;

using RebelSoftware.Logging;

namespace backend_crud_CSharpTest
{
    [TestFixture]
    public class EmployeeControllerTest
    {
        [Test]
        public void DummyTest()
        {
            var logger = new Mock<ILoggingService>();

            logger.Object.LogException(new Exception());
            
            logger.Verify(l => l.LogException(It.IsAny<Exception>()), Times.Once());
            //Assert.IsTrue(true);
        }
    }
}