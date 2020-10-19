namespace RebelSoftware.Logic

open System

module Output = 
    type Output = 
        JsonResponse of obj //serialize this object and write to HTTP Response
        | MqWaitResponse of obj //serialize to JSON and write to MQ, wait for MQ response, and return that as HTTP Response
        | MqOutput of obj * obj //write first obj to MQ and write 2nd object to HTTP Response
        | LogFatalError of Exception
        
        //todo: | MqWaitAndCall of <obj, fn, type> //calls MQ, then calls a new logic layer function, deserializing the MQ response into an object of 'type'