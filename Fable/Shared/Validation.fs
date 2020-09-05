//todo: namespace?
module Validation

open System
open Model

//todo: Shared.fsproj does not need references like Fable.React, Fable.Axios (only for UI)

type EmployeeValidator = {
    ValidateEmployee : Employee -> OperationResult
}

let private validateEmployee (employee:Employee) = 
    let firstNameResult = 
        match String.IsNullOrWhiteSpace(employee.FirstName) with 
            | true -> ValidationResults.FirstNameBlank
            | false -> ValidationResults.Success

    let lastNameResult = 
        match String.IsNullOrWhiteSpace(employee.LastName) with 
            | true -> ValidationResults.LastNameBlank
            | false -> ValidationResults.Success
    { ValidationResult = firstNameResult ||| lastNameResult }        

let getEmployeeValidator () = 
    { ValidateEmployee = validateEmployee }