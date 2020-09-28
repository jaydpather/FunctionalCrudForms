namespace RebelSoftware.EmployeeLogic
{
    public interface IEmployeeLogicService
    {
        Model.OperationResult ValidateEmployee(Model.Employee employee);
    }

    public static class EmployeeLogicServiceFactory
    {
        public static IEmployeeLogicService CreateEmployeeLogicService()
        {
            return new EmployeeLogicService();
        }
    }
}