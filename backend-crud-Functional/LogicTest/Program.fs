namespace RebelSoftware.Logic.EntryPoint

open RebelSoftware.Logic.LogicTests

module Program = 
    [<EntryPoint>]
    let main args = 
        //this entry point is the only way to debug unit tests
        let testObj = LogicTest()
        testObj.Setup()
        testObj.TestTest()
        0