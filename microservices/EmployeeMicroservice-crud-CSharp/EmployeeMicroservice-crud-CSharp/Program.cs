using System;
using RebelSoftware.LoggingService;
using RebelSoftware.DataLayer;
using RebelSoftware.SerializationService;

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
                throw new Exception("test exception");
                var documentDbRepository = DocumentDb.createDocumentDbRepository ();
                var serializationService = Serialization.createSerializationService<Model.Employee>();
            }
            catch(Exception ex)
            {
                logger.LogException.Invoke(ex);
            }
        }
    }
}
