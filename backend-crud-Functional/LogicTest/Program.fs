namespace RebelSoftware.Logic.EntryPoint

module Program = 
    [<EntryPoint>]
    let main args = 
        //this entry point is the only way to debug unit tests
        let testObj = RebelSoftware.Logic.Employee.LogicTest()
        testObj.insert_validInput()
        //testObj.TestTest()
        0