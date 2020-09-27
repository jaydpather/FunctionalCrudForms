using System;
using RebelSoftware.LoggingService;
using RebelSoftware.DataLayer;
using RebelSoftware.SerializationService;
using RebelSoftware.MessageQueue;

namespace EmployeeMicroservice_crud_CSharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Employee microservice-CSharp running");
            var logger = Logging.createLogger();
            try
            {
                var documentDbRepository = DocumentDb.createDocumentDbRepository ();
                var serializationService = Serialization.createSerializationService<Model.Employee>();

                var employeeMicroservice = new EmployeeMicroservice(logger, documentDbRepository);

                Func<string, object> onMessageReceived = (message) => {
                    var employee = serializationService.DeserializeFromJson.Invoke(message);
                    var opResult = employeeMicroservice.InsertEmployee(employee);
                    var objResult = (object)opResult;
                    return objResult;
                };

                var fSharpOnMessageReceived = Microsoft.FSharp.Core.FuncConvert.FromFunc(onMessageReceived);

                MessageQueueServer.startMessageQueueListener<object>(serializationService.SerializeToJson, fSharpOnMessageReceived);
            }
            catch(Exception ex)
            {
                logger.LogException.Invoke(ex);
            }
        }
    }
}
