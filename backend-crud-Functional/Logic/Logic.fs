namespace RebelSoftware.Logic

open System
open Model
open RebelSoftware.Serialization

module Employee =

    let private validate employee = 
        employee
        |> Validation.getEmployeeValidator().ValidateEmployee 
    
    let private produceResponse employee validationOpResult =
        match validationOpResult.ValidationResult = ValidationResults.Success with 
        | true -> 
            employee :> obj
            |> Output.MqWaitResponse 
        | _ -> 
            validationOpResult :> obj
            |> Output.JsonResponse

    let insert employee = 
        employee
        |> validate
        |> produceResponse employee
