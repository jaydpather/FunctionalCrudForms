namespace RebelSoftware.Logic

open System

module Output = 
    type Output = 
        JsonResponse of obj //serialize this object and write to HTTP Response
        | MqWaitResponse of obj //serialize to JSON and write to MQ, wait for MQ response, and return that as HTTP Response
        | MqOutput of obj //write to MQ and don't wait for response
        | LogFatalError of Exception
        
        //todo: | MqWaitAndCall of <obj, fn, type> //calls MQ, then calls a new logic layer function, deserializing the MQ response into an object of 'type'