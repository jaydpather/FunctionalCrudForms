namespace RebelSoftware.UnitTests

open NUnit.Framework

module ControllerTests =
    [<TestFixture>]
    type TestClass () =

        [<Test>]
        member this.TestMethodPassing() =
            Assert.IsTrue(true)
    
