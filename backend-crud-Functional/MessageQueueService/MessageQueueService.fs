namespace RebelSoftware.MessageQueueService //todo: rename to RebelSoftware.MessageQueue

open System
open System.Text
open System.Collections.Concurrent

open RabbitMQ.Client
open RabbitMQ.Client.Events

module MessageQueueing =
    type private RPCInfo = {
        //todo: capitalize member names
        connection : IConnection;
        channel : IModel;
        consumer : EventingBasicConsumer;
        respQueue : BlockingCollection<string>;
        props : IBasicProperties;
    }

    let private onMessageReceived (respQueue:BlockingCollection<string>) correlationId (ea:BasicDeliverEventArgs) = 
        let response = Encoding.UTF8.GetString(ea.Body)
        if(ea.BasicProperties.CorrelationId = correlationId) then
            respQueue.Add(response)

    let private createRpcInfoObject () = 
        let factory = ConnectionFactory()
        factory.HostName <- "localhost" //todo: does F# have object initializer? //todo: caller gets host name from config file and passes it to createMessageQueuer
        let respQueue = new BlockingCollection<string>()
        let connection = factory.CreateConnection()
        let channel = connection.CreateModel()
        let consumer = EventingBasicConsumer(channel)
        let props = channel.CreateBasicProperties();
        props.CorrelationId <- Guid.NewGuid().ToString()
        props.ReplyTo <- channel.QueueDeclare().QueueName
        consumer.Received.Add(onMessageReceived respQueue props.CorrelationId)
        {
            connection = connection;
            channel = channel;
            consumer = consumer;
            respQueue = respQueue;
            props = props;
        }
    
    let private publishToMsgQueue (jsonString:string) rpcInfo = 
        let msgBytes = Encoding.UTF8.GetBytes(jsonString)
        rpcInfo.channel.BasicPublish(exchange=String.Empty, routingKey="employee", basicProperties=rpcInfo.props, body=msgBytes) //todo: routing key from config file
        rpcInfo.channel.BasicConsume(consumer = rpcInfo.consumer, queue=rpcInfo.props.ReplyTo, autoAck=true)

    let writeMessageAndGetResponse jsonString =
        
        let rpcInfo = createRpcInfoObject () //todo 1: create MQ Layer, startup injects function pointer
        rpcInfo
        |> publishToMsgQueue jsonString 
        |> ignore

        let mutable mqResponse = String.Empty
        match rpcInfo.respQueue.TryTake(&mqResponse, 30000) with //todo: timeout value from config file
        | true -> mqResponse
        | false -> 
            sprintf "timeout while waiting for microservice response to RabbitMQ message '%s'" jsonString
            |> Exception 
            |> raise