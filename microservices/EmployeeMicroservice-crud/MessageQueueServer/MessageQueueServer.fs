namespace RebelSoftware.MessageQueue

open System.Text
open RabbitMQ.Client

module MessageQueueServer =
    let startMessageQueueListener (serializeToJsonFn:obj->string) onMessageReceived = 
        let factory = ConnectionFactory()
        factory.HostName <- "localhost"
        use connection = factory.CreateConnection()
        use channel = connection.CreateModel()
        let queueResult = channel.QueueDeclare(queue = "employee", durable = false, exclusive = false, autoDelete = false, arguments = null)
        let consumer = QueueingBasicConsumer(channel)
        
        channel.BasicConsume(queue = "employee", autoAck = true, consumer = consumer) |> ignore
        while true do   
            let ea = consumer.Queue.Dequeue() //:> BasicDeliverEventArgs
            let body = ea.Body
            let props = ea.BasicProperties
            let replyProps = channel.CreateBasicProperties()
            replyProps.CorrelationId <- props.CorrelationId
            //todo 1:
            //  * move startMsgQueueListener to Message Queue layer
            //      * takes 1 param: business layer save function
            //  * extract the try/with into a new function: saveRecord
            //    * move into new business layer project
            //  * extract writeToMongo into new Data Layer
            //  * entry point (this file) passes the data layer func to bus layer, passes Bus Layer func to MessageQueueLayer
            //____

            let responseString = 
                Encoding.UTF8.GetString(body)
                |> onMessageReceived 
                |> fun opResult -> opResult :> obj
                |> serializeToJsonFn //todo: if we just use a function pointer for the serializeToJson function, can we avoid referencing Model?
            let responseBytes = Encoding.UTF8.GetBytes(responseString)
            let addr = PublicationAddress(exchangeName = "", exchangeType = ExchangeType.Direct, routingKey = props.ReplyTo)
            channel.BasicPublish(addr = addr, basicProperties = replyProps, body = responseBytes)
