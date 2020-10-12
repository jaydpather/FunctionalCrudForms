namespace RebelSoftware.Logic

open System
open Model
open RebelSoftware.Serialization

module Employee =
    
    let Insert employee = 

        let opResult = 
            employee
            |> Validation.getEmployeeValidator().ValidateEmployee 
        match opResult.ValidationResult = ValidationResults.Success with 
            | true -> 
                employee :> obj
                |> Output.MqWaitResponse 
            | _ -> 
                opResult :> obj
                |> Output.JsonResponse
