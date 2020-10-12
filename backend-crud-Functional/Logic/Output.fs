namespace RebelSoftware.Logic

open System

module Output = 
    type Output = 
        JsonResponse of obj //serialize this object and write to HTTP Response
        | MqWaitResponse of obj //serialize to JSON and write to MQ, wait for MQ response, and return that as HTTP Response
        | LogFatalError of Exception
        | MqOutput //don't wait for response