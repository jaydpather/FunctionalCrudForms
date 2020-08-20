module Validation

open System

//todo: Shared.fsproj does not need references like Fable.React, Fable.Axios (only for UI)

//this has to be a static class, b/c JS does not support enums or records
//(so, if this type was an enum or record, it wouldn't get rendered to the transpiled JS code)
[<AbstractClass; Sealed>]
type ValidationResults private () =
    static member Success = 0
    static member FirstNameBlank = 1
    static member Saving = 32
    static member UnknownError = 64
    static member New = 128

let validateFirstName firstName = 
    match String.IsNullOrWhiteSpace(firstName) with 
        | true -> ValidationResults.FirstNameBlank
        | false -> ValidationResults.Success
