module Validation

open System
open Model

//todo: Shared.fsproj does not need references like Fable.React, Fable.Axios (only for UI)
let validateFirstName firstName = 
    let validationResult = 
        match String.IsNullOrWhiteSpace(firstName) with 
            | true -> ValidationResults.FirstNameBlank
            | false -> ValidationResults.Success
    { ValidationResult = validationResult }        
