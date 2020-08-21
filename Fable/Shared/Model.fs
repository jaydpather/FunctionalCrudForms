module Model

open System.Runtime.Serialization
open System.Runtime.Serialization.Json

[<DataContract>]
type Employee = {
    [<field: DataMember>]
    Name : string;
}