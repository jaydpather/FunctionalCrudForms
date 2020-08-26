module Model

type Employee = {
    Name : string;
    LastName: string;
}

//this has to be a static class, b/c JS does not support enums or records
//(so, if this type was an enum or record, it wouldn't get rendered to the transpiled JS code)
[<AbstractClass; Sealed>]
type ValidationResults private () =
    static member Success = 0
    static member FirstNameBlank = 1
    static member LastNameBlank = 2
    static member Saving = 32
    static member UnknownError = 64
    static member New = 128

type OperationResult = {
    ValidationResult: int;
}