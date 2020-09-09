namespace MicroserviceTest

open RebelSoftware.UnitTests.MicroserviceTests

module Program = 
    [<EntryPoint>]
    let main args = 
        //this entry point is the only way to debug unit tests
        let mainTest = MainTest.MainTest()
        mainTest.Setup()
        mainTest.InsertEmployee_DbException ()
        0