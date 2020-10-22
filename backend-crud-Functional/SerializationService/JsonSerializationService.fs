namespace RebelSoftware.Serialization
open System

open Newtonsoft.Json

module Json =
    let deserialize<'a> (jsonString:string) = 
        let employee = JsonConvert.DeserializeObject<'a>(jsonString)
        employee

    let serialize object =
        let jsonString = JsonConvert.SerializeObject object
        jsonString