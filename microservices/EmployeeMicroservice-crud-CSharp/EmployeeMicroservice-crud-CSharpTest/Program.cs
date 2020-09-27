using System;

namespace EmployeeMicroservice_crud_CSharpTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("debug UT entry point running");

            var test = new EmployeeMicroserviceTest();
            test.DummyTest();
        }
    }
}
