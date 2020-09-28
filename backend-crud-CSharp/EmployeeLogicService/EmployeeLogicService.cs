using System;

namespace RebelSoftware.EmployeeLogic
{
    internal class EmployeeLogicService : IEmployeeLogicService
    {
        public Model.OperationResult ValidateEmployee(Model.Employee employee)
        {
            var firstNameResult = String.IsNullOrWhiteSpace(employee.FirstName) ? Model.ValidationResults.FirstNameBlank : Model.ValidationResults.Success;

            var lastNameResult = String.IsNullOrWhiteSpace(employee.LastName) ? Model.ValidationResults.LastNameBlank : Model.ValidationResults.Success;

            var finalResult = firstNameResult | lastNameResult;
            var retVal = new Model.OperationResult(finalResult);
            return retVal;
        }
    }
}
