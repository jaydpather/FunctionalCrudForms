using System;

namespace backend_crud_CSharpTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Custom UT entry point running...");
            var test = new EmployeeControllerTest();
            test.Setup();
            test.Create_DoesNotCallMQWhenEmployeeIsInvalid();
        }
    }
}
