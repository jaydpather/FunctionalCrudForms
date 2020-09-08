namespace RebelSoftware.DataLayer


module DocumentDb = 
    type DocumentDbRepository = {
        WriteToDb : obj -> unit
    }

    let createDocumentDbRepository () = {
        WriteToDb = MongoDb.writeToMongo
    }