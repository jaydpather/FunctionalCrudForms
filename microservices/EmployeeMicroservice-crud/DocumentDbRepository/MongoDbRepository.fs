namespace RebelSoftware.DataLayer

open MongoDB.Driver
open MongoDB.Bson
open Microsoft.FSharp.Reflection

module MongoDb =

    let convertToDictionary (record) =
        seq {
            //todo: load test to see how slow Reflection is
            //  * cache FSharpType.GetRecordFields and check how fast it is
            //  * compare to hardcoded conversion to dictionary
            for prop in FSharpType.GetRecordFields(record.GetType()) -> 
            prop.Name, prop.GetValue(record)
        } |> dict

    let writeToMongo record = 
        let client = MongoClient()
        let database = client.GetDatabase("FunctionalCrudForms") //todo: get db name from config file
        let collection = database.GetCollection<BsonDocument>("Employee"); //todo: get collection name from config file
        
        let document = 
            record 
            |> convertToDictionary 
            |> BsonDocument

        collection.InsertOne(document) 
            |> ignore
