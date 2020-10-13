namespace RebelSoftware.Logic

open System
open System.Threading.Tasks
open NUnit.Framework

open Model

module Employee =
    [<TestFixture>]
    type LogicTest () =
        [<SetUp>]
        member this.Setup() = 
            ()

        [<Test>]
        member this.produceResponse_valid() = 
            let employee = {
                FirstName = "abc";
                LastName = "def"
            }

            let result = Employee.insert employee  
                
            match result with
            | Output.MqWaitResponse(object) -> 
                Assert.AreEqual(employee, object) //should insert the original object into msg queue
            | other -> 
                other.GetType().ToString()
                |> sprintf "expected JsonResponse but got %s"
                |> Assert.Fail