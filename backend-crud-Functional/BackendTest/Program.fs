namespace BackendTest

open RebelSoftware.UnitTests.ControllerTests

module Program = 
    [<EntryPoint>]
    let main args = 
        //this entry point is the only way to debug unit tests
        let employeeControllerTest = EmployeeControllerTest()
        employeeControllerTest.Setup()
        employeeControllerTest.LogsMessageWhenExceptionThrown()
        0