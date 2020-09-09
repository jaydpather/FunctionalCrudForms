namespace RebelSoftware.SerializationService

open System

open Newtonsoft.Json

module Serialization =
    
    type SerializationService<'a> = { //todo: rename to TypeSerializer?
        DeserializeFromJson : string -> 'a; //todo: generic type param
        SerializeToJson : obj -> string;
    }

    let private deserializeFromJson<'a> (jsonString:string) = 
        let employee = JsonConvert.DeserializeObject<'a>(jsonString)
        employee

    let private serializeToJson object =
        let jsonString = JsonConvert.SerializeObject object
        jsonString

    let createSerializationService<'a> () = { 
        DeserializeFromJson = deserializeFromJson<'a>;
        SerializeToJson = serializeToJson;
    }
