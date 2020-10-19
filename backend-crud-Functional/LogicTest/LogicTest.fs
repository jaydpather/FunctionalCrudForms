namespace RebelSoftware.Logic

open System
open System.Threading.Tasks
open NUnit.Framework
open RebelSoftware.Serialization

open Model

module Employee =
    [<TestFixture>]
    type LogicTest () =
        [<SetUp>]
        member this.Setup() = 
            ()

        [<Test>]
        member this.insert_validInput() = 
            let employee = {
                FirstName = "abc";
                LastName = "def"
            }

            let result = 
                employee  
                |> Json.serialize
                |> Employee.insert

            match result with
            | Output.MqWaitResponse(object) -> 
                Assert.AreEqual(employee, object) //should insert the original object into msg queue
            | other -> 
                other.GetType().ToString()
                |> sprintf "expected JsonResponse but got %s"
                |> Assert.Fail

        [<Test>]            
        member this.insert_invalidInput() = 
            //expecting Output.JsonResponse if invalid
            NotImplementedException()
            |> raise