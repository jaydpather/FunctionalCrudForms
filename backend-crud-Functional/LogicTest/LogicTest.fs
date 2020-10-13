namespace RebelSoftware.Logic

open System
open System.Threading.Tasks

open NUnit.Framework

module LogicTests =
    [<TestFixture>]
    type LogicTest () =
        let dummy = ""

        [<SetUp>]
        member this.Setup() = 
            ()

        [<Test>]
        member this.TestTest() = 
            Assert.IsTrue(true)
           